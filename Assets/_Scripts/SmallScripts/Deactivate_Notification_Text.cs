using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Deactivate_Notification_Text : MonoBehaviour
{

    private float DisplayTimer = 0;
    private float DisplayTime = 2;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup < DisplayTimer)
        {
            this.gameObject.GetComponent<Text>().color = new Color(0, 0, 0, (DisplayTimer - Time.realtimeSinceStartup));
        } else
        {
            this.gameObject.GetComponent<Text>().color = new Color(0, 0, 0, 0);
            
        }

    }

    public void SetNotification()
    {
        DisplayTimer = Time.realtimeSinceStartup + DisplayTime;


    }
}
