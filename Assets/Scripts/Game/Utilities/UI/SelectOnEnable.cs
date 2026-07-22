using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SelectOnEnable : MonoBehaviour
{
    private EventSystem eventSystem;

    public SelectOn selectOn;
    public enum SelectOn
    {
        Enable = 0, 
        Event
    }

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();

    }

    private void OnEnable()
    {
        ControllerDetection.instance.OnControlTypeChange += OnControlChanged;
        
        if (selectOn == SelectOn.Enable)
        {
            Select();
        }
    }

    public void Select()
    {
        if (ControllerDetection.instance.ControlType == ControlType.Controller)
        {
            eventSystem.SetSelectedGameObject(gameObject);
        }
    }

    private void OnControlChanged(ControlType t)
    {
        if (eventSystem.currentSelectedGameObject || t is ControlType.Keyboard) return;
        eventSystem.SetSelectedGameObject(gameObject);
    }
    
    private void OnDisable()
    {
        ControllerDetection.instance.OnControlTypeChange -= OnControlChanged;
    }
    private void OnDestroy()
    {
        ControllerDetection.instance.OnControlTypeChange -= OnControlChanged;
    }
}
