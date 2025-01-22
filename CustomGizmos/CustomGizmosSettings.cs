using UnityEngine;

namespace Party.CustomGizmos
{
    [System.Serializable]
    public class CustomGizmosSettings<T>
    {
        public bool DrawGizmos;
        public bool DrawSolid;
        public T Size;
        public Color Color;
    }
}