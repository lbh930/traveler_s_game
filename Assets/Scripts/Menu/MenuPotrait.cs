using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPotrait : MonoBehaviour
{
    bool initialized = false;
    [HideInInspector] public IdentityList list;

    Animator ani;
    public Image image;
    public Text nameText;
    public string id;
    public bool haveStory;
    public bool shouldOnline;

    public bool toBeAdded = false;
    public bool interactable = true;

    public bool isOpponentPotrait = false;//自动改为对手的头像

    MenuManager menu;
    [HideInInspector] public int startLine;

    [HideInInspector]public TxtReader txt;



    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (initialized) return;
        txt = GetComponent<TxtReader>();
        string path = Application.streamingAssetsPath + "/Plots/" + id;

        int language = PlayerPrefs.GetInt("language");
        if (language == 2)
        { //英语？
            //print("read English");
            txt.Read(path, id + "_en" + ".csv");
        }
        else
        {//英语以外的简体或繁体
            //print("read t_cn");
            txt.Read(path, id + ".csv");
        }
        //txt.Read(path, id + ".csv");
        menu = GameObject.FindObjectOfType<MenuManager>();
        ani = GetComponent<Animator>();

        shouldOnline = Random.Range(0, 6) == 0 ? true : false;

        if (isOpponentPotrait)
        {
            if (MenuManager.actualCharaterId != null && MenuManager.actualCharaterId.Length > 0)
            {
                id = MenuManager.actualCharaterId;
            }
            else
            {
                id = MenuManager.nextStoryCharacterId;
            }
        }
        UpdatePotrait();

        if (!interactable) image.raycastTarget = false;
        initialized = true;
    }
    // Update is called once per frame
    void Update()
    {
        Initialize();
    }
    public void UpdatePotrait()
    {
        int index = GetIndex();

        if (index >= 0)
        {
            int language = PlayerPrefs.GetInt("language");

            if (language == 0 || language == 1)
            {
             //中文
                image.sprite = list.characters[index].potraits[0];
                nameText.text = list.characters[index].names[0];
            }
            else {
                //英语
                image.sprite = list.characters[index].potraits[0];
                nameText.text = list.characters[index].names[1];
            }
        }
    }

    void Popup() //让菜单中该头像的位置跳到最前面
    {
        if (menu == null) menu = GameObject.FindObjectOfType<MenuManager>(); //防止一些奇怪的null reference
        menu.UpdatePotraitPositions();
    }

    int GetIndex()
    {
        if (list == null) list = GameObject.FindObjectOfType<IdentityList>();

        for (int i = 0; i < list.characters.Length; i++)
        {
            if (list.characters[i].id == id)
            {
                return i;
            }
        }
        return -1;
    }

    public void Click()
    {
        if (!interactable) return;
        MenuManager.selectedPotrait = id;
    }

    void FindStory()
    {
        Initialize();//由于生成的时候从外部调用，会出现未初始化即调用的情况

        if (id == MenuManager.toAddCharacterId)
        {
            toBeAdded = true;
        }
        
        //是不是待添加的好友？
        if (toBeAdded)
        {
            ani.SetBool("new", true);
        }
        else
        {
            ani.SetBool("new", false);
        }

        if (id != MenuManager.nextStoryCharacterId) return; //是不是要到这个角色的故事了
        print("potrait named: " + id + " is loading");
        if (MenuManager.nextStoryCharacterId != MenuManager.nextStoryFileId)
        { //重新定向角色
            string path = Application.streamingAssetsPath + "/Plots/" + MenuManager.nextStoryFileId;

            int language = PlayerPrefs.GetInt("language");
            if (language == 2)
            { //英语？
                print("read English: " + MenuManager.nextStoryFileId);
                txt.Read(path, MenuManager.nextStoryFileId + "_en" + ".csv");
            }
            else
            {//英语以外的简体或繁体
                print("read t_cn: " + MenuManager.nextStoryFileId);
                txt.Read(path, MenuManager.nextStoryFileId + ".csv");
            }
            //txt.Read(path, MenuManager.nextStoryFileId + ".csv");
        }
        //print(txt.m_ArrayData.Count);
        for (int i = 0; i < txt.m_ArrayData.Count; i++)
        {
            bool storyFound = false;
            if (txt.m_ArrayData[i] == null || txt.m_ArrayData[i].Length <= 0)
            {
                continue;
            }
            //print(txt.getString(i, 0));
            if (txt.getString(i, 0) == "*plot") //寻找故事起始标识
            {
                string plotName = txt.getString(i, 1);
                print(plotName + "  " + MenuManager.nextStoryName);
                if (plotName == MenuManager.nextStoryName)
                {
                    print("i is: " + i);
                    //如果plot行有第三项，说明实际上的对战者是第三行写的人
                    if (txt.m_ArrayData[i].Length > 2 && txt.getString(i, 2).Length > 1)
                    {/*
                        MenuManager m = Camera.main.transform.GetComponent<MenuManager>();
                        string actualOpponent = txt.getString(i, 2);
                        print("actual: " + actualOpponent);
                        //遍历全部potraits找故事
                        for (int j = 0; j < m.potraits.Length; j++)
                        {
                            if (m.potraits[j] == null) continue;
                            print(m.potraits[j].id);
                            if (m.potraits[j].id == actualOpponent)
                            {*/
                                haveStory = true;
                                startLine = i;
                                UpdateAnimatorParameters();
                                Popup(); //popup放在最后面，因为这个操作会改变index
                            /*    break;
                            }
                                
                        }*/
                    }
                    else
                    {
                        //否则storyline写的是谁就是谁的对战
                        haveStory = true;
                        startLine = i;
                        storyFound = true;
                        Popup();
                        break;
                    }
                }
                
            }
        }

        //这个角色现在有没有故事？
        if (haveStory && !toBeAdded)
        {
            ani.SetBool("clash", true);
        }
        else 
        {
            ani.SetBool("clash", false);
        }
    }

    public void UpdateAnimatorParameters()
    {
        //是不是待添加的好友？
        if (toBeAdded)
        {
            ani.SetBool("new", true);
        }
        else
        {
            ani.SetBool("new", false);
        }

        //这个角色现在有没有故事？
        if (haveStory && !toBeAdded)
        {
            ani.SetBool("clash", true);
        }
        else
        {
            ani.SetBool("clash", false);
        }
    }

    public void LoadSavefile()
    {
        string path = Application.streamingAssetsPath + "/Plots/" + id;

        int language = PlayerPrefs.GetInt("language");
        if (language == 2)
        { //英语？
            //print("read English");
            txt.Read(path, id + "_en" + ".csv");
        }
        else
        {//英语以外的简体或繁体
            //print("read t_cn");
            txt.Read(path, id + ".csv");
        }

        FindStory();
    }

    public void ClickBattle()
    {
        if (haveStory)
        {
            MenuManager.startLine = startLine;
            MenuManager.haveStory = true;
            MenuManager.nextStoryCharacterId = MenuManager.nextStoryFileId;

            menu.canvasAni.SetTrigger("Loading");
        }else if (id == "random")
        {//说明选择了和电脑战斗
            MenuManager.startLine = 0;
            MenuManager.haveStory = false;
            MenuManager.nextStoryCharacterId = "random";
            MenuManager.actualCharaterId = "random";
            MenuManager.troopAccess = 0;
            menu.canvasAni.SetTrigger("Loading");
        }else if (id == "sandbox")
        {
            SceneManager.LoadScene("SandboxMaps");
        }
    }
}
