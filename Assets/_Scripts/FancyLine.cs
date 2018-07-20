using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

public class FancyLine : MonoBehaviour {

    //public Vector3 point1;
    //public Vector3 point2;
    public UILineRenderer lineRenderer;
    // Use this for initialization
    void Start () {
        lineRenderer = GetComponent<UILineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {

        if (dnd.draggingObject != null && dnd.instance.MouseVector != null)
        {
            //point1 = dnd.draggingObject.transform.position;
            //point2 = dnd.instance.MouseVector;
        }
   
        
        


    }
}
