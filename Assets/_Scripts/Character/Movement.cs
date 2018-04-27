﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Movement : MonoBehaviour
{
    //movement Speed
    public float mSpeed = 7f;
    public float slideTime = 3;
    public float slowTime = 2;
    public float slowMultiplicator = 2;


    public bool move_block = false;
    public bool stunned = false;
    public bool reversed = false;

    //Used to adjust sliding-force
    private float forceMultiplier = 10f;

    //store initial values
    private float initalDrag;
    public float initialmSpeed;

    //states
    private bool sliding = false;
    private bool slow = false;

    //used to measure the duration of a effect
    private float slowTimer = 0;
    private float slideTimer = 0;

    void Start()
    {
        initalDrag = GetComponent<Rigidbody>().drag;
        initialmSpeed = mSpeed;
    }

    void Update()
    {

        move_block = false;

        GameObject[] shrines = GameObject.FindGameObjectsWithTag("Shrine");
        foreach (GameObject shrine in shrines)
        {
            if (shrine.GetComponent<Shrine>().shrine_cooldown_timer > 0)
            {
                move_block = true;
            }
        }

        if (!GameManager.instance.gamePaused && !stunned && !move_block && this.GetComponent<Dash>().dashTimer <= 0)
        {
            #region oldMovment
            /*
            //bool a = (CrossPlatformInputManager.GetButtonDown("Horizontal") && CrossPlatformInputManager.GetAxisRaw("Horizontal") < 0);
            // Moves the player forward if WAS or D is pressed
            if (Input.GetAxis("Vertical") < 0||
                Input.GetAxis("Vertical") > 0 ||
                Input.GetAxis("Horizontal") < 0 ||
                Input.GetAxis("Horizontal") > 0)
            {
                //adjust the speed if slowed
                mSpeed = slow ? (initialmSpeed / slowMultiplicator) : (initialmSpeed);

                //move differently for sliding and not sliding
                if (sliding)
                {
                    
                    //Add Force to the character --> floaty feel
                    gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * forceMultiplier * GetComponent<Rigidbody>().mass * (1 - (gameObject.GetComponent<Rigidbody>().velocity.magnitude) / mSpeed));
                    

                }
                else
                {
                    //set the velocity of the character --> responsive
                    gameObject.GetComponent<Rigidbody>().velocity = transform.forward * mSpeed;
                }
            }
            */
            #endregion

            //temporary Velocity variable to shorten code
            Vector3 tempVelo = gameObject.GetComponent<Rigidbody>().velocity;

            Vector3 Direction = new Vector3(CrossPlatformInputManager.GetAxisRaw("Horizontal"), 0, CrossPlatformInputManager.GetAxisRaw("Vertical"));
            Vector3 DirectionVelocity = Direction.normalized * mSpeed;

            //check for horizontal input
            if (CrossPlatformInputManager.GetAxisRaw("Horizontal") != 0)
            {
                //set the horizontal movement of the character according to the input manager
                tempVelo = new Vector3(CrossPlatformInputManager.GetAxisRaw("Horizontal") * mSpeed, tempVelo.y, tempVelo.z);
            }

            //check for vertical input
            if (CrossPlatformInputManager.GetAxisRaw("Vertical") != 0)
            {
                //set the vertical movement of the character according to the input manager
                tempVelo = new Vector3(tempVelo.x, tempVelo.y, CrossPlatformInputManager.GetAxisRaw("Vertical") * mSpeed);
            }

            //set the calculated velocity to the character velocity
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(DirectionVelocity.x, tempVelo.y, DirectionVelocity.z);


            #region OldRotate
            
            // Rotates the player into the correct direction
            Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));
            movement *= reversed ? -1 : 1;
            if (movement != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movement), 0.15F);
            }
            
            #endregion
        }

        //Check if timer are finished
        if (sliding && slideTime < slideTimer)
        {
            GetComponent<Rigidbody>().drag = initalDrag;
            sliding = false;
        }
        else
        {

            //Increase Timer
            slideTimer += Time.deltaTime;
        }

        if (slow && slowTime < slowTimer)
        {
            slow = false;

        }
        else
        {
            //Increase Timer
            slowTimer += Time.deltaTime;
        }

    }

    public void SetSliding()
    {
        sliding = true;
        slideTimer = 0;
        GetComponent<Rigidbody>().drag = 0.3f;
    }

    public void SetSlow()
    {
        slow = true;
        slowTimer = 0;
    }
}
