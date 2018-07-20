using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioMananger : MonoBehaviour
{


    public AudioClip[] footStep_normal;
    public AudioClip[] footStep_water;
    public AudioSource PlayerAudioSource;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FootStep()
    {
        //Takes a random footstep sound from the footstep array
        if (Player.instance.in_water)
        {
            PlayerAudioSource.PlayOneShot(footStep_water[(int)(Random.value * footStep_water.Length) % footStep_water.Length]);
        } else
        {
            PlayerAudioSource.PlayOneShot(footStep_normal[(int)(Random.value * footStep_normal.Length) % footStep_normal.Length]);
        }
      

       

    }
}