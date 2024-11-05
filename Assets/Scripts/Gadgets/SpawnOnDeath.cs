using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnDeath : MonoBehaviour
{
    bool initialized = false;
    public GameObject[] toSpawn;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }
    void Initialize()
    {
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
    }

    public void Spawn()
    {
        foreach (GameObject go in toSpawn)
        {
            Instantiate(go, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}
