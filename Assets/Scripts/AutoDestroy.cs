using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeToDestruct;
    bool initialized = false;
    float time = 0;
    void Initialize()
    {
        
        if (initialized) return;
        time = 0;
        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        time += Time.deltaTime;
        if (time >= timeToDestruct) Destroy(gameObject);
    }
}
