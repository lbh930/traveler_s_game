using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryFrame : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    public int itemIdentity = 0;
    public bool occupied = false;
    public int belonging = -1;
    public bool isQuickFrame = false;
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
