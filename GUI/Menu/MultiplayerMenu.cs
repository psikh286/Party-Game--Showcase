using System;
using Party.PlayerInput;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Party.GUI.Menu
{
    public abstract class MultiplayerMenu : BaseMenu
    {
        [SerializeField] private CanvasData[] _canvasDatas;

        [Serializable]
        protected struct CanvasData
        {
            public GameObject Canvas;
            public GameObject FirstSelected;
        }
        
        protected override void OnMenuOpen()
        {
            base.OnMenuOpen();
            
            foreach (var player in PlayerInputInstancesTracker.Current.Players)
                AssignCanvas(player);
        }
        
        protected override void OnPlayerConnectionChanged(bool isConnected, PlayerInputReferencesProvider playerInput) => AssignCanvas(playerInput, isConnected);

        private void AssignCanvas(PlayerInputReferencesProvider playerInput, bool isConnected = true)
        {
            if (!isConnected)
            {
                playerInput.EventSystem.SetSelectedGameObject(null);
                return;
            }

            if (_canvasDatas == null) 
                return;

            playerInput.EventSystem.playerRoot = _canvasDatas[playerInput.PlayerIndex].Canvas;
            playerInput.EventSystem.firstSelectedGameObject = _canvasDatas[playerInput.PlayerIndex].FirstSelected;
        }
    }
}