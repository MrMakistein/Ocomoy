using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class InteractiveSettings : MonoBehaviour {
public class InteractiveSettings : MonoBehaviour
{
    public Material normalMaterial;
    public Material collectibleMaterial;
    public float collectible_weight = 10;
    public bool isCollectible = false;
    public bool collectibleOnFloor = true;
    public int combo_state = 0;
    public GameObject combo_particles1;
    public GameObject combo_particles2;
    public GameObject combo_particles3;
    public float combo_particle_reset_timer = 0;
    public float shrink_timer = 0;
    public GameObject water_particles;
    public ParticleSystem water_particle_system;
    public AudioClip[] splash_sounds;
    public AudioSource AudioSource;
    public float splash_cd;


    //used to reset the position in the "CollectibleOutOfReach" script 
    public float illegal_position_reset_timer = 0;
    

    // Use this for initialization
    void Start()
    {
        splash_cd = 10;
        //isCollectible = false; //default: not a collectible; will be changed by SpawnController	
        //isCollectible = false;
        water_particle_system = water_particles.GetComponent<ParticleSystem>();
        GetComponent<Rigidbody>().isKinematic = false; //disable kinematics -> can be grabbed
        GetComponent<Collider>().isTrigger = false;
    }

    //entweder: ausgehen von boolean isCollectible
    //oder: extra Methode ausführen von SpawnController aus! Der spawnt die Collectibles und ändert die Settings!
    public void SetCollectible()
    {


        isCollectible = true; //mark as collectible
                              ////GetComponent<Collider> ().isTrigger = true; //set as trigger => you can enter it to deactivate!

        //Update the mass of the collectible
        this.gameObject.GetComponent<ThrowObject>().InitialMass = collectible_weight;

       
    }

    void FixedUpdate()
    {
        if (splash_cd >= 0)
        {
            splash_cd = splash_cd - 0.1f;
        }

        if (shrink_timer > 0)
        {
            shrink_timer += 0.01f;
            if (shrink_timer > 1) {
                this.gameObject.transform.localScale = new Vector3(this.gameObject.transform.localScale.x * 0.9f, this.gameObject.transform.localScale.y * 0.9f, this.gameObject.transform.localScale.z * 0.9f);
            }
            if (shrink_timer > 2)
            {
                shrink_timer = 0;
                this.gameObject.SetActive(false);

                GameObject[] signposts = GameObject.FindGameObjectsWithTag("Signpost");
                foreach (GameObject go in signposts)
                {
                    go.GetComponent<Signpost>().UpdateSignpostDetection();
                }
            }
        }

        if (combo_particle_reset_timer > 0)
        {
            if (combo_particle_reset_timer >= 2.3f && ComboManager.instance.combo_level == 3)
            {
                combo_particles1.SetActive(true);
                combo_particles1.GetComponent<ComboParticles>().StartParticles();
            }
            else if (combo_particle_reset_timer >= 2.3f && ComboManager.instance.combo_level == 4)
            {
                combo_particles2.SetActive(true);
                combo_particles2.GetComponent<ComboParticles>().StartParticles();
            }
            else if (combo_particle_reset_timer >= 2.3f && ComboManager.instance.combo_level == 5)
            {
                combo_particles3.SetActive(true);
                combo_particles3.GetComponent<ComboParticles>().StartParticles();
                
            }

            combo_particle_reset_timer = combo_particle_reset_timer - 0.01f;

            if (combo_particle_reset_timer > 2.0 && combo_particle_reset_timer <= 2.29)
            {
                combo_particles2.GetComponent<ComboParticles>().StopParticles();
                combo_particles3.GetComponent<ComboParticles>().StopParticles();
                combo_particle_reset_timer = 2.0f;
            }

            if (combo_particle_reset_timer <= 0)
            {
                combo_particles1.SetActive(false);
                combo_particles2.SetActive(false);
                combo_particles3.SetActive(false);
            }
        }
    }

    public void PlaySplashSound()
    {
        
        if (splash_cd <= 0)
        {
            AudioSource.PlayOneShot(splash_sounds[(int)(Random.value * splash_sounds.Length) % splash_sounds.Length]);
            splash_cd = 2;
        }
    }

    void Update()
    {
      
    }

}
