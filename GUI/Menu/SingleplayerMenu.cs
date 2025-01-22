using Party.PlayerInput;

namespace Party.GUI.Menu
{
    public abstract class SingleplayerMenu : BaseMenu
    {
        //Only 1 person can interact with it
        
        protected override void OnPlayerConnectionChanged(bool isConnected, PlayerInputReferencesProvider playerInput)
        {
        }
    }
}