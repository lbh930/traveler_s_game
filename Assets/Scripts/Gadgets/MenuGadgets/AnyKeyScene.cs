using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AnyKeyScene : MonoBehaviour
{
    // Start is called before the first frame update
    public float timer1 = 3.5f;
    public float timer2 = float.PositiveInfinity;
    bool fade = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer1 -= Time.deltaTime;
        if (timer1 < 0 && Input.anyKeyDown && !fade)
        {
            fade = true;
            timer2 = 0.6f;
            GetComponent<Animator>().SetTrigger("fade");
        }

        if (fade)
        {
            timer2 -= Time.deltaTime;
            if (timer2 < 0)
            {
                SceneManager.LoadScene("intro");
            }
        }
    }
}
