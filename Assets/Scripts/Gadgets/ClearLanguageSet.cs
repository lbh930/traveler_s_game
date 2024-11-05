using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLanguageSet : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PlayerPrefs.DeleteAll();
        print(float.Parse(" 1.0 "));
    }
}
