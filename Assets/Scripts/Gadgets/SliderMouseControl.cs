using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderMouseControl : MonoBehaviour
{
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (slider != null)
        {
            slider.value -= Input.mouseScrollDelta.y*Time.deltaTime*1000;
        }
    }
}
