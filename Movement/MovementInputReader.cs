using System;
using Party.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Party.Movement
{
    public class MovementInputReader : BaseInputReader
    {
        protected override string _actionMapName => "GroundMovement";
        
        public event Action<Vector2> MoveEvent;
        public event Action<bool> JumpEvent;
        public event Action PunchEvent; 
        
        
        protected override void Subscribe()
        {
            base.Subscribe();
            
            _actionMap.FindAction("Move").performed += OnMove;
            _actionMap.FindAction("Move").canceled += OnMove;
            
            _actionMap.FindAction("Jump").performed += OnJump;
            _actionMap.FindAction("Jump").canceled += OnJump;

            _actionMap.FindAction("Punch").performed += OnPunch;
        }

        protected override void Unsubscribe()
        {
            if(!CanUnsubscribe())
                return;
            
            _actionMap.FindAction("Move").performed -= OnMove;
            _actionMap.FindAction("Move").canceled -= OnMove;
            
            _actionMap.FindAction("Jump").performed -= OnJump;
            _actionMap.FindAction("Jump").canceled -= OnJump;
            
            _actionMap.FindAction("Punch").performed -= OnPunch;
        }
        
        private void OnMove(InputAction.CallbackContext context) => MoveEvent?.Invoke(context.ReadValue<Vector2>());
        private void OnJump(InputAction.CallbackContext context) => JumpEvent?.Invoke(context.performed);
        private void OnPunch(InputAction.CallbackContext context) => PunchEvent?.Invoke();
    }
}