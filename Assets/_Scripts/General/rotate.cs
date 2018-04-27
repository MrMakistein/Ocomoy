using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour {
    public float _rotationSpeed = 30.0f;


	// Use this for initialization
	void Start () {
		 
	}
	
	// Update is called once per frame
	void Update () {
        // Rotate the object around its local X axis at 1 degree per second
        transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * _rotationSpeed, Space.World);
        
    }
}
