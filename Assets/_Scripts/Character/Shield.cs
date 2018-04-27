
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shield : MonoBehaviour {
  


    // Use this for initialization
    void Start () {
	
    }



    // Update is called once per frame
    void Update () {
    
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactive" &&
            other.gameObject.GetComponent<ThrowObject>().dmg_cooldown >= 1 &&
            !other.gameObject.GetComponent<InteractiveSettings>().isCollectible)
        {
            Destroy(other.gameObject);


        }

        
    }
}