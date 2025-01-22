using UnityEngine;

namespace Party.Input
{
    public class PlayerInputHelper : MonoBehaviour
    {
        [SerializeField] private UnityEngine.InputSystem.PlayerInput _playerInput;
        
        private void OnEnable() => InputManager.Current.UpdateCurrentActionMap(_playerInput.playerIndex);
    }
}