using System;
using System.Linq;
using DG.Tweening;
using MoreMountains.Feedbacks;
using Party.Debugging;
using Party.Player;
using Party.PlayerInput;
using UnityEngine;
using UnityEngine.UI;

namespace Party.GUI.DeviceSetup
{
    [RequireComponent(typeof(Image))]
    public class DeviceSetupPlayer : MonoBehaviour
    {
        public static event Action ReadyEvent;
        
        public bool IsReady { get; private set; }

        [SerializeField] private DebugLogger _logger;
        [SerializeField] private int _index;
        
        [SerializeField] private Sprite _disconnectedDeviceIcon;
        
        [SerializeField] private MMF_Player _errorFeedback;

        [SerializeField] private MMF_Player _readyFeedback;
        [SerializeField] private MMF_Player _unreadyFeedback;
        
        [SerializeField] private MMF_Player _connectFeedback;
        [SerializeField] private MMF_Player _disconnectFeedback;
        
        
        private Image _image;
        private PlayerInputReferencesProvider _playerInput;

        private bool _isMoving;
        
        
        private void Awake() => _image = GetComponent<Image>();
        private void OnDestroy() => Unsubscribe();
        
        private void Subscribe()
        {
            _playerInput = PlayerInputInstancesTracker.Current.Players.FirstOrDefault(r => r.PlayerIndex == _index);

            if (_playerInput == null)
            {
                _logger.LogError($"Tried to get player from PlayerInstancesTracker, but it's null" ,this);
                return;
            }
            
            _playerInput.DeviceSetupInputReader.MoveEvent += OnMove;
            _playerInput.DeviceSetupInputReader.SubmitEvent += OnSubmit;
            _playerInput.DeviceSetupInputReader.DeleteEvent += OnDelete;
        }
        private void Unsubscribe()
        {
            if(!_playerInput) return;
            
            _playerInput.DeviceSetupInputReader.MoveEvent -= OnMove;
            _playerInput.DeviceSetupInputReader.SubmitEvent -= OnSubmit;
            _playerInput.DeviceSetupInputReader.DeleteEvent -= OnDelete;
        }
        
        public void Connect(Sprite deviceIcon)
        {
            Subscribe();
            OnMove(PlayersData.PlayerPosition[_index]);
            
            _image.sprite = deviceIcon;
            _connectFeedback.PlayFeedbacks();
        }
        public void Disconnect()
        {
            Unsubscribe();
            
            PlayersData.PlayerPosition[_index] = 0;
            IsReady = false;
            
            _image.sprite = _disconnectedDeviceIcon;
            _unreadyFeedback.PlayFeedbacks();
            _disconnectFeedback.PlayFeedbacks();
            
            _image.rectTransform.DOKill();
            _isMoving = true;
            _image.rectTransform.DOLocalMoveX(0, 0.5f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => _isMoving = false);
        }
        
        private void OnMove(int value)
        {
            if(_isMoving || IsReady || PlayersData.PlayerPosition[_index] == value) return;
            _isMoving = true;
            
            PlayersData.PlayerPosition[_index] = value;
            
            _image.rectTransform
                .DOLocalMoveX(value * 600f, 0.5f)
                .SetEase(Ease.Linear)
                .OnComplete(() => _isMoving = false);
        }
        private void OnDelete()
        {
            Disconnect();
            
            PlayerInputInstancesTracker.Current.DisconnectPlayer(_playerInput);
        }
        
        public void OnSubmit()
        {
            if(_isMoving || PlayersData.PlayerPosition[_index] == 0) return;
            
            IsReady = !IsReady;
            
            (IsReady ? _readyFeedback : _unreadyFeedback).PlayFeedbacks();
            
            ReadyEvent?.Invoke();
        }

        public void OnError() => _errorFeedback.PlayFeedbacks();
    }
}