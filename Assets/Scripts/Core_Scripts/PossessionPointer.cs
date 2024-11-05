using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionPointer : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform tip;
    public Transform root;
    public Transform model;
    public Transform rootPos;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        root.transform.rotation = Quaternion.Euler(
            root.transform.rotation.eulerAngles.x, root.transform.rotation.eulerAngles.y, 0);
        tip.transform.rotation = Quaternion.Euler(
            0, tip.transform.rotation.eulerAngles.y, 0);
        root.gameObject.SetActive(false);
    }
}
