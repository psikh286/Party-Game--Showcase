using Party.Minigames.General;
using Party.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace Party.Minigames.Spawners
{
    public class InZoneSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SpawnableObject _prefab;
        [SerializeField] private BoxCollider _zone;
        [SerializeField] private MinigameTimer _timer;

        [Header("Settings")]
        [SerializeField] private AnimationCurve _spawnRateCurve;
        [SerializeField] private float _timeToReachFinalRate = 40f;
        [SerializeField] private float _yOffset;
        
        private ObjectPool<SpawnableObject> _pool;
        private float _tickTimer;
        private float _tickDelay;

        private bool _activated;

        private void Awake() => InitializePool();

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
            
            _tickTimer += Time.deltaTime;
            while (_tickTimer >= _tickDelay)
            {
                _tickTimer -= _tickDelay;
                
                OnTick();
            }
        }
        
        private void OnTick()
        {
            _pool.Get();
            
            var timeRatio = Mathf.Clamp01(_timer.TimePassed / _timeToReachFinalRate);
            var newDelay = _spawnRateCurve.Evaluate(timeRatio);
                
            _tickDelay = newDelay;
        }
        
        
        /// <summary>
        /// Used to initialize Object Pool for bombs
        /// </summary>
        private void InitializePool()
        {
            _pool = new ObjectPool<SpawnableObject>(
                () => Instantiate(_prefab, transform.position, Quaternion.identity),
                Init,
                agent => agent.gameObject.SetActive(false), 
                bomb => Destroy(bomb.gameObject),
                false, 15, 15);
            
            return;
            // Initialize a bomb instance
            void Init(SpawnableObject spawnable)
            {
                spawnable.transform.position = GetRandomPosition();
                spawnable.gameObject.SetActive(true);
                spawnable.Init(ReleaseBomb);
            }

            void ReleaseBomb(SpawnableObject spawnable)
            {
                _pool.Release(spawnable);
            }
        }
        
        /// <summary>
        /// Used to get random position within <see cref="_zone"/>
        /// </summary>
        private Vector3 GetRandomPosition()
        {
            var size = _zone.size;
            
            var randomPosition = new Vector3(
                Utility.RandomFloat(-size.x * 0.5f, size.x * 0.5f),
                _yOffset, 
                Utility.RandomFloat(-size.z * 0.5f, size.z * 0.5f));

            return _zone.center + randomPosition;
        } 
    }
}