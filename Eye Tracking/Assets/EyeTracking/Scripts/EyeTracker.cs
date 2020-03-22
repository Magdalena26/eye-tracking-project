using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARFace))]
public class EyeTracker : MonoBehaviour
{
    [SerializeField] private GameObject _eyePrefab;

    private GameObject _leftEye;
    private GameObject _rightEye;
    
    private ARFace _arFace;

    private void Awake()
    {
        _arFace = GetComponent<ARFace>();
    }

    private void OnEnable()
    {
        ARFaceManager arFaceManager = FindObjectOfType<ARFaceManager>();
        if (arFaceManager != null && arFaceManager.subsystem != null && arFaceManager.subsystem.SubsystemDescriptor.supportsEyeTracking)
        {
            _arFace.updated += OnFaceUpdated;
        }
        else
        {
            Debug.Log("Eye tracking not supported");
        }
    }

    private void OnDisable()
    {
        _arFace.updated -= OnFaceUpdated;
        SetEyeVisibility(false);
    }

    private void OnFaceUpdated(ARFaceUpdatedEventArgs obj)
    {
        if (_arFace.leftEye != null && _leftEye == null)
        {
            _leftEye = Instantiate(_eyePrefab, _arFace.leftEye);
            _leftEye.SetActive(false);
        }
        
        if (_arFace.rightEye != null && _rightEye == null)
        {
            _rightEye = Instantiate(_eyePrefab, _arFace.rightEye);
            _rightEye.SetActive(false);
        }

        bool isVisible = _arFace.trackingState == TrackingState.Tracking && ARSession.state > ARSessionState.Ready;
        SetEyeVisibility(isVisible);
    }

    private void SetEyeVisibility(bool isVisible)
    {
        if (_rightEye != null) _rightEye.SetActive(isVisible);
        if (_leftEye != null) _leftEye.SetActive(isVisible);
    }
}