using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    private GameObject player;
    private Camera currentCamera;
    private GameObject dummy;
    private Vector3 playerScreenPos;
    //These two values control at which distance the camera movement will trigger
    public float thresholdWidth = 50;
    public float thresholdHeight = 50;
    public float smoothTime = 0.2f;
    //This value will added to the position of the player, in the direction he is currently looking


    private Vector3 cameraVelocity = Vector3.zero;
    private Vector3 moveToPosition;

    private bool movementTriggered = false;

    public float additionToPosition = 5f;

    //Value that defines the intesity of the camera shake
    public float cameraShakeStrength = 0.2f;

    //Time if CameraShake is called without a timer
    public float defaultShakeTime = 0.1f;

    //Timer for camera shake
    private float cameraShakeTimer = 0;
    private float cameraShakeTime = 0;


    //Singelton
    public static CameraControl instance;

    private void Awake()
    {
        instance = this;
    }

    // Use this for initialization
    void Start() {
        //Find the Objects.
        player = GameObject.Find("Player");
        dummy = GameObject.Find("CameraDummy");
        currentCamera = Camera.main;
        //Convert screen dependent values, to fitting values for the current game screen.
        thresholdWidth = dnd.ScreenSizeCompensation(thresholdWidth);
        thresholdHeight = dnd.ScreenSizeCompensation(thresholdHeight);
    }
    // Update is called once per frame
    void Update() {

        
        //check for nullpointer
        if (player != null && dummy != null)
        {
            //transform the position of the player to the camera screen
            playerScreenPos = currentCamera.WorldToScreenPoint(player.transform.position);


     

            //if the player is on the edge of the screen --> realign camera to center
            if (playerScreenPos.x < thresholdWidth || Screen.width - thresholdWidth < playerScreenPos.x || playerScreenPos.y < thresholdHeight || Screen.height - thresholdHeight < playerScreenPos.y)
            {
                //Move to the player position, and a bit more in the direction he is facing. This is solved over cos and sin. They need radients of the angel. 
                moveToPosition = new Vector3(player.transform.position.x + (additionToPosition * Mathf.Sin(Mathf.Deg2Rad * player.transform.eulerAngles.y)), player.transform.position.y, player.transform.position.z + (additionToPosition * Mathf.Cos(Mathf.Deg2Rad * player.transform.eulerAngles.y)));
                movementTriggered = true;
            }
            if (movementTriggered)
            {
                //SmoothDamp is a function for smooth camera movement.
                dummy.transform.position = Vector3.SmoothDamp(dummy.transform.position, moveToPosition, ref cameraVelocity, smoothTime);
                if(Vector3.Distance(dummy.transform.position, moveToPosition) < 0.1)
                {
                    movementTriggered = false;
                }
            }

            //applys the camera shake
            if (cameraShakeTimer < cameraShakeTime)
            {
                // has to defferentiate between moving and standing still
                if (movementTriggered)
                {
                    dummy.transform.position = Vector3.Lerp(dummy.transform.position + new Vector3(Random.Range(-cameraShakeStrength, cameraShakeStrength), Random.Range(-cameraShakeStrength, cameraShakeStrength), Random.Range(-cameraShakeStrength, cameraShakeStrength)), dummy.transform.position, cameraShakeTimer / cameraShakeTime);
                }
                else
                {
                    dummy.transform.position = Vector3.Lerp(moveToPosition + new Vector3(Random.Range(-cameraShakeStrength, cameraShakeStrength), Random.Range(-cameraShakeStrength, cameraShakeStrength), Random.Range(-cameraShakeStrength, cameraShakeStrength)), moveToPosition, cameraShakeTimer / cameraShakeTime);

                }
                cameraShakeTimer += Time.deltaTime;
            }


        }
        else
        {
            Debug.LogError("Reference not found in CameraControl");
        }
	}

    public void CameraShake(float shakeTime)
    {
        cameraShakeTimer = 0;
        cameraShakeTime = shakeTime;
    }


    public void CameraShake()
    {
        cameraShakeTimer = 0;
        cameraShakeTime = defaultShakeTime;
    }
}

    