using System;
using System.Collections;
using Party.Cameras;
using Party.Interactions.Interfaces;
using Party.Minigames.General;
using Party.Movement.Interfaces;
using Party.Player.Visuals;
using Party.Utilities;
using UnityEngine;

namespace Party.Movement
{
    public class PlayerMovement : MonoBehaviour, IKillable, IPlayer
    {
        public float SpeedRatio => CalculateSpeed();
        public event Action JumpedEvent;
        public event Action LandedEvent;
        public event Action DeathEvent;
        public event Action WonEvent;
        
        public bool CanMove { get; set; } = true;
        public float SpeedFactor = 1f;

        [Header("Input")] 
        [SerializeField] private int _index;
        public int Index => _index;

        [Header("Dependencies")] 
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private PlayerSkinHandler _skinHandler;
        [SerializeField] private Camera _camera;
        [SerializeField] private MovementSettings _settings;
        [SerializeField] private MovementInputReader _inputReader;
        [Space]
        [SerializeField] private Transform _model;
        [SerializeField] private float _turnSpeed;
        
        
        private Vector3 _moveInput;
        private Vector3 _goalVel;

        private Coroutine _cooldownCoroutine;

        private float _time;
        private float _timeLeftGrounded = float.MinValue;

        private bool _groundHit;
        private bool _grounded;
        
        private RaycastHit _hitInfo;
        
        private bool _performingJump;
        private bool _jumpToConsume;
        private bool _bufferedJumpUsable;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private bool _jumpHeld;
        private float _timeJumpWasPressed;

        private bool _hasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _settings.JumpBuffer;
        private bool _canUseCoyote => _coyoteUsable && !_grounded && _time < _timeLeftGrounded + _settings.CoyoteTime;
        
        private readonly WaitForFixedUpdate _waitForFixedUpdate = new();
        private WaitUntil _hasLanded;
        private WaitUntil _hasStopped;

        private bool _hasWon;
        private bool _hasDied;
        
        
        private void Awake()
        {
            MinigameFinisherBase.MinigameFinishedEvent += OnMinigameFinished;
            SplitScreenManager.CameraChangedEvent += OnCameraChanged;

            _inputReader.MoveEvent += OnMove;
            _inputReader.JumpEvent += OnJump;
            _inputReader.PunchEvent += OnPunch;
            
            if (!_model)
                _skinHandler.SkinSpawnedEvent += OnSkinSpawned;
            
            if(_camera == null)
                _camera = Camera.main;
            
            _hasLanded = new WaitUntil(() => _grounded);
            _hasStopped = new WaitUntil(() => _goalVel.x + _goalVel.z == 0f);
            _time = _settings.JumpBuffer;
        }
        private void OnDestroy()
        {
            MinigameFinisherBase.MinigameFinishedEvent -= OnMinigameFinished;
            SplitScreenManager.CameraChangedEvent -= OnCameraChanged;
            
            _inputReader.MoveEvent -= OnMove;
            _inputReader.JumpEvent -= OnJump;
            _inputReader.PunchEvent -= OnPunch;
            
            _skinHandler.SkinSpawnedEvent -= OnSkinSpawned;
        }
        
        #region On Events
        
        private void OnJump(bool started)
        {
            if(_settings.JumpPower == 0)
                return;
            
            if (started)
            {
                _jumpHeld = true;
                _jumpToConsume = true;
                _timeJumpWasPressed = _time;
            }
            else
            {
                _jumpHeld = false;
            }
        }

        private void OnPunch()
        {
            var hits = new RaycastHit[10];
            var hitCount = Physics.BoxCastNonAlloc(transform.position, Vector3.one * 0.5f, _model.forward, hits, Quaternion.identity, 1f);

            for (var i = 0; i < hitCount; i++)
            {
                var raycastHit = hits[i];
                if (raycastHit.transform == transform)
                    continue;

                if (raycastHit.transform.TryGetComponent(out IHittable hittable))
                    hittable.OnHit(Index);

                if (raycastHit.transform.TryGetComponent(out PlayerMovement player))
                {
                    player.OnHit(_model.forward);
                    return;
                }
                
                if(raycastHit.rigidbody == null)
                    return;

                raycastHit.rigidbody.AddForceAtPosition(_model.forward * _settings.ObjectPunchForce, raycastHit.point, ForceMode.Impulse);
            }
        }
        
        private IEnumerator HitCooldownCoroutine()
        {
            CanMove = false;

            yield return _hasStopped;

            CanMove = true;
        }

        private void OnMove(Vector2 direction)
        {
            if(!CanMove && direction != Vector2.zero)
                return;
            
            _moveInput = direction.V22V3().AlignToCamera(_camera.transform);
        }

        private void OnSkinSpawned(Animator animator) => _model = animator.transform;

        private void OnCameraChanged(Camera[] cameras) => _camera = cameras.Length == 1 ? cameras[0] : cameras[Index];
        
        public void OnHit(Vector3 direction, float? force = null, bool cooldown = true)
                {
                    force ??= _settings.PlayerPunchForce;
                    _goalVel += direction * (float)force;

                    if(_cooldownCoroutine != null)
                        StopCoroutine(_cooldownCoroutine);
                    
                    if(cooldown) 
                        _cooldownCoroutine = StartCoroutine(HitCooldownCoroutine());
                }

        private void OnMinigameFinished(int winnerIndex)
        {
            if(winnerIndex == Index)
                OnWon();
            else
                OnDeath();
        }
        
        public void OnDeath()
        {
            if (_hasDied || _hasWon)
                return;

            _hasDied = true;
            CanMove = false;
            OnMove(Vector2.zero);
            
            DeathEvent?.Invoke();
        }

        public void OnWon()
        {
            if (_hasDied || _hasWon)
                return;
            
            _hasWon = true;
            CanMove = false;
            OnMove(Vector2.zero);
            
            WonEvent?.Invoke();
        }
        
        #endregion
        
        private void Update()
        {
            _time += Time.deltaTime;

            if (!CanMove)
                _moveInput = Vector3.zero;
            
            Look();
            UpdateModel();
            CalculateSpeed();
        }
        private void Look()
        {
            if (_moveInput == Vector3.zero) return;

            var rot = Quaternion.LookRotation(_moveInput, Vector3.up);
            _model.rotation = Quaternion.RotateTowards(_model.rotation, rot, _turnSpeed * Time.deltaTime);
        }
        private void UpdateModel()
        {
            if(!_grounded || _performingJump)
                return;

            var position = _model.position;
            var desiredPosition = new Vector3(position.x, _hitInfo.point.y, position.z);
            
            _model.position = position.y - _hitInfo.point.y < 0f ? desiredPosition : Vector3.MoveTowards(_model.position, desiredPosition, Time.deltaTime);
        }

        private float CalculateSpeed()
        {
            var velocity = Vector3.Scale(_rigidbody.velocity, Vector3.one - Vector3.up);
            
            var direction = Vector3.Dot(velocity.normalized, _model.forward);

            var speed = velocity.magnitude / _settings.MaxSpeed;
            
            return speed * direction;
        }


        private void FixedUpdate()
        {
            CheckCollisions();
            
            Suspension();
            HandleDirections();
            
            HandleJump();
            HandleGravity();

            _rigidbody.velocity = _goalVel;
        }
        private void CheckCollisions()
        {
            _groundHit = Physics.Raycast(transform.position, Vector3.down, out _hitInfo, _settings.RestPosition);
            
            // Landed on the Ground
            if (!_grounded && _groundHit)
            {
                _grounded = true;
                _coyoteUsable = true;
                _bufferedJumpUsable = true;
                _endedJumpEarly = false;
            }
            // Left the Ground
            else if (_grounded && !_groundHit)
            {
                _grounded = false;
                _timeLeftGrounded = _time;
            }
        }
        
        private void Suspension()
        {
            if (!_groundHit || _performingJump) return;
            
            var vel = _rigidbody.velocity;
            var rayDir = -transform.up;
            
            var hitBody = _hitInfo.rigidbody;
            var otherVel = hitBody == null ? Vector3.zero : hitBody.velocity;
            
            var rayDirVel = Vector3.Dot(rayDir, vel);
            var otherDirVel = Vector3.Dot(rayDir, otherVel);

            var relVel = rayDirVel - otherDirVel;
                
            var offset = _hitInfo.distance - _settings.RestPosition;
                
            var springForce = (offset * _settings.SpringStrength) - (relVel * _settings.SpringDamper);
                
            _rigidbody.AddForce(rayDir * springForce);

            if (hitBody != null) 
                hitBody.AddForceAtPosition(rayDir * -springForce, _hitInfo.point);
        }
        private void HandleDirections()
        {
            if (_moveInput.magnitude != 0)
            {
                var velDot = Vector3.Dot(_moveInput, _goalVel.normalized);

                var acceleration = _settings.Acceleration * _settings.AccelerationCurve.Evaluate(velDot);
            
                var cachedY = _goalVel.y;
            
                _goalVel = Vector3.MoveTowards(_goalVel, _moveInput * _settings.MaxSpeed * SpeedFactor, acceleration * Time.fixedDeltaTime);
                _goalVel.y = cachedY;
            }
            else
            {
                var cachedY = _goalVel.y;
                var deceleration = _grounded ? _settings.GroundDeceleration : _settings.AirDeceleration;
                
                _goalVel = Vector3.MoveTowards(_goalVel, Vector3.zero, deceleration * Time.fixedDeltaTime);
                _goalVel.y = cachedY;
            }
        }
        
        private void HandleJump()
        {
            if (!_endedJumpEarly && !_grounded && !_jumpHeld && _rigidbody.velocity.y > 0) _endedJumpEarly = true;

            if (!_jumpToConsume && !_hasBufferedJump) return;

            if (_grounded || _canUseCoyote) ExecuteJump();

            _jumpToConsume = false;
        }
        private void ExecuteJump()
        {
            _performingJump = true;
            _endedJumpEarly = false;
            _timeJumpWasPressed = 0;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            _goalVel.y = _settings.JumpPower;
            
            StopAllCoroutines();
            StartCoroutine(ResetJumpCoroutine());
        }
        private IEnumerator ResetJumpCoroutine()
        {
            JumpedEvent?.Invoke();
            yield return _waitForFixedUpdate;
            yield return _waitForFixedUpdate;
            yield return _hasLanded;
            
            _performingJump = false;
            LandedEvent?.Invoke();
        }
        
        private void HandleGravity()
        {
            if (_grounded && _goalVel.y <= 0f)
            {
                _goalVel.y = -_settings.GroundingForce;
            }
            else
            {
                float inAirGravity;

                if (_goalVel.y > 0f)
                {
                    inAirGravity = _settings.RiseAcceleration;
                    
                    if (_endedJumpEarly) 
                        inAirGravity *= _settings.JumpEndEarlyGravityModifier;
                }
                else
                {
                    inAirGravity = _settings.FallAcceleration;
                }
                
                _goalVel.y = Mathf.MoveTowards(_goalVel.y, -_settings.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }

        #region Gizmos

        private void OnDrawGizmos()
        {
            if(_model)
                return;
            
            Gizmos.color = Index == 0 ? Color.red: Color.blue;
            Gizmos.DrawCube(transform.position, Vector3.up + Vector3.one);
        }
        
        #endregion
    }
}