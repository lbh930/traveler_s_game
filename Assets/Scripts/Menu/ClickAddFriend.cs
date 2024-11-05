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

                //添加好友后取消选择
                MenuManager.selectedPotrait = "";
                MenuManager.toAddCharacterId = "";//要添加的角色已经被添加了

                //更新potraits位置
                menu.UpdatePotraitPositions();
                break;
            }
        }


    }
}
