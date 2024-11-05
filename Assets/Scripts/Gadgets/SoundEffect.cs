using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource audio;
    bool initialized = false;

    public float[] pitchRange = { 0.9f, 1.1f };
    public float[] volumnRange = { 0.8f, 1.0f };
    public AudioClip[] randomFrom;

    public bool playOnAwake = true;

    public bool constantUpdate = false;
    void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        if (initialized) return;
        if (audio == null) audio = GetComponent<AudioSource>();

        SetValues();

        if (playOnAwake)
            audio.Play();

        initialized = true;
    }

    public void SetValues()
    {
        if (randomFrom.Length > 0)
        {
            audio.clip = randomFrom[Random.Range(0, randomFrom.Length)];
        }

        audio.pitch = Random.Range(pitchRange[0], pitchRange[1]);
        audio.volume = Random.Range(volumnRange[0], volumnRange[1]);
        if (pitchRange[0] > 0.99f) audio.pitch = 1;
        if (volumnRange[0] > 0.99f) audio.volume = 1;
        SetVolume();
    }

    public void SetVolume()
    {
        
        audio.volume *= PauseScript.mastervolume * 0.01f;

        audio.volume *= PauseScript.soundfx * 0.01f * 0.58f;
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        if (constantUpdate)
        {
            audio.volume = 1;
            SetVolume();
        }
    }

    public void PlayAudio()
    {
        SetValues();
        audio.Play();
    }
}
