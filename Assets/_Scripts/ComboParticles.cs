using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboParticles : MonoBehaviour {

    public GameObject particle_system;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StopParticles()
    {
        particle_system.GetComponent<ParticleSystem>().Stop(true);
    }

    public void StartParticles()
    {
        if (!particle_system.GetComponent<ParticleSystem>().isPlaying)
        { 
         particle_system.GetComponent<ParticleSystem>().Play(true);
        }
    }
}
