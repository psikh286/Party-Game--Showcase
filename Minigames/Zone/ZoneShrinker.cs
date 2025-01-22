using Party.CustomGizmos;
using Party.Minigames.General;
using Party.Utilities;
using UnityEngine;

namespace Party.Minigames.Zone
{
    [RequireComponent(typeof(MinigameTimer))]
    public class ZoneShrinker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoxCollider _startCollider;
        [SerializeField] private BoxCollider _finishCollider;
        [SerializeField] private BoxCollider _zoneCollider;
        [SerializeField] private MinigameTimer _timer;
    
        [Header("Settings")]
        [SerializeField] private float _timeToShrink;
        [SerializeField] private CustomGizmosSettings<float> _gizmosSettings;
        
        private bool _activated;
        
        
        private void Awake()
        {
            RandomizeZone();
            InitializeZone();
        }
        
        private void OnEnable()
        {
            MinigameLoader.MinigameStartedEvent += OnMinigameStarted;
            MinigameFinisherBase.MinigameFinishedEvent += OnMinigameFinished;
        }
        private void OnDisable()
        {
            MinigameLoader.MinigameStartedEvent -= OnMinigameStarted;
            MinigameFinisherBase.MinigameFinishedEvent -= OnMinigameFinished;
        }
        
        private void OnMinigameStarted() => _activated = true;
        private void OnMinigameFinished(int _) => _activated = false;
        
        private void Update()
        {
            if(!_activated)
                return;
            
            var ratio = Mathf.Clamp01(_timer.TimePassed / _timeToShrink);
            
            _zoneCollider.size = Vector3.Lerp(_startCollider.size, _finishCollider.size, ratio);
            _zoneCollider.center = Vector3.Lerp(_startCollider.center, _finishCollider.center, ratio);
        }


        private void RandomizeZone()
        {
            var startSize = _startCollider.size;
            var endSize = _finishCollider.size;
            
            _finishCollider.center = new Vector3(
                GetPointInBounds(startSize.x, endSize.x),
                _finishCollider.center.y , 
                GetPointInBounds(startSize.z, endSize.z));
            
            return;

            float GetPointInBounds(float start, float finish)
            {
                var validRange = (start - finish) * 0.5f;
            
                return Utility.RandomFloat(-validRange, validRange);
            }
        }
        
        private void InitializeZone()
        {
            _zoneCollider.isTrigger = true;
            
            _zoneCollider.size = _startCollider.size;
            _zoneCollider.center = _startCollider.center;
        }

        private void OnDrawGizmos()
        {
            if(!_gizmosSettings.DrawGizmos)
                return;

            Gizmos.color = _gizmosSettings.Color;

            if (_gizmosSettings.DrawSolid)
                Gizmos.DrawCube(_zoneCollider.center, _zoneCollider.size);
            else
                Gizmos.DrawWireCube(_zoneCollider.center, _zoneCollider.size);
        }
    }
}