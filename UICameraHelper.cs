using UnityEngine;

public class UICameraHelper : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] private Camera mainCamera;

    private bool _isCameraOrtho;

    private void Start()
    {
        _camera = GetComponent<Camera>();
        _isCameraOrtho = _camera.orthographic;
    }
    
    void Update()
    {
        if (_isCameraOrtho)
        {
            _camera.orthographicSize = mainCamera.orthographicSize;
        }
        else
        {
            _camera.fieldOfView = mainCamera.fieldOfView;
        }
        
    }
}
