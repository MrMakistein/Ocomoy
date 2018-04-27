using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signpost : MonoBehaviour {

    [HideInInspector] public GameObject[] collectibles;
    private GameObject arena;
    private Transform target;
    private GameObject[] shrines;
    private GameObject finalDestination;
    private float angle;
    private float wiggle_angle;
    [HideInInspector] public bool collectibleMoving;
    public float wiggleSmooth = 0.2f;           // How smooth the signposts rotation will be smoothed. lower smooth -> rotates slower
    public float wiggleFrequency = 10.0f;       // How often the signpost will change direction. lower frequency -> faster wiggle
    public float wiggleFrequencyRange = 5.0f;   // how much the frequency fluctuates.
    float wiggleTimer = 2;
    public float wiggle_strength = 40;  // How many degrees the signpost will fluctuate
    
    private int debugcounter = 0;


    // Use this for initialization
    void Start () {
        arena = GameObject.Find("Arena");
        collectibles = null;
    }


    public void SetNewWiggleRotation()
    {
        wiggle_angle = Random.Range((angle - wiggle_strength), (angle + wiggle_strength));

    }


    public void FindWinShrine()
    {
        shrines = GameObject.FindGameObjectsWithTag("Shrine");
        foreach (GameObject shrine in shrines)
        {
            if (shrine.GetComponent<Shrine>().shrine_id == 1)
            {
                finalDestination = shrine;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
      if (wiggleTimer > 0)
        {
            wiggleTimer -= Time.deltaTime * 2f;
        }
      if (wiggleTimer <= 0)
        {
            wiggleTimer = Random.Range((wiggleFrequency - wiggleFrequencyRange), (wiggleFrequency + wiggleFrequencyRange));
            SetNewWiggleRotation();
        }


        transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(0, 360-wiggle_angle, 0), wiggleSmooth/100);

        
        // Sets collectible moving to true if an active collectible is being moved.
        collectibleMoving = false;
        GetCollectiblesArray();

        foreach (GameObject g in collectibles)
        {
            if (g != null && g.GetComponent<InteractiveSettings>().isCollectible)
            {
                Rigidbody rb = g.GetComponent<Rigidbody>();

                if (rb.velocity != Vector3.zero && g.activeSelf)
                {
                    collectibleMoving = true;
                }
            }
        }
        // Signpost Rotation is only updated when a collectible is moving.
        if (collectibleMoving)
        {
            UpdateSignpostDetection();
        }
    }

    public void UpdateSignpostDetection() // Updates the all fenceposts to face toward the closest collectible
    {

        if (arena.GetComponent<SpawnController>().allCollected) {
            target = finalDestination.transform;
        } else {
            target = GetClosestCollectible().transform;
        }

        if (!target) return;

        var myPos = transform.position;
        myPos.y = 0;

        var targetPos = target.position;
        targetPos.y = 0;

        Vector3 toOther = (myPos - targetPos).normalized;

        angle = Mathf.Atan2(toOther.z, toOther.x) * Mathf.Rad2Deg + 180;

        }
   

    float Atan2Approximation(float y, float x)
    {
        float t0, t1, t3, t4;
        t3 = Mathf.Abs(x);
        t1 = Mathf.Abs(y);
        t0 = Mathf.Max(t3, t1);
        t1 = Mathf.Min(t3, t1);
        t3 = 1f / t0;
        t3 = t1 * t3;
        t4 = t3 * t3;
        t0 = -0.013480470f;
        t0 = t0 * t4 + 0.057477314f;
        t0 = t0 * t4 - 0.121239071f;
        t0 = t0 * t4 + 0.195635925f;
        t0 = t0 * t4 - 0.332994597f;
        t0 = t0 * t4 + 0.999995630f;
        t3 = t0 * t3;
        t3 = (Mathf.Abs(y) > Mathf.Abs(x)) ? 1.570796327f - t3 : t3;
        t3 = (x < 0) ? 3.141592654f - t3 : t3;
        t3 = (y < 0) ? -t3 : t3;
        return t3;
    }


    public void GetCollectiblesArray()
    {
        if (arena == null)
        {
            arena = GameObject.Find("Arena");
        }
        collectibles = arena.GetComponent<SpawnController>().collectibles;
    }


    public GameObject GetClosestCollectible()
    {
        GameObject gMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (GameObject g in collectibles)
        {
            float dist = Vector3.Distance(g.transform.position, currentPos);
            if (dist < minDist && g.gameObject.activeSelf)
            {
                gMin = g;
                minDist = dist;
            }
        }
        return gMin;
    }

    public void InitializeSignpostRotation()
    {
        UpdateSignpostDetection();
    }
}
