using System;
using MoreMountains.Tools;
using Party.Debugging;
using Party.PlayerInput;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Party.GUI.Menu
{
    public abstract class BaseMenu : MonoBehaviour
    {
        public static event Action<MenuID> MenuOpenedEvent;
        public static event Action<MenuID> MenuClosedEvent;

        protected abstract MenuID _menuID { get; }

        [Header("Debugging")]
        [SerializeField] private DebugLogger _logger;
        [SerializeField] [MMInspectorButton("OnMenuOpen")] private bool _openMenu;
        [SerializeField] [MMInspectorButton("OnMenuClosed")] private bool _closeMenu;
        
        [Header("Settings")]
        [SerializeField] protected GameObject _menuUi;
        [SerializeField] protected bool _openOnStart;
        
        
        protected virtual void Awake() => PlayerInputInstancesTracker.PlayerConnectionChangedEvent += OnPlayerConnectionChanged;
        protected virtual void OnDestroy() => PlayerInputInstancesTracker.PlayerConnectionChangedEvent -= OnPlayerConnectionChanged;
        protected void OnDisable() => OnMenuClosed();

        private void Start()
        {
            if(_openOnStart)
                OnMenuOpen();
        }

        protected virtual void OnMenuOpen()
        {
            _menuUi.SetActive(true);
            
            MenuOpenedEvent?.Invoke(_menuID);
        }
        protected virtual void OnMenuClosed()
        {
            _menuUi.SetActive(false);
            
            MenuClosedEvent?.Invoke(_menuID);
        }

        protected void Log(object message)
        {
            if(_logger) 
                _logger.Log(message, this);
        }
        protected void LogError(object message)
        {
            if(_logger) 
                _logger.LogError(message, this);
        }

        protected abstract void OnPlayerConnectionChanged(bool isConnected, PlayerInputReferencesProvider playerInput);
    }
}