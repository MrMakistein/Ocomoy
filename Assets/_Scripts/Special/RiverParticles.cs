using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class RiverParticles : MonoBehaviour {

    ParticleSystem ps;
    

    void Awake() {
        ps = GetComponent<ParticleSystem>();
        
        // ps.Play();
    }
	// Use this for initialization
	void Start () {
        //TODO: Disable Emission
        

        /*var main = ps.main;

        ps.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 100, 200), new ParticleSystem.Burst(5.0f, 10, 20) });*/
    }
	
	// Update is called once per frame
	void Update () {
        //TODO: on trigger enter: was genau? -> Position abfragen
        //on trigger stay: was genau? -> Position abfragen
        //burst an position erzeugen!! 
        
    }
}
