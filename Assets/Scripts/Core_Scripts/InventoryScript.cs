using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    // Start is called before the first frame update
    public InventoryFrame[] frames;
    public InventoryFrame[] quickFrames;
    public Material[] framesMat;
    public int length;
    public int height;
    public int quickCount = 4;
    bool initialized = false;
    void Initialize()
    {      
        if (initialized) return;
        initialized = true;
        frames = new InventoryFrame[length * height+quickCount];
        framesMat = new Material[length * height+quickCount];
        int index = 0;
        foreach (InventoryFrame i in gameObject.GetComponentsInChildren<InventoryFrame>())
        {
            frames[index] = i;
            framesMat[index] = i.gameObject.GetComponent<MeshRenderer>().material;
            index++;
        }
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
}
