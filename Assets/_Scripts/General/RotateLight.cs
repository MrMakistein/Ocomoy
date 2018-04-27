using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLight : MonoBehaviour {
    public float _rotationSpeed = 100.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * _rotationSpeed, Space.World);
        Debug.Log("Pos:" + Input.GetAxis("Mouse X"));
        Vector3 rotateX = new Vector3(0, Input.GetAxis("Mouse X") * _rotationSpeed, 0);
        Vector3 rotateY = new Vector3 (- Input.GetAxis("Mouse Y") * _rotationSpeed,0, 0);

        //transform.Rotate(new Vector3(/*Input.GetAxis("Mouse Y")*/0, Input.GetAxis("Mouse X"), 0) * Time.deltaTime * _rotationSpeed);
        transform.Rotate(rotateX * Time.deltaTime * _rotationSpeed, Space.Self);
        transform.Rotate(rotateY * Time.deltaTime * _rotationSpeed, Space.Self);
    }
}
