using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdentityScript : MonoBehaviour
{
    public enum IdType
    {
        item,
        building,
        creature,
        terrain,
    }
    // Start is called before the first frame update
    public int id = 0;
    public IdType type = IdType.item;

    [Header("Item Properties")]
    public Vector3 pivotInPack;
    public Vector3 pivotRotationInPack;
    public float scaleInPack = 1f;
    public int length = 1;
    public int height = 1;

    [HideInInspector] public bool available = true;
    [HideInInspector] public MeshRenderer meshRenderer;
    [HideInInspector] public Color originColor = Color.white;
    bool initialized = false;
    void Initialize()
    {
        
        if (initialized) return;
        initialized = true;
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            originColor = meshRenderer.material.color;
    }

    void Awake()
    {
        Initialize();
    }

    void OnEnable()
    {
        Initialize();
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

    public void ClearPhysics()
    {
        Rigidbody rigid = GetComponent<Rigidbody>();
        if (rigid != null) Destroy(rigid);
        Collider[] collis = GetComponents<Collider>();
        for (int i = 0; i < collis.Length; i++)
        {
            if (collis[i]!=null)
            Destroy(collis[i]);
        }
    }
}
