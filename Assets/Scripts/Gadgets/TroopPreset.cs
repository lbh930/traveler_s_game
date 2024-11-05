using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopPreset : MonoBehaviour
{
    // Start is called before the first frame update
    public int[] deck = new int[10];
    public List<HealthScript[]> waves = new List<HealthScript[]>();
    public bool isRandom = false;

    IdentityList list;
    
    
    bool initialized = false;
    public int forceId = -1;
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (initialized) return;
        initialized = true;
        RoundManager r = FindObjectOfType<RoundManager>();
        HumanAi[] h = GetComponentsInChildren<HumanAi>();
        list = FindObjectOfType<IdentityList>();


        for (int i = 0; i < 5; i++)
        {
            Transform group = transform.GetChild(i);
            waves.Add(group.GetComponentsInChildren<HealthScript>());
        }

        for (int i = 0; i < h.Length; i++)
        {
            if (forceId >= 0)
            {
                h[i].gameObject.GetComponent<HealthScript>().teamIndex = forceId;
            }
            h[i].gameObject.SetActive(false);
        }

        if (isRandom) RandomDeck();
    }

    void RandomDeck()
    {
        int count = Random.Range(8, 11);
        deck = new int[count];
        for (int i = 0; i < count; i++)
        {
            deck[i] = 0;
            for (int j = 0; j < 100; j++)
            {
                int index = Random.Range(0, 33);
                if (list.unitList[index] != null)
                {
                    deck[i] = index;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
    }
}
