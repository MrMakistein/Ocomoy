using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thunder : MonoBehaviour {
    public float radius = 12;
    public int timeStunned = 2;

    public GameObject lightning;
    private GameObject player;
    public GameObject bolt;
    Collider[] colliders;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");        
        
        if (player != null)
        {
            bolt = Instantiate(lightning, new Vector3(transform.position.x, 12.2f, transform.position.z), Quaternion.identity);
            colliders = Physics.OverlapSphere(transform.position, radius);
            //searches if the player was in reach of the lightning
            foreach (Collider c in colliders)
            {
                if (c.gameObject != player)
                {
                    continue;
                }
                else
                {
                    player.GetComponent<Movement>().stunned = true;
                    Invoke("unStunnPlayer", LightningTime());
                }
            }

            Destroy(bolt, 3);
            Destroy(this, GetComponent<AudioSource>().clip.length);
        }
        else
        {
            Debug.LogError("Player not found");
        }
    }

    //is used to unstunn the player, if the lightning effect was shorter than the audio signal
    private void unStunnPlayer()
    {
        player.GetComponent<Movement>().stunned = false;
    }


    private float LightningTime()
    {
        //calculates time stunned based on distance to the epicenter
        return timeStunned * (1-(Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(player.transform.position.x, player.transform.position.z)))/radius);
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
        unStunnPlayer();
        Destroy(this.gameObject);
    }  

}

