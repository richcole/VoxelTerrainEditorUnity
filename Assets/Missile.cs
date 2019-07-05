using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    public float spawnTime;
    public float lifeTime;
    public float speed;

	// Use this for initialization
	void Start () {
        spawnTime = Time.time;
        GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
	
	// Update is called once per frame
	void Update () {
		if (spawnTime + lifeTime < Time.time)
        {
            Destroy(gameObject);
        }
	}
}
