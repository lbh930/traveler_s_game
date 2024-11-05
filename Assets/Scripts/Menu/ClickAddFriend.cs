using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAddFriend : MonoBehaviour
{
    // Start is called before the first frame update
    public AudioSource audio;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickAddFriendButton()
    {
        if (audio != null)
        {
            print("add friend sound played");
            audio.Play();
        }
        MenuManager menu = GameObject.FindObjectOfType<MenuManager>();
        for (int i = 0; i < menu.potraits.Length; i++)
        {
            if (menu.potraits[i].id == MenuManager.selectedPotrait)
            {
                menu.potraits[i].toBeAdded = false;
                menu.potraits[i].UpdateAnimatorParameters();
                menu.Progress();

                //��Ӻ��Ѻ�ȡ��ѡ��
                MenuManager.selectedPotrait = "";
                MenuManager.toAddCharacterId = "";//Ҫ��ӵĽ�ɫ�Ѿ��������

                //����potraitsλ��
                menu.UpdatePotraitPositions();
                break;
            }
        }


    }
}
