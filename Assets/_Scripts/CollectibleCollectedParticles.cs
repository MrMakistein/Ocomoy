using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleCollectedParticles : MonoBehaviour {


    public Vector3 start_position;
    public Vector3 end_position;
    public Vector3 spawnshrine_position;
    public Vector3 direction_vector;
    public GameObject particles;

    public float collected_fade_timer = 0;

    public static CollectibleCollectedParticles instance;

    // Use this for initialization
    void Start () {
        

    }

    void Awake()
    {
        CollectibleCollectedParticles.instance = this;
    }

    // Update is called once per frame
    void Update () {
	
        /*if (Input.GetKeyDown("space"))
        {
            start_position = Player.instance.transform.position;
            end_position = start_position - (direction_vector = (start_position - spawnshrine_position).normalized) * 6;
            particles.transform.position = start_position;
            var iTweenPath = this.GetComponent<iTweenPath>();
            iTweenPath.nodes[0] = new Vector3(start_position.x, start_position.y, start_position.z);
            iTweenPath.nodes[1] = new Vector3(end_position.x, end_position.y + 1, end_position.z);
            particles.GetComponent<ParticleSystem>().Play();

            iTween.MoveTo(particles, iTween.Hash("path", iTweenPath.GetPath("CollectibleCollected"), "easetype", iTween.EaseType.easeInQuad, "time", 2, "oncompleteTarget", this.gameObject, "oncomplete", "Finish"));
        }
        */
	}

    private void FixedUpdate()
    {
        if (collected_fade_timer > 0)
        {
            collected_fade_timer += 0.1f;



        }
    }

    IEnumerator StartDissolve()
    {
        end_position = start_position - (direction_vector = (start_position - spawnshrine_position).normalized) * 6;
        var iTweenPath = this.GetComponent<iTweenPath>();
        particles.transform.position = start_position;
        iTweenPath.nodes[0] = new Vector3(start_position.x, start_position.y, start_position.z);
        iTweenPath.nodes[1] = new Vector3(end_position.x, end_position.y + 1, end_position.z);
        particles.GetComponent<ParticleSystem>().Play();
        Debug.Log("set things");
        yield return new WaitForSeconds(0.2f);
        iTween.MoveTo(particles, iTween.Hash("path", iTweenPath.GetPath("CollectibleCollected"), "easetype", iTween.EaseType.easeInQuad, "time", 2, "oncompleteTarget", this.gameObject, "oncomplete", "Finish"));
        Debug.Log("start path");

    }

    public void Finish()
    {
        Debug.Log("Path finished");
    }
}
