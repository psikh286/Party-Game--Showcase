using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Party.Debugging;
using Party.Essentials;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Party.PlayerInput
{ 
    /// <summary>
    /// Handles player Join/Left logic and stores references to active players
    /// </summary>
    public class PlayerInputInstancesTracker : MonoBehaviour
    {
        /// <summary>
        /// Shortcut
        /// </summary>
        public static PlayerInputInstancesTracker Current => GameManager.Instance.ReferenceProvider.PlayerInputInstancesTracker;

        /// <summary>
        /// Invoked when player is joined or left
        /// </summary>
        public static event Action<bool, PlayerInputReferencesProvider> PlayerConnectionChangedEvent;
        
        /// <summary>
        /// Reference to all active players in a only-read format
        /// </summary>
        public ReadOnlyCollection<PlayerInputReferencesProvider> Players => _players.ToList().AsReadOnly();
        
        [SerializeField] private PlayerInputManager _playerInputManager;
        [SerializeField] private DebugLogger _logger;
        
        /// <summary>
        /// List of all active players
        /// </summary>
        private readonly HashSet<PlayerInputReferencesProvider> _players = new();
        
        
        private void Awake()
        {
            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;
        }
        private void OnDestroy()
        {
            _playerInputManager.onPlayerJoined -= OnPlayerJoined;
            _playerInputManager.onPlayerLeft -= OnPlayerLeft;
        }
    
        private void OnPlayerJoined(UnityEngine.InputSystem.PlayerInput playerInput)
        {
            playerInput.onDeviceLost += OnPlayerLeft;
            
            if (!playerInput.TryGetComponent(out PlayerInputReferencesProvider player))
            {
                _logger.LogError("Tried to get PlayerReferencesProvider from the playerInput but it doesn't exist", this);
                OnPlayerLeft(playerInput);
                return;
            }
            if (!_players.Add(player))
            {
                _logger.Log("Tried to add player to hashset twice", this);
            }
            
            playerInput.transform.SetParent(transform);
            
            PlayerConnectionChangedEvent?.Invoke(true, player);

            if (_playerInputManager.playerCount == _playerInputManager.maxPlayerCount)
                _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        }
        private void OnPlayerLeft(UnityEngine.InputSystem.PlayerInput playerInput)
        {
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenJoinActionIsTriggered;
            
            playerInput.onDeviceLost -= OnPlayerLeft;
            
            if (!playerInput.TryGetComponent(out PlayerInputReferencesProvider player))
            {
                _logger.LogError($"<PlayerInstancesTracker> Tried to get PlayerReferencesProvider from the playerInput but it doesn't exist", this);
                return;
            }
            
            PlayerConnectionChangedEvent?.Invoke(false, player);
            
            _players.Remove(player);
            Destroy(playerInput.gameObject);
        }

        public void DisconnectPlayer(PlayerInputReferencesProvider playerInput) => Destroy(playerInput.gameObject);

        public bool TryGetPlayer(int playerIndex, out PlayerInputReferencesProvider playerInput)
        { 
            playerInput = _players.FirstOrDefault(p => p.PlayerIndex == playerIndex);

            return playerInput != null;
        }
    }
}