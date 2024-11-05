/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexNoise : MonoBehaviour
{
    bool initialized = false;
    public int seed = 0;
    public float intensity = 2.0f;
    public MeshFilter mesh;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }
    void Initialize()
    {
        initialized = true;
        if (mesh == null) mesh = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
        if (Input.GetKey(KeyCode.H)) ApplyNoise();
    }

    public void ApplyNoise()
    {
        for (int i = 0; i < mesh.mesh.vertices.Length; i++) { 
            Vector3 dir = new Vector3(Algori.SeedRandom(-1.0f, 1.0f, Algori.STS(seed, i)),
                Algori.SeedRandom(-1.0f, 1.0f, seed), Algori.SeedRandom(-1.0f, 1.0f, 
                Algori.STS(seed, i)));
            mesh.mesh.vertices[i] += dir.normalized * Algori.SeedRandom(0.0f, intensity, Algori.STS(seed, i));
        }
    }
}
*/
