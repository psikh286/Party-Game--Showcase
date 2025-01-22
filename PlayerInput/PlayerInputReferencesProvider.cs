using Party.GUI.DeviceSetup;
using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace Party.PlayerInput
{
    public class PlayerInputReferencesProvider : MonoBehaviour
    {
        public int PlayerIndex => PlayerInput.playerIndex;
        [field: SerializeField] public UnityEngine.InputSystem.PlayerInput PlayerInput { get; private set; }
        [field: SerializeField] public MultiplayerEventSystem EventSystem { get; private set; }
        
        [field: Header("Input Readers")]
        [field: SerializeField] public DeviceSetupInputReader DeviceSetupInputReader { get; private set; }
    }
}
