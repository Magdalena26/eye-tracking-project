using EyeTracking.Scripts;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

#if UNITY_IOS && !UNITY_EDITOR
using UnityEngine.XR.ARKit;
#endif

/// <summary>
/// Visualizes the eye poses for an <see cref="ARFace"/>.
/// </summary>
/// <remarks>
/// Face space is the space where the origin is the transform of an <see cref="ARFace"/>.
/// </remarks>
[RequireComponent(typeof(ARFace))]
public class EyePoseVisualizer : MonoBehaviour
{
    private GameObject _leftEyeGameObject;
    private GameObject _rightEyeGameObject;
    private ARFace _face;
    private ObjectSelector _objectSelector;
    private UIObject _currentlySelected;

    private void Awake()
    {
        _face = GetComponent<ARFace>();
        _objectSelector = FindObjectOfType<ObjectSelector>();
    }

    private void OnEnable()
    {
        var faceManager = FindObjectOfType<ARFaceManager>();
        if (faceManager != null && faceManager.subsystem != null &&
            faceManager.subsystem.SubsystemDescriptor.supportsEyeTracking)
        {
            SetVisible((_face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready));
            _face.updated += OnUpdated;
        }
        else
        {
            enabled = false;
        }
    }

    private void OnDisable()
    {
        _face.updated -= OnUpdated;
        SetVisible(false);
    }

    private void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
    {
        CreateEyeGameObjectsIfNecessary();
        SetVisible((_face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready));
    }

    private void CreateEyeGameObjectsIfNecessary()
    {
        if (_objectSelector.GetSelectedObject() != null)
        {
            DestroyPreviouslySelectedObjects();

            _currentlySelected = _objectSelector.GetSelectedObject();

            var eyeObject = _currentlySelected.ObjectToSpawn();
            if (_face.leftEye != null && _leftEyeGameObject == null)
            {
                _leftEyeGameObject = Instantiate(eyeObject, _face.leftEye);
                _leftEyeGameObject.SetActive(false);
            }

            if (_face.rightEye != null && _rightEyeGameObject == null && _objectSelector.GetSelectedObject() != null)
            {
                _rightEyeGameObject = Instantiate(eyeObject, _face.rightEye);
                _rightEyeGameObject.SetActive(false);
            }
        }
    }

    private void DestroyPreviouslySelectedObjects()
    {
        if (_currentlySelected != null && _currentlySelected.Name == _objectSelector.GetSelectedObject().Name)
        {
            if (_leftEyeGameObject != null)
            {
                Destroy(_leftEyeGameObject);
                _leftEyeGameObject = null;
            }

            if (_rightEyeGameObject != null)
            {
                Destroy(_rightEyeGameObject);
                _rightEyeGameObject = null;
            }
        }
    }

    private void SetVisible(bool visible)
    {
        if (_leftEyeGameObject != null && _rightEyeGameObject != null)
        {
            _leftEyeGameObject.SetActive(visible);
            _rightEyeGameObject.SetActive(visible);
        }
    }
}