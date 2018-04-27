using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rain : MonoBehaviour {


    public float radius = 12;
    public float TimeAlive = 10;

    private GameObject player;
    
    Collider[] colliders;

    // Use this for initialization
    void Start()
    {
        Destroy(this.gameObject, TimeAlive);
        player = GameObject.Find("Player");
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Update()
    {
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
                    player.GetComponent<Movement>().SetSliding();
                }
            }
        }
        else
        {
            Debug.LogError("No player defined in rain effect!");
        }
    }
}
