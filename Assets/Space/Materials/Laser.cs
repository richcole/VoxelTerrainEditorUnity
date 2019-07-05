using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour
{

    private LineRenderer lr;
    public GameObject fireFrom;
    public GameObject fireTo;

    // Use this for initialization
    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lr.SetPosition(0, fireFrom.transform.position);
        Vector3 forward = fireTo.transform.position - fireFrom.transform.position;
        RaycastHit hit;
        if (Physics.Raycast(fireFrom.transform.position, forward, out hit))
        {
            if (hit.collider)
            {
                lr.SetPosition(1, hit.point);
            }
        }
        else
        {
            lr.SetPosition(1, fireFrom.transform.position + (forward * 5000));
        }
    }
}