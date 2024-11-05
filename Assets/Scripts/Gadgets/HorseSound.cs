using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseSound : MonoBehaviour
{
    bool initialized = false;
    AudioSource audio;
    Rigidbody rigid;
    public float maxVelocity = 5;

    HumanAi ai;

    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        audio = GetComponent<AudioSource>();
        rigid = GetComponent<Rigidbody>();
        audio.volume *= PauseScript.mastervolume * 0.01f;
        audio.volume *= PauseScript.soundfx * 0.01f;

        ai = GetComponent<HumanAi>();

        float r = Random.Range(0.9f, 1.1f);
        GetComponent<Animator>().speed = r;
        audio.pitch = r;
          
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();

        if (audio == null || rigid == null) return;

        if (rigid.velocity.magnitude < 0.4f)
        {
            audio.Stop();
        }
        else if (ai.inBattle)
        {
            if (!audio.isPlaying)
            {
                audio.Play();
            }
            audio.volume = rigid.velocity.magnitude / maxVelocity;
            audio.volume = Mathf.Clamp(audio.volume, 0, 1);

            audio.volume *= PauseScript.mastervolume * 0.01f;
            audio.volume *= PauseScript.soundfx * 0.01f;
        }
    }
}
