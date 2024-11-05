using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAndDrop : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    public HealthScript health;
    SingleTrunk trunk;
    void Initialize()
    {
        
        if (initialized) return;
        health = GetComponent<HealthScript>();
        trunk = GetComponent<SingleTrunk>();
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
        if (health != null && health.health <= 0)
        {
            if (trunk != null) trunk.nextTrunk.transform.SetParent(transform.parent);
            Destroy(gameObject);
        }
    }
}
