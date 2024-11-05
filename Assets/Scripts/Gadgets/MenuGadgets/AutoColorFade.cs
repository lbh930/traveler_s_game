using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoColorFade : MonoBehaviour
{
    public RawImage image;
    public float delay = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;
        if (delay < 0) {
            if (image != null)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b,
                    Mathf.MoveTowards(image.color.a, 0, Time.deltaTime*2.5f));
            }
        }
    }
}
