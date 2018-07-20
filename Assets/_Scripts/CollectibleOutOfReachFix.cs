using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleOutOfReachFix : MonoBehaviour {

    public Vector3 original_position;
    public Vector3 middle_position;
    public Vector3 current_position;
    public Vector3 reset_position;
    public Vector3 original_scale;
    public Vector3 current_scale;
    public float scale_factor = 1;



    public string pathName;
    public float time;
    public GameObject dissolveparticles;
    public GameObject reset_collectible;


    // Use this for initialization
    void Start () {
        original_position = new Vector3(0, 0, 0);
        
    }
	
	// Update is called once per frame
	void Update () {

        //Shrink collectible when teleport off of platform
        if (scale_factor < 1)
        {
            current_scale.x = current_scale.x * scale_factor;
            current_scale.y = current_scale.y * scale_factor;
            current_scale.z = current_scale.z * scale_factor;
            reset_collectible.transform.localScale = current_scale;

            if (current_scale.x <= 0.02)
            {
                scale_factor = 1;
            }
            reset_collectible.GetComponent<InteractiveSettings>().collectibleOnFloor = true;
        }

        //Scale collectible back to original size at end of teleportation
        if (scale_factor > 1)
        {
            current_scale.x = current_scale.x * scale_factor;
            current_scale.y = current_scale.y * scale_factor;
            current_scale.z = current_scale.z * scale_factor;
            reset_collectible.transform.localScale = current_scale;

            if (current_scale.x >= original_scale.x)
            {
                scale_factor = 1;
                reset_collectible.transform.localScale = original_scale;
            }
            reset_collectible.GetComponent<InteractiveSettings>().collectibleOnFloor = true;
        }




        //Detect if collectible is at illegal position
        foreach (GameObject col in Player.instance.collectibles) {
            if (col != null) { 
                if (!col.GetComponent<InteractiveSettings>().collectibleOnFloor){

                    if (col.GetComponent<Rigidbody>().velocity == new Vector3(0, 0, 0) && dnd.draggingObject == null) {
                    


                    
                        reset_collectible = col;
                        reset_position = original_position;
                        var iTweenPath = this.GetComponent<iTweenPath>();
                        current_position = col.transform.position;

                        original_scale = col.transform.localScale;
                        current_scale = original_scale;
                        scale_factor = 0.94f;
                        iTweenPath.nodes[0] = new Vector3(current_position.x, current_position.y, current_position.z);
                        iTweenPath.nodes[1] = new Vector3(original_position.x, original_position.y, original_position.z);
                        dissolveparticles.SetActive(true);
                        dissolveparticles.transform.position = new Vector3(current_position.x, current_position.y, current_position.z);
                        StartCoroutine("DissolveCollectible");


                   


                        col.GetComponent<InteractiveSettings>().collectibleOnFloor = true;
                    }
                }
            }
        }

        // Update reset position for collectible
        if (dnd.draggingObject != null)
        {
            RaycastHit hit;
            RaycastHit hit2;
            RaycastHit hit3;
            RaycastHit hit4;
            Ray landingRay = new Ray(new Vector3(current_position.x - 0.5f, current_position.y, current_position.z), Vector3.down);
            Ray landingRay2 = new Ray(new Vector3(current_position.x + 0.5f, current_position.y, current_position.z), Vector3.down);
            Ray landingRay3 = new Ray(new Vector3(current_position.x, current_position.y, current_position.z - 0.5f), Vector3.down);
            Ray landingRay4 = new Ray(new Vector3(current_position.x, current_position.y, current_position.z + 0.5f), Vector3.down);


            //current_position = dnd.draggingObject.transform.position;
            current_position = dnd.draggingObject.GetComponent<Collider>().bounds.center;

            float dist = Vector3.Distance(middle_position, current_position);
            if (dist >= 3)
            {
               
                if (Physics.Raycast(landingRay, out hit, Mathf.Infinity) && 
                    Physics.Raycast(landingRay2, out hit2, Mathf.Infinity) &&
                    Physics.Raycast(landingRay3, out hit3, Mathf.Infinity) &&
                    Physics.Raycast(landingRay4, out hit4, Mathf.Infinity))
                {
                    if (hit.collider.tag == "isGround" && hit2.collider.tag == "isGround" && hit3.collider.tag == "isGround" && hit4.collider.tag == "isGround")
                    {
                        original_position = middle_position;
                        //middle_position = dnd.draggingObject.transform.position;
                        middle_position = dnd.draggingObject.GetComponent<Collider>().bounds.center;



                        //Debug.Log("updated original position");



                    }
                }
            }
        }

  
    }

    IEnumerator DissolveCollectible()
    {
        
        //Start Particles
        dissolveparticles.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.7f);


        //Hide collectible
        reset_collectible.SetActive(false);
        yield return new WaitForSeconds(0.05f);

        //Start path
        iTween.MoveTo(dissolveparticles, iTween.Hash("path", iTweenPath.GetPath(pathName), "easetype", iTween.EaseType.easeInOutSine, "time", time, "oncompleteTarget", this.gameObject, "oncomplete", "Finish"));
        yield return new WaitForSeconds(1.6f);

        reset_collectible.transform.position = reset_position;
        reset_collectible.SetActive(true);
        scale_factor = 1.1f; //Scale collectible back up


    }


    void Finish()
    {
        reset_collectible.transform.position = reset_position;

    }


    void OnTriggerExit(Collider other)
    {
        var script = other.gameObject.GetComponent<InteractiveSettings>();

        if (script != null)
        {

            if (other.gameObject.GetComponent<InteractiveSettings>().isCollectible)
            
                other.gameObject.GetComponent<InteractiveSettings>().collectibleOnFloor = false;
            }
        }

    

    void OnTriggerEnter(Collider other)
    {
             
        var script = other.gameObject.GetComponent<InteractiveSettings>();

        if (script != null)
        {

            if (other.gameObject.GetComponent<InteractiveSettings>().isCollectible)
            {
                
                other.gameObject.GetComponent<InteractiveSettings>().collectibleOnFloor = true;
            }
        }

    }
}
