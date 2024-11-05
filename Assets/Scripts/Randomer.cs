using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomer : MonoBehaviour
{
    bool initialized = false;
    public int seed;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }
    void Initialize()
    {
        initialized = true;
        RandomTransform[] rt = GetComponentsInChildren<RandomTransform>();
        if (rt.Length > 0)
        {
            for (int i = 0; i < rt.Length; i++)
            {
                rt[i].seed = Algori.STS(seed, i);
                rt[i].Randomize();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
    }
}
