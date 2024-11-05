using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTranslateAndRotate : MonoBehaviour
{
    public Vector3 posSpeed;
    public Vector3 rotateSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(posSpeed*Time.deltaTime, Space.Self);
        transform.Rotate(rotateSpeed*Time.deltaTime, Space.Self);
    }
}
