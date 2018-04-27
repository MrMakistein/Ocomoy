using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodEffectSpawnPosition : MonoBehaviour {

    public bool in_use = false;
    public int uid;
    public float new_spawn_timer = 0;
    public float new_spawn_duration = 30;
    public GameObject godObject;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if (new_spawn_timer >= 1)
        {
            new_spawn_timer -= Time.deltaTime * 5;
        }
        if (new_spawn_timer > 0 && new_spawn_timer < 4)
        {
            // Spawn New God Effect at random god position that is not in use already.
            GameObject[] godObjectSpawnPositions = GameObject.FindGameObjectsWithTag("GodObjectSpawnPosition");
            int rand;
            bool found = false;
            do
            {
                rand = UnityEngine.Random.Range(0, godObjectSpawnPositions.Length);
                if (!godObjectSpawnPositions[rand].GetComponent<GodEffectSpawnPosition>().in_use)
                {
                    found = true;

                    int uid = godObjectSpawnPositions[rand].gameObject.GetInstanceID();
                    godObjectSpawnPositions[rand].GetComponent<GodEffectSpawnPosition>().uid = uid;
                    godObjectSpawnPositions[rand].GetComponent<GodEffectSpawnPosition>().in_use = true;

                    GameObject godItem = Instantiate(godObject) as GameObject;
                    godItem.transform.position = godObjectSpawnPositions[rand].transform.position;
                    godItem.GetComponent<GodEffects>().uid = uid;

                    }
            } while (!found);

            in_use = false;
            new_spawn_timer = 0;
        }
	}
}
