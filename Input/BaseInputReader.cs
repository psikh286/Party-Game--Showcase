using Party.Debugging;
using Party.PlayerInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Party.Input
{
    public abstract class BaseInputReader : MonoBehaviour
    {
        [SerializeField] private int _index;
        [SerializeField] private DebugLogger _logger;
        
        protected abstract string _actionMapName { get; }

        private UnityEngine.InputSystem.PlayerInput _playerInput;
        protected InputActionMap _actionMap;
        
       
        protected virtual void Awake()
        {
            
            PlayerInputInstancesTracker.PlayerConnectionChangedEvent += OnPlayerConnectionChanged;
        }
        private void OnDestroy()
        {
            PlayerInputInstancesTracker.PlayerConnectionChangedEvent -= OnPlayerConnectionChanged;
        }
        
        private void OnEnable()
        {
            if (!PlayerInputInstancesTracker.Current.TryGetPlayer(_index, out var player))
            {
                _logger.LogError($"Tried to get player #{_index}, but it doesn't exist", this);
                return;
            }

            _playerInput = player.PlayerInput;

            Subscribe();
        }

        protected virtual void Subscribe()
        {
            _actionMap = _playerInput.actions.FindActionMap(_actionMapName);
        }
        protected abstract void Unsubscribe();

        protected bool CanUnsubscribe()
        {
            return _playerInput != null && _actionMap != null;
        }
        
        private void OnPlayerConnectionChanged(bool connected, PlayerInputReferencesProvider playerInput)
        {
            if (playerInput.PlayerIndex != _index) 
                return;
            
            if (connected)
            {
                Unsubscribe();
                _playerInput = playerInput.PlayerInput;
                Subscribe();
            }
            else
            {
                Unsubscribe();
            }
        }
    }
}