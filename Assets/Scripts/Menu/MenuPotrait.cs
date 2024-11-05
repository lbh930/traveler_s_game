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

    public bool isOpponentPotrait = false;//�Զ���Ϊ���ֵ�ͷ��

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
        { //Ӣ�
            //print("read English");
            txt.Read(path, id + "_en" + ".csv");
        }
        else
        {//Ӣ������ļ������
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
             //����
                image.sprite = list.characters[index].potraits[0];
                nameText.text = list.characters[index].names[0];
            }
            else {
                //Ӣ��
                image.sprite = list.characters[index].potraits[0];
                nameText.text = list.characters[index].names[1];
            }
        }
    }

    void Popup() //�ò˵��и�ͷ���λ��������ǰ��
    {
        if (menu == null) menu = GameObject.FindObjectOfType<MenuManager>(); //��ֹһЩ��ֵ�null reference
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
        Initialize();//�������ɵ�ʱ����ⲿ���ã������δ��ʼ�������õ����

        if (id == MenuManager.toAddCharacterId)
        {
            toBeAdded = true;
        }
        
        //�ǲ��Ǵ���ӵĺ��ѣ�
        if (toBeAdded)
        {
            ani.SetBool("new", true);
        }
        else
        {
            ani.SetBool("new", false);
        }

        if (id != MenuManager.nextStoryCharacterId) return; //�ǲ���Ҫ�������ɫ�Ĺ�����
        print("potrait named: " + id + " is loading");
        if (MenuManager.nextStoryCharacterId != MenuManager.nextStoryFileId)
        { //���¶����ɫ
            string path = Application.streamingAssetsPath + "/Plots/" + MenuManager.nextStoryFileId;

            int language = PlayerPrefs.GetInt("language");
            if (language == 2)
            { //Ӣ�
                print("read English: " + MenuManager.nextStoryFileId);
                txt.Read(path, MenuManager.nextStoryFileId + "_en" + ".csv");
            }
            else
            {//Ӣ������ļ������
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
            if (txt.getString(i, 0) == "*plot") //Ѱ�ҹ�����ʼ��ʶ
            {
                string plotName = txt.getString(i, 1);
                print(plotName + "  " + MenuManager.nextStoryName);
                if (plotName == MenuManager.nextStoryName)
                {
                    print("i is: " + i);
                    //���plot���е����˵��ʵ���ϵĶ�ս���ǵ�����д����
                    if (txt.m_ArrayData[i].Length > 2 && txt.getString(i, 2).Length > 1)
                    {/*
                        MenuManager m = Camera.main.transform.GetComponent<MenuManager>();
                        string actualOpponent = txt.getString(i, 2);
                        print("actual: " + actualOpponent);
                        //����ȫ��potraits�ҹ���
                        for (int j = 0; j < m.potraits.Length; j++)
                        {
                            if (m.potraits[j] == null) continue;
                            print(m.potraits[j].id);
                            if (m.potraits[j].id == actualOpponent)
                            {*/
                                haveStory = true;
                                startLine = i;
                                UpdateAnimatorParameters();
                                Popup(); //popup��������棬��Ϊ���������ı�index
                            /*    break;
                            }
                                
                        }*/
                    }
                    else
                    {
                        //����storylineд����˭����˭�Ķ�ս
                        haveStory = true;
                        startLine = i;
                        storyFound = true;
                        Popup();
                        break;
                    }
                }
                
            }
        }

        //�����ɫ������û�й��£�
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
        //�ǲ��Ǵ���ӵĺ��ѣ�
        if (toBeAdded)
        {
            ani.SetBool("new", true);
        }
        else
        {
            ani.SetBool("new", false);
        }

        //�����ɫ������û�й��£�
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
        { //Ӣ�
            //print("read English");
            txt.Read(path, id + "_en" + ".csv");
        }
        else
        {//Ӣ������ļ������
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
        {//˵��ѡ���˺͵���ս��
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
