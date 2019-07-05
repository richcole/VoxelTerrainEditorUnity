using UnityEngine;

public class DeviceInput : MonoBehaviour
{
    public Device device;
    public CustomEventsManager eventsManager;

    public void Start()
    {
        eventsManager = FindObjectOfType<CustomEventsManager>();
    }

    public virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab))
        {
            eventsManager.StopEditingDevice();
        }
    }

    public void SetDevice(Device device)
    {
        this.device = device;
    }
}
