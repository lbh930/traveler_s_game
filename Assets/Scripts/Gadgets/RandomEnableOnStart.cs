using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnableOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    public float enableRate = 0.5f;
    void Start()
    {
        if (Random.Range(0.0f,1.0f) < enableRate)
        {

        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
