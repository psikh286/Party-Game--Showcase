using System;
using System.Collections;
using Party.Minigames.General;
using Party.Movement;
using Party.Movement.Interfaces;
using UnityEngine;

namespace Party.Minigames.Canoe
{
    public class CanoeMovement : MonoBehaviour, IPlayer
    {
        #region IPlayer
        
        [SerializeField] private int _index;
        public int Index => _index;
        public bool CanMove { get; set; } = true;
        public event Action DeathEvent;

        #endregion

        [SerializeField] private float _rotSpeed;
        [SerializeField] private float _playerRot;
        [SerializeField] private float _loseRot;
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _lerpSpeed;
        [SerializeField] private MovementInputReader _inputReader;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Transform _gizmosWaypoint;

        private float _inputValue;

        private bool _canMove = true;
        private bool _isDead;

        private void Awake()
        {
            _inputReader.MoveEvent += OnMove;
            
            MinigameFinisherBase.MinigameFinishedEvent += OnMinigameFinished;
        }
        
        private void OnDestroy()
        {
            _inputReader.MoveEvent -= OnMove;
            
            MinigameFinisherBase.MinigameFinishedEvent += OnMinigameFinished;
        }
        
        private void OnMinigameFinished(int winnerIndex)
        {
            if (winnerIndex == _index)
                return;
            
            OnDeath();
        }



        private void OnMove(Vector2 input)
        {
            if(!_canMove || _isDead || input.x == 0f)
                return;
            
            _canMove = false;
            
            _inputValue = input.x;
            
            StartCoroutine(Cooldown());
        }

        private IEnumerator Cooldown()
        {
            _meshRenderer.material.color = Color.red;

            yield return new WaitForSeconds(1f);
            _inputValue = 0f;
            
            yield return new WaitForSeconds(.4f);

            _canMove = true;
            
            _meshRenderer.material.color = Color.green;
        }
        

        private void Update()
        {
            if(_isDead)
                return;
            
            var rotation = transform.eulerAngles;
            var angle = (rotation.z  > 180) ?  rotation.z - 360f :  rotation.z;
            
            TiltCanoe(angle);
            TurnCanoe(angle);
            
            if ((angle  < _loseRot && angle > -_loseRot) || _isDead)
                return;
            
        }

        private void OnDeath()
        {
            _isDead = true;
            _meshRenderer.material.color = Color.black;
        }

        private void TiltCanoe(float angle)
        {
            var goalRotation = transform.eulerAngles;
            
            goalRotation.z += Mathf.Sign(angle) * _rotSpeed * Time.deltaTime;
            goalRotation.z += _playerRot * _inputValue * Time.deltaTime;
            
            transform.eulerAngles = goalRotation;
        }

        private void TiltCanoeLerp(float angle)
        {
            var goalRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + Mathf.Sign(angle) * _rotSpeed + _playerRot * _inputValue);
    
            // Smoothly interpolate towards the target rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, goalRotation, Time.deltaTime * _lerpSpeed);

        }

        private void TurnCanoe(float angle)
        {
            var goalPosition = transform.position;
            
            goalPosition.x -= angle * _turnSpeed * Time.deltaTime;
            
            transform.position = goalPosition;
        }


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.magenta;

            var origin = transform.position + Vector3.back * 1f;
            
            Gizmos.DrawLine(origin, _gizmosWaypoint.position);
            
            Gizmos.color = Color.black;
            
            var angle = 2f * Mathf.PI * ((_loseRot + 90 )/360);

            var dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            var offset = new Vector3(dir.x, dir.y, 0f);

            Gizmos.DrawLine(origin, origin + offset * 2f);
            
            offset = new Vector3(-dir.x, dir.y, 0f);
            Gizmos.DrawLine(origin, origin + offset * 2f);

        }
    }
}