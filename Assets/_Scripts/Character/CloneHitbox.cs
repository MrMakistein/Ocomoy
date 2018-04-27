using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneHitbox : MonoBehaviour {

    public GameObject clone1;
    public GameObject clone2;
    public GameObject clone3;
    public GameObject clone4;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.name);

        if (col.gameObject.tag == "Interactive")
        {
            if (this.name == "CloneHitbox1")
            {
                Invoke("KillClone1", 0.08f);
            }
            if (this.name == "CloneHitbox2")
            {
                Invoke("KillClone2", 0.08f);
            }
            if (this.name == "CloneHitbox3")
            {
                Invoke("KillClone3", 0.08f);
            }
            if (this.name == "CloneHitbox4")
            {
                Invoke("KillClone4", 0.08f);
            }

        }
    }

    void KillClone1()
    {
        gameObject.SetActive(false);
        clone1.SetActive(false);
    }
    void KillClone2()
    {
        gameObject.SetActive(false);
        clone2.SetActive(false);
    }
    void KillClone3()
    {
        gameObject.SetActive(false);
        clone3.SetActive(false);
    }
    void KillClone4()
    {
        gameObject.SetActive(false);
        clone4.SetActive(false);
    }
}
