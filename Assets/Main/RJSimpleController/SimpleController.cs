using System.Collections.Generic;
using UnityEngine;

public class SimpleController : MonoBehaviour {

    public Head head;
    public Rigidbody body;
    public float mouseSpeed = 2;
    public float bodySpeed = 30;
    public float jumpSpeed = 10;
    public float rx = 0f;
    public float ry = 0f;
    public Vector3 velocity = Vector3.zero;
    private HashSet<int> collisionObjects = new HashSet<int>();
    public CustomEventsManager customEvents;

    void Start () {
        head = GetComponentInChildren<Head>();
        body = GetComponent<Rigidbody>();
        rx = 0f;
        ry = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        customEvents = FindObjectOfType<CustomEventsManager>();
        customEvents.MouseModeChanged(InputEnabled.Disabled);
    }

    void Update () {
        if (customEvents.ShouldHandleMoveEvents())
        {
            HandleMouseMove();
            HandleKeyboardMove();
        }
    }

    public void HandleMouseMove()
    {
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");
        rx += mx * mouseSpeed;
        ry += my * mouseSpeed;
        head.transform.rotation = Quaternion.Euler(-ry, rx, 0);
    }

    public void HandleKeyboardMove()
    {
        velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            velocity += head.transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            velocity -= head.transform.forward;
        }

        if (Input.GetKey(KeyCode.A))
        {
            velocity -= head.transform.right;
        }

        if (Input.GetKey(KeyCode.D))
        {
            velocity += head.transform.right;
        }

        velocity.y = 0;
        velocity = velocity.normalized;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            velocity *= 2;
        }

        Vector3 xzDrag = body.velocity * -0.1f;
        xzDrag.y = 0;
        velocity += xzDrag;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (collisionObjects.Count > 0)
            {
                body.AddForce(transform.up * jumpSpeed, ForceMode.Impulse);
            }
        }

        body.AddForce(velocity * bodySpeed, ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        collisionObjects.Add(collision.collider.gameObject.GetHashCode());
    }

    private void OnCollisionExit(Collision collision)
    {
        collisionObjects.Remove(collision.collider.gameObject.GetHashCode());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        var origin = transform.position + Vector3.down * 0.95f;
        if (collisionObjects.Count > 0)
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawLine(origin, origin + Vector3.down * 0.5f);
        
    }
}
