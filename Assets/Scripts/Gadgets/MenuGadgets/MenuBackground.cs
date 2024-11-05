using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    public GameObject stars;
    public GameObject moons;
    // Start is called before the first frame update
    void Start()
    {
        setBG();
    }

    void setBG()
    {
        if (MenuManager.day >= 57)
        {
            stars.SetActive(false);
            moons.gameObject.SetActive(true);
        }
        else
        {
            stars.SetActive(true);
            moons.gameObject.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        setBG();
    }
}
