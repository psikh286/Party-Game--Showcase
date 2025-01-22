using System;
using Cinemachine;
using UnityEngine;

namespace Party.Cameras
{
    public class SplitScreenManager : MonoBehaviour
    {
        public static event Action<Camera[]> CameraChangedEvent;
        
        [Header("Dependencies")]
        [SerializeField] private GameObject _sharedCamParent;
        [SerializeField] private GameObject _splitCamsParent;
        [SerializeField] private Camera _sharedCamera;
        [SerializeField] private Camera[] _splitCameras;
        [SerializeField] private CinemachineVirtualCamera[] _splitVirtualCameras;
        [SerializeField] private CinemachineTargetGroup _targetGroup;
        [SerializeField] private Transform[] _players;
        [SerializeField] private RectTransform _splitImageRect;
        
        [Header("Settings")]
        [SerializeField] private float _maxDistance;
        [SerializeField] private bool _horizontal;

        private bool _wasNearLastFrame;

        private void Awake()
        {
            for (var i = 0; i < 2; i++)
            {
                _targetGroup.m_Targets[i] = new CinemachineTargetGroup.Target { target = _players[i], radius = 0f, weight = 1f};
                _splitVirtualCameras[i].Follow = _players[i];
            }
           
            if (_horizontal)
            {
                _splitCameras[0].rect = new Rect(0f, 0.5f, 1f, 0.5f);
                _splitCameras[1].rect = new Rect(0f, 0f, 1f, 0.5f);
                _splitImageRect.sizeDelta = new Vector2(1920f, 4f);
            }
            else
            {
                _splitCameras[0].rect = new Rect(-0.5f, 0f, 1f, 1f);
                _splitCameras[1].rect = new Rect(0.5f, 0f, 1f, 1f);
                _splitImageRect.sizeDelta = new Vector2(4f, 1080f);
            }
        }

        private void Update()
        {
            var near = Vector3.Distance(_players[0].position, _players[1].position) < _maxDistance;
            
            if (_wasNearLastFrame == near)
                return;

            _wasNearLastFrame = near;
            
            CameraChangedEvent?.Invoke(near?new []{_sharedCamera}:_splitCameras);
            
            _sharedCamParent.SetActive(near);
            
            _splitCamsParent.SetActive(!near);
        }
    }
}