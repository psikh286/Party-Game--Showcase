using UnityEngine;

namespace Party.Debugging
{
    [CreateAssetMenu(fileName = "DebugLogger", menuName = "Party/Debug/Logger")]
    public class DebugLogger : ScriptableObject
    {
        [SerializeField] private bool _isActive;
        
        [Header("Prefix")]
        [SerializeField] private string _prefixMessage;
        [SerializeField] private Color _prefixColor;

        private string _hexColor;

        private void OnValidate()
        {
            _hexColor = "#" + ColorUtility.ToHtmlStringRGBA(_prefixColor);
        }

        public void Log(object message, Object sender)
        {
            if(_isActive)
                Debug.Log($"<color={_hexColor}>{_prefixMessage}</color> " + $"[{sender.GetType().Name}] " + message, sender);
        }
        
        public void LogError(object message, Object sender)
        {
            if(_isActive)
                Debug.Log($"<color={_hexColor}>{_prefixMessage}</color> " + $"[{sender.GetType().Name}] " + message, sender);
        }
    }
}