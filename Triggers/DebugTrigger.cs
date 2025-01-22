using System;
using Party.Debugging;
using UnityEngine;

namespace Party.Triggers
{
    public class DebugTrigger : MonoBehaviour
    {
        [SerializeField] private DebugLogger _logger;
        [SerializeField] private string _tag;

        private void OnTriggerEnter(Collider other)
        {
            if (_logger && other.CompareTag(_tag)) _logger.Log("On Trigger", this);
        }

        private void OnTriggerExit(Collider other)
        {
            if (_logger && other.CompareTag(_tag)) _logger.Log("Not On Trigger", this);
        }
    }
}
