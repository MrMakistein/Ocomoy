using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class avalanche : MonoBehaviour {
    public float radius = 12;
    public float height = 10;
    public int count = 10;

    private GameObject[] spawnObject;
    // Use this for initialization
    void Start()
    {
        spawnObject = GameObject.FindGameObjectsWithTag("Interactive");
        int iteratorObjects = 0;
        for (int i = 0; i < count; i++)
        {
            //an iterator for the objects is used to spawn an eben distribution of gameobjects in the scene
            iteratorObjects = iteratorObjects % spawnObject.Length;
            if (spawnObject[iteratorObjects].GetComponent<InteractiveSettings>().isCollectible == false) { 
                spawnObject[iteratorObjects].GetComponent<ThrowObject>().dmg_cooldown = spawnObject[iteratorObjects].GetComponent<ThrowObject>().dmg_cooldown_max * 3;
                Instantiate(spawnObject[iteratorObjects], (transform.position + (Vector3.up * height) + new Vector3(-2f, 0f, -2f)) + new Vector3(Random.Range(0, radius), Random.Range(0, radius), Random.Range(0, radius)), Quaternion.identity);
            }
            iteratorObjects++;
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }


}

