using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativeSprite : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    public SpriteRenderer ParentRenderer;
    SpriteRenderer renderer1;
    void Initialize()
    {
        
        if (initialized) return;
        renderer1 = GetComponent<SpriteRenderer>();
        if (ParentRenderer == null && transform.parent != null)
            ParentRenderer = transform.parent.GetComponent<SpriteRenderer>();
        if (ParentRenderer != null)
            renderer1.enabled = ParentRenderer.enabled;
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
        if (ParentRenderer != null)
            renderer1.enabled = ParentRenderer.enabled;
    }
}
