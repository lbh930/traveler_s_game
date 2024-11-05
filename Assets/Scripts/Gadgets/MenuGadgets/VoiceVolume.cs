using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceVolume : MonoBehaviour
{
    AudioSource audio;
    public float basicVolume = 1;
    bool initialized = false;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (initialized) return;
        audio = GetComponent<AudioSource>();
        initialized = true;

        audio.volume = basicVolume;
        audio.volume *= PauseScript.mastervolume * 0.01f;
        audio.volume *= PauseScript.voice * 0.01f;
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();

        audio.volume = basicVolume;

        audio.volume *= PauseScript.mastervolume * 0.01f;
        audio.volume *= PauseScript.voice * 0.01f;
    }
}
