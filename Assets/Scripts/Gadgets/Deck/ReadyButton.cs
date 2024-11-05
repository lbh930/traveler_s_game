using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ReadyButton : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    OnePlayer[] players;
    public Sprite notReady;
    public Sprite ready;

    public Text notreadyText;
    public Text readyText;


    public SoundEffect unReadyAudio;

    Image image;

    bool lastReady = false;

    void Initialize(){
        image = GetComponent<Image>();
        players = FindObjectsOfType<OnePlayer>();
       
        UpdateSprite();

        initialized = true;
    }

    void Start()
    {
        if (!initialized)Initialize();
    }

    void LateUpdate()
    {
        UpdateSprite();
    }

    void UpdateSprite()
    {
        if (players.Length <= 0) return;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].myControl)
            {
                if (players[i].ready)
                {
                    image.sprite = ready;
                    if (notreadyText != null) notreadyText.gameObject.SetActive(false);
                    if (readyText != null) readyText.gameObject.SetActive(true);
                }
                else
                {
                    image.sprite = notReady;
                    if (notreadyText != null) notreadyText.gameObject.SetActive(true);
                    if (readyText != null) readyText.gameObject.SetActive(false);
                }
                /*
                if (lastReady != players[i].ready)
                {
                    GetComponent<SoundEffect>().PlayAudio();
                    lastReady = players[i].ready;
                }*/
            }
        }
    }

    public void PlaySound()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].myControl)
            {
                if (players[i].ready)
                {
                    GetComponent<SoundEffect>().PlayAudio();
                }
                else
                {
                    unReadyAudio.PlayAudio();
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
    }
}
