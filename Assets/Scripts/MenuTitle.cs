using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuTitle : MonoBehaviour
{
    public Text[] titles;
    public int[] dayIndexes;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = titles.Length - 1; i >= 0; i--)
        {
            titles[i].enabled = false;
        }

        for (int i = titles.Length - 1; i >= 0; i--)
        {
            if (MenuManager.day >= dayIndexes[i])
            {
                titles[i].enabled = true;
                break;
            }
        }
    }
}
