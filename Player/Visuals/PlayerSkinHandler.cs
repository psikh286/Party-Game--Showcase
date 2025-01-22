using System;
using UnityEngine;

namespace Party.Player.Visuals
{
    public class PlayerSkinHandler : MonoBehaviour
    {
        public event Action<Animator> SkinSpawnedEvent;
            
        [SerializeField] private int _playerIndex;
        
        private void Start()
        {
            var skinPrefab = Resources.Load<PlayerSkinListPreset>("Skin List").PlayerSkins[PlayerVisualData.SkinIndex[_playerIndex][0]];

            var skin = Instantiate(skinPrefab, transform);
            
            SkinSpawnedEvent?.Invoke(skin);
        }
    }
}