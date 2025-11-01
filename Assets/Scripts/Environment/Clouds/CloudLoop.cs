using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CloudLoop : MonoBehaviour
{
    [System.Serializable]
    public class Cloud
    {
        public Transform transform;
        [HideInInspector] public float speed;
        [HideInInspector] public float yOffsetFromCamera;
    }

    #region variables

    [Header("References")]
    [SerializeField] private Cloud[] _clouds;
    [SerializeField] private Camera _mainCamera;

    [Header("CloudLoop Properties")]
    [SerializeField] private Vector2 _speedRange = new Vector2(0.05f, 0.15f);
    [SerializeField] private Vector2 _yOffsetRange = new Vector2(-0.3f, 0.3f);
    [Tooltip("How far offscreen before respawn")]
    [SerializeField] private float _offscreenBuffer = 5f;

    [Header("Vertical Smoothing")]
    [Tooltip("How smoothly clouds ajust to the vertical camera movement.")]
    [SerializeField, Range(0.1f, 0.5f)] private float _verticalSmoothSpeed = 2f; 

    private float _leftBoundary;
    private float _rightBoundary;

    #endregion

    #region Start
    private void Start()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        //cache each cloud's initial Y position and random speed
        foreach (var cloud in _clouds)
        {
            cloud.speed = Random.Range(_speedRange.x, _speedRange.y);
            cloud.yOffsetFromCamera = cloud.transform.position.y - _mainCamera.transform.position.y;
        }
    }
    #endregion

    #region FixedUpdate
    private void FixedUpdate()
    {
        //update the camera bounds each frame (if camera is moving)
        Vector3 leftEdge = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0.5f, Mathf.Abs(_mainCamera.transform.position.z)));
        Vector3 rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, Mathf.Abs(_mainCamera.transform.position.z)));
        _leftBoundary = leftEdge.x - _offscreenBuffer;
        _rightBoundary = rightEdge.x + _offscreenBuffer;

        //smoothly apparoach targetY offset
        foreach (var cloud in _clouds)
        {
            //move cloud horizontally to the left
            cloud.transform.position += Vector3.left * cloud.speed * Time.deltaTime;

            float targetY = _mainCamera.transform.position.y + cloud.yOffsetFromCamera;

            //smoothly apply vertical offset with camera movement
            Vector3 pos = cloud.transform.position;
            pos.y = Mathf.Lerp(pos.y, targetY, Time.deltaTime * _verticalSmoothSpeed);
            cloud.transform.position = pos; 

            #region Cloud Horizontal Movement Looping
            //if cloud moves past the left boundary, loop back to the right
            if (cloud.transform.position.x < _leftBoundary)
            {
                float newYOffset = cloud.yOffsetFromCamera + Random.Range(_yOffsetRange.x, _yOffsetRange.y);
                cloud.yOffsetFromCamera = newYOffset;
                cloud.transform.position = new Vector3(_rightBoundary, _mainCamera.transform.position.y + newYOffset, cloud.transform.position.z);
                cloud.speed = Random.Range(_speedRange.x, _speedRange.y);
            }
            else if (cloud.transform.position.x > _rightBoundary)
            {
                float newYOffset = cloud.yOffsetFromCamera + Random.Range(_yOffsetRange.x, _yOffsetRange.y);
                cloud.yOffsetFromCamera = newYOffset;
                cloud.transform.position = new Vector3(_leftBoundary, _mainCamera.transform.position.y + newYOffset, cloud.transform.position.z);
                cloud.speed = Random.Range(_speedRange.x, _speedRange.y);
            }

            #endregion


        }
    }
    #endregion
}
