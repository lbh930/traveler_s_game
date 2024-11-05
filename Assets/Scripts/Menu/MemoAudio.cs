using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoAudio : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    bool playing = false;
    public RectTransform pin;
    public AudioClip clip;
    AudioSource audio;
    void Initialize(){
        
        initialized = true;
        audio = GetComponent<AudioSource>();
        audio.clip = clip;
    }
    void Start()
    {
        if (!initialized)Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
        if (playing)
        {
            pin.eulerAngles = new Vector3(pin.eulerAngles.x, pin.eulerAngles.y, Mathf.MoveTowards(pin.eulerAngles.z, 21, Time.deltaTime * 67));

        }
        else
        {
            pin.eulerAngles = new Vector3(pin.eulerAngles.x, pin.eulerAngles.y, Mathf.MoveTowards(pin.eulerAngles.z, 47, Time.deltaTime * 67));
        }

        if (!audio.isPlaying) playing = false;
    }

    public void OnClick()
    {
        if (!playing)
        {
            audio.Play();
            playing = true;
        }
        else
        {
            audio.Stop();
            playing = false;
        }
    }
}
