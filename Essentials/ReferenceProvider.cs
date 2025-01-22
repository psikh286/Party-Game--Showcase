using System;
using Party.GUI.Menu;
using Party.Input;
using Party.PlayerInput;
using UnityEngine;

namespace Party.Essentials
{
    [Serializable]
    public class ReferenceProvider
    {
        [field: SerializeField] public PlayerInputInstancesTracker PlayerInputInstancesTracker { get; private set; }
        [field: SerializeField] public InputManager InputManager { get; private set; }
        
        public MenuManager MenuManager = new();
    }
}
