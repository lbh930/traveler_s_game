using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTrunk : MonoBehaviour
{
    // Start is called before the first frame update
    public SingleTrunk previousTrunk;
    public SingleTrunk nextTrunk;
    [HideInInspector] public SingleTrunk[] branches;
    public Transform startBone;
    public Transform endBone;
    [HideInInspector] public float height;
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

    public void SetCollider()
    {
        CapsuleCollider collider = GetComponentInChildren<CapsuleCollider>();
        collider.radius = (endBone.localScale.x + startBone.localScale.x) / 4;
        collider.height = Vector3.Distance(endBone.position, startBone.position);
        collider.transform.up = endBone.position - startBone.position;
        collider.transform.position = (endBone.position + startBone.position) / 2;
    }

    public void DisableCollider()
    {
        CapsuleCollider collider = GetComponentInChildren<CapsuleCollider>();
        Destroy (collider);
    }
}
