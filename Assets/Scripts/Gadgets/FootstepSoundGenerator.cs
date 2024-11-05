using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioClip singleWalk;
    public AudioClip groupWalk;
    public AudioSource[] audios;

    public int maxSingleCount = 3;

    public float radius = 20f;

    bool initialized = false;
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (initialized) return;
        initialized = true;

    }

    // Update is called once per frame
    void Update()
    {
        Initialize();

        int countInside = 0;
        if (TargetList.targets.Count > 0)
        {
            for (int i = 0; i < TargetList.targets.Count; i++)
            {
                if (Vector3.Distance(Camera.main.transform.position, TargetList.targets[i].transform.position) < radius)
                {
                    countInside++;
                }
            }
        }
    }
}
