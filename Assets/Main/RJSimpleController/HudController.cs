using UnityEngine;

public class HudController : MonoBehaviour {

    public CustomEventsManager eventsManager;
    public TargetImage target;

	// Use this for initialization
	void Start () {
        eventsManager = FindObjectOfType<CustomEventsManager>();
        eventsManager.OnMouseModeChanged += OnMouseModeChanged;
        eventsManager.OnDiggingChanged += OnDiggingChanged;
        target = GetComponentInChildren<TargetImage>();
	}

    public void OnMouseModeChanged(InputEnabled mode)
    {
        bool showTarget =
            eventsManager.mouseMode == InputEnabled.Disabled &&
            eventsManager.diggingMode == DiggingMode.NotDigging;
        target.gameObject.SetActive(showTarget);
    }

    public void OnDiggingChanged(DiggingMode diggingMode)
    {
        bool showTarget =
            eventsManager.mouseMode == InputEnabled.Disabled &&
            eventsManager.diggingMode == DiggingMode.NotDigging;
        target.gameObject.SetActive(showTarget);
    }
}
