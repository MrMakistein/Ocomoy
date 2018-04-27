using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{

    private GameObject[] collectibles;
    private float angle;
    private GameObject arena;
    private Transform target;
    public float rotation_smooth = 1;

    // Use this for initialization
    void Start()
    {
        collectibles = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateCompassDetection() // Updates the all fenceposts to face toward the closest collectible
    {
        arena = GameObject.Find("Arena");
        if (!arena.GetComponent<SpawnController>().allCollected)
        {
            transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.Euler(0, 360 - angle, 0), rotation_smooth / 10);
            GetCollectiblesArray();
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

}
