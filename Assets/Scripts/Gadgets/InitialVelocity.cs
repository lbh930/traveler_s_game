using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialVelocity : MonoBehaviour
{
    Rigidbody[] rigids;
    public Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        rigids = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigid in rigids)
        {
            rigid.velocity = velocity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
