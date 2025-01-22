using Party.Movement.Interfaces;
using UnityEngine;

namespace Party.Minigames.General
{
    /// <summary>
    /// Finishes minigame when player enters a trigger
    /// </summary>
    public class MinigameTriggerFinish : MinigameFinisherBase
    {
        [SerializeField] private bool _canMove;
        [SerializeField] private bool _enterToLose;

        private static bool _finished;

        private void Awake() => _finished = false;

        private void OnTriggerEnter(Collider other)
        {
            if (_finished)
                return;
            
            if (!other.CompareTag("Player"))
                return;

            if (!other.transform.TryGetComponent(out IPlayer player))
            {
                LogError($"Object: {other}. Doesn't have PlayerMovement script attached, but have a tag.");
                return;
            }

            player.CanMove = _canMove;
            _finished = true;
            
            OnMinigameFinished(_enterToLose ? player.Index^1 : player.Index);
        }
    }
}