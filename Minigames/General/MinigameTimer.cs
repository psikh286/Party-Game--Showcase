using UnityEngine;

namespace Party.Minigames.General
{
    public class MinigameTimer : MonoBehaviour
    {
        public float TimePassed { get; private set; }

        private bool _activated;

        private void OnEnable()
        {
            MinigameLoader.MinigameStartedEvent += StartTimer;
            MinigameFinisherBase.MinigameFinishedEvent += StopTimer;
        }
        private void OnDisable()
        {
            MinigameLoader.MinigameStartedEvent -= StartTimer;
            MinigameFinisherBase.MinigameFinishedEvent -= StopTimer;
        }

        private void StartTimer() => _activated = true;
        private void StopTimer(int _) => _activated = false;

        private void Update()
        {
            if(!_activated)
                return;
            
            TimePassed += Time.deltaTime;
        }
    }
}