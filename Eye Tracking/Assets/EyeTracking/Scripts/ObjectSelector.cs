using System;
using System.Collections.Generic;
using EyeTracking.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private ObjectDescription[] _objectDescriptions;
    [SerializeField] private UIObject _objectPrefab;
    [SerializeField] private RectTransform _content;
    [SerializeField] private ToggleGroup _toggleGroup;

    private List<UIObject> _spawnedObjects = new List<UIObject>();
    private UIObject _currentlySelected;

    private void Awake()
    {
        foreach (var objectDescription in _objectDescriptions)
        {
            var spawnedObject = Instantiate(_objectPrefab, _content);
            spawnedObject.UpdateDescription(objectDescription);
            spawnedObject.Toggle.group = _toggleGroup;
            _spawnedObjects.Add(spawnedObject);
            spawnedObject.Toggle.onValueChanged.AddListener((x) => OnSelectionChanged());
        }
    }

    private void OnSelectionChanged()
    {
        foreach (var spawnedObject in _spawnedObjects)
        {
            if (spawnedObject.Toggle.isOn)
            {
                _currentlySelected = spawnedObject;
                break;
            }
        }
    }

    public UIObject GetSelectedObject()
    {
        return _currentlySelected;
    }

    private void OnDestroy()
    {
        foreach (var spawnedObject in _spawnedObjects)
        {
            spawnedObject.Toggle.onValueChanged.RemoveAllListeners();
        }
    }
}