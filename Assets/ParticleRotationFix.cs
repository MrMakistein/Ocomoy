using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleRotationFix : MonoBehaviour {

    public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.rotation = Quaternion.Euler(0, player.transform.rotation.y * -1, 0);

    }
}
