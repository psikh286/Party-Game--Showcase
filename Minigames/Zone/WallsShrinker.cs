using Party.CustomGizmos;
using Party.Minigames.General;
using UnityEngine;

namespace Party.Minigames.Zone
{
    [RequireComponent(typeof(MinigameTimer))]
    public class WallsShrinker : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BoxCollider _startCollider;
        [SerializeField] private BoxCollider _finishCollider;
        [SerializeField] private MinigameTimer _timer;
        
        [Header("Settings")]
        [SerializeField] private float _timeToShrink;
        [SerializeField] private CustomGizmosSettings<float> _gizmosSettings;
        
    
        private readonly BoxCollider[] _walls = new BoxCollider[4];
        private readonly Vector3[] _wallEndPositions = new Vector3[4];
        private readonly Vector3[] _wallStartPositions = new Vector3[4];
        private bool _activated;
        
        
        private void Start() => InstantiateWalls();

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

            var ratio = _timer.TimePassed / _timeToShrink;
            
            for (var i = 0; i < 4; i++)
                _walls[i].center = Vector3.Lerp(_wallStartPositions[i], _wallEndPositions[i], ratio);
        }
        
        
        private void InstantiateWalls()
        {
            var startingSize = _startCollider.size;
            var startingCenter = _startCollider.center;
        
            for (var i = 0; i < 4; i++)
            {
                var boxCollider = gameObject.AddComponent<BoxCollider>();
                _walls[i] = boxCollider;
                
                boxCollider.size = DesiredSize(startingSize);
                var startCenter = DesiredCenter(startingSize, startingCenter);
                
                boxCollider.center = startCenter;
                _wallStartPositions[i] = startCenter;
            
                _wallEndPositions[i] = DesiredCenter(_finishCollider.size, _finishCollider.center);
                continue;
                
                
                Vector3 DesiredSize(Vector3 size)
                {
                    var x = i > 1 ? size.x : 1;
                    var y = _startCollider.size.y;
                    var z = i > 1 ? 1 : size.x;
                
                    return new Vector3(x,y,z);
                }
                
                Vector3 DesiredCenter(Vector3 size, Vector3 center)
                {
                    var x = i > 1
                        ? center.x
                        : center.x + (2f * (i % 2) - 1f) * (size.x * 0.5f + 0.5f);
                    
                    var z = i < 2
                        ? center.z
                        : center.z + (2f * (i % 2) - 1f) * (size.z * 0.5f + 0.5f);
                
                    return new Vector3(x, center.y, z);
                }
            }
        }
    
        private void OnDrawGizmos()
        {
            if(!_gizmosSettings.DrawGizmos) 
                return;
        
            Gizmos.color = _gizmosSettings.Color;
            foreach (var wall in _walls)
            {
                if(wall) Gizmos.DrawCube(transform.TransformPoint(wall.center), wall.size);
            }
        }
    }
}