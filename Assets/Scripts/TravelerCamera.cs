using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelerCamera : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    public TravelerController player;
    public Transform moveDir;
    public Vector3 shift;
    public Transform forceFollow;
    
    [HideInInspector]public Vector3 tempRotation;
    [HideInInspector] public bool lockView = true;
    void Initialize()
    {
        
        if (initialized) return;
        initialized = true;
        if (player != null)
        {
            player.cam = this;
        }
        tempRotation = transform.rotation.eulerAngles;
    }

    void Start()
    {
        Initialize();
        
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();    
    }

    void LateUpdate()
    {
        if (forceFollow != null)
        {
            transform.position = forceFollow.position;
            transform.rotation = forceFollow.rotation;
        }
        else
        {
            if (player != null && lockView)
            {
                transform.rotation = Quaternion.Euler(tempRotation);
                transform.position = player.transform.position + shift;
            }
            else
            {
                //tempRotation = transform.rotation.eulerAngles;
            }
        }
    }
}
