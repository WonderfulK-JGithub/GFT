using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomRaycaster : MonoBehaviour
{
    [SerializeField] GraphicRaycaster raycaster;
    [SerializeField] EventSystem eventSystem;

    public IClickable lastRaycasted;
    private void Awake()
    {
        if (eventSystem == null) eventSystem = FindObjectOfType<EventSystem>();
    }

    private void Update()
    {
        IClickable _newRaycast = GetRaycastClickable();
        if (_newRaycast != null)
        {
            if (_newRaycast != lastRaycasted)
            {
                _newRaycast.StartHover();
            }
            if (Input.GetMouseButtonDown(0))
            {
                _newRaycast.Click();
            }
        }
        lastRaycasted = _newRaycast;
    }

    public IClickable GetRaycastClickable()
    {
        PointerEventData _pointerEventData = new PointerEventData(eventSystem);
        _pointerEventData.position = Input.mousePosition;

        List<RaycastResult> _results = new List<RaycastResult>();
        raycaster.Raycast(_pointerEventData, _results);

        foreach (var _result in _results)
        {
            if (_result.gameObject.TryGetComponent(typeof(IClickable), out Component _comp))
            {
                return _comp as IClickable;
            }
        }

        return null;
    }
}


public interface IClickable
{
    void StartHover();
    void Click();
}