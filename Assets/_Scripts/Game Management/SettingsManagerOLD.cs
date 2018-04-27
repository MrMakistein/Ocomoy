using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManagerOLD : MonoBehaviour {
	//FOR LOADING/SAVING MENU
	//public string path = "D:\\Leo\\Documents\\FH Bachelor MTD\\WS17-18\\PRO3\\COLLAB\\Ocomoy V2\\Ocomoy\\CustomSettingsTEST\\OcomoySettings.txt"; //for saving the settings file (JSON format)
	private string logPath;// = Application.dataPath + "\\MISC\\OcomoySettingsFile.txt";
	private string path;
	[Header("GameObjects")]
	//SETTINGS REGARDING PLAYER
	private GameObject _player;
	//SETTINGS REGARDING GOD
	[Space(10)]
	private GameObject _god;
	[Space(10)]
	//panels etc.
	private GameObject _canvas;
	private GameObject _checkbox_dashToggle;
	private GameObject _slider_regenSpeed;
	private GameObject _input_notes;

	
	//GENERAL SETTINGS

	[Header("SETTINGS VARIABLES")]
	//SETTINGS VARIABLES
	public bool _playerDashEnabled;
	//public double _slider_DashSpeed;
	public double _playerSpeed;
	public double _playerRegenSpeed;
	/* //for later: animation finetuning:
	private double _playerHairDamping;
	private double _playerHairElasticity;
	private double _playerHairStiffness;
	*/
	public string notes;


	// Use this for initialization
	void Start () {
		//Autointialize
		_player = GameObject.Find ("Player");
		_god = GameObject.Find ("TheosGott");
		_canvas = GameObject.Find ("Canvas_PauseOverlay");

		_checkbox_dashToggle = GameObject.Find ("P_DashToggle");
		////_slider_dashSpeed = _player.GetComponent<Dash>().dashSpeed;
		_slider_regenSpeed = GameObject.Find("P_RegenSpeed");
		_input_notes = GameObject.Find ("AdditionalNotes");
		//TODO: create/initialize other UI GameObjects for the settings!


		logPath = Application.dataPath + "/MISC/OcomoySettingsLog.txt";
		path = Application.dataPath + "/MISC/OcomoyLatestSettings.txt";

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//for parsing and loading the settings from a json object

	public void LoadSettingsFromJSON(){

		/*
		if (File.Exists(path)) {
			Debug.Log ("loading");
			string lastSettings = File.ReadAllText (path);
			if (lastSettings.Length > 0) {
				//JsonUtility.FromJsonOverwrite (File.ReadAllText (path), this);
				SettingsManager settingsToLoad = JsonUtility.FromJson(path, this); //create settings object
				//_player.GetComponent<Dash> ().dashSpeed = settingsToLoad._slider_dashSpeed;

				//manually overwrite all relevant values from settings
				this.notes = settingsToLoad.notes;
				this._playerSpeed = settingsToLoad._playerSpeed;
			}
		}
		*/
		//TODO: @Leo CHANGE: read stuff from json but set values manually! (or it will only be the settings for this object, not for linked stuff u dimwit)"
	}

	public void WriteSettingsIntoJSON(){
		notes = _input_notes.GetComponentInChildren<Text> ().text;
		//Debug.Log ("Text entered: " + notes);

		System.DateTime currentTime = System.DateTime.Now;
		string timeStamp = "Date: " + currentTime.Day + "." + currentTime.Month + "." + currentTime.Year + ", \tTime: " + currentTime.Hour + ":" + currentTime.Minute + ":" + currentTime.Second;


		string settings = "\n------------- SETTINGS -------------\n" +timeStamp + "\n" + JsonUtility.ToJson (this, true) + "\n";

		// This text is added only once to the file.
		if (!File.Exists (logPath)) {
			// Create a file to write to.
			File.WriteAllText (logPath, settings);
		} else {
			string appendText = settings + "\n_____________\nthis was not overwritten!\n";
			File.AppendAllText (logPath, appendText);
		}
		File.WriteAllText (path, JsonUtility.ToJson(this)); //only for loading from settings; saves latest settings. always overwrite!
	}
}

