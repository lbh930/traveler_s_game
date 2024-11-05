using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyClear : MonoBehaviour
{
    // Start is called before the first frame update
    public float timeLeft = -1;
    bool initialized = false;
    public GameObject effect;

    RagdollScript rs;
    bool spawned = false;
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (initialized) return;
        initialized = true;
        if (timeLeft < 0)
        {
            timeLeft = Random.Range(0.1f, 1.2f);
        }
        rs = GetComponent<RagdollScript>();
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        if (timeLeft <= 0.1f && !spawned)
        {
            if (effect != null)
            {
                Instantiate(effect, rs == null ? transform.position : rs.spineToAddForce.position,
                    transform.rotation);
            }
            spawned = true;            
        }
        if (timeLeft <= 0)
        {
            Destroy(gameObject);
        }

        timeLeft -= Time.deltaTime;
    }
}
