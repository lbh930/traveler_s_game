using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MousePointState))]
public class PullPole : MonoBehaviour
{
    MousePointState mouseState;
    public Rigidbody forceAddObject;
    public Rigidbody endBone;
    public Rigidbody startBone;
    public Vector3 tiePosOffest;
    public float force = 0.2f;
    public float straightThreshold = 2;
    public float breakThreshold = 3;
    Rigidbody rigid;
    float originMaxSpeed = 3;
    bool straighted = false;
    bool released = false;

    Rigidbody playerRigid;
    Transform tempParent;
    bool tied = false;
    Collider[] colliders;

    // Start is called before the first frame update
    bool initialized = false;
    void Initialize()
    {
        
        if (initialized) return;
        mouseState = GetComponent<MousePointState>();
        colliders = GetComponentsInChildren<Collider>();
        rigid = GetComponent<Rigidbody>();
        tempParent = endBone.transform.parent;
        playerRigid = mouseState.controller.GetComponent<Rigidbody>();
        originMaxSpeed = mouseState.controller.maxSpeed;
        if (forceAddObject == null)
        {
            Transform t = transform;
            while (t.parent != null)
            {
                t = t.parent;
            }
            forceAddObject = t.GetComponent<Rigidbody>();
        }
        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }

    void FixedUpdate()
    {

        if (tied)
        {

            for (int i = 1; i < colliders.Length; i++) colliders[i].enabled = false;
            endBone.transform.SetParent(mouseState.controller.transform);
            endBone.isKinematic = true;
            endBone.transform.localPosition = tiePosOffest;

            float dis = Vector3.Distance(startBone.transform.position, endBone.transform.position);
            if (dis > breakThreshold){
                UnTie();
            }
            else if (dis > straightThreshold)
            {
                released = false;
                if (!straighted)
                {
                    //刚刚拉直，紧绷
                    straighted = true;
                    Vector3 sharedVelocity =
                        (playerRigid.velocity * playerRigid.mass + forceAddObject.velocity * forceAddObject.mass) /
                        (playerRigid.mass + forceAddObject.mass);

                    Vector3 deltaV = (startBone.velocity - endBone.velocity);

                    forceAddObject.AddForceAtPosition(
                        (deltaV*
                        playerRigid.mass*0.8f)/(forceAddObject.mass + playerRigid.mass), 
                        transform.position, ForceMode.VelocityChange);

                    playerRigid.AddForce(deltaV*-1 *
                        (forceAddObject.mass) / (forceAddObject.mass + playerRigid.mass), ForceMode.VelocityChange);

                    int avoidCrush = 0;

                    while (Vector3.Project(forceAddObject.velocity, playerRigid.velocity).magnitude > playerRigid.velocity.magnitude && avoidCrush < 6666)
                    {
                        avoidCrush++;
                        print(avoidCrush + " " + rigid.velocity);
                        forceAddObject.velocity *= 0.999f;
                        forceAddObject.angularVelocity *= 0.99f;
                    }
                }
                else
                {
                    //已经拉直，限制速度
                    mouseState.controller.maxSpeed = 
                        originMaxSpeed * playerRigid.mass / (playerRigid.mass + forceAddObject.mass);
                    forceAddObject.velocity = Vector3.zero;
                    forceAddObject.AddForceAtPosition((endBone.position - startBone.position).normalized 
                        * playerRigid.velocity.magnitude, 
                        transform.position, ForceMode.VelocityChange);

                    Vector3 deltaV = (endBone.transform.position - startBone.transform.position).normalized;

                    if (dis > straightThreshold * 1.3f)
                    {
                        forceAddObject.AddForceAtPosition(force * deltaV * playerRigid.mass / (forceAddObject.mass + playerRigid.mass),
                            transform.position, ForceMode.VelocityChange);
                        playerRigid.AddForce(force * deltaV * -1 *
                           (forceAddObject.mass) / (forceAddObject.mass + playerRigid.mass), ForceMode.VelocityChange);
                    }
                }
            }
            else
            {
                straighted = false;
                if (!released)
                {
                    ResetMaxSpeed();
                    released = true;
                }
            }
        }
    }
    // Update is called once per frame
    void UnTie()
    {
        for (int i = 1; i < colliders.Length; i++) colliders[i].enabled = true;
        endBone.transform.SetParent(tempParent);
        endBone.isKinematic = false;
        released = true;
        straighted = false;
        tied = false;
        ResetMaxSpeed();
    }
    void ResetMaxSpeed()
    {
        mouseState.controller.maxSpeed = originMaxSpeed;
    }
    void Update()
    {
        Initialize();
        if (mouseState.pointing && Input.GetKeyDown(KeyCode.E))
        {
            if (!tied) tied = true;
            else UnTie();
        }
    }

    void LateUpdate()
    {

    }
}
