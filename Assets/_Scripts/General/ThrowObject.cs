using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public bool update_weight = true;
    [HideInInspector]public int weight_class = 0;
    public static float weight_class_1_limit = 3;
    public static float weight_class_2_limit = 10;
    public static float weight_class_3_limit = 18;
    public float dmg_cooldown = 10;
    public float dmg_cooldown_max = 10;
    public int object_damage = 4;
    public bool isclone = false;
    public float combo_kill_timer = 0;
    public float combo_kill_speed;
    Vector3 finalDirection;
    private float initialMass;
    public GameObject mesh;

    // Use this for initialization
    void Start()
    {
        combo_kill_speed = 0.08f;
        initialMass = gameObject.GetComponent<Rigidbody>().mass;
        if (update_weight == true)
        {
            if (gameObject.GetComponent<Rigidbody>().mass < weight_class_1_limit)
            {
                weight_class = 1;
            }
            else if (gameObject.GetComponent<Rigidbody>().mass < weight_class_2_limit)
            {
                weight_class = 2;
            }
            else if (gameObject.GetComponent<Rigidbody>().mass < weight_class_3_limit)
            {
                weight_class = 3;
            }
            else
            {
                weight_class = 4;
            }
        }
    }

    void FixedUpdate()
    {
        if (dnd.draggingObject != null)
        {
            dnd.draggingObject.GetComponent<ThrowObject>().dmg_cooldown = dmg_cooldown_max;
        }

        if (combo_kill_timer > 0)
        {

            if (combo_kill_timer <= 10)
            {
                mesh.SetActive(false);
                this.GetComponent<BoxCollider>().enabled = false;
            }

            combo_kill_timer = combo_kill_timer - combo_kill_speed;
            if (combo_kill_timer <= 0)
            {
                Destroy(this.gameObject);
            }
        }

        // Dmg cooldown gradually goes back to 0 
        if (dmg_cooldown > 0)
        {
            dmg_cooldown -= Time.deltaTime * 10;

        }
    }


    // Update is called once per frame
    void Update()
    {
        /* if (clonehit_timer > 0)
         {
             clonehit_timer -= Time.deltaTime * 10;
         }
         */

        // If no object is selected the dmg_cooldown for the interactive object is reset to the maximum
       

        if (InitialMass != GetComponent<Rigidbody>().mass && !Player.slowEffect)
        {
            GetComponent<Rigidbody>().mass = InitialMass;
        }
    }


    public float InitialMass
    {
        get
        {
            return initialMass;
        }

        //If set, update the weight class and so on
        set
        {
            initialMass = value;
            gameObject.GetComponent<Rigidbody>().mass = initialMass;
            if (gameObject.GetComponent<Rigidbody>().mass < weight_class_1_limit)
            {
                weight_class = 1;
            }
            else if (gameObject.GetComponent<Rigidbody>().mass < weight_class_2_limit)
            {
                weight_class = 2;
            }
            else if (gameObject.GetComponent<Rigidbody>().mass < weight_class_3_limit)
            {
                weight_class = 3;
            }
            else
            {
                weight_class = 4;
            }

        }
    }

}
