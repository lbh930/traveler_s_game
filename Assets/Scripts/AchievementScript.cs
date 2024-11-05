using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementScript : MonoBehaviour
{
    bool initialized = false;

    TxtReader txt;

    void Initialize()
    {
        if (initialized) return;

        txt = GetComponent<TxtReader>();
        txt.Read(Application.streamingAssetsPath, "Save.txt", ';');

        /*
        Steamworks.SteamUserStats.ClearAchievement("laolu");
        Steamworks.SteamUserStats.ClearAchievement("chapter0");
        Steamworks.SteamUserStats.ClearAchievement("chapter1");
        Steamworks.SteamUserStats.ClearAchievement("chapter2");
        Steamworks.SteamUserStats.ClearAchievement("chapter3");
        Steamworks.SteamUserStats.ClearAchievement("kingdom");
        Steamworks.SteamUserStats.ClearAchievement("dynasty");
        Steamworks.SteamUserStats.ClearAchievement("bandit");
        Steamworks.SteamUserStats.ClearAchievement("urban");
        */
        
        
        if (MenuManager.day >= 0)
        {
            bool achieved;
            Steamworks.SteamUserStats.GetAchievement("laolu", out achieved);
            if (achieved == false)
            {
                Steamworks.SteamUserStats.SetAchievement("laolu");
            }
        }


        if (MenuManager.day >= 2)
        {
            bool achieved;
            Steamworks.SteamUserStats.GetAchievement("chapter0", out achieved);
            if (achieved == false)
            {
                Steamworks.SteamUserStats.SetAchievement("chapter0");
            }
        }

        if (MenuManager.day >= 7)
        {
            bool achieved;
            Steamworks.SteamUserStats.GetAchievement("chapter1", out achieved);
            if (achieved == false)
            {
                Steamworks.SteamUserStats.SetAchievement("chapter1");
            }
        }

        if (MenuManager.day >= 33)
        {
            bool achieved;
            Steamworks.SteamUserStats.GetAchievement("chapter2", out achieved);
            if (achieved == false)
            {
                Steamworks.SteamUserStats.SetAchievement("chapter2");
            }
        }

        if (MenuManager.day >= 58)
        {
            bool achieved;
            Steamworks.SteamUserStats.GetAchievement("chapter3", out achieved);
            if (achieved == false)
            {
                Steamworks.SteamUserStats.SetAchievement("chapter3");
            }
        }

        if (txt.getInt(0, 2) >= 4)
        {
            bool achieved;
            Steamworks.SteamUserStats.GetAchievement("kingdom", out achieved);
            if (achieved == false)
            {
                Steamworks.SteamUserStats.SetAchievement("kingdom");
            }
        }


        if (txt.getInt(0, 2) >= 15)
        {
            bool achieved;
            Steamworks.SteamUserStats.GetAchievement("dynasty", out achieved);
            if (achieved == false)
            {
                Steamworks.SteamUserStats.SetAchievement("dynasty");
            }
        }

        if (txt.getInt(0, 2) >= 32)
        {
            bool achieved;
            Steamworks.SteamUserStats.GetAchievement("bandit", out achieved);
            if (achieved == false)
            {
                Steamworks.SteamUserStats.SetAchievement("bandit");
            }
        }

        if (txt.getInt(0, 2) >= 60)
        {
            bool achieved;
            Steamworks.SteamUserStats.GetAchievement("urban", out achieved);
            if (achieved == false)
            {
                Steamworks.SteamUserStats.SetAchievement("urban");
            }
        }

        initialized = true;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
    }
}
