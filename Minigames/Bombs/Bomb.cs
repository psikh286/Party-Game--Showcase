using Party.CustomGizmos;
using Party.Minigames.Spawners;
using UnityEngine;

namespace Party.Minigames.Bombs
{
    /// <summary>
    /// Base generic class for a bomb, where T should be an interface
    /// </summary>
    public abstract class Bomb<T> : SpawnableObject
    {
        [SerializeField] private float _radius;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private CustomGizmosSettings<float> _gizmosSettings;

        private readonly Collider[] _overlapColliders = new Collider[10];
        
        /// <summary>
        /// Used when bomb is exploded.
        /// Calls OnExplosion for each collider with T attached
        /// </summary>
        protected void Explode()
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, _radius, _overlapColliders, _layerMask);

            for (var i = 0; i < size; i++)
            {
                if(!_overlapColliders[i].TryGetComponent(out T component))
                    continue;
                
                OnExplosion(component);
            }
            
            _releaseAction.Invoke(this);
        }

        /// <summary>
        /// Used when bomb is exploded and should contain custom functionality for each bomb
        /// </summary>
        protected abstract void OnExplosion(T target);

        private void OnDrawGizmosSelected()
        {
            _gizmosSettings.Size = _radius;
            
            if(!_gizmosSettings.DrawGizmos)
                return;
            
            Gizmos.color = _gizmosSettings.Color;

            if (_gizmosSettings.DrawSolid)
                Gizmos.DrawSphere(transform.position, _radius);
            else
                Gizmos.DrawWireSphere(transform.position, _radius);
        }

    }
}