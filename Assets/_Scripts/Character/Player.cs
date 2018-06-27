
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;
using UnityStandardAssets.CrossPlatformInput;


public class Player : MonoBehaviour {
    // Health Variables
    public float maxHealth = 100;
    public float currentHealth;
    public Image healthBar;
    public float regenSpeed = 2;
    [HideInInspector] public bool healthOnPickup = false;

    //Clone Ability Variables
    public float clone_time = 0;
    public int CloneAmount = 4;
    public GameObject cloneObj;

    //Shield Ability Variables
    public float shield_rotation_speed = 200;
    public float shield_timer = 0;
    public float shield_duration = 50;
    public GameObject shield;

    //Slippery Hands Ability Variables
    public int slip_uses = 5;
    private int slips_left = 0;

    //Compass Ability Variables
    public float compass_timer = 0;
    public float compass_duration = 50;
    public GameObject compass;

    //Slowdown Ability Variables
    public static bool slowEffect = false;
    private float slow_timer = 0;
    public float slow_duration = 15;
    public static float slow_mass = 15;

    // Random Shit
    public Texture[] collectibleImages;
    public int equipped_ability = 0;
    public float hit_cooldown_timer;
    public float hit_cooldown = 10;
    public int collectibleCount;
    private GameObject arena;
    private GameObject god;
    public GameObject healthbar;
    public GameObject display_shield;
    public GameObject display_compass;
    public GameObject display_slippy;
    public GameObject display_clone;
    public GameObject display_slow;
    public GameObject display_slip_uses;
    public bool in_shrine;
    public GameObject collectibles_gui;
    public GameObject Camera;
    public PostProcessingProfile m_Profile;
    public GameObject[] collectibles;
    public GameObject human_win_screen;
    public GameObject god_win_screen;
    public float collectible_distance;
    public float combo_dmg_multiplier = 1;
    public Animator anim;

    // Audio 
    public AudioClip[] audio_clips;
    public AudioSource PlayerAudioSource;
    public static bool clonesActive = false;
    public static bool shieldActive = false;
    public static bool collectSound = false;
    public static int slip_count;
    public static bool compassActive = false;
    public static bool slowActive = false;

    public static Player instance;

    private void Awake()
    {
        Player.instance = this;
    }

    // Use this for initialization
    void Start() {
        arena = GameObject.Find("Arena");
        slip_count = 5;
        collectibleCount = 0;
        healthBar.fillAmount = 1;
        currentHealth = maxHealth;
        god = GameObject.Find("TheosGott");
        anim = this.gameObject.GetComponentInChildren<Animator>();
        if (cloneObj == null)
        {
            Debug.LogError("No clone specified in player!");
        }

    }


    // Update is called once per frame
    void Update() {

        UpdateVignette();

        //Update Collectibles Display
        if (collectibleCount == 1)
        {
            collectibles_gui.GetComponent<RawImage>().texture = collectibleImages[0];
        }
        if (collectibleCount == 2)
        {
            collectibles_gui.GetComponent<RawImage>().texture = collectibleImages[1];
        }
        if (collectibleCount == 3)
        {
            collectibles_gui.GetComponent<RawImage>().texture = collectibleImages[2];
        }



        healthbar.transform.position = transform.position;
        //Hit Cooldown Timer
        if (hit_cooldown_timer > 0)
        {
            hit_cooldown_timer -= Time.deltaTime * 10;
        }

        // Health Regeneration
        if (currentHealth < maxHealth)
        {
            currentHealth = currentHealth + (regenSpeed / 200);
            healthBar.fillAmount = currentHealth / maxHealth;
        }
        if ( currentHealth <= 0 && currentHealth > -100)
        {
            currentHealth = -1000;
            SaveGameData();
            //print("Arena wins");
            god_win_screen.SetActive(true);
            Invoke("LoadMenu", 5);
        }

        // Ability Activation
        bool prayAttempt = CrossPlatformInputManager.GetButtonDown("Pray");
        if (prayAttempt && !gameObject.GetComponent<Movement>().move_block && !in_shrine)
        {
            display_shield.SetActive(false);
            display_compass.SetActive(false);
            display_slippy.SetActive(false);
            display_clone.SetActive(false);
            display_slow.SetActive(false);

            if (equipped_ability >= 2 && equipped_ability <= 6)
            {
                StatsManager.instance.round_powerups_used_player++;
            }

            if (equipped_ability == 2)
            {
                Start_clone_ability();
            }


            if (equipped_ability == 3)
            {
                shield_timer = 1;
                shield.SetActive(true);
            }

            if (equipped_ability == 4)
            {
                compassActive = true;
                compass_timer = 1;
                compass.SetActive(true);
            }

            if (equipped_ability == 5)
            {
                slips_left = slip_uses;
            }

            if (equipped_ability == 6)
            {
                slowActive = true;
                slow_timer = 0;
                slowEffect = true;
            }

            equipped_ability = 0;
        }

        if (shield_timer >= 1)
        {
            Shield_ability();
        }

        if (compass_timer >= 1)
        {
            Compass_ability();
        }

        if (slips_left >= 1)
        {
            Slipperyhands_ability();
        }

        if (slow_timer > slow_duration)
        {
            slowEffect = false;
        }
        else
        {
            slow_timer += Time.deltaTime;
        }

        //anim.SetBool("isPraying", true);

    }

    

    public void SaveGameData()
    {
        PlayerPrefs.SetFloat("totalRoundTime", (PlayerPrefs.GetFloat("totalRoundTime") + StatsManager.instance.round_time_played));
        PlayerPrefs.SetInt("finishedRoundCount", PlayerPrefs.GetInt("finishedRoundCount") + 1);
    }

    public GameObject GetClosestCollectible()
    {
        collectibles = arena.GetComponent<SpawnController>().collectibles;
        GameObject gMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject g in collectibles)
        {
            if (g != null && g.GetComponent<InteractiveSettings>().isCollectible)
            {
                float dist = Vector3.Distance(g.transform.position, currentPos);
                if (dist < minDist && g.gameObject.activeSelf)
                {
                    gMin = g;
                    minDist = dist;
                }
            }
        }
        return gMin;
    }




    private void UpdateVignette() //ADJUST
    {

        GameObject closest_collectible = GetClosestCollectible();
       
        if (closest_collectible != null)
        {
            float intensity = 0;
            collectible_distance = Vector3.Distance(transform.position, closest_collectible.transform.position);
            //print(distance);

            if (collectible_distance < 45f)
            {
                intensity = + ((45 - collectible_distance) / 72);
                if (intensity > 0.65f)
                {
                    intensity = 0.65f;
                }

            }
            var vignette = m_Profile.vignette.settings;
            vignette.intensity = intensity;
            m_Profile.vignette.settings = vignette;
        }
         if (collectibleCount >= 3)
        {
            var vignette = m_Profile.vignette.settings;
            vignette.intensity = 0.0f;
            m_Profile.vignette.settings = vignette;
        }

        


    }


    private void Slipperyhands_ability()
    {
        //slipperyActive = true;
            display_slip_uses.GetComponent<Text>().text = "" + slips_left;

        bool prayAttempt = CrossPlatformInputManager.GetButtonDown("Pray");
        if (prayAttempt)
        {
            god.GetComponent<dnd>().ReleaseObject();
            slips_left--;
            slip_count--;
            if (slips_left == 0)
            {
                slip_count = 5;
                display_slip_uses.GetComponent<Text>().text = "";
            }
        }

    }



    private void Compass_ability()
    {
        compass.transform.position = transform.position;
        compass.GetComponent<Compass>().UpdateCompassDetection();

        if (arena.GetComponent<SpawnController>().allCollected)
        {
            compass_timer = compass_duration;
        }

        compass_timer += Time.deltaTime * 5;
        if (compass_timer >= compass_duration)
        {
            compass_timer = 0;
            compass.SetActive(false);

        }
    }


    private void Start_clone_ability()
    {
        clonesActive = true;
        for (int i = 0; i < CloneAmount; i++)
        {
            GameObject currentSpawn = Instantiate(cloneObj, transform.position, Quaternion.identity);
            currentSpawn.transform.parent = gameObject.transform;
        }
    }


    private void Shield_ability()
    {
        shieldActive = true;
        shield.transform.position = transform.position;
        shield.transform.Rotate(Vector3.up * Time.deltaTime * shield_rotation_speed, Space.World);

        shield_timer += Time.deltaTime;
        if (shield_timer >= shield_duration)
        {
            shield_timer = 0;
            shield.SetActive(false);          
}
    }
    

    private void OnCollisionEnter(Collision col)
    {
        GameObject god = GameObject.Find("TheosGott");
        // Test for player/interactive collision and deal the correct amount of damage depening on the weight_class

        GameObject selectedClone = null;
        foreach (ContactPoint c in col.contacts)
        {
            if (c.thisCollider.gameObject.tag == "Clone")
            {
                Debug.Log("Clone hit!");
                selectedClone = c.thisCollider.gameObject;
            }
        }

        if (col.gameObject.tag == "Interactive" &&
        !col.gameObject.GetComponent<InteractiveSettings>().isCollectible &&
        col.gameObject.GetComponent<ThrowObject>().dmg_cooldown >= 1 &&
        hit_cooldown_timer <= 0 &&
        !this.GetComponent<Movement>().move_block &&
        System.Array.FindIndex(collectibles, o => o == col.gameObject) == -1) //checking if the array does not contain the collectable
        {

            god.GetComponent<dnd>().ReleaseObject(); // Makes the god drop the object he's holding
            CameraControl.instance.CameraShake();
            hit_cooldown_timer = hit_cooldown;
            if (selectedClone == null)
            {
                int hit_rand = Random.Range(0, 4);
                PlayerAudioSource.volume = 0.35f;
                float velocity = col.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
                float dmg = 5;
                ComboManager.instance.combo_timer = 10;
                StatsManager.instance.round_player_hits++;
                



                if (col.gameObject.GetComponent<ThrowObject>().weight_class == 1)
                {
                    PlayerAudioSource.pitch = 1.0f;
                    PlayerAudioSource.PlayOneShot(audio_clips[hit_rand]);

                    if (velocity < 14)
                    {
                        dmg = 2;
                    }
                    if (velocity >= 14 && velocity < 24)
                    {
                        dmg = 4;
                    }
                    if (velocity >= 24)
                    {
                        dmg = 7;
                    }


                    currentHealth = currentHealth - (dmg * combo_dmg_multiplier);
                    healthBar.fillAmount = currentHealth / maxHealth;
                }

                if (col.gameObject.GetComponent<ThrowObject>().weight_class == 2)
                {
                    PlayerAudioSource.pitch = 1.0f;
                    PlayerAudioSource.PlayOneShot(audio_clips[hit_rand]);

                    if (velocity < 8)
                    {
                        dmg = 6;
                    }
                    if (velocity >= 8 && velocity < 18)
                    {
                        dmg = 10;
                    }
                    if (velocity >= 18)
                    {
                        dmg = 15;
                    }

                    currentHealth = currentHealth - (dmg * combo_dmg_multiplier);
                    healthBar.fillAmount = currentHealth / maxHealth;
                }

                if (col.gameObject.GetComponent<ThrowObject>().weight_class == 3)
                {
                    PlayerAudioSource.pitch = 1.0f;
                    PlayerAudioSource.PlayOneShot(audio_clips[hit_rand]);

                    if (velocity < 4)
                    {
                        dmg = 12;
                    }
                    if (velocity >= 5 && velocity < 14)
                    {
                        dmg = 17;
                    }
                    if (velocity >= 15)
                    {
                        dmg = 21;
                    }
                    currentHealth = currentHealth - (dmg * combo_dmg_multiplier);
                    healthBar.fillAmount = currentHealth / maxHealth;
                }

                if (col.gameObject.GetComponent<ThrowObject>().weight_class == 4)
                {
                    PlayerAudioSource.pitch = 1f;
                    PlayerAudioSource.PlayOneShot(audio_clips[hit_rand]);

                    if (velocity < 3)
                    {
                        dmg = 16;
                    }
                    if (velocity >= 4 && velocity < 11)
                    {
                        dmg = 20;
                    }
                    if (velocity >= 11)
                    {
                        dmg = 24;
                    }
                    currentHealth = currentHealth - (dmg * combo_dmg_multiplier);
                    healthBar.fillAmount = currentHealth / maxHealth;
                }
            }
            else
            {
                //trow the object in a random direction
                col.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-10.0F, 10.0F), Random.Range(8.0F, 12.0F), Random.Range(-10.0F, 10.0F));

                //if a clone was hit, destroy him
                Destroy(selectedClone, 0.1f);
            }

            //Object Damage
            col.gameObject.GetComponent<ThrowObject>().object_damage -= 1;

            if (ComboManager.instance.combo_level >= 3) //Set Damage to 0 if object is burning
            {
                col.gameObject.GetComponent<ThrowObject>().object_damage = 0;
            }

            // Destroy broken objects with the correct time delay to fade out the particles
            if (col.gameObject.GetComponent<ThrowObject>().object_damage <= 0 && col.gameObject.GetComponent<ThrowObject>().combo_kill_timer <= 0)
            {
                if (ComboManager.instance.combo_level >= 3)
                {
                    col.gameObject.GetComponent<ThrowObject>().combo_kill_timer = 10;
                } else
                {
                    col.gameObject.GetComponent<ThrowObject>().combo_kill_timer = 0.2f;
                }
                //Destroy(col.gameObject, 0.1f);

            }
            

        }
        //searching again the array to double check. uses a lamda function, where also I have no idea wtf that is, but it is basically a find index 
        else if (col.gameObject.tag == "Interactive" && col.gameObject.GetComponent<InteractiveSettings>().isCollectible && col.gameObject.activeSelf && System.Array.FindIndex(collectibles, o=>o == col.gameObject) != -1)
        {
            //Remove the object from the array
            GameObject[] collectablesNew;
            if (collectibles.Length > 1)
            {
                collectablesNew = new GameObject[collectibles.Length - 1];
            }
            else
            {
                collectablesNew = new GameObject[1];
            }

            //array counter
            int j = 0;
            for (int i = 0; i < collectibles.Length; i++)
            {
                if (col.gameObject != collectibles[i]) {
                    Debug.Log("collectable new: " + collectablesNew.Length + "\n collectables: " + collectibles.Length);
                    collectablesNew[j++] = collectibles[i];
                }
            }
            arena.GetComponent<SpawnController>().collectibles = collectablesNew;
            //-- end remove

            // Set Starting Position for particle trail
            CollectibleCollectedParticles.instance.start_position = col.gameObject.transform.position;
            col.gameObject.GetComponent<InteractiveSettings>().shrink_timer = 0.9f;
            dnd.instance.ReleaseObject();
            CollectibleCollectedParticles.instance.StartCoroutine("StartDissolve");

            //col.gameObject.SetActive(false);


            //Color[] colors = { Color.red, Color.magenta, Color.cyan, Color.green, Color.gray }; //for testing!
            //GetComponent<Renderer>().material.color = colors[collectibleCount]; //for testing

            

            collectibleCount++;
            collectSound = true;
            if (healthOnPickup)
            {
                currentHealth = maxHealth;
                healthBar.fillAmount = 1;
            }
            
            if (collectibleCount >= arena.GetComponent<SpawnController>().numberOfCollectibles)
            {
                arena.GetComponent<SpawnController>().allCollected = true;
            }

           

        }        
    }

  
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Shrine") { 

            in_shrine = false;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        
            if (col.gameObject.tag == "Shrine" && col.gameObject.GetComponent<Shrine>().shrine_id > 1)
            {
                if (col.gameObject.GetComponent<Shrine>().blessing_spawn_cooldown_timer <= 0)
                {
                    in_shrine = true;
                } else
                {
                    in_shrine = false;
                }
            
            }
        // Tests for players pressing spacebar while standing in shrine
        bool prayAttempt = CrossPlatformInputManager.GetButtonDown("Pray");
        if (col.gameObject.tag == "Shrine" && prayAttempt && col.gameObject.GetComponent<Shrine>().shrine_cooldown_timer <= 0 && col.gameObject.GetComponent<Shrine>().blessing_spawn_cooldown_timer <= 0)
            {
            if (col.gameObject.GetComponent<Shrine>().shrine_id > 1) { 
                col.gameObject.GetComponent<Shrine>().shrine_cooldown_timer = col.gameObject.GetComponent<Shrine>().shrine_cooldown;
                col.gameObject.GetComponent<Shrine>().StartCoroutine("StartPray");
                // UNCOMMENT AFTER PRESENTATION equipped_ability = col.gameObject.GetComponent<Shrine>().shrine_id;
                equipped_ability = 2;


                // Reset Other Abilities before activating new one
                {
                    display_shield.SetActive(false);
                    display_compass.SetActive(false);
                    display_slippy.SetActive(false);
                    display_clone.SetActive(false);
                    display_slow.SetActive(false);
                    //PlayerAudioSource.volume = 0.2f;
                    //PlayerAudioSource.pitch = 1.0f;
                    //PlayerAudioSource.PlayOneShot(audio_clips[5]);


                    slowEffect = false;
                    slips_left = 0;
                    compass_timer = 0;
                    shield_timer = 0;
                    GameObject[] clones = GameObject.FindGameObjectsWithTag("Clone");
                    foreach (GameObject clone in clones)
                    {
                        Destroy(clone.gameObject);
                    }
                    display_slip_uses.GetComponent<Text>().text = "";
                    compass.SetActive(false);
                    shield.SetActive(false);
                }
            }

            //for animation
            if(col.gameObject.GetComponent<Shrine>().shrine_id >= 2 && col.gameObject.GetComponent<Shrine>().shrine_id <= 6)
            {
                PlayerAnimControl.playPrayAnimation();
            }

            display_clone.SetActive(true);
            /*if (col.gameObject.GetComponent<Shrine>().shrine_id == 2)
            {
                display_clone.SetActive(true);
            }
            if (col.gameObject.GetComponent<Shrine>().shrine_id == 3)
            {
                display_shield.SetActive(true);
            }
            if (col.gameObject.GetComponent<Shrine>().shrine_id == 4)
            {
                display_compass.SetActive(true);
            }
            if (col.gameObject.GetComponent<Shrine>().shrine_id == 5)
            {
                display_slippy.SetActive(true);
            }
            if (col.gameObject.GetComponent<Shrine>().shrine_id == 6)
            {
                display_slow.SetActive(true);
            }
            */
            if (col.gameObject.GetComponent<Shrine>().shrine_id == 1 && arena.GetComponent<SpawnController>().allCollected)
            {
                //print("Player wins");
                human_win_screen.SetActive(true);
                Invoke("LoadMenu", 5);
            }
            
        }
    }

    public void LoadMenu()
    {
        human_win_screen.SetActive(false);
        god_win_screen.SetActive(false);
        SceneManager.LoadScene("Menu");
    }


}