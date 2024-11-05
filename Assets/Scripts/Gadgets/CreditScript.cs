using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CreditScript : MonoBehaviour
{
    float timer = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
        {
            timer += Time.deltaTime*2.6f;
            GetComponent<Animator>().speed = 2.6f;
        }
        else
        {
            timer += Time.deltaTime;
            GetComponent<Animator>().speed = 1;
        }
        if (timer > 55f)
        {
            SceneManager.LoadScene("menu");
        }
    }
}
