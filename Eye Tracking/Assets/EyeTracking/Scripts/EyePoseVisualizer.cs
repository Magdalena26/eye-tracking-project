using System.Collections;
using System.Collections.Generic;
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
    GameObject m_LeftEyeGameObject;
    GameObject m_RightEyeGameObject;

    ARFace m_Face;
    private ObjectSelector _objectSelector;
    private UIObject _currentlySelected;

    void Awake()
    {
        m_Face = GetComponent<ARFace>();
        _objectSelector = FindObjectOfType<ObjectSelector>();
    }

    void CreateEyeGameObjectsIfNecessary()
    {
        if (_objectSelector.GetSelectedObject() != null)
        {
            DestroyPreviouslySelectedObjects();
            
            _currentlySelected = _objectSelector.GetSelectedObject();

            var eyeObject = _currentlySelected.ObjectToSpawn();
            if (m_Face.leftEye != null && m_LeftEyeGameObject == null)
            {
                m_LeftEyeGameObject = Instantiate(eyeObject, m_Face.leftEye);
                m_LeftEyeGameObject.SetActive(false);
            }

            if (m_Face.rightEye != null && m_RightEyeGameObject == null && _objectSelector.GetSelectedObject() != null)
            {
                m_RightEyeGameObject = Instantiate(eyeObject, m_Face.rightEye);
                m_RightEyeGameObject.SetActive(false);
            }
        }
    }

    private void DestroyPreviouslySelectedObjects()
    {
        if (_currentlySelected != null && _currentlySelected.Name == _objectSelector.GetSelectedObject().Name)
        {
            if (m_LeftEyeGameObject != null)
            {
                Destroy(m_LeftEyeGameObject);
                m_LeftEyeGameObject = null;
            }

            if (m_RightEyeGameObject != null)
            {
                Destroy(m_RightEyeGameObject);
                m_RightEyeGameObject = null;
            }
        }
    }

    void SetVisible(bool visible)
    {
        if (m_LeftEyeGameObject != null && m_RightEyeGameObject != null)
        {
            m_LeftEyeGameObject.SetActive(visible);
            m_RightEyeGameObject.SetActive(visible);
        }
    }

    void OnEnable()
    {
        var faceManager = FindObjectOfType<ARFaceManager>();
        if (faceManager != null && faceManager.subsystem != null &&
            faceManager.subsystem.SubsystemDescriptor.supportsEyeTracking)
        {
            SetVisible((m_Face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready));
            m_Face.updated += OnUpdated;
        }
        else
        {
            enabled = false;
        }
    }

    void OnDisable()
    {
        m_Face.updated -= OnUpdated;
        SetVisible(false);
    }

    void OnUpdated(ARFaceUpdatedEventArgs eventArgs)
    {
        CreateEyeGameObjectsIfNecessary();
        SetVisible((m_Face.trackingState == TrackingState.Tracking) && (ARSession.state > ARSessionState.Ready));
    }
}