using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionToCutoutShader : MonoBehaviour {
    Transform playerTransform;
	// Use this for initialization
	void Start () {
        playerTransform = GetComponent<Transform>();
	}
	// Update is called once per frame
	void Update () {
        Shader.SetGlobalVector(Shader.PropertyToID("_CutObjPos"), Camera.main.WorldToScreenPoint(playerTransform.position));

        Shader.SetGlobalVector(Shader.PropertyToID("_MousePos"), Input.mousePosition);
    }
}
