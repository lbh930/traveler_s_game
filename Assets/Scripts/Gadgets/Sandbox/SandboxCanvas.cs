using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxCanvas : MonoBehaviour
{
    public GameObject[] toDisable;
    public Animator originalCanvasAnimator;
    public GameObject[] toEnable;
    // Start is called before the first frame update
    void Awake()
    {
        for (int i = 0; i < toEnable.Length; i++)
        {
            toEnable[i].SetActive(false);
        }

        if (MenuManager.nextStoryCharacterId == "sandbox")
        {
            print("sandbox mode initialize");
            for (int i = 0; i < toEnable.Length; i++)
            {
                toEnable[i].SetActive(true);
            }

            for (int i = 0; i < toDisable.Length; i++)
            {
                toDisable[i].SetActive(false);
            }

            originalCanvasAnimator.enabled = false;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
