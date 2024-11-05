using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    bool initialized = false;

    IdentityList list;
    public Animator canvasAni;

    public GameObject potraitPrefab;
    public MenuPotrait[] potraits;
    public int potraitsPerRow = 3;
    public Vector2 matrixPadding = new Vector2(2,2);
    public RectTransform potraitsStartPoint;

    public MenuPotrait dialoguePotrait;
    public MenuPotrait addFriendPotrait;
    public GameObject panelFriend;
    public GameObject panelChat;

    MenuCalendar calendar;
    
    public Text dayNumber;

    public Button battleButton;

    public GameObject emptyDialogueBox;

    public static string selectedPotrait;
    public static int day;
    public static int storyIndex;
    public static int startLine;
    public static bool haveStory = false;
    public static bool quitToStoryMenu = false;

    public static int troopAccess = 0; //0 代表全部，1代表部分(原始，城邦，王国，王朝），2代表新手（原始和城邦)

    [HideInInspector] public TxtReader txt;
    [HideInInspector] public TxtReader txtStoryline;
    public static string nextStoryCharacterId;
    public static string actualCharaterId;
    public static string nextStoryFileId;
    public static string nextStoryName;
    public static string toAddCharacterId;
    //public static bool isArcade = false;
    public int currentProgram = -1;

    public static float nextProgressTimer = float.PositiveInfinity;
    float readStoryTimer = 1;
    public static bool quitFromBattle = false;

    public Button[] statusButtons;
    public Text[] statusTexts;

    public Texture cursorTexture;

    public enum menuState
    {
        menu,
        single,
        multi,
    }
    public Transform camPos1;
    public Transform camPos2;
    public menuState state = menuState.menu;

    string lastId = "a";

    RawImage cursor;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        if (!initialized) Initialize();
    }

    public void GoToNextDay(int newday)
    {
        print("gotonextday");
        //放一下日历特写动画
        GameObject.FindGameObjectWithTag("Menu_Force_Blackout").GetComponent<RawImage>().color = new Color(0, 0, 0, 1);
        Camera.main.transform.position = GameObject.FindGameObjectWithTag("Menu_CamCalendarPos").transform.position;
        if (canvasAni == null) {
            print("Canvas Animator Not ASSIGNED");
        }
        else
        {
            canvasAni.SetTrigger("FocusOnCalendar");
        }
        
        calendar.ChangeDate(day, newday);
        txt.Write(0, 0, newday.ToString(), "Save.txt", ';');
        txt.Write(0, 1, (0).ToString(), "Save.txt", ';');
        storyIndex = 0;
        day = newday;
    }
    public void DestoryPotraits()
    {
        if (potraits.Length > 0)
        {
            for (int i = 0; i < potraits.Length; i++)
            {
                if (potraits[i] != null && potraits[i].tag != "PlayerPotrait")
                    Destroy(potraits[i].gameObject);
            }
        }
    }

    bool CheckCharacterUnlocked(string id)
    {
        for (int i = 0; i < txt.m_ArrayData.Count; i++)
        {
            if (txt.getString(i, 0) == id) return true;
        }
        return false;
    }
    public void GeneratePotraits()
    {
        LoadSavefile();//重新读取存档再生成
        DestoryPotraits();

        int counter = 0;//counter表示实际上生成到第几个需要生成的头像了
        potraits = new MenuPotrait[list.characters.Length];

        potraits[0] = GameObject.FindGameObjectWithTag("PlayerPotrait").GetComponent<MenuPotrait>();

        for (int i = 1; i < list.characters.Length; i++)
        {
            if (CheckCharacterUnlocked(list.characters[i].id)) //如果该角色已经解锁
            {
                MenuPotrait mp = Instantiate(potraitPrefab, potraitsStartPoint).GetComponent<MenuPotrait>();
                RectTransform rt = mp.GetComponent<RectTransform>();
                Vector2 pos = new Vector2(potraitsStartPoint.rect.x + (counter%potraitsPerRow) * matrixPadding.x * rt.rect.width,
                    potraitsStartPoint.rect.y - Mathf.Floor(((counter) / potraitsPerRow)) * matrixPadding.y * rt.rect.height);

                rt.anchoredPosition = pos;
     
                mp.id = list.characters[i].id;
                mp.list = list;
                mp.UpdatePotrait();
                mp.txt = mp.GetComponent<TxtReader>();
                mp.LoadSavefile();
                potraits[counter + 1] = mp;
                counter++;
            }
        }

        //生成沙盒图标
        MenuPotrait mp1 = Instantiate(potraitPrefab, potraitsStartPoint).GetComponent<MenuPotrait>();
        RectTransform rt1 = mp1.GetComponent<RectTransform>();
        Vector2 pos1 = new Vector2(potraitsStartPoint.rect.x + (counter % potraitsPerRow) * matrixPadding.x * rt1.rect.width,
            potraitsStartPoint.rect.y - Mathf.Floor(((counter) / potraitsPerRow)) * matrixPadding.y * rt1.rect.height);

        rt1.anchoredPosition = pos1;

        mp1.id = "sandbox";
        mp1.UpdatePotrait();
        potraits[counter + 1] = mp1;
        counter++;

        UpdatePotraitPositions(); //更新一下位置

        //设置下简体繁体
        ChineseFontChanger cfc = GameObject.FindObjectOfType<ChineseFontChanger>();
        if (cfc != null)
        {
            cfc.ChangeFont();
        }
        //print(potraits.Length);
    }

    void Initialize()
    {
        startLine = 0;
        nextStoryCharacterId = "";
        nextStoryName = "";
        toAddCharacterId = "";
        actualCharaterId = "";

        troopAccess = 0;

        PressSkipNotice.noticeShowed = false; //初始化

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<RawImage>();
        cursor.texture = cursorTexture;
        cursor.rectTransform.position = new Vector2(Screen.width / 2, Screen.height / 2);
        cursor.rectTransform.sizeDelta = new Vector2(64, 64);

        RoundManager.gameIndex = 0;

        list = GameObject.FindObjectOfType<IdentityList>();
        selectedPotrait = "";
        haveStory = false;
        txt = gameObject.AddComponent<TxtReader>();
        txtStoryline = gameObject.AddComponent<TxtReader>();
        calendar = GameObject.FindGameObjectWithTag("Menu_Calendar").GetComponent<MenuCalendar>();

        //找到potrait和不同界面的panel
        dialoguePotrait = GameObject.FindGameObjectWithTag("MenuDialoguePotrait").GetComponent<MenuPotrait>();//找到聊天框头像
        addFriendPotrait = GameObject.FindGameObjectWithTag("MenuAddFriendPotrait").GetComponent<MenuPotrait>();
        panelChat = GameObject.FindGameObjectWithTag("Panel_Chat");
        panelFriend = GameObject.FindGameObjectWithTag("Panel_Friend");
        readStoryTimer = Random.Range(2.0f, 3.0f);


        RoundManager.startedAgain = false;

        LoadSavefile();

        if (quitToStoryMenu)
        {
            quitToStoryMenu = false;
            GetComponent<FlashMoveTowards>().SetNewDestination(camPos2.position, camPos2.rotation.eulerAngles, 4);
            canvasAni.SetTrigger("CamPos2");
            canvasAni.Update(10);
            canvasAni.Update(5);
            state = menuState.single;
        }

        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {



        Cursor.visible = false;
        cursor.rectTransform.position = Input.mousePosition;


        
        if (Input.GetKey(KeyCode.B)&&Input.GetKey(KeyCode.N))
        {
            Time.timeScale = 32.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        if (selectedPotrait != null)//用于修改人物页面中的在线、离线状态
        {
            for (int i = 0; i < potraits.Length; i++)
            {
                if (potraits[i] != null && potraits[i].id == selectedPotrait)
                {
                    if (potraits[i].id == "random")
                    {//如果选择了电脑玩家, 不走剧情， 一定可以匹配
                        statusTexts[0].gameObject.SetActive(true);
                        statusTexts[1].gameObject.SetActive(false);
                        statusTexts[2].gameObject.SetActive(false);
                        statusTexts[3].gameObject.SetActive(false);

                        statusButtons[0].gameObject.SetActive(true);
                        statusButtons[1].gameObject.SetActive(false);
                        statusButtons[2].gameObject.SetActive(false);
                    }
                    else if (potraits[i].id == "sandbox")
                    {
                        statusTexts[0].gameObject.SetActive(false);
                        statusTexts[1].gameObject.SetActive(false);
                        statusTexts[2].gameObject.SetActive(false);
                        statusTexts[3].gameObject.SetActive(true);

                        statusButtons[0].gameObject.SetActive(true);
                        statusButtons[1].gameObject.SetActive(false);
                        statusButtons[2].gameObject.SetActive(false);
                    }
                    else
                    { //否则是在走剧情
                        if (potraits[i].haveStory)
                        {//说明应该显示为在线
                            statusTexts[0].gameObject.SetActive(true);
                            statusTexts[1].gameObject.SetActive(false);
                            statusTexts[2].gameObject.SetActive(false);
                            statusTexts[3].gameObject.SetActive(false);

                            statusButtons[0].gameObject.SetActive(true);
                            statusButtons[1].gameObject.SetActive(false);
                            statusButtons[2].gameObject.SetActive(false);
                        }
                        else if (potraits[i].shouldOnline)
                        {
                            statusTexts[0].gameObject.SetActive(false);
                            statusTexts[1].gameObject.SetActive(true);
                            statusTexts[2].gameObject.SetActive(false);
                            statusTexts[3].gameObject.SetActive(false);

                            statusButtons[0].gameObject.SetActive(false);
                            statusButtons[1].gameObject.SetActive(true);
                            statusButtons[2].gameObject.SetActive(false);
                        }
                        else
                        {
                            statusTexts[0].gameObject.SetActive(false);
                            statusTexts[1].gameObject.SetActive(false);
                            statusTexts[2].gameObject.SetActive(true);
                            statusTexts[3].gameObject.SetActive(false);

                            statusButtons[0].gameObject.SetActive(false);
                            statusButtons[1].gameObject.SetActive(false);
                            statusButtons[2].gameObject.SetActive(true);
                        }
                    }
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape) && currentProgram >= 0)
        {
            currentProgram = -1;
            canvasAni.SetTrigger("QuitProgram");
            DestoryPotraits();
        }

        if (!initialized) Initialize();

        //计时器递减
        nextProgressTimer -= Time.deltaTime;
        readStoryTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.F))
        {
            print(storyIndex);
        }
        //如果计时器到了0，说明倒计时完毕
        if (nextProgressTimer < 0 && readStoryTimer < 0)
        {
            nextProgressTimer = float.PositiveInfinity; //设置成一个永远到不了0的数
            storyIndex++;
            txt.Write(0, 1, storyIndex.ToString(), "Save.txt", ';'); //写进存档
            FindStory();//这是用来遍历所有potrait来查找故事的函数
            UpdatePotraitPositions();//有故事的或者有1添加请求的放前面来
        }

        if (dialoguePotrait == null)
        {
            dialoguePotrait = GameObject.FindGameObjectWithTag("MenuDialoguePotrait").GetComponent<MenuPotrait>();//找到聊天框头像
        }

        if (addFriendPotrait == null)
        {
            addFriendPotrait = GameObject.FindGameObjectWithTag("MenuAddFriendPotrait").GetComponent<MenuPotrait>();
        }

        if (dialoguePotrait != null && selectedPotrait != null && selectedPotrait.Length > 0)


        {
            for (int i = 0; i < list.characters.Length; i++)
            {
                if (list.characters[i].id == selectedPotrait)
                {
                    dialoguePotrait.image.sprite = list.characters[i].potraits[0];
                    

                    addFriendPotrait.image.sprite = list.characters[i].potraits[0];
                    

                    if (lastId != list.characters[i].id)
                    {
                        lastId = list.characters[i].id;

                        int language = PlayerPrefs.GetInt("language");

                        if (language == 0 || language == 1)
                        {
                            //中文
                            dialoguePotrait.nameText.text = list.characters[i].names[0];
                            addFriendPotrait.nameText.text = list.characters[i].names[0];
                        }
                        else
                        {
                            //英语
                            dialoguePotrait.nameText.text = list.characters[i].names[1];
                            addFriendPotrait.nameText.text = list.characters[i].names[1];
                        }

                        dialoguePotrait.nameText.GetComponent<ChineseFontAutoChange>().Change();
                        addFriendPotrait.nameText.GetComponent<ChineseFontAutoChange>().Change();
                    }
                }
            }
        }

        if (selectedPotrait.Length > 0)
        {
            emptyDialogueBox.SetActive(false);
            MenuPotrait potrait = null; //试着找到对应的potrait方便日后操作
            for (int i = 0; i < potraits.Length; i++)
            {
                if (potraits[i] == null) continue;
                if (potraits[i].id == selectedPotrait)
                {
                    potrait = potraits[i];
                }
            }
            if (potrait != null)
            {
                if (!potrait.toBeAdded)
                {
                    panelChat.SetActive(true);
                    panelFriend.SetActive(false);
                }
                else
                {
                    panelChat.SetActive(false);
                    panelFriend.SetActive(true);
                }
            }
            else
            {
                //默认不开聊天窗
                panelChat.SetActive(false);
                panelFriend.SetActive(false);
                emptyDialogueBox.gameObject.SetActive(true);
            }

            for (int i = 1; i < potraits.Length; i++)
            {
                if (potraits[i] != null && selectedPotrait == potraits[i].id)
                {
                    if (potraits[i].haveStory)
                    {
                        battleButton.GetComponent<Animator>().SetBool("Invited", true);
                    }
                    else
                    {
                        battleButton.GetComponent<Animator>().SetBool("Invited", false);
                    }
                    break;
                }
            }
        }
        else
        {
            if (emptyDialogueBox != null)
            {
                emptyDialogueBox.SetActive(true);
            }
            //同样，默认不开聊天窗
            if (panelChat != null)
            {
                panelChat.SetActive(false);
            }
            if (panelFriend != null)
            {
                panelFriend.SetActive(false);
            }
            if (emptyDialogueBox != null)
            {
                emptyDialogueBox.gameObject.SetActive(true);
            }
        }
    }

    public void UpdatePotraitPositions()
    {        
        
         for (int i = 1; i < potraits.Length; i++)
        {
            if (i > 0 && potraits[i] != null && potraits[i].toBeAdded)
            {
                Vector2 temp = potraits[1].GetComponent<RectTransform>().position;
                MenuPotrait tempP = potraits[i];
                for (int j = i; j >= 2; j--)
                {
                    Vector2 temp1 = potraits[j - 1].GetComponent<RectTransform>().position;
                    MenuPotrait tempP1 = potraits[j - 1];
                    potraits[j - 1].GetComponent<RectTransform>().position = potraits[j].GetComponent<RectTransform>().position;
                    potraits[j].GetComponent<RectTransform>().position = temp1;
                    potraits[j - 1] = potraits[j];
                    potraits[j] = tempP1;
                }
                potraits[1] = tempP;
                potraits[1].GetComponent<RectTransform>().position = temp;
            }//优先：加好友的
        }

        for (int i = 1; i < potraits.Length; i++)
        {
            if (i > 0 && potraits[i] != null && potraits[i].haveStory)
            {
                Vector2 temp = potraits[1].GetComponent<RectTransform>().position;
                MenuPotrait tempP = potraits[i];
                for (int j = i; j >= 2; j--)
                {
                    Vector2 temp1 = potraits[j-1].GetComponent<RectTransform>().position;
                    MenuPotrait tempP1 = potraits[j-1];
                    potraits[j - 1].GetComponent<RectTransform>().position = potraits[j].GetComponent<RectTransform>().position;
                    potraits[j].GetComponent<RectTransform>().position = temp1;
                    potraits[j-1] = potraits[j];
                    potraits[j] = tempP1;
                }
                potraits[1] = tempP;
                potraits[1].GetComponent<RectTransform>().position = temp;
            }//其次：有故事
        }
    }

    public void ClickSingle()
    {
        GetComponent<FlashMoveTowards>().SetNewDestination(camPos2.position, camPos2.rotation.eulerAngles, 3);
        canvasAni.SetTrigger("CamPos2");
        GameObject.FindGameObjectWithTag("MenuFade").GetComponent<Animator>().SetBool("MenuFade", true); //桌子上的加载
        GameObject projectors = GameObject.FindGameObjectWithTag("Menu_Projectors_Side");
        for (int i = 0; i < projectors.transform.childCount; i++)
        {
            projectors.transform.GetChild(i).gameObject.SetActive(true); //打开侧向投影
        }

        state = menuState.single;

        GetComponent<SoundEffect>().PlayAudio();
    }

    public void LoadSavefile()
    {
        txt.Read(Application.streamingAssetsPath, "Save.txt");
        txtStoryline.Read(Application.streamingAssetsPath, "Storyline.txt");

        day = txt.getInt(0, 0);
        storyIndex = txt.getInt(0, 1);

        //设置日历
        if (calendar != null)
        {
            calendar.SetCalendar(day);
        }

        for (int i = 0; i < txtStoryline.m_ArrayData.Count; i++)
        {
            if (txtStoryline.m_ArrayData[i].Length <= 0) continue;

            if (txtStoryline.getString(i, 0) == "day" && txtStoryline.getInt(i,1) == day)
            {
                int pointingLine = i + storyIndex + 1; //现在的day和storyindex所指向的一行
                if (pointingLine < txtStoryline.m_ArrayData.Count)
                {
                    if (txtStoryline.getString (pointingLine, 0) == "day")
                    {//发现今天已经没有故事，则故事index清零，天数加1
                        GoToNextDay(txtStoryline.getInt(pointingLine, 1));
                    }
                    else
                    {
                        if (quitFromBattle)
                        {
                            canvasAni.SetTrigger("ForceBattleProgram");
                            currentProgram = 0;
                            GameObject.FindGameObjectWithTag("Menu_Force_Blackout").GetComponent<RawImage>().color = Color.black;
                            quitFromBattle = false;
                        }
                    }
                }

                break;
            }
        }

        if (dayNumber == null)dayNumber = GameObject.FindGameObjectWithTag("MenuDayNumber").GetComponent<Text>();




        SetDayText();
       

        FindStory();    
    }

    public void SetDayText()
    {
        if (dayNumber != null)
        {
            int language = PlayerPrefs.GetInt("language");
            if (language == 0 || language == 1)
            {
                dayNumber.text = "第 " + day.ToString() + " 天";
            }
            else
            {
                dayNumber.text = "DAY " + day.ToString();
            }
        }
    }

    //Progress函数用于推进游戏进度，进入下一阶段
    public void Progress()
    {
        //先找到对应的天
        for (int i = 0; i < txtStoryline.m_ArrayData.Count; i++)
        {
            if (txtStoryline.getString(i, 0) == "day") //寻找天数标识
            {
                if (txtStoryline.getInt(i, 1) == day) //找到对应天数区块
                {
                    if (txtStoryline.m_ArrayData[i + storyIndex + 1].Length >= 3)
                    { //大于等于3说明有储存时间信息
                        nextProgressTimer = txtStoryline.getFloat(i + storyIndex + 1, 2);
                    }
                    else
                    {
                        nextProgressTimer = 2.0f;//默认等两秒进入下一步
                    }
                }
            }
        }
    }


    public void WriteSavefileLine(string content)
    {
        txt.Write(txt.m_ArrayData.Count, 0, content, "Save.txt", ';');
    }

    public void IncrementDay() //加一天（待修改）
    {
        day = day + 1;
    }
    public void FindStory()
    {
        for (int i = 0; i < txtStoryline.m_ArrayData.Count; i++)
        {
            if (txtStoryline.getString(i, 0) == "day") //寻找天数标识
            {
                if (txtStoryline.getInt(i, 1) == day) //找到对应天数区块
                {
                    if (txtStoryline.getString(i + storyIndex + 1, 0) == "day")
                    {
                        IncrementDay();
                        FindStory();
                        break;
                    }
                    else if (txtStoryline.getString(i + storyIndex + 1, 0) == "new")
                    {
                        string id = txtStoryline.getString(i + storyIndex + 1, 1);
                        if (!CheckCharacterUnlocked(id))
                        {
                            WriteSavefileLine(id); //加上要加好友的角色
                        }
                        toAddCharacterId = id; //设置好要添加的
                        print("to be added: " + id);
                        //GeneratePotraits(); //重新生成一次potraits 这样新加的角色就有了
                        /*for (int j = 0; j < potraits.Length; j++)
                        {
                            if (potraits[j] == null) continue;//跳过空的

                            if (potraits[j].id == id) //找到对的那个potrait
                            {
                                potraits[j].toBeAdded = true;
                                potraits[j].GetComponent<Animator>().SetBool("new", true);
                                print("to be added: " + potraits[j].id);
                            }
                        }*/
                    }
                    else if (txtStoryline.getString(i + storyIndex + 1, 0) != "memo")
                    {
                        if (txtStoryline.m_ArrayData[i+storyIndex+1].Length >= 4) //第4位是表示了是否是新手，老玩家
                        {
                            if (txtStoryline.getString(i+storyIndex+1, 3) == "newbie")
                            {
                                troopAccess = 2;
                            }else if (txtStoryline.getString(i+storyIndex+1, 3) == "starter")
                            {
                                troopAccess = 1;

                            }
                            else
                            {
                                troopAccess = 0;
                            }
                        }
                        else
                        {
                            troopAccess = 0;
                        }

                        nextStoryCharacterId = txtStoryline.getString(i + storyIndex + 1, 0);
                        nextStoryFileId = nextStoryCharacterId;
                        print(nextStoryCharacterId);
                        nextStoryName = txtStoryline.getString(i + storyIndex + 1, 1);
                        print(nextStoryName);

                        if (txtStoryline.m_ArrayData[i + storyIndex + 1].Length >= 3)//是否需要定位到其它角色
                        {
                            print("should retarget other character");
                            if (txtStoryline.m_ArrayData[i + storyIndex + 1][2].Length > 0)
                            {
                                nextStoryCharacterId = txtStoryline.m_ArrayData[i + storyIndex + 1][2];
                                actualCharaterId = nextStoryCharacterId;
                                print("new charater id is :" + nextStoryCharacterId);
                            }
                        }
                        
                            //遍历全部potraits找故事
                            bool found = false;
                        for (int j = 0; j < potraits.Length; j++)
                        {
                            if (potraits[j] == null) continue;
                            if (potraits[j].id == nextStoryCharacterId || potraits[j].id == actualCharaterId)
                            {//只刷新目前有故事的角色
                                found = true;            
                                print(potraits[j].id + " is trying to load savefile  " + nextStoryCharacterId + "  " + potraits[j].id);
                                potraits[j].LoadSavefile();
                              
                            }
                        }

                        //找不到则添加这个角色

                            if (!CheckCharacterUnlocked(nextStoryCharacterId))
                            {
                                WriteSavefileLine(nextStoryCharacterId); //加上要加好友的角色
                                toAddCharacterId = nextStoryCharacterId; //设置好要添加的
                                print("to be added: " + nextStoryCharacterId);
                            }
                            
                            

                    }
                    else
                    {

                    }
                }
            }
        }
    }
}
