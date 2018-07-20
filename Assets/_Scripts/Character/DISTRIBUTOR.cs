using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//NEVERMIND: Probleme mit Static und dynamic :ccc -> wenn instance müssen verweise static sein, aber dann gibt es keine aktuellen Werte aus Player etc. 
//-> evtl. besser Container-Klasse??? 
public class DISTRIBUTOR : MonoBehaviour {
    public static DISTRIBUTOR instance = null;
    #region player

    [Header("Player")]
    public static GameObject _player;
    [Space(5)]
    //STATIC VARIABLES
    public float _totalHealth;

    //DYNAMIC VARIABLES
    //public float _currentHealth;
    #endregion player

    #region Menu
    [Header("UI Menu Stuff")]
    public static GameObject _Canvas;
    #endregion Menu


    public void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);
       // _player.GetComponent<>
    }

    // Use this for initialization
    void Start () {
        _totalHealth = _player.GetComponent<Player>().maxHealth;
        //_currentHealth = _player.GetComponent<Player>().currentHealth;
		
	}
	

    //public float GetCurrentHealth()
    //{
    //    return _player.GetComponent<Player>().currentHealth;
    //}
}
