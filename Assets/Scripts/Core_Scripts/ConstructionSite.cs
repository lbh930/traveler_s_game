using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(MousePointState))]
public class ConstructionSite : MonoBehaviour
{
    // Start is called before the first frame update
    public int craftId;
    public int[][] matId;
    public int[] craftTimes;
    public bool[] finisheds;
    public bool placed = false;
    public bool completed = true;
    bool initialized = false;
    MeshRenderer meshR;
    MousePointState mouseState;
    public enum SurfaceType
    {
        Opaque,
        Transparent
    }
    public enum BlendMode
    {
        Alpha,
        Premultiply,
        Additive,
        Multiply
    }
    void Initialize()
    {
        
        if (initialized) return;
        meshR = GetComponent<MeshRenderer>();
        SetOutlook();
        mouseState = GetComponent<MousePointState>();
        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }
    void SetOutlook()
    {
        if (!completed) {

        }
        else
        {
        }
    }

    public bool GetFinished()
    {
        bool finished = true;
        for (int i = 0; i < finisheds.Length; i++)
        {
            if (!finisheds[i])
            {
                finished = false;
            }
        }

        return finished;
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        SetOutlook();

        if (mouseState.pointing 
            && mouseState.controller.state == TravelerController.TravelerState.walking
            && Input.GetKeyDown (KeyCode.E) && !completed)
        {
            mouseState.controller.craft.StartConstructing(this);
        }
    }
}
