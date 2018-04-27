using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earthshatter : MonoBehaviour {
    public float radius = 12;
    public int timeReversed = 2;
    //texture is used to fade out the texture at the end of the effect
    public Material Shatter;
    //used to determin th time the object fades to compensate errors set to 1 at the start
    private float timeToDestroy = 1;
    public float fadeTime = 1;
    private bool startFade = false;
    private GameObject player;
    Collider[] colliders;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //Reset the material and fade
        startFade = false;
        Shatter.color = new Color(Shatter.color.r, Shatter.color.g, Shatter.color.b, 1);
        if (player != null)
        {
            colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider c in colliders)
            {
                if (c.gameObject != player)
                {
                    continue;
                }
                else
                {

                    player.GetComponent<Movement>().reversed = true;
                    //reset the shatter color (alpha to 1)
                    timeToDestroy = destroyTime();
                    //Subroutine to start fading
                    Invoke("startToFade", timeToDestroy - fadeTime);

                    //Destroys the object after destroytime
                    Destroy(this, timeToDestroy);
                }
            }
        }
        else
        {
            Debug.LogError("Player not found");
        }
    }

    void Update()
    {
        if (startFade)
        {
            Shatter.color = new Color(Shatter.color.r, Shatter.color.g, Shatter.color.b, Shatter.color.a <= 0 ? 0 : (Shatter.color.a - (Time.deltaTime / fadeTime)));
        }
    }

    private float destroyTime()
    {
        //calculates time reversed based on distance to the epicenter
        return timeReversed * (1-(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)))/radius);
    }
    
    private void startToFade()
    {
        startFade = true;
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void OnDestroy()
    {
        foreach (Transform child in transform)
        {
                Destroy(child.gameObject);
        }
        player.GetComponent<Movement>().reversed = false;
        Destroy(this);
    }  
    
}

