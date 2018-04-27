using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

/**
 * Responsible for handling settings menu basic functions (logging settings and calling functions to adjust settings accordingly). 
 * Accessed by GameManager.
 * Must be attached to the GameManager Object to work.
 **/
public class MenuManager : MonoBehaviour {
    //FOR LOADING/SAVING MENU
    //public string path = "D:\\Leo\\Documents\\FH Bachelor MTD\\WS17-18\\PRO3\\COLLAB\\Ocomoy V2\\Ocomoy\\CustomSettingsTEST\\OcomoySettings.txt"; //for saving the settings file (JSON format)
    public string logPath; //for the logging file
    public string path; //for last settings copy

    private GameObject m_pauseOverlay; //the canvas of the whole pause overlay
    private Settings _settings;
    private string _settingsCopy; //loaded when "revert" is requested (saves last settings)
    private string _defaultSettings;
    //UI ELEMENTS
    private Toggle dashToggle;
    private GameObject[] test;
    private Toggle heightLockToggle;
    private Toggle musicToggle;
    private Slider screenShakeSlider;
    private Slider ratingSlider;
    private Dropdown charSpeedDropdown;
    private Dropdown healthSystemDropdown;
    private Dropdown objectSpeedDropdown;
    private Dropdown musicDropdown;
    //private InputField notes;

    private Button saveData;

    #region prevent Esc from deleting notes content
    private InputField notes;
    string notesBackup;
    // call in "on value Changed" in Input Field settings
    public void OnEditting() {
        if (!Input.GetKeyDown(KeyCode.Escape)) {
            notesBackup = notes.text; //writing backup
        }
    }

    // call in "on end edit" in Input Field settings
    public void OnEndEdit() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            notes.text = notesBackup;
            _settings.SetNotes(notesBackup);
        }
    }
    #endregion

    public void Awake() {
        //careful: must be found before settings canvas is set to inactive!! (go.Find() only returns active elements)
        //AUTOINITIALIZING UI ELEMENTS
        dashToggle = GameObject.Find("TOGGLE_DashEnabled").GetComponent<Toggle>();
        heightLockToggle = GameObject.Find("TOGGLE_HeightLockEnabled").GetComponent<Toggle>();
        musicToggle = GameObject.Find("TOGGLE_MusicEnabled").GetComponent<Toggle>();
        charSpeedDropdown = GameObject.Find("DROPDOWN_PlayerSpeed").GetComponent<Dropdown>();
        healthSystemDropdown = GameObject.Find("DROPDOWN_HealthVariation").GetComponent<Dropdown>();
        objectSpeedDropdown = GameObject.Find("DROPDOWN_ObjectSpeed").GetComponent<Dropdown>();
        musicDropdown = GameObject.Find("DROPDOWN_MusicVariation").GetComponent<Dropdown>();
        notes = GameObject.Find("INPUTFIELD_Notes").GetComponent<InputField>();
        screenShakeSlider = GameObject.Find("SLIDER_ScreenshakeMode").GetComponent<Slider>();
        ratingSlider = GameObject.Find("SLIDER_Xp").GetComponent<Slider>();

        saveData = GameObject.Find("BUTTON_Send_Nudes").GetComponent<Button>();
    }

    void Start() {
        _settings = this.gameObject.GetComponent<Settings>();
        SetDefault();
        m_pauseOverlay = GameObject.Find("Canvas_PauseOverlay");
        m_pauseOverlay.SetActive(false); //deactivate pause menu

        //(hopefully) platform independent relative paths
        string logPathPart = "LOGDATA (please send us this folder)" + Path.DirectorySeparatorChar + "OcomoySettingsLog.txt";
        string pathPart = "LOGDATA (please send us this folder)" + Path.DirectorySeparatorChar + "OcomoyLatestSettings.txt";
        logPath = Path.Combine(Application.dataPath, logPathPart);
        path = Path.Combine(Application.dataPath, pathPart);
        //TODO: @Maki @Theo funktioniert der path bei euch auch? :)

        File.WriteAllText(path, _defaultSettings); //set default as initial "last settings"
    }

    // Update is called once per frame
    void Update() { }


    public void OpenMenu() {
        _settings.SetNotes("");
        notes.text = "";
        m_pauseOverlay.SetActive(true); //toggle menu visibility
                                        //TODO: zur Sicherheit: load settings from actual data???
    }

    public void CloseMenu() {
        //notes.OnDeselect(new BaseEventData(EventSystem.current));
        //Debug.Log ("Notes: "+notes.text);

        //notes.DeactivateInputField ();
        //Debug.Log ("Notes: "+notes.text);
        notes.DeactivateInputField();
        _settings.SetNotes(notes.text);
        m_pauseOverlay.SetActive(false); //toggle menu visibility	
        //WriteSettingsIntoJSON(); //write settings on every close
        BackupSettings();
    }

    public void RevertSettings() {
        string lastSettings = File.ReadAllText(path);
        LoadSettingsFromJSON(lastSettings);
    }


    public void Notify(){
        (saveData.transform.Find("Notification").gameObject).SetActive(true);
        //TODO: @Leo continue: fade out text notification

    }

	//for parsing and loading the settings from a json object
	public void LoadSettingsFromJSON(string jsonstring){
		SettingsContainer temp = JsonUtility.FromJson<SettingsContainer>(jsonstring); //create temp settings instance with old settings from file
		// UPDATING VARIABLES & UI ELEMENTS
		_settings.ChangeCharacterSpeed (temp.characterSpeed);
		charSpeedDropdown.value = temp.characterSpeed;

		_settings.ChangeHealthSystem (temp.healthSystem);
		healthSystemDropdown.value = temp.healthSystem;

		_settings.ChangeMusicMode (temp.musicMode);
		musicDropdown.value = temp.musicMode;

		_settings.ChangeObjectSpeed (temp.objectSpeed);
		objectSpeedDropdown.value = temp.objectSpeed;

		_settings.EnableMusic (temp.musicEnabled);
		musicToggle.isOn = temp.musicEnabled;

		_settings.SetDashEnabled (temp.dashEnabled);
		dashToggle.isOn = temp.dashEnabled;

		//_settings.SetNotes (temp.notes);
		//notes.text = temp.notes;

		_settings.SetScreenshake (temp.screenShakeMode);
		screenShakeSlider.value = (float) temp.screenShakeMode;

        
        _settings.SetRating(temp.screenShakeMode);
        ratingSlider.value = (float)temp.rating;

        _settings.ToggleHeightLock (temp.heightLockEnabled);
		heightLockToggle.isOn = temp.heightLockEnabled;
	}

	public void WriteSettingsIntoJSON(){
		//Filling JSON...
		System.DateTime currentTime = System.DateTime.Now;
		string timeStamp = "Date: " + currentTime.Day + "." + currentTime.Month + "." + currentTime.Year + ", \tTime: " + currentTime.Hour + ":" + currentTime.Minute + ":" + currentTime.Second;
		string settings = "\n------------- SETTINGS -------------\n" + timeStamp + "\n" + JsonUtility.ToJson (_settings, true) + "\n";

		//Writing File...
		if (!Directory.Exists(Application.dataPath + Path.DirectorySeparatorChar + "LOGDATA (please send us this folder)") || !File.Exists (logPath)) {
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "LOGDATA (please send us this folder)"));
            File.Create(logPath);
			File.WriteAllText (logPath, settings);
		} else {
			File.AppendAllText (logPath, settings);
		}

        //display Save Successful
        GameObject.Find("Success_Notification").GetComponent<Deactivate_Notification_Text>().SetNotification();

        Debug.Log("Files saved into " +logPath);
		
	}


    public void BackupSettings()
    {
        string settingsCopy = JsonUtility.ToJson(_settings, false);
        File.WriteAllText(path, settingsCopy); //For Revert: saves latest settings. always overwrite!

        Debug.Log("Backup MiniJSON");
    }

	//creates default settings and updates values & ui elements
	public void SetDefault(){
		if (_defaultSettings == null) {
			_settings.ChangeCharacterSpeed (1);
			_settings.ChangeHealthSystem (0);
			_settings.ChangeMusicMode (1);
			_settings.ChangeObjectSpeed (1);
			_settings.EnableMusic (false);
			_settings.SetDashEnabled (true);
			//_settings.SetNotes ("");
			_settings.SetScreenshake (1.0f);
            _settings.SetRating(5);
			_settings.ToggleHeightLock (false);

			_defaultSettings = JsonUtility.ToJson (_settings);
		}
		LoadSettingsFromJSON (_defaultSettings);
	}
}
