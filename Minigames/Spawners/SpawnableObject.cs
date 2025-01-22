using System;
using UnityEngine;

namespace Party.Minigames.Spawners
{
    public abstract class SpawnableObject : MonoBehaviour
    {
        protected Action<SpawnableObject> _releaseAction;
        
        public virtual void Init(Action<SpawnableObject> releaseAction)
        {
            _releaseAction = releaseAction;
        }
    }
}