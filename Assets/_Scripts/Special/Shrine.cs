using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour {

    public float shrine_cooldown = 10; // how long the player is locked in the shrine while praying
    public float shrine_cooldown_timer = 0;
    public float blessing_spawn_cooldown = 100; // how long the shrine takes to generate a new item after it was picked up
    [HideInInspector] public float blessing_spawn_cooldown_timer = 0;
    public int shrine_id = 1;
    public bool shrine_swap = true;
    public GameObject altar_mesh;
    public GameObject spawn_mesh;
    public GameObject altar_orb;
    public GameObject altar_absorb;

	// Use this for initialization
	void Start () {
		
	}

    private void FixedUpdate()
    {

        if (blessing_spawn_cooldown_timer < 4 && blessing_spawn_cooldown_timer > 1)
        {
            if (shrine_swap && shrine_id != 1)
            {
                int new_shrine_id = Random.Range(2, 7);
                shrine_id = new_shrine_id;


            }
            blessing_spawn_cooldown_timer = 0;
        }

        // Decreases timer for blessing spawn at shrine
        if (blessing_spawn_cooldown_timer > 0)
        {
            blessing_spawn_cooldown_timer -= Time.deltaTime * 10;
        }

        // changes the color of the shrine depening on its state
        if (shrine_cooldown_timer > 0)
        {

            //GetComponent<Renderer>().material.color = Color.red;
            blessing_spawn_cooldown_timer = blessing_spawn_cooldown;
            shrine_cooldown_timer -= Time.deltaTime * 3;


        }
        else if (blessing_spawn_cooldown_timer > 0)
        {
            altar_orb.SetActive(false);
            //GetComponent<Renderer>().material.color = Color.gray;
        }
        else
        {
            altar_orb.SetActive(true);
            //GetComponent<Renderer>().material.color = new Color(0.2F, 0.2F, 0.2F, 1);
        }

    }

    IEnumerator StartPray()
    {
        altar_absorb.GetComponent<ParticleSystem>().Play(true);
        yield return new WaitForSeconds(1.5f);
        altar_orb.GetComponent<ParticleSystem>().Stop(true);




    }

  
    // Update is called once per frame
    void Update()
    {

      
    }

    public void SetMeshes()
    {
        if (shrine_id == 1)
        {
            spawn_mesh.SetActive(true);
            AudioManager.instance.spawn_position = spawn_mesh.transform.position;
            CollectibleCollectedParticles.instance.spawnshrine_position = this.gameObject.transform.position;
        } else
        {
            altar_mesh.SetActive(true);
        }

    }
}
