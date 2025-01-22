using System;
using UnityEngine;

namespace Party.Minigames.Hockey
{
    public class HockeyGate : MonoBehaviour
    {
        [Tooltip("Protector number")]
        [SerializeField] private int _index;

        public static event Action<int> GoalScoredEvent;
        
        private void OnTriggerEnter(Collider other)
        {
            if(!other.CompareTag("Ball"))
                return;

            if (other.attachedRigidbody != null) 
                other.attachedRigidbody.isKinematic = true;

            other.transform.position = Vector3.up;
            
            if (other.attachedRigidbody != null) 
                other.attachedRigidbody.isKinematic = false;
            
            GoalScoredEvent?.Invoke(_index^1);
        }
    }
}