using System;
using Party.Input;
using UnityEngine.InputSystem;

namespace Party.GUI.DeviceSetup
{
    public class DeviceSetupInputReader : BaseInputReader
    {
        protected override string _actionMapName => "DeviceSetupMenu";
        
        public event Action<int> MoveEvent;
        public event Action SubmitEvent;
        public event Action DeleteEvent; 
        
        private void OnEnable()
        {
            _actionMap.FindAction("Move").performed += OnMove;
            _actionMap.FindAction("Submit").performed += OnSubmit;
            _actionMap.FindAction("Delete").performed += OnDelete;
        }

        protected override void Unsubscribe()
        {
        }

        private void OnDisable()
        {
            _actionMap.FindAction("Move").performed -= OnMove;
            _actionMap.FindAction("Submit").performed -= OnSubmit;
            _actionMap.FindAction("Delete").performed -= OnDelete;
        }

        private void OnMove(InputAction.CallbackContext context) => MoveEvent?.Invoke((int)context.ReadValue<float>());
        private void OnSubmit(InputAction.CallbackContext context) => SubmitEvent?.Invoke();
        private void OnDelete(InputAction.CallbackContext context) => DeleteEvent?.Invoke();
    }
}