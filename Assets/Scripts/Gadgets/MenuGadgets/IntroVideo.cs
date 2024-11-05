using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class IntroVideo : MonoBehaviour
{
    
    VideoPlayer v;

    float playTimer = 1.5f;

    public GameObject skiptext;
    public GameObject loadingtext;
    public string loadSceneName = "Menu";
    public bool allowJump = true;
    float textTime = -1;

    // Start is called before the first frame update
    bool initialized = false;

    void Initialize(){
        playTimer = 1.1f;
        v = GetComponent<VideoPlayer>();
        
        initialized = true;
    }

    void Start()
    {
        if (!initialized)Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        playTimer -= Time.deltaTime;
        textTime -= Time.deltaTime;

        if (playTimer < 0)
        {
            v.Play();
            playTimer = float.PositiveInfinity;
        }

        if (!initialized)Initialize();

        if (v.time > v.clip.length - 0.1f) 
        {
            textTime = -1;
            skiptext.SetActive(false);
            loadingtext.SetActive(true);
            SceneManager.LoadScene(loadSceneName);
        }

        if (allowJump)
        {
            if (Input.anyKeyDown)
            {
                if (textTime > 0)
                {
                    v.time = v.clip.length - 0.1f;
                }
                else
                {
                    textTime = 5;
                }
            }
        }

        if (textTime > 0)
        {
            skiptext.SetActive(true);
        }
        else
        {
            skiptext.SetActive(false);
        }
    }
}
