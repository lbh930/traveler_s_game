using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public class VolumeModifier : MonoBehaviour
{

    Volume volume;
    bool initialized = false;


    public float dofStart = 22;
    public float dofEnd = 50;
    public static bool doBlur = false;
    public static float aniSpeed = 5;

    DepthOfField dof;

// Start is called before the first frame update
    
    void Initialize()
    {
        if (initialized) return;
        volume = GetComponent<Volume>();

        volume.sharedProfile.TryGet<DepthOfField>(out dof);

        doBlur = false;

        initialized = true;
    }
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        if (doBlur)
        {
            dof.gaussianStart.value = Mathf.MoveTowards(dof.gaussianStart.value, 0,
                Time.deltaTime * aniSpeed);
            dof.gaussianEnd.value = Mathf.MoveTowards(dof.gaussianEnd.value, 0, 
                Time.deltaTime * aniSpeed);
        }
        else
        {
            dof.gaussianStart.value = Mathf.MoveTowards(dof.gaussianStart.value, 
                dofStart, Time.deltaTime * aniSpeed);
            dof.gaussianEnd.value = Mathf.MoveTowards(dof.gaussianEnd.value, 
                dofEnd, Time.deltaTime * aniSpeed);
        }
    }
}
