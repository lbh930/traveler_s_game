using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlotMachine : MonoBehaviour
{
    // Start is called before the first frame update
    public int forceLineNow = -1;
    public bool ignoreBattleState = false;

    bool quitAndHold = false;
    bool initialized = false;

    public AudioClip[] clipList;
    SubtitlePotrait subtitlePotrait;

    IdentityList list;
    AudioSource source;
    public static int lineNow;
    public static bool nextRoundAvailable = false;

    bool charaSet = false;
    bool lineWorking = false;
    float clipTimeToEnd = -1;

    float lineKeepTimer = 100000;
    float holdTalkTimer = 2.0f;
    public float quitTimer = float.PositiveInfinity;
    public float setReadyTimer = float.PositiveInfinity;
    float emptyTimer = float.PositiveInfinity;
    float quitBattleTimer = float.PositiveInfinity;

    float startedAgainTimer = float.PositiveInfinity;

    CanvasPhoto canvasPhoto;
    VoiceVolume vv;

    [HideInInspector]public bool holding = false;

    TxtReader txt;
    public TxtReader savefileTxt;
    RoundManager round;
    bool savefileIncremented = false;

    bool quitnopop = false; //�Ƿ�����ʾ���˳�

    float[] clipsample = new float[512];

    public struct Chara{
        public string id;
        public AudioClip audio;
        public bool isPlayer;
    }

    public Chara[] chara;

    int lastLanguage = 2;
    void Start()
    {
        Initialize();
    }
    void Initialize()
    {
        
        if (initialized) return;

        vv = GetComponent<VoiceVolume>();
        txt = GetComponent<TxtReader>();
        source = GetComponent<AudioSource>();
        list = GameObject.FindObjectOfType<IdentityList>();
        round = GameObject.FindObjectOfType<RoundManager>();
        canvasPhoto = GameObject.FindObjectOfType<CanvasPhoto>();
        holding = false;
        nextRoundAvailable = false;
        lineWorking = false;
        startedAgainTimer = float.PositiveInfinity;

        savefileIncremented = false;
        quitBattleTimer = float.PositiveInfinity;

        GameObject sp = GameObject.FindGameObjectWithTag("SubtitlePotrait");
        if (sp != null)
        {
            subtitlePotrait = sp.GetComponent<SubtitlePotrait>();
            SubtitlePortraitDisable();
        }
        initialized = true;

        lastLanguage = PlayerPrefs.GetInt("language");
    }

    // Update is called once per frame
    void Update()
    {

        startedAgainTimer -= Time.deltaTime;
        //print(RoundManager.startedAgain););
        Initialize();

        if (source != null) {
            if (PauseScript.paused)
            {
                source.pitch = 0;

            }
            else
            {
                source.pitch = 1;
            }
        }
        startedAgainTimer -= Time.deltaTime;
        if (startedAgainTimer < 0)
        {
            RoundManager.startedAgain = false;//���δ���
            startedAgainTimer = float.PositiveInfinity;
        }
        /*if (Input.GetKeyDown(KeyCode.V)) //����debug
        {
            savefileTxt.Read(Application.persistentDataPath, "Save.txt", ';');
            int storyIndexNow = savefileTxt.getInt(0, 1);
            savefileTxt.Write(0, 1, (storyIndexNow + 1).ToString(), "Save.txt", ';');

            GameObject.FindGameObjectWithTag("CanvasAnimator").GetComponent<Animator>().SetTrigger("QuitBattle");
            quitBattleTimer = 2.2f;
        }*/
    

       /* if (forceLineNow > 0)
        {
            lineNow = forceLineNow;
            source.Stop();
            lineKeepTimer = -1;
            lineWorking = false;
            forceLineNow = -1;
        }*/

        if (source.clip != null) {//��amplitude�����������Լ���clicking sound
            float amp = 0;
            
            /*
            source.clip.GetData(clipsample, source.timeSamples);
            for (int i = clipsample.Length/2; i < clipsample.Length; i++)
            {
                amp += Mathf.Abs(clipsample[i]);
            }
            amp /= clipsample.Length;
            if (amp < 0.0004f)
            {
                if (vv != null)
                {
                    vv.basicVolume = 0;
                }
            }
            else
            {
                if (vv != null)
                {
                    vv.basicVolume = 1;
                }
            }*/
        }


        if (MenuManager.haveStory)
        {
            if (!charaSet) SetChara();

            Plot();
        }

        lineKeepTimer -= Time.deltaTime;
        setReadyTimer -= Time.deltaTime;
        holdTalkTimer -= Time.deltaTime; //������0ʱ�����ٳ����¶Ի�
        quitBattleTimer -= Time.deltaTime;
        quitTimer -= Time.deltaTime;
        emptyTimer -= Time.deltaTime;

        if (lineKeepTimer <= 0) SubtitlePortraitDisable();
        if (setReadyTimer <= 0)
        {
            for (int i = 0; i < round.players.Length; i++)
            {
                if (!round.players[i].myControl)
                {
                    round.players[i].ready = true;
                }
            }
            setReadyTimer = float.PositiveInfinity;
        }

        if (quitBattleTimer < 0)
        {
            print("quitbattletimer < 0, load menu");
            MenuManager.quitFromBattle = true;
            SceneManager.LoadScene("Menu");
        }

        //���û�й涨��ʲôʱ��׼�������Զ�׼��
        TryAutoReady();

        //������Ա�����Զ������ĵ�
        if (PlayerPrefs.GetInt("language") != lastLanguage)
        {
            LoadFileByLanguage();
        }

        if (quitTimer < 0)
        {
            /*
            Animator ani = GameObject.FindGameObjectWithTag("CanvasAnimator").GetComponent<Animator>();
            ani.SetTrigger("QuitGameFade"); //��Եfadeһ��

            if (quitTimer < -1.1f) //fade���������1��
            {
                MenuManager.quitToStoryMenu = true;

                RoundManager.startedAgain = false; //��ֹ��ѧbug
                SceneManager.LoadScene("Menu");
            }*/
            

            if (!savefileIncremented) //����ֻд��һ��
            {
                print("trying to save");
                if (!quitnopop)
                {
                    print("quitnopop: " + quitnopop);
                    NoticePoper.PopNotice(0);
                    print("quitnopop!: " + quitnopop);
                }

                savefileIncremented = true;
                if (savefileTxt != null)
                {
                    savefileTxt.Read(Application.streamingAssetsPath, "Save.txt", ';');
                    int storyIndexNow = savefileTxt.getInt(0, 1);//�ȶ��������ڵ�index
                    print("save story index as: " + (storyIndexNow+1).ToString());
                    savefileTxt.Write(0, 1, (storyIndexNow + 1).ToString(), "Save.txt", ';');
                }
                else
                {
                    print("savefiletxt not exist");
                    NoticePoper.PopNotice(1);//��������ʧ�ܵ���ʾ
                    
                }
            }

            if ((quitTimer < -3.6 && clipTimeToEnd <= 0) && !quitAndHold)
            {
                if (quitBattleTimer > 1000000)
                {
                    GameObject.FindGameObjectWithTag("CanvasAnimator").GetComponent<Animator>().SetTrigger("QuitBattle");
                    quitBattleTimer = 1.5f;
        
                }
            }
        }
    }

    void TryAutoReady()
    {
        if (setReadyTimer < 1000) return;

        bool shouldAutoReady = false;

        if (RoundManager.gameIndex == 0 && txt.getString(lineNow, 2) != "start")
        {
            shouldAutoReady = true;
        }
        if (RoundManager.gameIndex == 1 && txt.getString(lineNow, 2) != "set0" && txt.getString(lineNow, 2) != "start")
        {
            shouldAutoReady = true;
        }
        if (RoundManager.gameIndex == 2 && txt.getString(lineNow, 2) != "set1" && txt.getString(lineNow, 2) != "set0" && txt.getString(lineNow, 2) != "start")
        {
            shouldAutoReady = true;
        }
        if (RoundManager.gameIndex == 6 && txt.getString(lineNow, 2) != "set2" && txt.getString(lineNow, 2) != "wait1" && txt.getString(lineNow, 2) != "battle1")
        {
            shouldAutoReady = true;
        }
        if (RoundManager.gameIndex == 10 && txt.getString(lineNow, 2) != "set3" && txt.getString(lineNow, 2) != "wait2" && txt.getString(lineNow, 2) != "battle2")
        {
            shouldAutoReady = true;
        }
        if (RoundManager.gameIndex == 14 && txt.getString(lineNow, 2) != "set4" && txt.getString(lineNow, 2) != "wait3" && txt.getString(lineNow, 2) != "battle3")
        {
            shouldAutoReady = true;
        }

        if (txt.getString(lineNow, 2) == "") //����wait��Ӱ��
        {
            shouldAutoReady = false;
        }

        if (nextRoundAvailable) //����Ѿ�׼�������¾��ˣ������������Ϸ�׶α�ʶ���ٱ���˳����ǿ���Զ�׼��
        {
            shouldAutoReady = true;
        }

        //���û���£�Ҫ�Զ�׼��
        if (MenuManager.nextStoryCharacterId == null || MenuManager.nextStoryCharacterId == "" || MenuManager.nextStoryCharacterId == "random")
        {
            shouldAutoReady = true;
        }

        if (shouldAutoReady)
        {
            /*print(RoundManager.gameIndex);
            print(txt.getString(lineNow, 2));
            print(lineNow);*/

            for (int i = 0; i < round.players.Length; i++)
            {
                if (!round.players[i].myControl && !round.players[i].ready)
                {
                    setReadyTimer = Random.Range(3.2f, 5.2f);
                }
            }    
        }
        //print(RoundManager.gameIndex.ToString() + " " + txt.getString(lineNow, 2));
    }
    
    void LoadFileByLanguage()
    {
        string path = Application.streamingAssetsPath + "/Plots/" + MenuManager.nextStoryCharacterId;

        print("reading: " + MenuManager.nextStoryCharacterId + ".csv");

        int language = PlayerPrefs.GetInt("language");
        lastLanguage = language;
        if (language == 2)
        { //Ӣ�
            print("read English");
            txt.Read(path, MenuManager.nextStoryCharacterId + "_en" + ".csv", ';');
        }
        else
        {//Ӣ������ļ������
            print("read t_cn");
            txt.Read(path, MenuManager.nextStoryCharacterId + ".csv", ';');
        }

        if (txt.m_ArrayData.Count <= 0)
        {//���Ӣ�Ķ�ȡʧ��
            txt.Read(path, MenuManager.nextStoryCharacterId + ".csv", ';');
        }
    }

    void SetChara()
    { 
        int indexNow = MenuManager.startLine + 1;

        LoadFileByLanguage(); //�������Զ�ȡ�ĵ�
        

        int charCount = 0;

        while (txt.getString(indexNow,0) == "*voice")
        {
            //print(txt.getString(indexNow, 1));
            charCount++;
            indexNow++;
            
        }
        
        
        indexNow -= charCount;
        chara = new Chara[charCount];

        Debug.Log("linenow was: " + lineNow.ToString());
        if (!RoundManager.startedAgain) //�����������ս�֣����Ǹս���ս�֣��ų�ʼ��linenow
        {
            lineNow = MenuManager.startLine + 1 + charCount;
            Debug.Log("not started again");
        }
        else
        {
            Debug.Log("started again");
        }
        Debug.Log(RoundManager.startedAgain + "  started again");
        Debug.Log("linenow: " + lineNow.ToString());

        for (int i = 0; i < charCount; i++)
        {
            chara[i].id = txt.getString(indexNow + i, 1);
            if (chara[i].id == "zrg") //��⵽id��zrg����Ϊ����
            {
                chara[i].isPlayer = true;
            }
            else
            {
                chara[i].isPlayer = false;
            }
            for (int j = 0; j < clipList.Length; j++)
            {
                if (clipList[j].name == txt.getString(indexNow + i, 2))
                {
                    chara[i].audio = clipList[j];
                }
            }
        }

        charaSet = true;
    }
    void Plot()
    {

        if (lineNow < txt.m_ArrayData.Count && txt.m_ArrayData[lineNow].Length > 1) //δ���в���������
        {

            if (!lineWorking && !holding){ //��ǰû��������һ�жԻ����ǲ��Ǵ��ڵȴ���һ�ֵ�״̬��

                if (txt.getString(lineNow, 0) == "empty" || txt.getString(lineNow, 0) == "wait") //��һλ�ŵ��ǿշ��ţ�
                {
                    if (emptyTimer > 100000)
                    {
                        emptyTimer = txt.getFloat(lineNow, 1);
                        print("emptyTimer: " + emptyTimer.ToString());
                    }
                    if (emptyTimer <= 0)
                    {
                        emptyTimer = float.PositiveInfinity;
                        lineNow++; //��ʱ����ɾ���һ��
                    }
                }else if (txt.getString(lineNow, 0) == "photo")
                {
                    //չʾ��Ƭ
                    if (canvasPhoto != null)
                    {
                        if (txt.getInt(lineNow,1) == -1)
                        {
                            //-1 ��������Ƭ
                            print("removing photo");
                            canvasPhoto.RemovePhoto();
                        }
                        else
                        {
                            print("showing photo and linenow is " + lineNow.ToString() + " " + txt.getInt(lineNow, 1).ToString());
                            canvasPhoto.SetPhoto(txt.getInt(lineNow, 1));
                        }
                    }
                    else
                    {
                        print("canvasPhoto not assigned");
                    }
                    lineNow++;
                }
                else if (txt.getString(lineNow,0) != "*plot") //���ǵ�����һ���籾��������һ�е�һλ�ŵ��Ǹ��������,�����ǿո�
                {
                    if (txt.getString(lineNow, 0).Length < 1)
                    {//����ǿ��У�ɶҲ���
                        lineNow++;

                        return;
                    }
                    string condi = "";
                    condi = txt.getString(lineNow, 2);

                    if (holdTalkTimer <= 0 && CheckPlotCondition(condi))
                    {
                        lineWorking = true;

                        bool foundChara = false;
                        for (int i = 0; i < chara.Length; i++)
                        {
                            //print(chara.Length);
                            //print(chara[i].id + "  " + txt.getString(lineNow, 0));
                            if (chara[i].id == txt.getString(lineNow, 0))
                            {
                                foundChara = true;
                                source.clip = chara[i].audio;
                                float time = txt.getFloat(lineNow, 3);
                                if (time > 0)
                                {
                                    source.time = time;
                                    if (txt.m_ArrayData[lineNow].Length >= 5)
                                    {
                                        clipTimeToEnd = txt.getFloat(lineNow, 4) - time;
                                    }
                                    else
                                    {
                                        clipTimeToEnd = 5;
                                    }
                                }
                                else
                                {
                                    source.time = (lineNow - MenuManager.startLine) * 10;
                                    clipTimeToEnd = 8;
                                }
                                source.Play();
                                SubtitlePotraitActive();
                            }
                        }

                        if (!foundChara)//���û�ҵ� ������һ��
                        {
                            lineNow++;
                            print("chara not found");
                        }
                    }
                }
            }

            string text = txt.getString(lineNow, 1); //text���ڼ����а�����ָ��

            if (clipTimeToEnd < 0 && lineWorking) //��ⷢ������仰˵�����Ժ�
            {
                if (!text.Contains("*reserve"))
                {
                    lineKeepTimer = 0.8f;
                }
                if (text.Contains("*ready"))
                {
                    setReadyTimer = 0.5f;
                }
                if (text.Contains("*2ready"))
                {
                    setReadyTimer = 2.0f;
                }
                if (text.Contains("*3ready"))
                {
                    setReadyTimer = 3.0f;
                }
                if (text.Contains("*quit") || text.Contains("#quit") || text.Contains("1quit"))
                {
                    quitTimer = 0.5f;
                    print("quit timer set to 0.5");
                }
                if (text.Contains("*3quit") || text.Contains("#3quit") || text.Contains("3quit"))
                {
                    quitTimer = 3.0f;
                    print("quit timer set to 3");

                }
                if (text.Contains("*2quit") || text.Contains("#2quit") || text.Contains("2quit"))
                {
                    quitTimer = 2.0f;
                    print("quit timer set to 2");
                }
                if (text.Contains("*quitnopop") || text.Contains("#1quitnopop"))
                {
                    quitTimer = 1.0f;
                    quitnopop = true;
                    print("quitnopop");
                }
                if ((text.Contains("*newround") || text.Contains("* newround")) || text.Contains("newround") || (lineNow < txt.m_ArrayData.Count-1 && txt.getString(lineNow+1, 2) == "startagain"))
                {
                    print("set nextroundavailable to true");
                    nextRoundAvailable = true;
                }
                if (text.Contains("*hold") || text.Contains("* hold") || text.Contains("#hold") || text.Contains ("1hold"))
                {
                    holding = true;
                }
               
                if (text.Contains("*10quit"))
                {
                    quitTimer = 10.0f;
                }

                if (text.Contains("*qandh"))
                {
                    quitAndHold = true;
                }
                if (text.Contains("*stopqh"))
                {
                    quitAndHold = false;
                }

                lineWorking = false;
                source.Pause();
                lineNow++;
                print("lineNow is : " + lineNow.ToString());
            }
        }

        if (lineWorking)
        {
            clipTimeToEnd -= Time.deltaTime;
            //����F����
            if (Input.GetKeyDown(KeyCode.F))
            {
                clipTimeToEnd = 0;
            }
        }   
    }

    void SubtitlePotraitActive()
    {
        if (subtitlePotrait == null) return;

        subtitlePotrait.gameObject.SetActive(true);
        for (int i = 0; i < list.characters.Length; i++)
        {
            if (txt.getString (lineNow, 0) == list.characters[i].id)
            {
                subtitlePotrait.image.sprite = list.characters[i].potraits[0];

                int language = PlayerPrefs.GetInt("language");

                if (language == 0 || language == 1)
                {
                    subtitlePotrait.name.text = list.characters[i].names[0];
                }
                else
                {
                    subtitlePotrait.name.text = list.characters[i].names[1];
                }
                break;
            }
        }

        string s = txt.getString(lineNow, 1);

        s = s.Replace("*reserve", ""); //��ȥ���б�ʶ��
        s = s.Replace("*ready", "");
        s = s.Replace("*2ready", "");
        s = s.Replace("*quitnopop", "");
        s = s.Replace("*quit", "");
        s = s.Replace("*2quit", "");
        s = s.Replace("*3quit", "");
        s = s.Replace("*1quit", "");
        //s = s.Replace("1quit", "");
        //s = s.Replace("2quit", "");
        //s = s.Replace("3quit", "");
        s = s.Replace("#1quitnopop", "" );
        s = s.Replace("#quit", "");
        s = s.Replace("#1quit", "");
        s = s.Replace("#2quit", "");
        s = s.Replace("#3quit", "");
        s = s.Replace("*quit", "");
        s = s.Replace("*3ready", "");
        s = s.Replace("*newround", "");
        s = s.Replace("*hold", "");
        s = s.Replace("#newround", "");
        s = s.Replace("#hold", "");
        s = s.Replace("newround", "");
        s = s.Replace("1hold", "");
        s = s.Replace("* hold", "");
        s = s.Replace("*newround", "");
        s = s.Replace("* newround", "");
        s = s.Replace("*10quit", "");
        s = s.Replace("*qandh", "");
        s = s.Replace("*stopqh", "");

        s = s.TrimStart();

        //�Զ���д
        if (s.Length > 0 && s[0] > 'a' && s[0] < 'z')
        {
            char c = (char)(s[0] + 'A' - 'a');
            string toInsert = c.ToString();
            s = s.Remove(0, 1);
            s = s.Insert(0, toInsert);
          
            
        }
        
        subtitlePotrait.text.text = s;
        lineKeepTimer = 100000f;
    }

    void SubtitlePortraitDisable()
    {
        if (subtitlePotrait != null)
            subtitlePotrait.gameObject.SetActive(false);
        lineKeepTimer = 100000;
    }
    bool CheckPlotCondition(string condi)
    {
        if (ignoreBattleState) return true;
        //ɾ����Ϊ��������Ŀո�
        condi = condi.TrimEnd();
        condi = condi.TrimStart();

        if (condi != "result" && condi != "startagain" && RoundManager.startedAgain)
        {
            if (startedAgainTimer > 100)
            {
                startedAgainTimer = 7.0f;
            } //��ֹstartedagainû���Լ���0
        }

        if (condi == "start")
        {
            return true;
        }
        else if (condi == "set0")
        {
            if (RoundManager.gameIndex >= 1)
            {
                return true;
            }
        }
        else if (condi == "set1")
        {
            if (RoundManager.gameIndex >= 2)
            {
                return true;
            }
        }
        else if (condi == "set2")
        {
            if (RoundManager.gameIndex >= 6)
            {
                return true;
            }
        }
        else if (condi == "set3")
        {
            if (RoundManager.gameIndex >= 10)
            {
                return true;
            }
        }
        else if (condi == "set4")
        {
            if (RoundManager.gameIndex >= 14)
            {
                return true;
            }
        }
        else if (condi == "count1")
        {
            if (RoundManager.gameIndex >= 3)
            {
                return true;
            }
        }
        else if (condi == "count2")
        {
            if (RoundManager.gameIndex >= 7)
            {
                return true;
            }
        }
        else if (condi == "count3")
        {
            if (RoundManager.gameIndex >= 11)
            {
                return true;
            }
        }
        else if (condi == "count4")
        {
            if (RoundManager.gameIndex >= 15)
            {
                return true;
            }
        }
        else if (condi == "battle1")
        {
            if (RoundManager.gameIndex >= 4)
            {
                return true;
            }
        }
        else if (condi == "battle2")
        {
            if (RoundManager.gameIndex >= 8)
            {
                return true;
            }
        }
        else if (condi == "battle3")
        {
            if (RoundManager.gameIndex >= 12)
            {
                return true;
            }
        }
        else if (condi == "battle4")
        {
            if (RoundManager.gameIndex >= 16)
            {
                return true;
            }
        }
        else if (condi == "wait1")
        {
            if (RoundManager.gameIndex >= 5)
            {
                return true;
            }
        }
        else if (condi == "wait2")
        {
            if (RoundManager.gameIndex >= 9)
            {
                return true;
            }
        }
        else if (condi == "wait3")
        {
            if (RoundManager.gameIndex >= 13)
            {
                return true;
            }
        }
        else if (condi == "wait4")
        {
            if (RoundManager.gameIndex >= 17)
            {
                return true;
            }
        }
        else if (condi == "result")
        {
            if (RoundManager.gameIndex >= 18)
            {
                return true;
            }
        }else if (condi == "startagain")
        {
            if (RoundManager.startedAgain)
            {
                if (startedAgainTimer > 100)
                {
                    startedAgainTimer = 1.0f;
                }
              

                return true;
            }
        }
        else
        {
            return true;
        }

        return false; 
    }
}
