using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickBattle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ClickBattleButton()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null) audio.Play();
        MenuManager menu = GameObject.FindObjectOfType<MenuManager>();
        for (int i = 0; i < menu.potraits.Length; i++)
        {
            if (menu.potraits[i].id == MenuManager.selectedPotrait)
            {
                menu.potraits[i].ClickBattle();
                break;
            }
        }

    }
    public void ClickBusyButton()
    {
        NoticePoper.PopNotice(4);
    }

    public void ClickOfflineButton()
    {
        NoticePoper.PopNotice(5);
    }
}
