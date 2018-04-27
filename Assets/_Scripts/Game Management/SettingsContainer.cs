using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Class for deserializing JSON objects of type "Settings" (e.g. for loading from temp files) 
 **/
public class SettingsContainer
{
	public bool dashEnabled;
	public int characterSpeed; //0-slow, 1-medium, 2-fast
	public int healthSystem; //0-regen, 1-health packs

	public int objectSpeed; //0-slow, 1-medium, 2-fast
	public bool heightLockEnabled;

	public string notes;
	public int screenShakeMode;
    public int rating;
    public bool musicEnabled;
	public int musicMode; //0-with lead, 1-no lead
}


