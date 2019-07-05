using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterNavAgentController : MonoBehaviour {

    public NavMeshAgent agent;
    public PlayerController player;
    public Animator anim;
    public float attackDistance = 3f;


    void Start () {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();

        // agent.updatePosition = false;
        // agent.updateRotation = true;
    }

    void Update () {

        if ((player.transform.position - transform.position).magnitude < attackDistance)
        {
            agent.isStopped = true;
            anim.SetFloat("Speed", 0);
            anim.SetBool("Attack", true);
        }
        else
        {
            agent.isStopped = false;
            anim.SetFloat("Speed", 3);
            anim.SetBool("Attack", false);
        }
	}

    void OnAnimatorMove()
    {
        agent.velocity = anim.deltaPosition / Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(player.transform.position - transform.position);
    }

}
