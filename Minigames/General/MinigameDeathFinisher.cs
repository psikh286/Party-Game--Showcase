using Party.Movement.Interfaces;
using UnityEngine;

namespace Party.Minigames.General
{
    /// <summary>
    /// Finishes minigame when one or more players are died
    /// </summary>
    public class MinigameDeathFinisher : MinigameFinisherBase
    {
        [SerializeField] private GameObject[] _playersObjects;
        private readonly IPlayer[] _players = new IPlayer[2];
        
        private bool _onePlayerDied;
        private int _winnerIndex;

        private void Awake()
        {
            for (var i = 0; i < _playersObjects.Length; i++)
            {
                if (!_playersObjects[i].TryGetComponent(out IPlayer player))
                {
                    LogError("referenced game object doesn't have IPlayer attached");
                    continue;
                }

                _players[i] = player;
            }
        }

        protected void OnEnable()
        {
            _players[0].DeathEvent += OnFirstDeathWrapper;
            _players[1].DeathEvent += OnSecondDeathWrapper;
        }
        private void OnDisable()
        {
            _players[0].DeathEvent -= OnFirstDeathWrapper;
            _players[1].DeathEvent -= OnSecondDeathWrapper;
        }

        private void Update()
        {
            if (!_onePlayerDied) 
                return;
            
            enabled = false;
            OnMinigameFinished(_winnerIndex);
        }

        private void OnDeath(int i)
        {
            if (_onePlayerDied)
            {
                _winnerIndex = 2;
                return;
            }

            _onePlayerDied = true;
            _winnerIndex = i^1;
        }

        private void OnFirstDeathWrapper() => OnDeath(_players[0].Index);
        private void OnSecondDeathWrapper() => OnDeath(_players[1].Index);
    }
}