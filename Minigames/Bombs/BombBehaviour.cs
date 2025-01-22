using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Party.Minigames.Bombermen
{
    public class BombBehaviour : MonoBehaviour
    {
        #region References

        [Header("References")]
    
        [SerializeField] private Slider _bombSlider;
        [SerializeField] private Image _fillImage;
    
        [SerializeField] private RectTransform _shadowTransform;
    
        [SerializeField] private GameObject _explosionParticles;

        #endregion

        #region Delays
    
        [Header("Delays")]

        [SerializeField] private float _bombDelay = 8f;
        [SerializeField] private float _bombStartDelay = 2f;

        #endregion

        #region Colors
    
        [Header("Slider Colors")]

        [SerializeField] private Color _startColor;
        [SerializeField] private Color _endColor;

        #endregion

        private float _currentTimer;
        private readonly List<Collider> _overlapPlayers = new();
    
        private void OnEnable()
        {
            _currentTimer = _bombDelay;
        
            StartCoroutine(BombCountdown());
        }
        private void OnDisable()
        {
            Destroy(gameObject);
        }

        private IEnumerator BombCountdown()
        {
            yield return new WaitForSeconds(_bombStartDelay);

            while (_currentTimer > 0)
            {
                _currentTimer -= Time.deltaTime;
                var ratio = _currentTimer / _bombDelay;
            
                _bombSlider.value = ratio;
                _fillImage.color = Color.Lerp(_startColor, _endColor, 1 - ratio);
                yield return null;
            }
        
            ExplodeBomb();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player")) _overlapPlayers.Add(other);
        }
        private void OnTriggerExit(Collider other)
        {
            if (_overlapPlayers.Contains(other)) _overlapPlayers.Remove(other);
        }
        // ReSharper disable Unity.PerformanceAnalysis
        private void ExplodeBomb()
        {
            var currentTransform = transform;
            Instantiate(_explosionParticles, currentTransform.position, currentTransform.rotation);
            Destroy(gameObject);
        
            foreach (var t in _overlapPlayers)
            {
                t.gameObject.GetComponent<BombermenPlayer>().KillPlayer();
            }
        }
    
        private void Update() => SetShadowPosition();
        private void SetShadowPosition()
        {
            if (!_shadowTransform.gameObject.activeSelf) return;

            var position = transform.position;
            var startPosition = new Vector3(position.x, position.y - 1.4f, position.z);
        
            var downRay = new Ray(startPosition, Vector3.down);

            if(!Physics.Raycast(downRay, out var downHit,  50f)) return;
        
            if (downHit.distance > 0.1f)
            {
                _shadowTransform.gameObject.SetActive(true);
                var shadowPosition = _shadowTransform.position;
                shadowPosition = new Vector3(shadowPosition.x, downHit.point.y, shadowPosition.z);
                _shadowTransform.position = shadowPosition;
            }
            else
            {
                _shadowTransform.gameObject.SetActive(false);
            }
        
        }
    }
}
