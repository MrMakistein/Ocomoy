using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FancyTether : MonoBehaviour {

    public Camera main;
    public Vector2 point1;
    public Vector3 point2;
    public GameObject tether_visual;
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        point1 = customCursor.instance.objectPoint2D;
        point2 = Input.mousePosition;
        var iTweenPath = this.GetComponent<iTweenPath>();

        if (dnd.draggingObject != null && dnd.instance.MouseVector != null)
        {
            iTweenPath.nodes[0] = dnd.draggingObject.transform.position;
            iTweenPath.nodes[1] = dnd.instance.MouseVector;
        }

        if (Input.GetKeyDown("space")) { 
            

            iTween.MoveTo(tether_visual, iTween.Hash("path", iTweenPath.GetPath("Tether"), "easetype", iTween.EaseType.linear, "time", 0.3, "oncompleteTarget", this.gameObject, "oncomplete", "Finish"));
        }


    }

    void Finish()
    {
        iTween.MoveTo(tether_visual, iTween.Hash("path", iTweenPath.GetPath("Tether"), "easetype", iTween.EaseType.linear, "time", 0.3f, "oncompleteTarget", this.gameObject, "oncomplete", "Finish"));
    }
}
