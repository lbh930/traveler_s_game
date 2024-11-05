using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceApplier : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    public Vector3 dir;
    Rigidbody rigid;
    void Initialize()
    {
        
        if (initialized) return;
        rigid = GetComponent<Rigidbody>();
        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }

    void FixedUpdate()
    {
        rigid.velocity = dir;
    }
    // Update is called once per frame
    void Update()
    {
        Initialize();
       
    }
}
