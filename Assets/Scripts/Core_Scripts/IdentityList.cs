using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IdentityList : MonoBehaviour
{
    [System.Serializable]public struct CharacterInfo
    {
        public string id;
        public string[] names;
        public Sprite[] potraits;
    }
    // Start is called before the first frame update
    public GameObject[] list;
    public GameObject[] bList;
    public GameObject[] unitList;

    public TileBase[] tilelist;
    public GameObject[] blockList;

    public string[] sceneList;

    public CharacterInfo[] characters;

    RoundManager r;
    
    bool initialized = false;
    void Initialize()
    {
        
        if (initialized) return;
        r = GameObject.FindObjectOfType<RoundManager>();
        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.B)&&Input.GetKeyDown(KeyCode.H) && Input.GetKeyDown(KeyCode.M))
        {
            if (Time.timeScale < 2)
                Time.timeScale = 10;
            else
                Time.timeScale = 1;
        }
        else if (!PauseScript.paused)
        {
            //Time.timeScale = 1;
        }

        Initialize();
        if (r == null) r = GameObject.FindObjectOfType<RoundManager>();
       
        
        if (Input.GetKeyDown(KeyCode.R) && Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < r.players.Length; i++)
            {
                r.players[i].ready = true;
            }
        }
    }
}
