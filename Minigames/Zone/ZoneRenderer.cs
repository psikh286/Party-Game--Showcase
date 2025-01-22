using UnityEngine;

namespace Party.Minigames.Zone
{
    public class ZoneRenderer : MonoBehaviour
    {
        [SerializeField] private BoxCollider _zoneCollider;


        private void Update()
        {
            transform.localPosition = _zoneCollider.center;
            transform.localScale = _zoneCollider.size;
        }
    }
}