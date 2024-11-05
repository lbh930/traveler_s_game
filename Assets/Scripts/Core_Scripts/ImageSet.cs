using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSet : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    public Sprite[] images;
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
