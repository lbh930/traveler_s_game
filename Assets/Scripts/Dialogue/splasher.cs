using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class splasher : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject splash;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider colli)
    {
        if (colli.enabled)
        {
            colli.enabled = false;
            Rigidbody rigid = colli.transform.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                rigid.drag = 5;
                rigid.angularDrag = 2.5f;
               
            }
            if (splash != null)
            {
                GameObject g = (GameObject)Instantiate(splash, colli.transform.position, colli.transform.rotation);
                g.transform.forward = Vector3.up;
                g.transform.position -= Vector3.up * 0.12f;
            }
        }
    }
}
