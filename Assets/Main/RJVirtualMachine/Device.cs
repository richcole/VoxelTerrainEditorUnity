using UnityEngine;

public class Device : MonoBehaviour, Interactor
{
    public Computer computer;
    public int port;

    public CustomEventsManager eventsManager;

    public virtual void Start()
    {
        eventsManager = FindObjectOfType<CustomEventsManager>();
    }

    public void Interact(Ray ray, RaycastHit hit)
    {
        if (Input.GetMouseButtonDown(0))
        {
            eventsManager.StartEditingDevice(this);
        }
    }

    public virtual void Update()
    {
    }
}
