using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : MonoBehaviour {

    public PlayerController player;

    public Animator anim;
    public float remainingDistance;
    public GridArray gridArray;
    public Vector3 target;
    public bool haveTarget = false;
    public Rigidbody body;
    public Vector3 fwd;
    public float speed;


    // Use this for initialization
    void Start () {
        player = FindObjectOfType<PlayerController>();
        gridArray = FindObjectOfType<GridArray>();
        anim = FindObjectOfType<Animator>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
        if (! haveTarget || (transform.position - target).magnitude < 1)
        {
            NavMesh navMesh = gridArray.navMesh;
            if (gridArray.navMesh != null)
            {
                List<CellIndex> path = navMesh.FindPath(player.transform.position, transform.position);
                if (path != null && path.Count > 1)
                {
                    target = navMesh.GetPosition(path[1]).Value;
                    haveTarget = true;
                    anim.SetFloat("Speed", 3);
                }
                else
                {
                    haveTarget = false;
                    anim.SetFloat("Speed", 0);
                }
            }
        }

        if (haveTarget)
        {
            TurnTowards(target);
        }
    }

    public void TurnTowards(Vector3 target)
    {
        fwd = (target - transform.position).normalized;
        if (fwd.y < 0)
        {
            fwd.y = 0;
        }
        body.rotation = Quaternion.LookRotation(fwd, Vector3.up);
        body.AddForce(fwd * speed, ForceMode.Force);
    }

    void OnAnimatorMove()
    {
    }

}
