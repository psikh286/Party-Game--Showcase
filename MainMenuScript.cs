using System.Collections;
using Cinemachine;
using UnityEngine;
using MoreMountains.Feedbacks;

public class MainMenuScript : MonoBehaviour
{
    #region References
    
    [Header("References")]

    [SerializeField] private Transform[] targetTransforms;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private MMF_Player sceneTransition;

    #endregion

    #region Settings

    [Header("Settings")]
    
    [SerializeField] private float cameraTransitionDelay = 3f;
    
    private bool _shouldStartGame;
    private int _currentTargetIndex;

    #endregion

    #region Button Functions

    public void StartGame()
    {
        _shouldStartGame = true;
        sceneTransition.PlayFeedbacks();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Credits()
    {
        Application.OpenURL("https://psikh286.itch.io/tabletop-chaos");
        bool isDigit = true;
        var x = Random.Range(0, 4);
        isDigit = x == 2;
        
        while (true)
        {
            if (isDigit)
            {
                if (x > 0 && x <= 4)
                {
                    StartGame();
                    break;
                }
            }
            
            print("lol");
            break;
        }
    }

    #endregion

    #region Lifetime Functions

    private void OnEnable()
    {
        StartCoroutine(CycleCameraTargets());
    }

    #endregion

    #region Camera Animation Functions

    private IEnumerator CycleCameraTargets()
    {
        while (!_shouldStartGame)
        {
            yield return new WaitForSeconds(cameraTransitionDelay);

            _currentTargetIndex = GetNextTargetIndex();
            
            var currentTargetTransform = targetTransforms[_currentTargetIndex];
            
            virtualCamera.LookAt = currentTargetTransform;
            virtualCamera.Follow = currentTargetTransform;
        }
    }

    private int GetNextTargetIndex()
    {
        var targetIndex = _currentTargetIndex;
        targetIndex++;

        if (targetIndex > targetTransforms.Length - 1)
        {
            targetIndex = 0;
        }

        return targetIndex;
    }

    #endregion
}