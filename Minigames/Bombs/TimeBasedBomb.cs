using System;
using Party.Minigames.Spawners;
using UnityEngine;

namespace Party.Minigames.Bombs
{
    /// <summary>
    /// Bomb that explodes after certain amount of time
    /// </summary>
    public abstract class TimeBasedBomb<T> : Bomb<T>
    {
        [SerializeField] private float _timeBeforeExplosion;

        private float _timePassed;

        public override void Init(Action<SpawnableObject> releaseAction)
        {
            base.Init(releaseAction);

            _timePassed = 0f;
        }

        private void Update()
        {
            _timePassed += Time.deltaTime;
            
            if(_timePassed < _timeBeforeExplosion)
                return;

            enabled = false;
            Explode();
        }
    }
}