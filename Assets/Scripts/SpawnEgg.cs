using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEgg : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform chicken;
    public GameObject eggPrefab;
    private float time = 120f;

    void Start()
    {
        InvokeRepeating("Spawn", time, time);
    }

    void Spawn()
    {
        if (chicken == null || eggPrefab == null)
        {
            return;
        }
        Instantiate(eggPrefab, chicken.position, Quaternion.identity);
    }
}
