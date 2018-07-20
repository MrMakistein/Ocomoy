using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using _Decal;

public class UpdateDecalPosition : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.SetPositionAndRotation(this.GetComponentInParent<Transform>().position, this.GetComponentInParent<Transform>().rotation);
        DecalBuilder.BuildAndSetDirty(this.GetComponent<Decal>());
	}
}
