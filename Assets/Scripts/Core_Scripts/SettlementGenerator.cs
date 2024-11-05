using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SettlementGenerator : MonoBehaviour
{
    public int seed;
    [Serializable]
    public struct Prefabs
    {
        public GameObject prefab;
        public Vector3 bound;
    }
    [Serializable]
    public struct RadiationRow
    {
        public float minRadius;
        public float maxRadius;
        public float chanceOfRow;
        public int minCount;
        public int maxCount;
        public float maxRotationOffset;
        public int[] prefabIndex;
        public float[] chanceOfEach;
    }
    bool initialized = false;
    public enum SettlementMode
    {
        Radiation,
    }
    public SettlementMode mode;
    [Header("General")]
    public Prefabs[] prefabs;
    [Header("Radiation")]
    public RadiationRow[] rows;


    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }
    void Initialize()
    {
        initialized = true;
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
    }

    void Generate()
    {
        if (mode == SettlementMode.Radiation)
        {
            RadiationGeneration();
        }
    }

    void RadiationGeneration()
    {
        for (int i = 0; i < rows.Length; i++)
        {
            if (Algori.SeedRandom(0.0f, 1.0f, Algori.STS(seed, i)) > rows[i].chanceOfRow) continue;
            int count = Algori.SeedRandom(rows[i].minCount, rows[i].maxCount + 1, Algori.STS(seed, i));
            for (int j = 0; j < count; j++)
            {
                int secSeed = Algori.STS(seed, i);
                int index = Algori.SeedWeightedRandom(rows[i].chanceOfEach, Algori.STS(secSeed, j));
                float radius = Algori.SeedRandom(rows[i].minRadius, rows[i].maxRadius, Algori.STS(secSeed, j));
                float angle = Algori.SeedRandom(360.0f / count * j, 360.0f / count * ((float)j + 0.8f), Algori.STS(secSeed, j));
                Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                GameObject obj = (GameObject)Instantiate(prefabs[rows[i].prefabIndex[index]].prefab, 
                    transform.position + dir*radius, transform.rotation);            
                obj.transform.forward = dir;
                obj.transform.eulerAngles += Vector3.up * Algori.SeedRandom(0.0f, rows[i].maxRotationOffset, Algori.STS(secSeed, j));
                if (!CheckSpace(obj.transform, prefabs[rows[i].prefabIndex[index]].bound)) Destroy(obj);

            }
        }
    }

    bool CheckSpace(Transform trans, Vector3 bounds)
    {
        bool available = true;
        trans.gameObject.SetActive(false);
        Vector3 starting = trans.position - trans.right * 0.5f * bounds.x - trans.forward * 0.5f * bounds.z;
        for (float i = 0; i < bounds.x; i+=0.5f)
        {
            for (float j = 0; j < bounds.z; j += 0.5f)
            {
                Vector3 posNow = starting + i * trans.right + j * trans.forward + Vector3.up*-0.01f;
                RaycastHit hit;
                if (Physics.Raycast(posNow, Vector3.up, out hit, bounds.y) && 
                    hit.transform.gameObject.tag == "StaticSolid")
                {
                    available = false;
                    break;
                }
            }
            if (!available) break;
        }
        trans.gameObject.SetActive(true); 
        return available;
    }
}
