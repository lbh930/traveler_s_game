using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    public List<HumanAi> unitsPresent = new List<HumanAi>();
    // Start is called before the first frame update
    public static int gameIndex = 0;
    public static bool startedAgain = false;
    int lastIndex = -1;
    float timer;
    public int[] playerScores = { 0, 0 };
    public bool[] playerReady = { true, true };
    public OnePlayer[] players;

    float setToFightTimer = 1.0f;
    float battleOverTimer = 1.0f;
    float resultTimer = float.PositiveInfinity;

    int winnerIndex = -1;
    bool finished = false;

    bool waitForNextRoundFade = false;

    bool winCountIncremented = false;
    
    public int forceSetGameIndex = -1;

    bool drumPlayed = false;
    bool victoryPlayed = false;

    TxtReader txt;//���ڴ浵


    Animator canvasAni;
    Animator readyCanvasAni;
    Canvas deckCanvas;
    Canvas setCanvas;
    ObjectSet objects;
    FlashMoveTowards cameraMove;

    bool initialized = false;
    void Initialize()
    {
        if (initialized) return;
        initialized = true;
        objects = GetComponent<ObjectSet>();
        cameraMove = Camera.main.GetComponent<FlashMoveTowards>();

        GameObject enemyReadyCanvas = GameObject.FindGameObjectWithTag("EnemyReadyCanvas");
        if (enemyReadyCanvas != null) {
            readyCanvasAni = enemyReadyCanvas.GetComponent<Animator>();
        }

        GameObject canvasAnimatorObject = GameObject.FindGameObjectWithTag("CanvasAnimator");
        if (canvasAnimatorObject != null) {
            canvasAni = canvasAnimatorObject.GetComponent<Animator>();
        }
        gameIndex = 0;

        winCountIncremented = false;

        txt = GetComponent<TxtReader>();
        txt.Read(Application.streamingAssetsPath, "Save.txt", ';');
    }

    private void Start()
    {
        Initialize();
    }

    void SetScoreText()
    {
        int indexOfPlayer = 0;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].myControl)
            {
                indexOfPlayer = i;
            }
        }

        for (int i = 0; i < playerScores.Length; i++)
        {
            if (i == indexOfPlayer)
            {
                GameObject g = GameObject.FindGameObjectWithTag("MyScore");
                if (g != null)
                {
                    Text t = g.GetComponent<Text>();
                    t.text = playerScores[i].ToString();
                }
            }
            else
            {
                GameObject g = GameObject.FindGameObjectWithTag("OpponentScore");
                if (g != null)
                {
                    Text t = g.GetComponent<Text>();
                    t.text = playerScores[i].ToString();
                }
            }
        }
    }

    public void NewRound() //���뱾���Ի�����һ�ֶ�ս
    {
        gameIndex = 0;
        startedAgain = true;
        RoundManager.startedAgain = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Ŀǰû�п��Ǹı䳡��
    }

    /*public void EndRound()
    {
        startedAgain = false;
    }*/



    // Update is called once per frame
    void Update()
    {
        Initialize();

        if (MenuManager.nextStoryCharacterId == "sandbox")
        {
            return; //ɳ��ģʽ������
        }

        resultTimer -= Time.deltaTime; //��С��0ʱ��������һ��

        if (forceSetGameIndex >= 0)
        {
            gameIndex = forceSetGameIndex;
            forceSetGameIndex = -1;
        }

        SetScoreText();
        SetReadyCanvas();



        switch (gameIndex)
        {
            case 0://ѡ����
                if (Refresh(gameIndex))
                {
                    GameObject inicam = GameObject.FindGameObjectWithTag("InitialCameraPos");
                    cameraMove.SetNewDestination(inicam.transform.position, 
                        inicam.transform.rotation.eulerAngles, 1);

                    deckCanvas = GameObject.FindGameObjectWithTag("DeckCanvas").GetComponent<Canvas>();
                    setCanvas = GameObject.FindGameObjectWithTag("SetCanvas").GetComponent<Canvas>();
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (players[i].myControl)
                        {
                            players[i].GetButtons();
                            players[i].SetDeckButtons();
                            players[i].state = OnePlayer.PlayerState.deck;
                            players[i].round = this;
                        }
                    }
                    deckCanvas.gameObject.SetActive(true);
                    setCanvas.gameObject.SetActive(false);

                    if (readyCanvasAni != null)
                        readyCanvasAni.SetTrigger("ShowReady");
                }
                if (CheckReady())
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        players[i].ready = false;
                    }
                    readyCanvasAni.SetTrigger("ReadyFade");
                    gameIndex = 1;
                }
                break;
            case 1://Ԥ�±�
                if (Refresh(gameIndex))
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        players[i].currency = 2000;
                        players[i].state = OnePlayer.PlayerState.set;
                        if (players[i].myControl)
                        {
                            players[i].SetArrangeButtons();                            

                            cameraMove.SetNewDestination(
                                players[i].controllerCameraPos.position, players[i].controllerCameraPos.rotation.eulerAngles, 2);
                        }
                    }
                    deckCanvas.gameObject.SetActive(true);
                    setCanvas.gameObject.SetActive(true);
                    canvasAni.SetBool("DeckToSet", true);
                    readyCanvasAni.SetTrigger("ShowReady");

                }

                if (CheckReady())
                {
                    UpdateUnitList();
                    for (int i = 0; i < players.Length; i++)
                    {
                        players[i].troopDone = false;
                        players[i].GetExistedUnits(); //Ҫ��unitpresent�����ִ��
                        players[i].ready = false;
                    }
                    readyCanvasAni.SetTrigger("ReadyFade");
                    canvasAni.SetTrigger("SetToFight");
                    setToFightTimer = 1;
                    gameIndex++;
                }
                break;
            case 2://��һ���±�
                if (Refresh(gameIndex))
                {
                    //�л������ֵ��ӽ�(��ʱ�������������)
                    /*
                    for (int i = 0; i < players.Length; i++)
                    {
                        if (!players[i].myControl)
                        {
                            cameraMove.SetNewDestination(players[i].inspectorCameraPos, 0.5f);
                        }
                        else
                        {
                            //�޸�inspect index

                            players[i].inspectIndex = 0;
                        }
                    }*/

                    for (int i = 0; i < players.Length; i++)
                    {
                        players[i].currency = 1000;
                        players[i].state = OnePlayer.PlayerState.set;
                        if (players[i].myControl)
                        {
                            players[i].SetArrangeButtons();

                            cameraMove.SetNewDestination(
                                players[i].controllerCameraPos.position, players[i].controllerCameraPos.rotation.eulerAngles, 2);
                        }
                        else
                        {
                            for (int j = 0; j < unitsPresent.Count; j++)
                            {
                                unitsPresent[j].gameObject.SetActive(true);
                                unitsPresent[j].canEdit = false;
                                if (unitsPresent[j].unitTag != null && unitsPresent[j].unitTag.show != false)
                                {
                                    unitsPresent[j].unitTag.dontShow = true; //����Ԥ�±���tagΪ���ɱ༭
                                }
                                //print(unitsPresent[j].name);
                                if (unitsPresent[j].horse != null)
                                {
                                    unitsPresent[j].horse.gameObject.SetActive(true);
                                }
                            }
                        }
                    }
                    deckCanvas.gameObject.SetActive(true);
                    setCanvas.gameObject.SetActive(true);
                    canvasAni.SetBool("EmptyToSet", true);
                    readyCanvasAni.SetTrigger("ShowReady");

                }

                if (CheckReady())
                {
                    canvasAni.SetTrigger("SetToFight");
                    setToFightTimer = 1;
                    gameIndex++;
                }
                break;
            case 3://���ֵ���1
                BeforeFight();
                break;
            case 4://��һ��
                if (Refresh(gameIndex))
                {
                    StartBattle();       
                }
                CheckWin(1);
                break;
            case 5://��һ�ֽ���
                RoundOverWait();
                break;
            case 6://�ڶ����±�
                if (Refresh(gameIndex))
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        players[i].state = OnePlayer.PlayerState.set;
                        players[i].currency = 2500;
                    }
                    canvasAni.SetTrigger("EmptyToSet");
                    readyCanvasAni.SetTrigger("ShowReady");
                }
                if (CheckReady())
                {
                    canvasAni.SetTrigger("SetToFight");
                    setToFightTimer = 1;
                    gameIndex++;
                }            
                break;
            case 7://���ֵ���2
                BeforeFight();
                break;
            case 8://�ڶ��ֿ�ս
                if (Refresh(gameIndex))
                {
                    StartBattle();
                }
                CheckWin(2);
                break;
            case 9://�ڶ��ֽ���
                RoundOverWait();
                break;
            case 10://�������±�
                if (Refresh(gameIndex))
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        players[i].state = OnePlayer.PlayerState.set;
                        players[i].currency = 3000;
                    }
                    canvasAni.SetTrigger("EmptyToSet");
                    readyCanvasAni.SetTrigger("ShowReady");
                }
                if (CheckReady())
                {
                    canvasAni.SetTrigger("SetToFight");
                    setToFightTimer = 1;
                    gameIndex++;
                }
                break;
            case 11://���ֵ���3
                BeforeFight();
                break;
            case 12://������ս��
                if (Refresh(gameIndex))
                {
                    StartBattle();
                }
                CheckWin(3);
                break;
            case 13://�����ֽ���
                RoundOverWait();
                break;
            case 14://���ľ��±�
                if (Refresh(gameIndex))
                {
                    for (int i = 0; i < players.Length; i++)
                    {
                        players[i].state = OnePlayer.PlayerState.set;
                        players[i].currency = 3000;
                    }
                    canvasAni.SetTrigger("EmptyToSet");
                    readyCanvasAni.SetTrigger("ShowReady");
                }
                if (CheckReady())
                {
                    canvasAni.SetTrigger("SetToFight");
                    setToFightTimer = 1;
                    gameIndex++;
                }
                break;
            case 15://���ֵ���4
                BeforeFight();
                break;
            case 16://���ľ�ս��
                if (Refresh(gameIndex))
                {
                    StartBattle();
                }
                CheckWin(4);
                break;
            case 17://���ľֽ���
                finished = true;
                RoundOverWait();
                break;
            case 18://����
                if (Refresh(gameIndex))
                {
                    VolumeModifier.doBlur = true;

                    Text oppScore = GameObject.FindGameObjectWithTag("ResultOpponentScore").GetComponent<Text>();
                    Text myScore = GameObject.FindGameObjectWithTag("ResultPlayerScore").GetComponent<Text>();
                    MenuPotrait myPotrait = GameObject.FindGameObjectWithTag("ResultPlayerPotrait").GetComponent<MenuPotrait>();
                    MenuPotrait opponentPotrait = GameObject.FindGameObjectWithTag("ResultOpponentPotrait").GetComponent<MenuPotrait>();
                    for (int i = 0; i < playerScores.Length; i++)
                    {
                        if (players[i].myControl) myScore.text = "- " + playerScores[i].ToString() + " -";
                        else oppScore.text = "- " + playerScores[i].ToString() + " -";
                    }
                    if (MenuManager.haveStory)
                    {
                        PlotMachine plot = GameObject.FindObjectOfType<PlotMachine>();
                        for (int i = 0; i < plot.chara.Length; i++)
                        {
                            if (plot.chara[i].isPlayer)
                            {
                                myPotrait.id = plot.chara[i].id;
                                myPotrait.UpdatePotrait();
                            }
                            else
                            {
                                opponentPotrait.id = plot.chara[i].id;
                                opponentPotrait.UpdatePotrait();    
                            }
                        }
                    }

                    //˳��һ��������result timer
                    resultTimer = 6f;
                }
                int indexOfMe = 0;
                for (int i = 0; i < playerScores.Length; i++)
                {
                    if (players[i].myControl) indexOfMe = i;
                }
                for (int i = 0; i < players.Length; i++)
                {
                    if (!players[i].myControl)
                    {
                        if (playerScores[indexOfMe] > playerScores[i])
                        {
                            canvasAni.SetTrigger("ResultWin");

                            if (!winCountIncremented)
                            {
                                //�浵���¼ʤ������+1
                                int winCount = txt.getInt(0, 2);
                                winCount++;
                                txt.Write(0, 2, winCount.ToString(), "Save.txt", ';');
                                winCountIncremented = true;
                            }

                            if (!victoryPlayed)
                            {
                                GameObject.FindGameObjectWithTag("BattleBgm").GetComponent<BattleSceneBgm>().PlayVictory();
                                victoryPlayed = true;
                            }
                            
                        }
                        else if (playerScores[indexOfMe] < playerScores[i])
                        {
                            canvasAni.SetTrigger("ResultLose");

                            if (!victoryPlayed)
                            {
                                GameObject.FindGameObjectWithTag("BattleBgm").GetComponent<BattleSceneBgm>().PlayFailure();
                                victoryPlayed = true;
                            }
                        }
                        else
                        {
                            canvasAni.SetTrigger("Draw");

                            if (!victoryPlayed)
                            {
                                GameObject.FindGameObjectWithTag("BattleBgm").GetComponent<BattleSceneBgm>().PlayFailure();
                                victoryPlayed = true;
                            }

                        }
                        break;
                    }
                }

                //�ڱ��׶λ���resultTimer����ʱ�����ؿ�һ��
                if (resultTimer < 0 && CanNextRound())//Ҫô�о���������һ�֣�Ҫô���ھ���ģʽ
                {
                    print("set animator: new round");
                    canvasAni.SetTrigger("New_Round");
                    if (!waitForNextRoundFade)
                    {
                        GameObject.FindGameObjectWithTag("BattleBgm").GetComponent<BattleSceneBgm>().PlayClosure();            
                        waitForNextRoundFade = true;
                        resultTimer = 0;
                    }
                    //ĿǰNew_Round�����ֶ���ʱ��
                    if (resultTimer < -2f)
                    {
                        startedAgain = true;
                        RoundManager.startedAgain = true;
                        NewRound();//���¶�����
                    }
                }
                break;
        }
    }

    bool CanNextRound()
    {
        if (MenuManager.nextStoryCharacterId == null || MenuManager.nextStoryCharacterId == "" || MenuManager.nextStoryCharacterId == "random")
        {
            print("next round by not story mode <= 0");
            return true;
        }
        if (PlotMachine.lineNow <= 0)
        {
            print("next round by linenow <= 0");
            return true;
        }
        if (PlotMachine.nextRoundAvailable)
        {
            print("next round by nextroundavailable");
            return true;
        }
        print("next round not available");
        return false;
    }

    void SetReadyCanvas()
    {
        if (readyCanvasAni == null) return;

        bool allOpponentReady = true;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].ready == false && !players[i].myControl)
                allOpponentReady = false;
        }

        if (allOpponentReady)
        {
            readyCanvasAni.SetTrigger("OpponentReady");
        }
        else
        {
            readyCanvasAni.SetTrigger("OpponentNotReady");
        }
    }
    void RoundOverWait()
    {
        Camera.main.GetComponent<CameraFollow>().SetTarget(null);
        for (int i = 0; i < players.Length; i++)
        {
            players[i].state = OnePlayer.PlayerState.locked;
        }
        if (battleOverTimer < 0)
        {
            gameIndex++;
            ClearBattlefield();
            if (!finished)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    players[i].SpawnExistedUnits();
                    if (players[i].myControl)
                    {
                        players[i].state = OnePlayer.PlayerState.set;
                        cameraMove.SetNewDestination(
                            players[i].controllerCameraPos.position, players[i].controllerCameraPos.rotation.eulerAngles, 2);
                        players[i].inspectIndex = -1; //Ĭ�Ͽ��Լ�
                    }
                    //����ǰ���µı�
                }
            }
        }
        battleOverTimer -= Time.deltaTime;
        deckCanvas.gameObject.SetActive(false);
    }

    void BeforeFight()
    {
        if (!drumPlayed)
        {
            drumPlayed = true;
            GameObject.FindGameObjectWithTag("BattleBgm").GetComponent<BattleSceneBgm>().PlayDrum();
        }

        readyCanvasAni.SetTrigger("ReadyFade"); //׼��UI��ȥ

        for (int i = 0; i < unitsPresent.Count; i++)
        {
            if (unitsPresent[i] != null)
                unitsPresent[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < players.Length; i++)
        {          
            if (players[i].myControl)
            {
                players[i].state = OnePlayer.PlayerState.locked;
            }
        }
        deckCanvas.gameObject.SetActive(false);

        setToFightTimer -= Time.deltaTime;
        if (setToFightTimer < 0) gameIndex++;
    }

    bool CheckReady()
    {
        bool allReady = true;
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].ready == false) allReady = false;
        }
        return allReady;
    }

    void StartBattle()
    {
        drumPlayed = false;

        readyCanvasAni.ResetTrigger("ReadyFade"); //��ԭ���trigger��ֹbug
        setCanvas.gameObject.SetActive(false);
        deckCanvas.gameObject.SetActive(false);

        for (int i = 0; i < unitsPresent.Count; i++)
        {
            if (unitsPresent[i] == null) continue;
            if (unitsPresent[i].health == null) unitsPresent[i].Initialize();

            unitsPresent[i].inBattle = true;

            if (unitsPresent[i].unitTag != null) {
                unitsPresent[i].unitTag.Close();
            }

            if (unitsPresent[i].horse != null)
            {
                unitsPresent[i].horse.inBattle = true; //����������
            }

            unitsPresent[i].gameObject.SetActive(true);
            if (unitsPresent[i].horse != null)
            {
                unitsPresent[i].horse.gameObject.SetActive(true);
            }
            
            unitsPresent[i].health.neverAsTarget = false;
            unitsPresent[i].health.AddToTargetList();
            
        }

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].myControl)
            {
                players[i].state = OnePlayer.PlayerState.inspect;
            }
            else
            {
                players[i].state = OnePlayer.PlayerState.inspect;
            }
        }

        UpdateUnitList();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].GetExistedUnits(); //Ҫ��unitpresent�����ִ��
            players[i].ready = false;
        }
    }

    void UpdateUnitList()
    {
        for (int i = 0; i < unitsPresent.Count; i++)
        {
            if (unitsPresent[i] == null)
            {
                unitsPresent.RemoveAt(i);
                i--;

            }
        }
    }
    void CheckWin(int scoreToAdd = 1)
    {

        bool[] stillAlive = new bool[players.Length];
        UpdateUnitList();
        for (int i = 0; i < stillAlive.Length; i++) stillAlive[i] = false;
        for (int i = 0; i < unitsPresent.Count; i++)
        {
            if (unitsPresent[i] == null || unitsPresent[i].health == null) continue;
            stillAlive[unitsPresent[i].health.teamIndex] = true;
        }
        int lastAliveTeam = -1;
        bool ended = true;
        for (int i = 0; i < stillAlive.Length; i++)
        {
            if (stillAlive[i])
            {
                if (lastAliveTeam == -1)
                    lastAliveTeam = i;
                else
                    ended = false;
            }
        }

        if (ended)
        {
            

            gameIndex++;
            battleOverTimer = 3;
            winnerIndex = lastAliveTeam;
            playerScores[lastAliveTeam] += scoreToAdd;

            bool iWin = false;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i].myControl && players[i].teamIndex == winnerIndex)
                {
                    iWin = true;
                    canvasAni.SetTrigger("Win");
                    GameObject.FindGameObjectWithTag("BattleBgm").GetComponent<BattleSceneBgm>().PlayWin();
                }
            }
            if (!iWin)
            {
                canvasAni.SetTrigger("Lose");
                GameObject.FindGameObjectWithTag("BattleBgm").GetComponent<BattleSceneBgm>().PlayLose();
            }

            Text roundtext = GameObject.FindGameObjectWithTag("RoundScoreText").GetComponent<Text>();
            if (roundtext != null)
            {
                roundtext.text = "";

                int language = PlayerPrefs.GetInt("language");

                if (language == 0 || language == 1)
                {
                    //����
                    roundtext.text += "����";
                    roundtext.text += scoreToAdd.ToString();
                    roundtext.text += "С��  ";
                    if (iWin)
                    {
                        roundtext.text += "����+" + scoreToAdd.ToString() + "��";
                    }
                    else
                    {
                        roundtext.text += "����+" + scoreToAdd.ToString() + "��";
                    }
                }
                else
                {
                    //Ӣ��
                    roundtext.text += "[ROUND ";
                    roundtext.text += scoreToAdd.ToString();
                    roundtext.text += " ";
                    if (iWin)
                    {
                        roundtext.text += "YOUR SIDE + " + scoreToAdd.ToString() + " ]";
                    }
                    else
                    {
                        roundtext.text += "OPPONENT + " + scoreToAdd.ToString() + " ]";
                    }
                }

            }
        }
    }

    bool Refresh(int index)
    {
        if (lastIndex != gameIndex)
        {
            lastIndex = gameIndex;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DeckDone()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].myControl)
            {
                players[i].ready = true;
                
            }
        }
    }

    public void SetDone()
    {
        for (int i = 0; i < players.Length; i++)
        {
             if (players[i].myControl)
            {
                players[i].ready = !players[i].ready;
            }
        }
    }

    public void ClearBattlefield()
    {
        UpdateUnitList();

        RagdollScript[] bodies = FindObjectsOfType<RagdollScript>();
        ToolScript[] tools = FindObjectsOfType<ToolScript>();
        HumanAi[] ais = FindObjectsOfType<HumanAi>();

        for (int i = 0; i < bodies.Length; i++)
        {
            BodyClear b =  bodies[i].gameObject.AddComponent<BodyClear>();
            b.effect = objects.objects[0];
        }
        /*for (int i = 0; i < unitsPresent.Count; i++){
            unitsPresent[i].inBattle = false;
            BodyClear b = unitsPresent[i].gameObject.AddComponent<BodyClear>();
            b.effect = objects.objects[0];
        }*/
        
        for (int i = 0; i < tools.Length; i++)
        {
            BodyClear b = tools[i].gameObject.AddComponent<BodyClear>();
            b.effect = objects.objects[0];
        }
        for (int i = 0; i < ais.Length; i++)
        {
            ais[i].enabled = false;
            BodyClear b = ais[i].gameObject.AddComponent<BodyClear>();
            b.effect = objects.objects[0];
        }
        unitsPresent.Clear();
    }
}
