using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGrower : MonoBehaviour {

    public GameObject[] trees;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GrowTree()
    {
        int x = Random.Range(0, 40);
        int z = Random.Range(0, 40);

        Vector3 origin = new Vector3(x, 100, z);
        Vector3 direction = new Vector3(0, -1, 0);


        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, 100))
        {
            GameObject tree = Instantiate(trees[0], hit.point, Quaternion.identity);
            tree.name = "A Tree";
            Debug.Log("Grew a tree at " + hit.point);
        }
        else
        {
            Debug.Log("No intersection at " + origin + " " + direction);
        }

    }
}
