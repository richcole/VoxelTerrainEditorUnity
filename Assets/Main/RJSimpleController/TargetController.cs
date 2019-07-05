using UnityEngine;

public class TargetController : MonoBehaviour {

    public CustomEventsManager eventsManager;
    public Camera playerCamera;

	void Start () {
        eventsManager = FindObjectOfType<CustomEventsManager>();
        playerCamera = FindObjectOfType<Camera>();
	}
	
	void Update () {

        if (eventsManager.ShouldHandleTargetEvents())
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject obj = hit.collider.gameObject;
                Interactor interactor = obj.GetComponentInParent<Interactor>();
                
                if (interactor != null)
                {
                    interactor.Interact(ray, hit);
                }
            }
        }
    }

}
