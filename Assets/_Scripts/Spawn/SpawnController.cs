using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpawnController : MonoBehaviour {
    //PUBLIC CONSTS & VARIABLES
	public int numberOfCollectibles = 3; //ADJUST constant for number of objects to be searched
	public GameObject[] collectibles;
	public List<GameObject> spawnAreaObjects;
	public GameObject[] areas; //areas in which collectibles spawn (1 per area)
    public List<GameObject> chosenCollectibles; //List for all collectibles
    public GameObject[] godObjectSpawnPositions;
    public int initialGodObjects = 3;
    public GameObject godObject;
    public Vector3 player_pos;
    public GameObject player;

    //PRIVATE VARIABLES
    private GameObject[] interactives; //all interactive GameObjects in the scene
    public bool allCollected = false;


    // Use this for initialization
    void Start () {
        SpawnShrines();
        SpawnGodObjects();
        Invoke("DetermineAreas", 0.1f);
        Invoke("SetPlayerPosition", 0.15f);
        Invoke("DetermineCollectibles", 0.2f);
        Invoke("ResetStats", 0.3f);
    }

    // Update is called once per frame
    void Update () {
		
	}
    
    public void SetPlayerPosition()
    {
        player.transform.position = player_pos;
        CameraControl.instance.CenterCamera();

        //player.transform.position = new Vector3(59.8f, 7.5f, 27f); // Manually set player position

    }

    public void ResetStats()
    {
        StatsManager.instance.ResetStats();
    }

    public static string GetUniqueID()
    {
        string key = "ID";

        var random = new System.Random();
        DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
        double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;

        string uniqueID = String.Format("{0:X}", Convert.ToInt32(timestamp))                //Time
                +  String.Format("{0:X}", Convert.ToInt32(Time.time * 1000000))        //Time in game
                + String.Format("{0:X}", random.Next(1000000000));                //random number

        if (PlayerPrefs.HasKey(key))
        {
            uniqueID = PlayerPrefs.GetString(key);
        }
        else
        {
            PlayerPrefs.SetString(key, uniqueID);
            PlayerPrefs.Save();
        }

        return uniqueID;
    }


    public void SpawnGodObjects()
    {
        godObjectSpawnPositions = GameObject.FindGameObjectsWithTag("GodObjectSpawnPosition");

        while (initialGodObjects > 0)
        {
            int rand = UnityEngine.Random.Range(0, godObjectSpawnPositions.Length);
            if (!godObjectSpawnPositions[rand].GetComponent<GodEffectSpawnPosition>().in_use)
            {
                int uid = godObjectSpawnPositions[rand].gameObject.GetInstanceID();
                godObjectSpawnPositions[rand].GetComponent<GodEffectSpawnPosition>().uid = uid;
                initialGodObjects--;
                godObjectSpawnPositions[rand].GetComponent<GodEffectSpawnPosition>().in_use = true;
                GameObject godItem = Instantiate(godObject) as GameObject;
                godItem.transform.position = godObjectSpawnPositions[rand].transform.position;
                godItem.GetComponent<GodEffects>().uid = uid;
            }
        }
    }

	public void DetermineAreas(){
		spawnAreaObjects = new List<GameObject> (GameObject.FindGameObjectsWithTag("Area"));

        int winZoneIndex = 0;
        for (int a = 0; a < spawnAreaObjects.Count; a++)
        {
            if (spawnAreaObjects[a].gameObject.GetComponent<Areas>().isWinZone == true)
            {
                winZoneIndex = a;
            }
        }
        

        List<int> list = new List<int>(new int[] { 1, 2, 3, 4, 5, 6});
        list.RemoveAt(winZoneIndex);


        GameObject winZone = spawnAreaObjects[winZoneIndex];
        winZone.GetComponent<Areas>().setWinZone(); //set as winzone
        spawnAreaObjects.Remove(winZone); //exclude from SpawnAreas
        Destroy(winZone);

        int rand = 0;
        GameObject[] shrines = GameObject.FindGameObjectsWithTag("Shrine");

        for (int i = 0; i < 2; i++)
        {
            rand = UnityEngine.Random.Range(0, list.Count);

            GameObject emptyZone = spawnAreaObjects[rand];
            emptyZone.GetComponent<Areas>().setWinZone();
            //AudioManager.instance.spawn_position = emptyZone.transform.position;
            spawnAreaObjects.Remove(emptyZone);
            Destroy(emptyZone);

            list.RemoveAt(rand);
        }

    





    }

    public void DetermineCollectibles() {
		chosenCollectibles = new List<GameObject>(); //List for all collectibles

		//ITERATE OVER REMAINING ZONES
		for (int a = 0; a < spawnAreaObjects.Count; a++){
			
			GameObject zone = spawnAreaObjects[a];
			List<GameObject> interactivesInZone = zone.GetComponent<Areas>().getAreaInteractives();

			//Debug.Log ("Number of Interactives in # "+zone.GetComponent<Areas>().name +" # : " +interactivesInZone.Count); //TESTING
		
			//choose one of the interactives in the zone randomly
			var rnd2 = new System.Random ();
			

			//CHOOSE A RANDOM INTERACTIVE IN THE ZONE TO SET AS COLLECTIBLE
			int r = rnd2.Next (0, interactivesInZone.Count - 1); 
			interactivesInZone[r].GetComponent<InteractiveSettings>().SetCollectible();

			//ADD IT TO THE COLLECTIBLE LIST
			chosenCollectibles.Add(interactivesInZone[r]); //add the game object to the collectibles List
	
			Destroy(zone); //then destroy the zone! (isn't needed anymore)
		}

        collectibles = chosenCollectibles.ToArray(); //to array cuz what r lists
        Debug.Log("Number of Collectibles: " + collectibles.Length);


        GameObject[] signposts = GameObject.FindGameObjectsWithTag("Signpost");
        foreach (GameObject go in signposts)
        {
            go.GetComponent<Signpost>().FindWinShrine();
            go.GetComponent<Signpost>().GetCollectiblesArray();
            go.GetComponent<Signpost>().UpdateSignpostDetection();
        }

    }

    private void SpawnShrines()
    {
        // Assigns random id from 1 to 6 to all of the shrines (no repeating values)
        List<int> list = new List<int>(new int[] { 1, 2, 3, 4, 5, 6 });
        int rand = 0;
        GameObject[] shrines = GameObject.FindGameObjectsWithTag("Shrine");
		foreach (GameObject shrine in shrines) {
			rand = UnityEngine.Random.Range (0, list.Count);
			shrine.gameObject.GetComponent<Shrine>().shrine_id = list [rand];
            shrine.gameObject.GetComponent<Shrine>().SetMeshes();

            list.RemoveAt (rand);
		}
    }


}



