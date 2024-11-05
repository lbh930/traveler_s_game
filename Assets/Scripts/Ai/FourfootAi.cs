using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourfootAi : MonoBehaviour
{
    bool initialized = false;
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
}
