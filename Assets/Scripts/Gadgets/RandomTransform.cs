using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomTransform : MonoBehaviour
{
    bool initialized = false;
    public Vector3[] positions;
    public Vector3[] rotations;
    public Vector3[] scales;
    public int seed;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }
    void Initialize()
    {
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
    }
    public void Randomize()
    {
        int a = Algori.SeedRandom(0, positions.Length, seed);
        int b = Algori.SeedRandom(0, rotations.Length, seed + 1);
        int c = Algori.SeedRandom(0, scales.Length, seed + 2);
        if (positions.Length > 0)
            transform.localPosition = positions[a];
        if (rotations.Length > 0)
            transform.localRotation = Quaternion.Euler(rotations[b]);
        if (scales.Length > 0)
            transform.localScale = scales[c];
    }
}
