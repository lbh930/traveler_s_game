using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    public GameObject[] blocks;
    int placementState;
    public bool[] aroundOccupation;
    public float height = 2.5f;
    public HealthScript health;
    public ObjectSet objects;
    [HideInInspector]public int layer;
    [HideInInspector] public MapScript mapScript;
    Vector3[] around = { Vector3.forward, Vector3.right, Vector3.forward * -1, Vector3.right * -1 };
    void Initialize()
    {
        
        if (initialized) return;
        SetBlock();
        if (health == null) health = GetComponent<HealthScript>();
        if (objects == null) objects = GetComponent<ObjectSet>();
        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }

    public void SetBlock()
    {
        int occCount = 0;
        for (int i = 0; i <= 3; i++)
        {
            if (aroundOccupation[i]) occCount++;
        }

        placementState = occCount;
        if (occCount == 2 && ((aroundOccupation[0]&&aroundOccupation[2]) ||
            (aroundOccupation[1] && aroundOccupation[3])))
        {
            placementState = 5;
        }

        for (int i = 0; i < blocks.Length; i++)
        {
            if (i == placementState) blocks[i].SetActive(true);
            else blocks[i].SetActive(false);
        }

        switch (placementState)
        {
            case 1:
                for (int i = 0; i <= 3; i++)
                {
                    if (aroundOccupation[i])
                    {
                        transform.forward = around[i];
                    }
                }
                break;
            case 2:
                if (aroundOccupation[0] && aroundOccupation[1]) transform.forward = around[0];
                if (aroundOccupation[1] && aroundOccupation[2]) transform.forward = around[1];
                if (aroundOccupation[2] && aroundOccupation[3]) transform.forward = around[2];
                if (aroundOccupation[3] && aroundOccupation[0]) transform.forward = around[3];
                break;
            case 3:
                for (int i = 0; i <= 3; i++)
                {
                    if (!aroundOccupation[i])
                    {
                        transform.forward = around[i]*-1;
                        
                    }
                }
                break;
            case 5:
                if (aroundOccupation[0]) transform.forward = around[1];
                if (aroundOccupation[1]) transform.forward = around[0];
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        SetBlock();
        if (health.health <= 0)
        {
            if (objects != null)
                Instantiate(objects.objects[1], transform.position, transform.rotation);
            mapScript.SetTilemap(transform.position + Vector3.up*height, layer, null);
            mapScript.RefreshBlock(transform.position + Vector3.up * height, layer);
            Destroy(gameObject);
        }
    }
}
