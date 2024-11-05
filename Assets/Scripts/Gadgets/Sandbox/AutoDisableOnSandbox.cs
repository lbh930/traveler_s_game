using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoDisableOnSandbox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Menu") return;
        if (MenuManager.nextStoryCharacterId == "sandbox") gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
