using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform spineToAddForce;
    public SkinnedMeshRenderer skinnedRenderer;
    bool initialized = false;
    void Initialize()
    {
        
        if (initialized) return;
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
        
    }
}
