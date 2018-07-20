using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour {

    public int round_player_hits;
    public int round_objects_picked_up;
    public float round_time_played;
    public float round_distance_walked;
    public int round_powerups_used_god;
    public int round_powerups_used_player;
    public int round_highest_combo;
    public int round_ignited_objects;


    private Vector3 lastPos;
    private Vector3 currentPos;

    //Player Prefs
    public float totalRoundTime = 0;
    public int finishedRoundCount = 0;
    public float totalGameTime = 0;
    public float totalDistanceWalked = 0;
    public int total_player_hits = 0;
    public int total_objects_picked_up = 0;
    public int total_powerups_used_god = 0;
    public int total_powerups_used_player = 0;
    public int total_highest_combo = 0;
    public int total_ignited_objects = 0;

    public static StatsManager instance;

    // Use this for initialization
    void Start () {
        totalRoundTime = PlayerPrefs.GetFloat("totalRoundTime", 0);
        finishedRoundCount = PlayerPrefs.GetInt("finishedRoundCount", 0);
        totalGameTime = PlayerPrefs.GetFloat("totalGameTime", 0);
        totalDistanceWalked = PlayerPrefs.GetFloat("totalDistanceWalked", 0);
        total_player_hits = PlayerPrefs.GetInt("total_player_hits", 0);
        total_objects_picked_up = PlayerPrefs.GetInt("total_objects_picked_up", 0);
        total_powerups_used_god = PlayerPrefs.GetInt("total_powerups_used_god", 0);
        total_powerups_used_player = PlayerPrefs.GetInt("total_powerups_used_player", 0);
        total_highest_combo = PlayerPrefs.GetInt("total_highest_combo", 0);
        total_ignited_objects = PlayerPrefs.GetInt("total_ignited_objects", 0);
    }

    void Awake()
    {
        StatsManager.instance = this;
    }

    // Update is called once per frame
    void Update () {

        currentPos = Player.instance.transform.position;
        round_distance_walked += Vector3.Distance(currentPos, lastPos);
        lastPos = currentPos;
        round_time_played += Time.deltaTime;
	}

    public void ResetStats()
    {
        round_player_hits = 0;
        round_objects_picked_up = 0;
        round_time_played = 0;
        round_distance_walked = 0;
        round_powerups_used_god = 0;
        round_powerups_used_player = 0;
        round_highest_combo = 0;
        round_ignited_objects = 0;
        lastPos = Player.instance.transform.position;
        currentPos = Player.instance.transform.position;
    }

    private void OnDestroy()
    {
        PlayerPrefs.SetFloat("totalGameTime", (PlayerPrefs.GetFloat("totalGameTime") + round_time_played));
        PlayerPrefs.SetFloat("totalDistanceWalked", (PlayerPrefs.GetFloat("totalDistanceWalked") + round_distance_walked));
        PlayerPrefs.SetInt("total_objects_picked_up", (PlayerPrefs.GetInt("total_objects_picked_up") + round_objects_picked_up));
        PlayerPrefs.SetInt("total_player_hits", (PlayerPrefs.GetInt("total_player_hits") + round_player_hits));
        PlayerPrefs.SetInt("total_powerups_used_god", (PlayerPrefs.GetInt("total_powerups_used_god") + round_powerups_used_god));
        PlayerPrefs.SetInt("total_powerups_used_player", (PlayerPrefs.GetInt("total_powerups_used_player") + round_powerups_used_player));
        if (round_highest_combo > PlayerPrefs.GetInt("total_highest_combo")) { 
            PlayerPrefs.SetInt("total_highest_combo", round_highest_combo);
        }
        PlayerPrefs.SetInt("total_ignited_objects", (PlayerPrefs.GetInt("total_ignited_objects") + round_ignited_objects));
    }
}
