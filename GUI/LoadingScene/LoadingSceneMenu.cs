using MoreMountains.Tools;
using Party.GUI.Menu;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Party.GUI.LoadingScene
{
    public class LoadingSceneMenu : MultiplayerMenu
    {
        protected override MenuID _menuID => MenuID.LoadingScreenMenu;
        
        [Header("Dependencies")]
        [SerializeField] private MMAdditiveSceneLoadingManager _mmAdditiveSceneLoadingManager;
        [SerializeField] private Toggle[] _toggles;
        
        [Header("Data")]
        [SerializeField] private MMSerializableDictionary<string, LoadingScreenData> _loadingData;

        [Header("References")]
        [SerializeField] private LocalizeStringEvent _titleText;
        [SerializeField] private LocalizeStringEvent _informationText;
        
        private readonly bool[] _confirmed = new bool[2];


        protected override void Awake()
        {
            base.Awake();
            
            for (var i = 0; i < _toggles.Length; i++)
            {
                var index = i;
                _toggles[i].onValueChanged.AddListener(_ => OnConfirm(index));
            }

            if (!_loadingData.TryGetValue(_mmAdditiveSceneLoadingManager.DestinationSceneName, out var data))
            {
                LogError($"doesn't contain scene named {_mmAdditiveSceneLoadingManager.DestinationSceneName}");
                return;
            }

            _titleText.StringReference = data.Title;
            _informationText.StringReference = data.Information;
        }

        protected override void OnDestroy()
        {
            foreach (var toggle in _toggles) 
                toggle.onValueChanged.RemoveAllListeners();
        }

        private void OnConfirm(int i)
        {
            _confirmed[i] = !_confirmed[i];

            if (_confirmed[0] && _confirmed[1])
                _mmAdditiveSceneLoadingManager.SceneReadyToUnload = true;
        }
    }
}