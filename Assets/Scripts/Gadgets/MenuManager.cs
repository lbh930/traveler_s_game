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

    public static int troopAccess = 0; //0 ����ȫ����1������(ԭʼ���ǰ��������������2�������֣�ԭʼ�ͳǰ�)

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
        //��һ��������д����
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
        LoadSavefile();//���¶�ȡ�浵������
        DestoryPotraits();

        int counter = 0;//counter��ʾʵ�������ɵ��ڼ�����Ҫ���ɵ�ͷ����
        potraits = new MenuPotrait[list.characters.Length];

        potraits[0] = GameObject.FindGameObjectWithTag("PlayerPotrait").GetComponent<MenuPotrait>();

        for (int i = 1; i < list.characters.Length; i++)
        {
            if (CheckCharacterUnlocked(list.characters[i].id)) //����ý�ɫ�Ѿ�����
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

        //����ɳ��ͼ��
        MenuPotrait mp1 = Instantiate(potraitPrefab, potraitsStartPoint).GetComponent<MenuPotrait>();
        RectTransform rt1 = mp1.GetComponent<RectTransform>();
        Vector2 pos1 = new Vector2(potraitsStartPoint.rect.x + (counter % potraitsPerRow) * matrixPadding.x * rt1.rect.width,
            potraitsStartPoint.rect.y - Mathf.Floor(((counter) / potraitsPerRow)) * matrixPadding.y * rt1.rect.height);

        rt1.anchoredPosition = pos1;

        mp1.id = "sandbox";
        mp1.UpdatePotrait();
        potraits[counter + 1] = mp1;
        counter++;

        UpdatePotraitPositions(); //����һ��λ��

        //�����¼��己��
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

        PressSkipNotice.noticeShowed = false; //��ʼ��

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

        //�ҵ�potrait�Ͳ�ͬ�����panel
        dialoguePotrait = GameObject.FindGameObjectWithTag("MenuDialoguePotrait").GetComponent<MenuPotrait>();//�ҵ������ͷ��
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

        if (selectedPotrait != null)//�����޸�����ҳ���е����ߡ�����״̬
        {
            for (int i = 0; i < potraits.Length; i++)
            {
                if (potraits[i] != null && potraits[i].id == selectedPotrait)
                {
                    if (potraits[i].id == "random")
                    {//���ѡ���˵������, ���߾��飬 һ������ƥ��
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
                    { //���������߾���
                        if (potraits[i].haveStory)
                        {//˵��Ӧ����ʾΪ����
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

        //��ʱ���ݼ�
        nextProgressTimer -= Time.deltaTime;
        readStoryTimer -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.F))
        {
            print(storyIndex);
        }
        //�����ʱ������0��˵������ʱ���
        if (nextProgressTimer < 0 && readStoryTimer < 0)
        {
            nextProgressTimer = float.PositiveInfinity; //���ó�һ����Զ������0����
            storyIndex++;
            txt.Write(0, 1, storyIndex.ToString(), "Save.txt", ';'); //д���浵
            FindStory();//����������������potrait�����ҹ��µĺ���
            UpdatePotraitPositions();//�й��µĻ�����1�������ķ�ǰ����
        }

        if (dialoguePotrait == null)
        {
            dialoguePotrait = GameObject.FindGameObjectWithTag("MenuDialoguePotrait").GetComponent<MenuPotrait>();//�ҵ������ͷ��
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
                            //����
                            dialoguePotrait.nameText.text = list.characters[i].names[0];
                            addFriendPotrait.nameText.text = list.characters[i].names[0];
                        }
                        else
                        {
                            //Ӣ��
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
            MenuPotrait potrait = null; //�����ҵ���Ӧ��potrait�����պ����
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
                //Ĭ�ϲ������촰
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
            //ͬ����Ĭ�ϲ������촰
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
            }//���ȣ��Ӻ��ѵ�
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
            }//��Σ��й���
        }
    }

    public void ClickSingle()
    {
        GetComponent<FlashMoveTowards>().SetNewDestination(camPos2.position, camPos2.rotation.eulerAngles, 3);
        canvasAni.SetTrigger("CamPos2");
        GameObject.FindGameObjectWithTag("MenuFade").GetComponent<Animator>().SetBool("MenuFade", true); //�����ϵļ���
        GameObject projectors = GameObject.FindGameObjectWithTag("Menu_Projectors_Side");
        for (int i = 0; i < projectors.transform.childCount; i++)
        {
            projectors.transform.GetChild(i).gameObject.SetActive(true); //�򿪲���ͶӰ
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

        //��������
        if (calendar != null)
        {
            calendar.SetCalendar(day);
        }

        for (int i = 0; i < txtStoryline.m_ArrayData.Count; i++)
        {
            if (txtStoryline.m_ArrayData[i].Length <= 0) continue;

            if (txtStoryline.getString(i, 0) == "day" && txtStoryline.getInt(i,1) == day)
            {
                int pointingLine = i + storyIndex + 1; //���ڵ�day��storyindex��ָ���һ��
                if (pointingLine < txtStoryline.m_ArrayData.Count)
                {
                    if (txtStoryline.getString (pointingLine, 0) == "day")
                    {//���ֽ����Ѿ�û�й��£������index���㣬������1
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
                dayNumber.text = "�� " + day.ToString() + " ��";
            }
            else
            {
                dayNumber.text = "DAY " + day.ToString();
            }
        }
    }

    //Progress���������ƽ���Ϸ���ȣ�������һ�׶�
    public void Progress()
    {
        //���ҵ���Ӧ����
        for (int i = 0; i < txtStoryline.m_ArrayData.Count; i++)
        {
            if (txtStoryline.getString(i, 0) == "day") //Ѱ��������ʶ
            {
                if (txtStoryline.getInt(i, 1) == day) //�ҵ���Ӧ��������
                {
                    if (txtStoryline.m_ArrayData[i + storyIndex + 1].Length >= 3)
                    { //���ڵ���3˵���д���ʱ����Ϣ
                        nextProgressTimer = txtStoryline.getFloat(i + storyIndex + 1, 2);
                    }
                    else
                    {
                        nextProgressTimer = 2.0f;//Ĭ�ϵ����������һ��
                    }
                }
            }
        }
    }


    public void WriteSavefileLine(string content)
    {
        txt.Write(txt.m_ArrayData.Count, 0, content, "Save.txt", ';');
    }

    public void IncrementDay() //��һ�죨���޸ģ�
    {
        day = day + 1;
    }
    public void FindStory()
    {
        for (int i = 0; i < txtStoryline.m_ArrayData.Count; i++)
        {
            if (txtStoryline.getString(i, 0) == "day") //Ѱ��������ʶ
            {
                if (txtStoryline.getInt(i, 1) == day) //�ҵ���Ӧ��������
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
                            WriteSavefileLine(id); //����Ҫ�Ӻ��ѵĽ�ɫ
                        }
                        toAddCharacterId = id; //���ú�Ҫ��ӵ�
                        print("to be added: " + id);
                        //GeneratePotraits(); //��������һ��potraits �����¼ӵĽ�ɫ������
                        /*for (int j = 0; j < potraits.Length; j++)
                        {
                            if (potraits[j] == null) continue;//�����յ�

                            if (potraits[j].id == id) //�ҵ��Ե��Ǹ�potrait
                            {
                                potraits[j].toBeAdded = true;
                                potraits[j].GetComponent<Animator>().SetBool("new", true);
                                print("to be added: " + potraits[j].id);
                            }
                        }*/
                    }
                    else if (txtStoryline.getString(i + storyIndex + 1, 0) != "memo")
                    {
                        if (txtStoryline.m_ArrayData[i+storyIndex+1].Length >= 4) //��4λ�Ǳ�ʾ���Ƿ������֣������
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

                        if (txtStoryline.m_ArrayData[i + storyIndex + 1].Length >= 3)//�Ƿ���Ҫ��λ��������ɫ
                        {
                            print("should retarget other character");
                            if (txtStoryline.m_ArrayData[i + storyIndex + 1][2].Length > 0)
                            {
                                nextStoryCharacterId = txtStoryline.m_ArrayData[i + storyIndex + 1][2];
                                actualCharaterId = nextStoryCharacterId;
                                print("new charater id is :" + nextStoryCharacterId);
                            }
                        }
                        
                            //����ȫ��potraits�ҹ���
                            bool found = false;
                        for (int j = 0; j < potraits.Length; j++)
                        {
                            if (potraits[j] == null) continue;
                            if (potraits[j].id == nextStoryCharacterId || potraits[j].id == actualCharaterId)
                            {//ֻˢ��Ŀǰ�й��µĽ�ɫ
                                found = true;            
                                print(potraits[j].id + " is trying to load savefile  " + nextStoryCharacterId + "  " + potraits[j].id);
                                potraits[j].LoadSavefile();
                              
                            }
                        }

                        //�Ҳ�������������ɫ

                            if (!CheckCharacterUnlocked(nextStoryCharacterId))
                            {
                                WriteSavefileLine(nextStoryCharacterId); //����Ҫ�Ӻ��ѵĽ�ɫ
                                toAddCharacterId = nextStoryCharacterId; //���ú�Ҫ��ӵ�
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
