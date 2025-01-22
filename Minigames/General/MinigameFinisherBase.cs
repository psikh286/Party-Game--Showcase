using System;
using MoreMountains.Tools;
using Party.Debugging;
using UnityEngine;

namespace Party.Minigames.General
{
    /// <summary>
    /// This is a base class for any class that finishes a minigame
    /// </summary>
    public abstract class MinigameFinisherBase : MonoBehaviour
    {
        /// <summary>
        /// Send Winner index
        /// </summary>
        public static event Action<int> MinigameFinishedEvent;

        [SerializeField] private string _destinationSceneName;
        [SerializeField] private string _loadingSceneName;
        [SerializeField] private DebugLogger _logger;
        
        /// <summary>
        /// Used when finished minigame condition is met.
        /// Finishes the minigame.
        /// </summary>
        protected void OnMinigameFinished(int i)
        {
            MinigameFinishedEvent?.Invoke(i);

           MMAdditiveSceneLoadingManager.LoadScene(_destinationSceneName, new MMAdditiveSceneLoadingManagerSettings {LoadingSceneName = _loadingSceneName, UnloadSceneLoader = false});
        }
        
        protected void LogError(object message) => _logger.LogError(message, this);
    }
}