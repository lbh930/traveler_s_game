using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public Animator canvasAni;
    public BgmVolume bgmNoise;
    public BgmVolume bgmNoNoise;
    public AudioSource loadingSource;
    bool initialized = false;

    public bool stopNoise = false;

    bool introPlayed = false;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (initialized) return;
        initialized = true;

        bgmNoise.basicVolume = 1;
        bgmNoNoise.basicVolume = 0;

       
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();

        if (stopNoise)
        {
            bgmNoNoise.basicVolume = Mathf.MoveTowards(bgmNoNoise.basicVolume, 1.0f, Time.deltaTime * 0.7f);
            bgmNoise.basicVolume = Mathf.MoveTowards(bgmNoise.basicVolume, 0.0f, Time.deltaTime * 0.7f);
        }
        else
        {
            bgmNoNoise.basicVolume = Mathf.MoveTowards(bgmNoNoise.basicVolume, 0.0f, Time.deltaTime * 0.5f);
            bgmNoise.basicVolume = Mathf.MoveTowards(bgmNoise.basicVolume, 1.0f, Time.deltaTime * 0.5f);
        }

        AnimatorStateInfo info =  canvasAni.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("UiInitialize") || info.IsName ("MenuStartup") || info.IsName("RimFadeOut") || info.IsName("FocusOnCalendar"))
        {
            stopNoise = false;
        }
        else
        {
            stopNoise = true;
        }
            

        if (info.IsName("RimFadeOut") && !introPlayed)
        {
            introPlayed = true;
            loadingSource.Play();
        }
    }

    public void StopNoise()
    {
        stopNoise = true;
    }
}
