using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClone : MonoBehaviour {

    private GameObject player;

    public float rotation;
    public float rotation_speed = 10;
    public float offset = 2;
    public float speed = 10;
    public float expandTime = 2;
    private float expandTimer = 0;
    private float cloneTime;
    private float cloneTimer = 0;
    public int cloneAmount = 0;

    // Use this for initialization

    private GameObject[] Clones;
    
	void Start ()
    {
        player = GameObject.Find("Player");
        this.transform.position = (player.transform.position + new Vector3(offset * Mathf.Sin(Mathf.Deg2Rad * rotation), player.transform.position.y, offset * Mathf.Cos(Mathf.Deg2Rad * rotation)));

        cloneTime = player.GetComponent<Player>().clone_time;
    }

    // Update is called once per frame
    void LateUpdate () {

        if (cloneTimer > cloneTime)
        {
            Destroy(this.gameObject);
        }
        else
        {
            cloneTimer += Time.deltaTime;
        }

        //position update

        if (expandTimer < expandTime)
        {
            this.transform.position = Vector3.Lerp(transform.position, (player.transform.position + new Vector3(offset * Mathf.Sin(Mathf.Deg2Rad * rotation), 0, offset * Mathf.Cos(Mathf.Deg2Rad * rotation))), expandTimer/expandTime);
            expandTimer += Time.deltaTime;
        }
        else
        {
            this.transform.position = (player.transform.position + new Vector3(offset * Mathf.Sin(Mathf.Deg2Rad * rotation), 0, offset * Mathf.Cos(Mathf.Deg2Rad * rotation)));
        }

        this.transform.rotation = player.transform.rotation;

        Clones = GameObject.FindGameObjectsWithTag("Clone");
        if(cloneAmount != Clones.Length)
        {
            for (int i = 0; i < Clones.Length; i++)
            {
                if (Clones[i] == this.gameObject)
                {
                    rotation = (360.0f / Clones.Length) * i;
                }
            }
            cloneAmount = Clones.Length;
        }
         rotation += rotation_speed * Time.deltaTime;
    }
    

    private void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.tag == "Interactive" &&
       other.gameObject.GetComponent<ThrowObject>().dmg_cooldown >= 1 &&
       Player.instance.hit_cooldown_timer <= 0 &&
       !Player.instance.GetComponent<Movement>().move_block &&
       System.Array.FindIndex(Player.instance.collectibles, o => o == other.gameObject) == -1)
        {
            Debug.Log(other);
            //trow the object in a random direction
            other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-10.0F, 10.0F), Random.Range(8.0F, 12.0F), Random.Range(-10.0F, 10.0F));
            other.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(Random.Range(-10.0F, 10.0F), Random.Range(8.0F, 12.0F), Random.Range(-10.0F, 10.0F));


            dnd.instance.ReleaseObject();
            //if a clone was hit, destroy him
            Destroy(this.gameObject, 0.1f);



        }
        //GameObject selectedClone = null;
        //foreach (ContactPoint c in col)
        //if (col.gameObject.tag == "Clone")
        //{
        //    Debug.Log("Clone hit!");
        //    selectedClone = col.gameObject;
        //    Debug.Log(selectedClone);
        //}
        //}
    }
}
    