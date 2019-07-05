using System;
using UnityEngine;

public enum InputEnabled
{
    Enabled,
    Disabled
}

public enum DiggingMode
{
    Digging,
    NotDigging
}


public class CustomEventsManager : MonoBehaviour {

    public delegate void OnMouseModeChangedAction(InputEnabled mode);

    public event OnMouseModeChangedAction OnMouseModeChanged;
    public InputEnabled mouseMode = InputEnabled.Disabled;

    public delegate void OnDiggingChangedAction(DiggingMode mode);
    public event OnDiggingChangedAction OnDiggingChanged;
    public DiggingMode diggingMode = DiggingMode.NotDigging;

    public ComputerInput computerInput;
    public InputEnabled computerInputMode;

    public DeviceInput deviceInput;
    public InputEnabled deviceInputMode;

    public void Start()
    {
        diggingMode = DiggingMode.NotDigging;
        mouseMode = InputEnabled.Disabled;

        computerInputMode = InputEnabled.Disabled;
        computerInput = FindObjectOfType<ComputerInput>();
        computerInput.gameObject.SetActive(false);

        deviceInputMode = InputEnabled.Disabled;
        deviceInput = FindObjectOfType<DeviceInput>();
        deviceInput.gameObject.SetActive(false);
    }

    public void Update()
    {
        HandleMouseVisibility();
    }
        
    public void MouseModeChanged(InputEnabled mouseMode)
    {
        this.mouseMode = mouseMode;
        OnMouseModeChanged?.Invoke(mouseMode);
    }

    public void DiggingChanged(DiggingMode diggingMode)
    {
        this.diggingMode = diggingMode;
        OnDiggingChanged?.Invoke(diggingMode);
    }

    public void HandleMouseVisibility()
    {
        if (computerInputMode == InputEnabled.Disabled && deviceInputMode == InputEnabled.Disabled)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SetMouseMode(InvertMouseMode(mouseMode));
            }
        }
    }

    public void SetMouseMode(InputEnabled mouseMode)
    {
        switch (mouseMode)
        {
            case InputEnabled.Disabled:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case InputEnabled.Enabled:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }
        MouseModeChanged(mouseMode);
    }

    public InputEnabled InvertMouseMode(InputEnabled mouseMode)
    {
        switch (mouseMode)
        {
            case InputEnabled.Disabled:
                return InputEnabled.Enabled;
            case InputEnabled.Enabled:
                return InputEnabled.Disabled;
        }
        return InputEnabled.Enabled;
    }

    public void StartEditingComputer(Computer computer)
    {
        computerInput.SetComputer(computer);
        computerInput.gameObject.SetActive(true);
        computerInputMode = InputEnabled.Enabled;
    }

    public void StopEditingComputer()
    {
        computerInput.SetComputer(null);
        computerInput.gameObject.SetActive(false);
        computerInputMode = InputEnabled.Disabled;
    }

    public bool ShouldHandleTargetEvents()
    {
        return mouseMode == InputEnabled.Disabled && 
            diggingMode == DiggingMode.NotDigging && 
            computerInputMode == InputEnabled.Disabled;
    }

    public bool ShouldHandleMoveEvents()
    {
        return mouseMode == InputEnabled.Disabled &&
            computerInputMode == InputEnabled.Disabled;
    }

    public void StartEditingDevice(Device device)
    {
        deviceInput.SetDevice(device);
        deviceInput.gameObject.SetActive(true);
        deviceInputMode = InputEnabled.Enabled;
    }

    internal void StopEditingDevice()
    {
        deviceInput.SetDevice(null);
        deviceInput.gameObject.SetActive(false);
        deviceInputMode = InputEnabled.Disabled;
    }


}
