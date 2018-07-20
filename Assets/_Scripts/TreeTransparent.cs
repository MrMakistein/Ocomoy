using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeTransparent : MonoBehaviour {


    public GameObject[] leaves;
    public Color green = new Color(148f / 255f, 171f / 255f, 36f / 255f, 0.2f);
    int lerp = 0;

    public float alpha = 1;
    public float transition_Speed = 0.3f;
    public float transparancy = 0.6f;



    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (lerp == 1 && alpha >= transparancy)
        {
            alpha -= transition_Speed * Time.deltaTime;
            green = new Color(148f / 255f, 171f / 255f, 36f / 255f, alpha);
            foreach (GameObject leaf in leaves)
            {
                leaf.GetComponent<Renderer>().material.color = green;
            }
        }

        if (lerp == 0 && alpha < 1)
        {
            alpha += transition_Speed * Time.deltaTime;
            green = new Color(148f / 255f, 171f / 255f, 36f / 255f, alpha);
            foreach (GameObject leaf in leaves)
            {
                leaf.GetComponent<Renderer>().material.color = green;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.name == "Player")
        {

            lerp = 1;
        }


    }

    private void OnTriggerExit(Collider other)
    {

        if (other.name == "Player")
        {
            lerp = 0;
            
        }


    }
}
