using Party.Essentials;

namespace Party.GUI.Menu
{
    public class MenuManager
    {
        public static MenuManager Current => GameManager.Instance.ReferenceProvider.MenuManager;

        public bool IsMenuActive(MenuID id)
        {
            return true;
        }
    }
}