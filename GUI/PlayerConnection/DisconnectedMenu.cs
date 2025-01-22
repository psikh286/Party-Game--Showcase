using Party.GUI.Menu;
using Party.PlayerInput;
using UnityEngine;

namespace Party.GUI.PlayerConnection
{
    public class DisconnectedMenu : BaseMenu
    {
        [SerializeField] private GameObject[] _disconnectedWindows;
        
        private void Start()
        {
            _disconnectedWindows[0].SetActive(false);
            _disconnectedWindows[1].SetActive(false);
        }

        protected override MenuID _menuID => MenuID.DisconnectedMenu;

        protected override void OnPlayerConnectionChanged(bool isConnected, PlayerInputReferencesProvider playerInput)
        {
            if (MenuManager.Current.IsMenuActive(MenuID.DeviceSetupMenu)) isConnected = true;
            
            _disconnectedWindows[playerInput.PlayerIndex].SetActive(!isConnected);
            
            _menuUi.SetActive(_disconnectedWindows[0].activeSelf || _disconnectedWindows[1].activeSelf);
        }
    }
}