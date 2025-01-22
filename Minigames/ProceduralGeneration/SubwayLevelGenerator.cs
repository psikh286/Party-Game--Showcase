using UnityEngine;

namespace Party.Minigames.ProceduralGeneration
{
    public class SubwayLevelGenerator : MonoBehaviour
    {
        [System.Serializable]
        private struct RowData
        {
            [field: SerializeField] public Vector3 StartPoint { get; private set; }
            [field: SerializeField] public Vector3 EndPoint { get; private set; }
        }

        [SerializeField] private RowData[] _rows;

        private void OnDrawGizmosSelected()
        {
            Color[] colors = {Color.blue, Color.white, Color.green };
            
            for (var i = 0; i < _rows.Length; i++)
            {
                var row = _rows[i];
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(row.StartPoint, 0.2f);
                
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(row.EndPoint, 0.2f);
                
                Gizmos.color = colors[i % 3];
                
                var center = (row.StartPoint + row.EndPoint) * 0.5f;
                var size = new Vector3(
                    Mathf.Abs(row.StartPoint.x - row.EndPoint.x),
                    Mathf.Abs(row.StartPoint.y - row.EndPoint.y),
                    Mathf.Abs(row.StartPoint.z - row.EndPoint.z));

                Gizmos.DrawCube(center, size);
            }
        }
    }
}