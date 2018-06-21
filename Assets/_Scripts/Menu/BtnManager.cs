using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnManager : MonoBehaviour {
    public GameObject level_changer;
    public bool skippable = false;

    public static BtnManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void NewGame(string newGameLevel)
    {
        //SceneManager.LoadScene(newGameLevel);
        level_changer.GetComponent<LevelChanger>().StartIntroFade();
    }

    public void SkipIntro()
    {
        if (skippable) { 
            Debug.Log("skip_intro");
            LevelChanger.instance.SkipIntro();
            skippable = false;
        }
    }

}
