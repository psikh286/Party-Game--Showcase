using UnityEngine;
using UnityEngine.Localization;

namespace Party.GUI.LoadingScene
{
    [CreateAssetMenu(menuName = "Party/LoadingScene/Loading Scene Data")]
    public class LoadingScreenData : ScriptableObject
    {
        [field: SerializeField] public LocalizedString Title;
        [field: SerializeField] public LocalizedString Information;
    }
}