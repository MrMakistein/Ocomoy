using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tornado : MonoBehaviour {

	
    public float radius = 12f;
    public float maxPullInLength = 24.96f;
    public float power = 50;
    public float spin = 50;
    public float upPower = 50;
    public float speed = 2;
    //The time the tronado turns to one direction.
    public float rotationTime = 1;
    public float rotationSpeed = 80;
    public float TimeAlive = 7;
    private float rotationTimer = 0;
    private float rotation;
    //-1 = left, 0 = NULL, 1 = right
    private int rotationDir = 0;
    
    //used to reduce code
    private Vector3 tempVector;
    Collider[] colliders;
    public GameObject[] interactives;

    // Use this for initialization
    void Start()
    {
        rotation = Random.Range(0, 360);
        Destroy(this.gameObject, TimeAlive);
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void Update()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 15); // ADJUST - Radius in which objects do damage around the tornado center
        foreach (Collider hitCollider in hitColliders) if (hitCollider.tag == "Interactive")
        {
            if (hitCollider.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 3 && !hitCollider.gameObject.GetComponent<InteractiveSettings>().isCollectible) // ADJUST - velocity needed for objects to deal damage.
            {
                hitCollider.gameObject.GetComponent<ThrowObject>().dmg_cooldown = hitCollider.gameObject.GetComponent<ThrowObject>().dmg_cooldown_max;
            }
        }


        colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider c in colliders)
        {
            if (c.GetComponent<Rigidbody>() == null)
            {
                continue;
            }
            Rigidbody rigidbody = c.GetComponent<Rigidbody>();
            if (Vector3.Distance(transform.position, c.transform.position) > maxPullInLength)
            {
                continue;
            }

            tempVector = (transform.position - c.transform.position) * power / Vector3.Distance(transform.position, c.transform.position);
            rigidbody.AddForceAtPosition(new Vector3(tempVector.x,0,tempVector.z), transform.position);
            rigidbody.AddForce(Quaternion.AngleAxis(90, Vector3.up) * (transform.position - c.transform.position).normalized * spin/Vector3.Distance(transform.position, c.transform.position));
            rigidbody.AddForce(Vector3.up * upPower / Vector3.Distance(transform.position, c.transform.position));
        }


        //Calculate the ramdom rotation of the tornado
        rotationTimer += Time.deltaTime;
        if (rotationTimer > rotationTime)
        {
            rotationDir = 0;
            rotationTimer = 0;
        }

        if (rotationDir == 0)
        {
            if(Random.Range(0,100) < 50)
            {
                rotationDir = -1;
            }
            else
            {

                rotationDir = 1;
            }
        }

        rotation += rotationDir * rotationSpeed * Time.deltaTime;

        //move the tornado
        this.gameObject.transform.eulerAngles = new Vector3(0,rotation,0);
        this.gameObject.transform.position += transform.forward * Time.deltaTime * speed;
    }
}


