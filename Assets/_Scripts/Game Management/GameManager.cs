using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	public GameObject m_character; //the character player
	public MenuManager m_menuManager;


	[Header("DEBUG")]
	public bool gamePaused; //for checking if game is paused

	void Awake()
	{
		GameManager.instance = this; //singleton
		gamePaused = false;
	}

	// Use this for initialization
	void Start () {
		m_character = GameObject.Find ("Player"); //autoinitialize player
		m_menuManager = this.gameObject.GetComponent<MenuManager>();

		SetGameState(GameState.startscreen);
	}
	
	// Update is called once per frame
	void Update () {
		#region responsible for managing / triggering state switch
		if (m_gameState == GameState.startscreen) {
			//TODO: set condition for disabling startScreen
			SetGameState(GameState.cutscene);
		} else if (m_gameState == GameState.cutscene) {
			//TODO: set condition for calling tutorial after finished cutscene
			SetGameState (GameState.tutorial);
		} else if (m_gameState == GameState.tutorial) {
			//TODO: set condition for determining the end of the tutorial and invoking the main game 
			//TODO: am besten in separate Szene auslagern --> SceneManager?
			SetGameState(GameState.mainGameLoop);	
		}

		//checking for pause input
		bool pauseAttempt = CrossPlatformInputManager.GetButtonDown ("Pause");
		///if(pauseAttempt && (GetGameState() == GameState.mainGameLoop) || (GetGameState() == GameState.pause)){ //Y U NO WORK!?
		if(GetGameState() == GameState.mainGameLoop || GetGameState() == GameState.pause) { //checking for pause query (pause only available during main game)
			if(pauseAttempt){
				//Debug.Log("pause attempt");
				SetGameState(GameState.pause);
			}
		} //end check for pause query
		#endregion 
	}

	#region State Machine
	public enum GameState { undefined, startscreen, cutscene, tutorial, mainGameLoop, pause, playerWins, godWins, end, restart } //namen für den status
	GameState m_gameState = GameState.undefined;



	public GameState GetGameState()
	{
		return m_gameState;
	}

	public void SetGameState(GameState theState)
	{
		switch (theState)
		{
		case GameState.undefined:
			break;
		case GameState.startscreen: //TODO: inkl. Menu with customization options? Plan whole startscreen as new scene!
			//TODO: @Maki enable startscreen
			EnableCharacter (false, false); //cant move anymore
			//Debug.Log ("### START SCREEN ###");


			//SetGameState (GameState.cutscene);
			//Debug.Log ("I'm a dumbass and went back to the initial case");
			break;
		case GameState.cutscene: //cut scene management (maybe make new scene? array?) 
			//Debug.Log ("### CUT SCENE ###");

			//TODO: check what cutscene to play
			//TODO: Set the correct gameState needed after the specific cutscene




			//---------- INFO für animationsfile ----------
			//m_introAnimation.Play(); //animation abspielen!
			//coroutine: state zeitverzögert setzen! (z.B. erst wenn animation fertig gespielt hat!)
			//StartCoroutine(SetStateDelayed(m_introAnimation.clip.length, GameState.mainGameLoop)); //starte die coroutine nach länge des animationsschnipsels (.clip.length) -> gehe in den mainGameloop!!!
			 

			break;
		case GameState.tutorial:
			Debug.Log ("### TUTORIAL ###");
			EnableCharacter(true, true); //TODO: evtl. only enable one at a time


			break;

		case GameState.pause:
			gamePaused = !gamePaused; 
			Debug.Log ("### GAME " + ((gamePaused) ? "WAS PAUSED " : "WAS UNPAUSED ") + "###");

			if (gamePaused) { //State was entered with intention of PAUSING
				EnableCharacter (false, false);
				m_menuManager.OpenMenu ();
                   
	            //this stops physics simulation -TK
	            Time.timeScale = 0;

			} else { //State was entered with intention of UNPAUSING
				EnableCharacter (true, true);

                //this starts the physics simulation again -TK
                Time.timeScale = 1;
                m_menuManager.CloseMenu ();

                //switch to mainGameLoop case
                theState = GameState.mainGameLoop;
				goto case GameState.mainGameLoop;
			}
			break;
		case GameState.mainGameLoop:
			Debug.Log ("### MAIN GAME LOOP ###");
			EnableCharacter(true, true);

            //this starts the physics simulation, when game was paused before -TK
            Time.timeScale = 1;


                break;
		case GameState.playerWins:
			Debug.Log ("### HUMANITY WINS ###");
			EnableCharacter (false, false);

			//TODO: trigger corresponding ending cutscenes, audio, make entries in highscore etc.


			break;
		case GameState.godWins:
			Debug.Log ("### GOD WINS ###");
			EnableCharacter (false, false);


			break;
		case GameState.end:
			Debug.Log ("### GAME END ###");
			EnableCharacter (false, false);

			//TODO: reset anything that needs resetting for restart to work properly
			//TODO: add entries to (planned) Game Statistics (total hits, all player/god wins, longest time played etc.)
			//TODO: add condition to trigger Game restart!

			break;
		case GameState.restart:
			Debug.Log ("### RESTART ###");
			RestartGame();



			break;
		default:
			
			break;
		}
		m_gameState = theState; //speichern des aktuellen states
	}
		
	//for triggering a state with delay (with coroutine)
	IEnumerator SetStateDelayed(float delayTime, GameState theState)
	{
		yield return new WaitForSeconds(delayTime);
		SetGameState(theState);
	}
	#endregion //state machine end

	#region Helper methods
	void RestartGame()
	{
        gamePaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //SceneManager.LoadScene("StartScreen"); //reload the scene //TODO: startscreen so einbinden
    }

    public void UnPause (){ //for un/pausing from outside (Menu exit)
		SetGameState(GameState.pause);
	}

    void EnableCharacter(bool enableOco, bool enableGod) //for halting interactivity 
    {

        ///---------- OCO EN/DISABLE ----------
        //TODO: @Maki whatever needs to be done when character should be disabled (input-sperre)
        if (enableOco) {

        } else {

        }


        ///---------- GOD EN/DISABLE ----------
        //TODO: @Theo whatever needs to be done when God is disabled (input-sperre)
        //TODO: @Theo check if below is bullshit <- it is ;D butt fixxed :P -TK
        //but do we really want to lock the cursor at any point? -TK
        if (enableGod) { 

            
            dnd.instance.enableGod = true;
        }
        else
        {
            dnd.instance.enableGod = false;
        }

    }

    public void doExitGame()
    {
        Application.Quit();
    }

    public void doRestartGame()
    {
        SetGameState(GameState.restart);
    }
    #endregion //helper methods end


}
