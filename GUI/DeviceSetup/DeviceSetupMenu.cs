using System.Collections.Generic;
using System.Linq;
using Party.GUI.Menu;
using Party.Helpers;
using Party.Input;
using Party.Player;
using Party.PlayerInput;
using UnityEngine;

namespace Party.GUI.DeviceSetup
{
    public class DeviceSetupMenu : MultiplayerMenu
    {
        protected override MenuID _menuID => MenuID.DeviceSetupMenu;
        
        [Header("General")]
        [SerializeField] private DeviceIcon[] _deviceIcons;
        [SerializeField] private DeviceSetupPlayer[] _devicePlayers;
        
        private readonly Dictionary<DeviceType, Sprite> _deviceSprites = new();

        protected override void Awake()
        {
            base.Awake();
            
            foreach (var deviceImage in _deviceIcons.Where(device => !_deviceSprites.TryAdd(device.InputDeviceName, device.Sprite)))
            {
                Log("Tried to add device image, but it's already added");
            }
        }

        protected override void OnMenuOpen()
        {
            base.OnMenuOpen();
            
            InputManager.Current.SwitchActionMap("DeviceSetupMenu");
            DeviceSetupPlayer.ReadyEvent += TryContinue;

            foreach (var player in PlayerInputInstancesTracker.Current.Players)
                _devicePlayers[player.PlayerIndex].Connect(GetDeviceType(player));
        }
        protected override void OnMenuClosed()
        {
            base.OnMenuClosed();
            
            DeviceSetupPlayer.ReadyEvent -= TryContinue;
            
            foreach (var player in PlayerInputInstancesTracker.Current.Players)
                _devicePlayers[player.PlayerIndex].Disconnect();
        }

        protected override void OnPlayerConnectionChanged(bool connected, PlayerInputReferencesProvider playerInput)
        {
            base.OnPlayerConnectionChanged(connected, playerInput);
            
            if(!_menuUi.activeSelf) return;

            if (connected) 
                _devicePlayers[playerInput.PlayerIndex].Connect(GetDeviceType(playerInput));
            else
                _devicePlayers[playerInput.PlayerIndex].Disconnect();
        }
        
        private void TryContinue()
        {
            if(!_devicePlayers[0].IsReady || !_devicePlayers[1].IsReady) return;
            
            if (PlayersData.PlayerPosition[0] == PlayersData.PlayerPosition[1])
            {
                _devicePlayers[0].OnError();
                _devicePlayers[1].OnError();
                return;
            }
            
            OnMenuClosed();
        }
        
        private Sprite GetDeviceType(PlayerInputReferencesProvider playerInput) => _deviceSprites[DeviceTypeHelper.GetDeviceType(playerInput.PlayerInput.devices[0].displayName)];
    }
}