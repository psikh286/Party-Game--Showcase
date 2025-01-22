using Party.Debugging;
using Party.Essentials;
using Party.PlayerInput;
using UnityEngine;

namespace Party.Input
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Current => GameManager.Instance.ReferenceProvider.InputManager;
        
        [SerializeField] private DebugLogger _logger;
        
        private static readonly string[] _currentActionMaps = new string[2];
        private static readonly string[] _previousActionMaps = new string[2];
        
        public void SwitchActionMap(string mapName)
        {
            SwitchActionMap(0, mapName);
            SwitchActionMap(1, mapName);
        }
        public void SwitchActionMap(int playerIndex, string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
            {
                _logger.LogError("Map name is empty", this);
                return;
            }
            
            _previousActionMaps[playerIndex] = _currentActionMaps[playerIndex];
            _currentActionMaps[playerIndex] = mapName;
            
            if (!PlayerInputInstancesTracker.Current.TryGetPlayer(playerIndex, out var player))
            {
                _logger.LogError($"Player #{playerIndex} doesn't exist", this);
                return;
            }
            
            player.PlayerInput.SwitchCurrentActionMap(mapName);
        }
        
        public void SwitchActionMapsBack()
        {
            SwitchActionMapBack(0);
            SwitchActionMapBack(1);
        }
        public void SwitchActionMapBack(int playerIndex)
        {
            var previousMap = _previousActionMaps[playerIndex];

            if (string.IsNullOrWhiteSpace(previousMap))
            {
                _logger.LogError($"Player #{playerIndex} has no previous action map", this);
                return;
            }
            
            _previousActionMaps[playerIndex] = _currentActionMaps[playerIndex];
            _currentActionMaps[playerIndex] = previousMap;
            
            if (!PlayerInputInstancesTracker.Current.TryGetPlayer(playerIndex, out var player))
            {
                _logger.LogError($"Player #{playerIndex} doesn't exist", this);
                return;
            }
            
            player.PlayerInput.SwitchCurrentActionMap(previousMap);
        }

        public void UpdateCurrentActionMap(int playerIndex)
        {
            if (string.IsNullOrWhiteSpace(_currentActionMaps[playerIndex] ))
            {
                _logger.LogError($"Player #{playerIndex} tried to update action map but it is empty", this);
                return;
            }
            
            if (!PlayerInputInstancesTracker.Current.TryGetPlayer(playerIndex, out var player))
            {
                _logger.LogError($"Player #{playerIndex} doesn't exist", this);
                return;
            }
            
            player.PlayerInput.SwitchCurrentActionMap(_currentActionMaps[playerIndex]);
        }
    }
}