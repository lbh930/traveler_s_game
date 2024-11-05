using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHomeIcon : MonoBehaviour
{
    // Start is called before the first frame update
    MenuManager menu;
    bool initialized = false;
    SoundEffect sound;
    void Start()
    {
        if (!initialized) Initialize();
    }

    void Initialize()
    {
        initialized = true;
        menu = GameObject.FindObjectOfType<MenuManager>();
        sound = GetComponent<SoundEffect>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) Initialize();
    }

    public void OnClick(int programIndex = 0)
    {
        GameObject programFade;
        programFade = GameObject.FindGameObjectWithTag("ProgramFade");
        programFade.GetComponent<RectTransform>().position = GetComponent<RectTransform>().position;

        menu.canvasAni.SetTrigger("ProgramClicked");
        menu.canvasAni.SetTrigger("EscNotice");
        menu.currentProgram = programIndex;
        menu.canvasAni.SetInteger("programIndex", programIndex);

        if (sound != null)
        {
            sound.PlayAudio();
        }
    }
}

