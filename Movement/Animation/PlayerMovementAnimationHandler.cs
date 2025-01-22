using Party.Player.Visuals;
using UnityEngine;

namespace Party.Movement.Animation
{
    public class PlayerMovementAnimationHandler : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerMovement _player;
        [SerializeField] private PlayerSkinHandler _skinHandler;
        
        private Animator _animator;
        
        private static readonly int _walkingSpeed = Animator.StringToHash("walkingSpeed");
        private static readonly int _isWalking = Animator.StringToHash("isWalking");
        private static readonly int _isFalling = Animator.StringToHash("isFalling");
        private static readonly int _hasJumped = Animator.StringToHash("hasJumped");
        private static readonly int _hasDashed = Animator.StringToHash("hasJumped");
        private static readonly int _startedDancing = Animator.StringToHash("startedDancing");
        private static readonly int _hasDied = Animator.StringToHash("hasDied");

        private void Awake() => _skinHandler.SkinSpawnedEvent += OnSkinSpawned;
        private void OnDestroy() => _skinHandler.SkinSpawnedEvent -= OnSkinSpawned;

        private void OnEnable()
        {
            _player.JumpedEvent += OnJumped;
            _player.LandedEvent += OnLanded;
            _player.DeathEvent += OnDeath;
            _player.WonEvent += OnWon;
        } 
        private void OnDisable()
        {
            _player.JumpedEvent -= OnJumped;
            _player.LandedEvent -= OnLanded;
            _player.DeathEvent -= OnDeath;
            _player.WonEvent -= OnWon;
        }
        
        private void OnSkinSpawned(Animator animator) => _animator = animator;


        private void Update()
        {
            _animator.SetFloat(_walkingSpeed, _player.SpeedRatio);
        }

        private void OnLanded()
        {
            _animator.SetBool(_isFalling, false);
            _animator.ResetTrigger(_hasJumped);
        }

        private void OnJumped()
        {
            _animator.SetTrigger(_hasJumped);
            _animator.SetBool(_isFalling, true);
        }

        private void OnDeath() => _animator.SetTrigger(_hasDied);

        private void OnWon() => _animator.SetTrigger(_startedDancing);
    }
}