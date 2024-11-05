using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFall : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    Rigidbody rigid;
    Collider collider;
    [HideInInspector] public SingleTrunk rootTrunk;
    [HideInInspector] public Transform bottomTrunk;
    [HideInInspector] public HealthScript rootHealth;
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

    // Update is called once per frame
    void Update()
    {
        Initialize();
        Fall();
    }

    public void Fall()
    {
        if (rootHealth != null && rootHealth.health <= 0)
        {
            rigid.isKinematic = false;
            rigid.AddForceAtPosition(rootHealth.dieForce/3, rigid.centerOfMass + Vector3.up);
            bottomTrunk.transform.SetParent(transform.parent);            
        }
    }
}
