using Party.Minigames.General;
using Party.Movement;
using Party.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Party.Minigames.RLGL
{
    // ReSharper disable once InconsistentNaming
    public class RLGLController : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private PlayerMovement[] _players;
        [SerializeField] private MeshRenderer[] _tiles;
        [SerializeField] private Transform _leadTransform;
        [SerializeField] private Image _leadImage;

        [Header("Settings")] 
        [SerializeField] private Vector2[] _minMaxDurations;
        [SerializeField] private Color[] _matColors;
        [SerializeField] private float _knockBackForce;
        [SerializeField] private Vector3 _knockBackDir;

        private float _tickTimer;
        private float _tickDelay;
        private int _tickCount;
        
        private MaterialPropertyBlock[] _materialBlocks;
        private static readonly int _baseColor = Shader.PropertyToID("_BaseColor");

        private float _leadInitialZ;

        private void Awake()
        {
            MinigameLoader.MinigameStartedEvent += OnMinigameStarted;
            MinigameFinisherBase.MinigameFinishedEvent += OnMinigameFinished;

            _materialBlocks = new []{new MaterialPropertyBlock(), new MaterialPropertyBlock(), new MaterialPropertyBlock()};
            
            for (var i = 0; i < _materialBlocks.Length; i++)
                _materialBlocks[i].SetColor(_baseColor, _matColors[i]);
            
            _tickDelay = Utility.RandomFloat(_minMaxDurations[0].x, _minMaxDurations[0].y);
            UpdateMaterial(0);

            _leadInitialZ = _leadTransform.position.z;
            
            enabled = false;
        }

        private void OnDestroy()
        {
            MinigameLoader.MinigameStartedEvent -= OnMinigameStarted;
            MinigameFinisherBase.MinigameFinishedEvent -= OnMinigameFinished;
        }

        private void Update()
        {
            var currentColorIndex = _tickCount % 3;

            if (currentColorIndex == 2) 
                CheckKnockBack();

            CheckRunningGap();
            
            _tickTimer += Time.deltaTime;
            while (_tickTimer >= _tickDelay)
            {
                _tickTimer -= _tickDelay;
                
                _tickCount++;
                currentColorIndex = _tickCount % 3;
                
                UpdateMaterial(currentColorIndex);
                
                _tickDelay = Utility.RandomFloat(_minMaxDurations[currentColorIndex].x, _minMaxDurations[currentColorIndex].y);

                if (currentColorIndex == 0)
                {
                    _players[0].CanMove = true;
                    _players[1].CanMove = true;
                }
            }
        }

        private void CheckRunningGap()
        {
            var difference = _players[0].transform.position.z - _players[1].transform.position.z;
            var loserIndex = difference > 0f ? 1 : 0;
            var winnerIndex = (loserIndex + 1) % 2;

            difference = Mathf.Abs(difference);
            
            _players[loserIndex].SpeedFactor = Mathf.Clamp(difference, 1f , 1.4f);
            _players[winnerIndex].SpeedFactor = 1f;
            
            UpdateLeadingIndicator(winnerIndex, _players[winnerIndex].transform.position.z);
        }
        
        private void UpdateLeadingIndicator(int winnerIndex, float position)
        {
            var currentPosition = _leadTransform.position;
            _leadTransform.position = new Vector3(currentPosition.x, currentPosition.y, Mathf.Clamp(position + 1f, _leadInitialZ, position + 1f));
            _leadImage.color = winnerIndex == 0 ? Color.red : Color.blue;
        }
        
        private void CheckKnockBack()
        {
            for (int i = 0; i < 2; i++)
            {
                if (_players[i].SpeedRatio == 0f || !_players[i].CanMove)
                    continue;
                
                _players[i].CanMove = false;
                _players[i].OnHit(_knockBackDir, _knockBackForce, false);
            }
        }

        private void OnMinigameFinished(int playerIndex) => enabled = false;

        private void OnMinigameStarted() => enabled = true;

        private void UpdateMaterial(int currentColorIndex)
        {
            foreach (var tile in _tiles) 
                tile.SetPropertyBlock(_materialBlocks[currentColorIndex]);
        }
    }
}