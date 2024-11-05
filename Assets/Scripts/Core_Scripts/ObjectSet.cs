using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSet : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    public GameObject[] objects;
    void Initialize()
    {
        
        if (initialized) return;
        initialized = true;

    }

    void Start()
    {
        Initialize();
        
    }
}
