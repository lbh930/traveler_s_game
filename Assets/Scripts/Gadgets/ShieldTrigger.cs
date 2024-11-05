using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    BoxCollider collider;
    public Transform pointer;
    public GameObject shield_Hit;
    public float damageMultipiler = 0.2f;//这个目前是写死的
    [HideInInspector]public HealthScript health;
    bool initialized = false;
    void Start()
    {
        if (!initialized) Initialize();
    }

    void Initialize()
    {
        collider = GetComponent<BoxCollider>();
        health = GetComponent<HealthScript>();
        health.neverAsTarget = true;

        gameObject.layer = 10;

        damageMultipiler = 0.2f;//目前是写死的

        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) Initialize();

        if (health != null && health.health <= 0) ShieldBreak();

        if (transform.parent.parent == null) Destroy(gameObject);
    }

    private void LateUpdate()
    {
        transform.forward = new Vector3(pointer.forward.x, 0, pointer.forward.z);
        transform.rotation = Quaternion.Euler(new Vector3(
            0, transform.rotation.eulerAngles.y, 0));
    }

    void ShieldBreak()
    {
        Rigidbody rigid = transform.parent.GetComponent<Rigidbody>();
    }
}
