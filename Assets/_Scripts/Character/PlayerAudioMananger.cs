using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioMananger : MonoBehaviour
{


    public AudioClip[] footStep;
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
        PlayerAudioSource.PlayOneShot(footStep[(int)(Random.value * footStep.Length) % footStep.Length]);
    }
}