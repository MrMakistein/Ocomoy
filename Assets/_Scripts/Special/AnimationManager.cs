using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}

    public void StartPlayerTrack()
    {
        CameraControl.instance.trackplayer = true;
        Movement.instance.move_block2 = false;
    }

    public void StartMainGameLoop()
    {
        GameManager.instance.SetMainGameLoop();
    }

    public void StartMusic()
    {
        AudioManager.instance.music_started = true;
    }

}
