using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOnDeath : MonoBehaviour
{
    [System.Serializable]
    public struct DropItem
    {
        public GameObject prefab;
        public int[] counts;
        public float[] rates;
    }
    public DropItem[] droppings;
    bool initialized = false;
    public float radius;
    public Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }
    void Initialize()
    {
        initialized = true;
    }
    void Update()
    {
        if (!initialized)Initialize();
    }
    public void Drop()
    {
        for (int i = 0; i < droppings.Length; i++)
        {
            int index = Algori.SeedWeightedRandom(droppings[i].rates, Random.Range(0, 10000));
            if (droppings[i].counts[index] <= 0) continue;
            
            for (int j = 0; j < droppings[i].counts[index]; j++) {
                Vector3 spawnPos = transform.position + transform.right * offset.x +
                    transform.up * offset.y + transform.forward * offset.z + Random.insideUnitSphere * radius;
                Instantiate(droppings[i].prefab, spawnPos, Random.rotation);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + transform.right*offset.x + 
            transform.up*offset.y + transform.forward*offset.z, radius);
    }
}
