using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.Serialization;

public class ControllerDetection : DontDestroySingleton<ControllerDetection>
{
    private Gamepad gamepad;
    private bool _keyboard;

    [SerializeField] private ControlType controlType;
    [SerializeField] private ControlSubtype controlSubtype;

    public delegate void SubtypeChange(ControlSubtype s);
    public event SubtypeChange OnControlSubtypeChange;
    
    public delegate void TypeChange(ControlType t);
    public event TypeChange OnControlTypeChange;
    
    public ControlSubtype ControlSubtype
    {
        get => controlSubtype;
        private set
        {
            if (controlSubtype == value) return;
            controlSubtype = value;
            //Call subtype change event when it changes (wow)
            OnControlSubtypeChange?.Invoke(ControlSubtype);
        }
    }
    
    public ControlType ControlType
    {
        get => controlType;
        private set
        {
            if (controlType == value) return;
            controlType = value;
            OnControlTypeChange?.Invoke(ControlType);
            
            if (controlType is ControlType.Controller)
                C_Cursor.HideAndLock();
        }
    }
    
    private bool IsKeyboard
    {
        get => _keyboard;
        set
        {
            if (_keyboard != value)
            {
                if (value) KeyboardLayout();
                else GamepadType();
            }
            _keyboard = value;
        }
    }

    private void DeviceEvent(InputDevice d, InputDeviceChange c)
    {
        switch (c)
        {
            case InputDeviceChange.Added:
                ControllerType();
                break;
            
            case InputDeviceChange.Removed:
                ControllerType();
                break;
            
            case InputDeviceChange.ConfigurationChanged:
                KeyboardLayout();
                break;
        }
    }

    private void ActionEvent(object obj, InputActionChange change)
    {
        if (gamepad != null)
        {
            if (obj is InputAction action)
            {
                if (action.activeControl == null) return;
                InputDevice lastDevice = action.activeControl.device;
                
                var actionName = action.activeControl.name;
                //I'm getting a weird input every frame from my controller,
                //this is a clunky solution to prevent it from setting the control type
                //to controller all the time and causing an ugly flicker
                //It appears to be working
                //Also it doesn't happen with Linger's controller, go figure
                if(actionName == "left" || actionName == "up" || actionName == "right" || actionName == "down" || actionName == "position") return;
                
                IsKeyboard = lastDevice.name is "Keyboard" or "Mouse";
            }
        }
    }
    
    protected override void Awake()
    {
        base.Awake();
        
        InputSystem.onDeviceChange += DeviceEvent;
        InputSystem.onActionChange += ActionEvent;
        
        ControllerType();
    }

    private void KeyboardLayout()
    {
        ControlType = ControlType.Keyboard;
        //failsafe in case layout is not recognised
        ControlSubtype = ControlSubtype.Qwerty;
            
        string layout = Keyboard.current.keyboardLayout;
        switch (layout)
        {
            case "0000040C":
                //print("AZERTY");
                ControlSubtype = ControlSubtype.Azerty;
                break;
            case "00000809":
                //print("QWERTY");
                ControlSubtype = ControlSubtype.Qwerty;
                break;
        }
    }

    private void GamepadType()
    {
        ControlType = ControlType.Controller;
        //failsafe in case controller is not recognised
        ControlSubtype = ControlSubtype.XInput;

        switch (gamepad)
        {
            case DualShockGamepad:
                ControlSubtype = ControlSubtype.DS4;
                break;
            case XInputController:
                ControlSubtype = ControlSubtype.XInput;
                break;
            case SwitchProControllerHID:
                ControlSubtype = ControlSubtype.Switch;
                break;
        }
    }
    
    private void ControllerType()
    {
        gamepad = Gamepad.current;

        if (gamepad != null)
            GamepadType();
        else
            KeyboardLayout();
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= DeviceEvent;
        InputSystem.onActionChange -= ActionEvent;
    }

    private void OnDestroy()
    {
        InputSystem.onDeviceChange -= DeviceEvent;
        InputSystem.onActionChange -= ActionEvent;
    }
}


public enum ControlType
{
    Keyboard, 
    Controller
}
public enum ControlSubtype
{
    [InspectorName("Keyboard/AZERTY")] Azerty,
    [InspectorName("Keyboard/QWERTY")] Qwerty,
    [InspectorName("Gamepad/DualShock4")] DS4,
    [InspectorName("Gamepad/XBox")] XInput,
    [InspectorName("Gamepad/Switch Pro")] Switch
}