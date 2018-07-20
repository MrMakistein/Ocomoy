using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : MonoBehaviour {

    public GameObject player;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == player)
        {

            if (Player.instance.water_count <= 0)
            {
                Debug.Log("entered");
                Player.instance.in_water = true;


            }
            Player.instance.water_count++;
        }

    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject == player)
        {

            if (Player.instance.water_count <= 1)
            {
                Debug.Log("exit");
                Player.instance.in_water = false;

            }

            Player.instance.water_count--;
        }

    }

   
}
