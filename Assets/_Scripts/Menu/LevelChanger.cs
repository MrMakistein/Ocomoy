using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


public class LevelChanger : MonoBehaviour {

    public Animator animator;
    public GameObject video_object;
    public Animation anim1;


    public static LevelChanger instance;

    private void Awake()
    {
        instance = this;
    }


    // Use this for initialization
    void Start () {
        video_object.GetComponent<VideoPlayer>().Prepare();
	}
	
	

    public void SkipIntro()
    {
        animator.SetTrigger("Skip");
    }

    public void StartIntroFade()
    {
        animator.SetTrigger("FadeOut");

    }
    public void IntroFinish()
    {
        SceneManager.LoadScene("LevelV9");
        Debug.Log("load scene");

    }

    public void ActivateSkippable()
    {
        BtnManager.instance.skippable = true;

    }

    public void DeactivateSkippable()
    {
        BtnManager.instance.skippable = false;

    }

    public void StartIntroScene()
    {
        video_object.GetComponent<VideoPlayer>().Play();

        //((MovieTexture)video_object.GetComponent<Renderer>().material.mainTexture).Play();
        Debug.Log("start vid");

    }
}
