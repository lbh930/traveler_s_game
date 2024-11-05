using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(TxtReader))]



public class OnePlayer : MonoBehaviour
{
    [HideInInspector] public RoundManager round;
    public struct UnitInfo
    {
        public Vector3 position;
        public int id;
    }

    public List<UnitInfo> existedUnits = new List<UnitInfo>();
    public enum PlayerState {
        locked,
        deck,
        battle,
        inspect,
        set,
    }
    IdentityList list;

    public PlayerState state = PlayerState.deck;
    public int teamIndex;
    public bool isCpu = false;
    public bool myControl = true;
    public bool ready = false;

    public int currency = 2000;
    public int deckCount = 10;
    [HideInInspector] public int selectedCount = 0;
    UnitButton[] unitButtons;
    UnitButton[] unitArrangeButtons;
    UnitButton[] unitSandboxButtons;
    public Vector2 border = new Vector2(10, 5);
    public Transform controllerCameraPos;
    public Transform inspectorCameraPos;

    public float camMoveSpeed = 5.0f;
    public Vector2 camRestrictX = new Vector2(-16, 16);
    public Vector2 camRestrictZ = new Vector2(-11, 47);
    float decideToControlTimer = 2.0f;

    [HideInInspector] public int inspectIndex = -1;
    [HideInInspector] public bool unitinfocardUpdated = false;
    [HideInInspector] public int unitinfocardId = -1; //������ui����ʾ���Ͽ�
    [HideInInspector] public Sprite unitinfocardSprite;

    GameObject unitModel;
    FlashMoveTowards cameraMove;
    Animator canvasAni;
    CameraFollow cam;
    TxtReader txt;
    UnitPotraitGenerator potraitGenerator;
    TroopInfoCard infoCard;
    TxtReader unitInfoTxt; //���ڶ�ȡ��λ��Ϣ
    HealthScript pointingUnit;

    Transform lastOutlined; //�����ڲ���ѡ�к�ȡ�����

    float spawnRate = 25;
    float nextSpawnTime = 0;
    bool shouldSpawn = false;
    int spawnIndex = 0;



    [HideInInspector] public HumanAi controlling;
    [HideInInspector] public PossessionPointer pointer;

    public AudioSource cameraWooshAudio;
    public AudioSource selectAudio;
    public AudioSource deleteAudio;

    bool initialized = false;

    //AI�ñ���
    public GameObject[] troopList;
    public TroopPreset troop;

    public GameObject selectEffect;
    public GameObject unPossessEffect;
    public Material[] selectEffectMaterial;

    Material placementZoneMat;

    int lastRoundIndex = -1;//��������troopdone

    int troopIndex = 0;
    [HideInInspector] public bool troopDone = false;

    [Header("Cursor")]
    [HideInInspector] public RawImage cursor;
    [HideInInspector] public Text cursorText;
    public Texture cursorNormal;
    public Texture cursorSelection;
    bool selectingTroop = false;
    public BuffIndicatorManager buff;

    GameObject uiHealthOutline = null;
    RectTransform uiHealthBar = null;
    GameObject uiLeftClick = null;
    GameObject uiDetach = null;
    GameObject uiRightClick = null;
    GameObject uiLeftClickPossess = null;
    GameObject uiRightClickPossess = null;
    GameObject uiSpaceCheckMe = null;
    GameObject uiSpaceCheckOpponent = null;

    

    int ix = 0;//����λ��
    int iy = 0;

    //bool viewOpponent = false; //�ӽǲ鿴���˻����Լ�

    //public Text cursorText;
    public Text cursorReloadText;


    public void SetCursor()
    {
        if (PauseScript.paused) return;

        //���޿ɲٿص�OnePlayer
        if (!myControl) return;

        if (cursor == null) return;


        //������ڲٿر��֣����ӹ����
        if (controlling != null)
        {
            //buff indicator�Ķ���
            if (selectingTroop)
            {
                buff.PlayApplyAnimation();
                selectingTroop = false;
            }
            return;
        }

        cursorText.text = "";
        if (RoundManager.gameIndex == 4 || RoundManager.gameIndex == 8 || RoundManager.gameIndex == 12 || RoundManager.gameIndex == 16)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            cursor.texture = cursorSelection;
            cursor.rectTransform.position = new Vector2(Screen.width / 2, Screen.height / 2);
            cursor.rectTransform.sizeDelta = new Vector2(64, 64);
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            cursor.texture = cursorNormal;
            cursor.rectTransform.sizeDelta = new Vector2(60, 60);
            cursor.rectTransform.position = Input.mousePosition;
        }

        if (cursorText != null) cursorText.text = "";
        if (cursorReloadText != null) cursorReloadText.text = "";
    }

    private void Start()
    {
        if (!initialized) Initialize();
    }

    void GetHealthUi()
    {
        uiHealthOutline = GameObject.FindGameObjectWithTag("uiHealthOutline");
        GameObject g = GameObject.FindGameObjectWithTag("uiHealthBar");
        if (g != null)
        {
            uiHealthBar = g.GetComponent<RectTransform>();
            uiLeftClick = GameObject.FindGameObjectWithTag("uiLeftClick");
            uiDetach = GameObject.FindGameObjectWithTag("UiDetach");
            uiRightClick = GameObject.FindGameObjectWithTag("uiRightClick");
            uiRightClickPossess = GameObject.FindGameObjectWithTag("uiRightClickC");
            uiLeftClickPossess = GameObject.FindGameObjectWithTag("uiLeftClickC");
            uiSpaceCheckMe = GameObject.FindGameObjectWithTag("uiSpaceButtonCheckMe");
            uiSpaceCheckOpponent = GameObject.FindGameObjectWithTag("uiSpaceButtonCheckOpponent");
            uiHealthOutline.SetActive(false);
            uiHealthBar.gameObject.SetActive(false);
            uiLeftClick.SetActive(false);
            uiRightClick.SetActive(false);
            if (uiSpaceCheckMe != null)
                uiSpaceCheckMe.SetActive(false);
            if (uiSpaceCheckOpponent != null)
                uiSpaceCheckOpponent.SetActive(false);
        }
    }
    void Initialize()
    {
        initialized = true;

        lastRoundIndex = RoundManager.gameIndex;

        GetHealthUi();

        GetButtons();


        if (myControl)
        {
            infoCard = GameObject.FindGameObjectWithTag("TroopInfoCard").GetComponent<TroopInfoCard>();
            infoCard.gameObject.SetActive(false);

            buff = GameObject.FindGameObjectWithTag("BuffIndicator").GetComponent<BuffIndicatorManager>();

            if (buff != null)
            {
                //buff.gameObject.SetActive(false);
            }
        }
        else
        {
            //�����ȡһ������
            if (troop == null)
            {
                int i = 0;
                if (MenuManager.troopAccess != 1 && MenuManager.troopAccess != 2)
                {
                    i = Random.Range(0, troopList.Length);
                }
                else if (MenuManager.troopAccess == 1)
                {
                    i = 21;
                } else if (MenuManager.troopAccess == 2)
                {
                    i = 20;
                }

                troop = GameObject.Instantiate(troopList[i], transform.position, transform.rotation).GetComponent<TroopPreset>();
                troop.transform.SetParent(transform);
            }
        }

        round = GameObject.FindObjectOfType<RoundManager>();
        list = GameObject.FindObjectOfType<IdentityList>();
        potraitGenerator = GameObject.FindGameObjectWithTag("UnitPotraitGenerator").GetComponent<UnitPotraitGenerator>();
        pointer = Camera.main.GetComponent<PossessionPointer>();
        pointer.model.gameObject.SetActive(false);
        cameraMove = Camera.main.GetComponent<FlashMoveTowards>();
        unitInfoTxt = gameObject.AddComponent<TxtReader>();
        unitInfoTxt.Read(Application.streamingAssetsPath, "UnitInfo.txt");

        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<RawImage>();
        cursorText = GameObject.FindGameObjectWithTag("CursorText").GetComponent<Text>();

        //������ñ߽����Ч

        placementZoneMat = GetComponentInChildren<MeshRenderer>(true).material;
        placementZoneMat.SetVector("Vector2_6925FF1E", new Vector4(1, 16, 0, 0));
        if (teamIndex == 0)
        {
            placementZoneMat.SetColor("Color_19C6C157", Color.blue);
        }
        else
        {
            placementZoneMat.SetColor("Color_19C6C157", Color.red);
        }
        transform.GetChild(0).gameObject.SetActive(false);

        cam = Camera.main.GetComponent<CameraFollow>();
        txt = GetComponent<TxtReader>();
        txt.Read(Application.streamingAssetsPath, "UnitVs.csv", ';');
        canvasAni = GameObject.FindGameObjectWithTag("CanvasAnimator").GetComponent<Animator>();

        cursorText = GameObject.FindGameObjectWithTag("CursorText").GetComponent<Text>();
        cursorReloadText = GameObject.FindGameObjectWithTag("CursorReloadText").GetComponent<Text>();
    }

    void ShortCutKey()
    {
        if (!myControl) return;

        if (state == PlayerState.set || state == PlayerState.deck)
        {//׼����ݼ�
            if (Input.GetKeyDown(KeyCode.R))
            {
                ready = !ready;
            }
        }

        if (state == PlayerState.set)
        {//ѡ�б��ֿ�ݼ�
            int buttonid = -1;
            if (Input.GetKeyDown(KeyCode.Alpha1)) buttonid = 0;
            if (Input.GetKeyDown(KeyCode.Alpha2)) buttonid = 1;
            if (Input.GetKeyDown(KeyCode.Alpha3)) buttonid = 2;
            if (Input.GetKeyDown(KeyCode.Alpha4)) buttonid = 3;
            if (Input.GetKeyDown(KeyCode.Alpha5)) buttonid = 4;
            if (Input.GetKeyDown(KeyCode.Alpha6)) buttonid = 5;
            if (Input.GetKeyDown(KeyCode.Alpha7)) buttonid = 6;
            if (Input.GetKeyDown(KeyCode.Alpha8)) buttonid = 7;
            if (Input.GetKeyDown(KeyCode.Alpha9)) buttonid = 8;
            if (Input.GetKeyDown(KeyCode.Alpha0)) buttonid = 9;

            if (buttonid >= 0)
            {
                //��ȡ����һ��ѡ�е�
                for (int i = 0; i < unitArrangeButtons.Length; i++)
                {
                    if (unitArrangeButtons[i] != null)
                    {
                        unitArrangeButtons[i].selected = false;
                    }
                }

                //ѡ�а�����
                if (unitArrangeButtons[buttonid] != null)
                {
                    unitArrangeButtons[buttonid].Clicked();
                }
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        unitinfocardUpdated = false;
    }
    void Update()
    {
        if (!initialized) Initialize();

        SetCursor();

        //��ݼ�
        ShortCutKey();

        //����buff indicator ��active
        if (myControl && buff != null)
        {
            if (state == PlayerState.battle || state == PlayerState.inspect)
            {
                buff.gameObject.SetActive(true);
            }
            else
            {
                selectingTroop = false;//��λ
                buff.gameObject.SetActive(false);
            }
        }

        if (myControl)
        
        {
            if (controlling != null)
            {
                if (Input.GetKey(KeyCode.O) && Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.I) && Input.GetKey(KeyCode.M))
                {
                    controlling.health.maxHealth = 10000;
                    controlling.tool.rangedDamage = 500;
                    controlling.maxSpeed = 6;
                    controlling.health.health = 10000;
                    controlling.tool.fireRate = 8;
                    controlling.tool.roundNow = 1000;
                    controlling.tool.roundEachLoad = 1000;
                    controlling.tool.damage = 3000;
                    controlling.tool.knockForce = 4100;
                    controlling.tool.rangedKnockForce = 4100;
                    if (controlling.horse != null)
                    {
                        controlling.horse.health.maxHealth = 10000;
                        controlling.horse.health.health = 5000;
                        
                    }
                    controlling.tool.hurtCount = 10;
                }
            }

            float X = Input.mousePosition.x; //��ȡ���λ�ö�Ӧ����Ļ����
            float Y = Input.mousePosition.y;

            if (infoCard != null)
            {
                RectTransform rect = infoCard.GetComponent<RectTransform>();
                rect.position = new Vector3(X, Y, 0);
                if (unitinfocardId > -1) //�������Ͽ�
                {
                    infoCard.gameObject.SetActive(true);
                    infoCard.icon.sprite = unitinfocardSprite;
                    if (unitinfocardId != potraitGenerator.id) //����potrait
                    {
                        potraitGenerator.ClearStatue();
                        potraitGenerator.statue = Instantiate(list.unitList[unitinfocardId], potraitGenerator.transform);
                        potraitGenerator.statue.transform.position = potraitGenerator.transform.position;
                        potraitGenerator.MakeItStatic();//ȥ�������ai
                        potraitGenerator.id = unitinfocardId;
                    }

                    if (unitinfocardId <= unitInfoTxt.m_ArrayData.Count) //д�����ֵ�
                    {
                        int language = PlayerPrefs.GetInt("language");

                        if (language == 0 || language == 1)
                        {
                            //����
                            infoCard.name.text = unitInfoTxt.getString(unitinfocardId, 0);
                            infoCard.description.text = unitInfoTxt.getString(unitinfocardId, 1);
                            if (language == 1)
                            { //����
                                infoCard.name.transform.GetComponent<ChineseFontAutoChange>().Change();
                                infoCard.description.transform.GetComponent<ChineseFontAutoChange>().Change();
                            }
                        }
                        else
                        {
                            //Ӣ��
                            infoCard.name.text = unitInfoTxt.getString(unitinfocardId, 2);
                            infoCard.description.text = unitInfoTxt.getString(unitinfocardId, 3);
                        }
                    }
                }
                else
                {
                    infoCard.gameObject.SetActive(false);
                    potraitGenerator.ClearStatue();
                }
            }


            //�вٿر��ֵ�ʱ����ʾѪ��
            if (controlling != null)
            {
                if (uiHealthBar == null) GetHealthUi();

                if (uiRightClickPossess != null)
                {
                    uiRightClickPossess.SetActive(false);
                    uiLeftClickPossess.SetActive(false);
                }

                if (uiHealthOutline != null)
                {
                    uiHealthOutline.SetActive(true);
                    uiHealthBar.gameObject.SetActive(true);
                    uiLeftClick.SetActive(true);
                    uiDetach.SetActive(true);
                    if ((controlling.tool != null && controlling.tool.type != ToolScript.ToolType.Melee) || controlling.secondaryTool != null)
                    {
                        uiRightClick.SetActive(true);
                    }
                    else
                    {
                        uiRightClick.SetActive(false);
                    }

                    if (controlling.health == null) controlling.health = controlling.gameObject.GetComponent<HealthScript>();
                    if (controlling.health != null)
                    {
                        float resize = 1.0f + controlling.health.maxHealth / 1333;
                        uiHealthBar.localScale = new Vector3(Mathf.Clamp(controlling.health.health / controlling.health.maxHealth, 0, 1) * resize, 1, 1);
                        uiHealthOutline.GetComponent<RectTransform>().sizeDelta = new Vector2(300 * resize, 40);
                    }
                }

            }
            else
            {
                if (myControl && uiRightClickPossess != null)
                {
                    //print(state);
                    if (state == OnePlayer.PlayerState.inspect)
                    {
                        //print("!");
                        uiRightClickPossess.SetActive(true);
                        if (Input.GetMouseButton(1))
                        {
                            if (!selectingTroop)//buff indicatorֻ����һ��
                            {
            
                                buff.PlayShowAnimation();
                                selectingTroop = true;
                            }
                            uiLeftClickPossess.SetActive(true);
                        }
                        else
                        {
                            if (selectingTroop)
                            {
                                selectingTroop = false;
                                buff.PlayCloseAnimation();
                            }
                            uiLeftClickPossess.SetActive(false);
                        }
                    }
                    else
                    {
                        uiRightClickPossess.SetActive(false);
                        uiLeftClickPossess.SetActive(false);
                    }
                }

                if (uiHealthOutline != null)
                {
                    uiHealthOutline.SetActive(false);
                    uiHealthBar.gameObject.SetActive(false);
                    uiLeftClick.SetActive(false);
                    uiDetach.SetActive(false);
                    uiRightClick.SetActive(false);
                }

            }
        }
        else
        {
            //���ڵ��Խ�ɫ ��װ�и����ٿ���
            if (controlling == null)
            {
                decideToControlTimer -= Time.deltaTime;
                if (decideToControlTimer < 0)
                {
                    decideToControlTimer = Random.Range(1.0f, 4f);
                    for (int i = 0; i < round.unitsPresent.Count; i++)
                    {
                        if (round.unitsPresent[i] != null &&
                            round.unitsPresent[i].health != null
                            && round.unitsPresent[i].health.teamIndex == teamIndex)
                        {
                            if (round.unitsPresent[i].inBattle == false) break;//˵��������ս���׶�

                            if (controlling != null) 
                                Algori.SetLayerOfChildren(controlling.transform, "0");
                            controlling = round.unitsPresent[i];

                            //����ѡ��Ч��
                            GameObject g = Instantiate(selectEffect, controlling.transform.position, controlling.transform.rotation);
                            g.transform.SetParent(controlling.transform);
                            g.GetComponent<MeshRenderer>().material = selectEffectMaterial[teamIndex];

                            Algori.SetLayerOfChildren(round.unitsPresent[i].transform, "red");
                            break;
                        }
                    }
                }
            }
        }
        /*if (Input.GetKeyDown(KeyCode.H) && teamIndex == 0)
        {
            for (int i = 0; i < round.unitsPresent.Count; i++)
            {
                if (round.unitsPresent[i].GetComponent<HealthScript>().teamIndex == 0)
                {
                    print(round.unitsPresent[i].name);
                }
            }
            for (int i = 0; i < existedUnits.Count; i++)
            {
                print(existedUnits[i].id);
            }
        }*/
        //����ǰ������
        if (shouldSpawn && Time.time > nextSpawnTime && spawnIndex < existedUnits.Count)
        {
            nextSpawnTime = Time.time + 1 / spawnRate;
            HealthScript health = Instantiate(list.unitList[existedUnits[spawnIndex].id],
                existedUnits[spawnIndex].position, transform.rotation).GetComponent<HealthScript>();
            health.teamIndex = teamIndex;
            health.Initialize();
            if (health.ai.horse != null)
            {
                health.ai.horse.Initialize();
                health.ai.horse.health.Initialize();
                health.ai.horse.health.teamIndex = teamIndex;
            }

            health.GetComponent<HumanAi>().inBattle = false;
            HumanAi ai = health.GetComponent<HumanAi>();

            ai.canEdit = false; //ǰ���µĲ���ɾ��
            GetDominantUnit(ai).canEdit = false;
            ai.Initialize();
            ai.unitTag.Show();

            round.unitsPresent.Add(GetDominantUnit(ai)); //��ӵ������ֲ�������
            


            spawnIndex++;
            if (spawnIndex == existedUnits.Count)
            {
                spawnIndex = 0;
                shouldSpawn = false;
            }
        }

        //���÷��ñ߽����Ч
        if (placementZoneMat != null)
        {
            if (state == PlayerState.set)
            {
                transform.GetChild(0).gameObject.SetActive(true);
                placementZoneMat.SetVector("Vector2_6925FF1E",
                    new Vector4(1, Mathf.MoveTowards(placementZoneMat.GetVector("Vector2_6925FF1E").y,
                    16f, Time.deltaTime * 20), 0, 0));

            }
            else
            {
                placementZoneMat.SetVector("Vector2_6925FF1E",
                    new Vector4(1, Mathf.MoveTowards(placementZoneMat.GetVector("Vector2_6925FF1E").y,
                    0.4f, Time.deltaTime * 50), 0, 0));
                if (Mathf.Abs(placementZoneMat.GetVector("Vector2_6925FF1E").y - 0.4f) < 0.001f)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }


        if (myControl)
        {
            if (state == PlayerState.set)
            {
                Text moneyText = GameObject.FindGameObjectWithTag("MoneyIndication").GetComponent<Text>();
                moneyText.text = currency.ToString();

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                Vector3 posSave = Vector3.zero;
                if (unitModel != null)
                {
                    posSave = unitModel.transform.position; //���ƿ�unitmodel����ֹ���߼�⵽�Լ���
                    unitModel.transform.position = Vector3.down * 10000;
                }

                if (Physics.Raycast (ray, out hit, 100, ~(1<<10)) && 
                    hit.transform.gameObject.tag == "Ground")
                {
                    if(unitModel != null)
                    {
                        unitModel.transform.position = posSave;//���߼������ԷŻ�unitmodel
                    }

                    int index = 0;
                    bool haveSelected = false;
                    for (int i = 0; i < unitArrangeButtons.Length; i++)
                    {
                        if (unitArrangeButtons[i] != null && unitArrangeButtons[i].selected)
                        {
                            index = unitArrangeButtons[i].unitId;
                            haveSelected = true;
                            break;
                        }
                    }

                    Vector2 mySize = Vector2.one * 0.5f;
                    HumanAi mySizeAi = list.unitList[index].GetComponent<HumanAi>();
                    if (mySizeAi != null)
                    {
                        mySize = mySizeAi.size;
                    }
                        

                    if (haveSelected && CheckPlacable (hit.point, mySize.x, mySize.y))
                    {
                        if (pointingUnit != null)
                        {
                            pointingUnit.ai.unitTag.PointerExit();//ԭ��ָ�ŵ�ȡ����
                            pointingUnit = null;
                        }

                        if (unitModel == null || list.unitList[index].name != unitModel.name)
                        {
                            if (unitModel != null) Destroy(unitModel);
                            unitModel = Instantiate(list.unitList[index], hit.point,
                                Quaternion.Euler(Vector3.zero));
                            HealthScript h = unitModel.GetComponent<HealthScript>();
                            h.Initialize();
                            h.SetPhysicsActive(false);

                            HumanAi ai = h.GetComponent<HumanAi>();
                            if (ai != null && ai.rider != null)
                            {
                                ai.rider.GetComponent<HealthScript>().SetPhysicsActive(false); //����Ҳ�ص���ײ
                            }

                            h.neverAsTarget = true;
                            h.teamIndex = teamIndex;
                            h.GetComponent<HumanAi>().inBattle = false;

                            unitModel.name = list.unitList[index].name;
                        }
                        unitModel.transform.position = hit.point;
                        unitModel.transform.rotation = transform.rotation;

                        //�������
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (currency >= list.unitList[index].GetComponent<HealthScript>().cost)
                            {
                                HealthScript h = unitModel.GetComponent<HealthScript>();

                                h.SetPhysicsActive(true);
                                HumanAi ai = h.GetComponent<HumanAi>();

                                if (ai.unitTag != null)
                                {
                                    ai.unitTag.Show();//����tag
                                    ai.unitTag.PointerEnter();
                                }

                                if (pointingUnit != null)//ǿ�ư�pointingUnit����Ϊ�շ��µ�
                                {
                                    pointingUnit.ai.unitTag.PointerExit();//ԭ��ָ�ŵ�ȡ����                                    
                                }
                                pointingUnit = h; //ǿ��ָ�½���

                                if (ai != null && ai.rider != null)
                                {
                                    ai.rider.GetComponent<HealthScript>().SetPhysicsActive(true); //����Ҳ������ײ
                                }

                                round.unitsPresent.Add(GetDominantUnit(ai));
                                unitModel = null;

                                currency -= (int)list.unitList[index].GetComponent<HealthScript>().cost;

                                //���Ŵ�����Ч
                                PlayDeleteSound pds = ai.transform.GetComponent<PlayDeleteSound>();
                                if (pds == null && ai.rider != null) pds = ai.rider.transform.GetComponent<PlayDeleteSound>();
                                if (pds != null) pds.Place();
                            }
                            else
                            {
                                NoticePoper.PopNotice(6);
                            }
                        }
                    }
                    else
                    {
                        if (unitModel != null) Destroy(unitModel);                
                    }
                }
                else
                {
                    //print(hit.transform.name);
                    if (unitModel != null) Destroy(unitModel);
                    if (hit.transform != null)
                    {
                        HealthScript h = hit.transform.GetComponent<HealthScript>();
                        
                        if (h != null && h.teamIndex == teamIndex)
                        {
                            HumanAi ai = h.GetComponent<HumanAi>(); //��ȥɾ������

                            if (h != pointingUnit && ai.unitTag != null) //�޸�tag
                            {
                                if (pointingUnit != null)
                                    pointingUnit.ai.unitTag.PointerExit();//ԭ��ָ�ŵ�ȡ����
                                pointingUnit = h;
                                ai.unitTag.PointerEnter();
                            }

                            if (Input.GetMouseButtonDown(1))
                            {
                                if (ai.canEdit && (ai.rider ==null || ai.rider.canEdit))
                                {
                                    if (deleteAudio != null)
                                    {
                                        print("play delete audio");
                                        deleteAudio.Play();//����ɾ����Ч
                                    }

                                    currency += (int)h.cost;
                                    for (int i = 0; i < round.unitsPresent.Count; i++)
                                    {
                                        if (round.unitsPresent[i] == null) continue;
                                        if (round.unitsPresent[i].gameObject == hit.transform.gameObject)
                                        {
                                            round.unitsPresent.RemoveAt(i);
                                        }
                                    }
                                    if (ai != null && ai.horse != null)
                                        Destroy(ai.horse.gameObject);
                                    else
                                        Destroy(hit.transform.gameObject);
                                }

                                
                            }
                        }
                        else
                        {
                            if (pointingUnit != null) {
                                pointingUnit.ai.unitTag.PointerExit();//ԭ��ָ�ŵ�ȡ����
                                pointingUnit = null;
                            }
                        }
                    }
                }

                if (Input.GetKeyDown(KeyCode.Space) && RoundManager.gameIndex >= 2)
                {
                    if (inspectIndex + 1 >= round.players.Length)
                    {
                        inspectIndex = -1;
                    }
                    else
                    {
                        bool a = false;
                        for (int i = inspectIndex+1; i < round.players.Length; i++)
                        {
                            if (round.players[i].teamIndex != teamIndex)
                            {
                                inspectIndex = i;
                                a = true;
                                break;
                            }
                        }
                        if (!a) inspectIndex = -1;
                    }

                    if (inspectIndex < 0)
                    {
                        cameraMove.SetNewDestination(controllerCameraPos, 0.5f);
                        //viewOpponent = false;
                    }
                    else
                    {
                        cameraMove.SetNewDestination(round.players[inspectIndex].inspectorCameraPos, 0.5f);
                        //viewOpponent = true;
                    }

                    //�������ӽ���Ч
                    if (cameraWooshAudio != null)
                    {
                        cameraWooshAudio.Play();
                    }
                }
            }
            else
            {
                if (unitModel != null) Destroy(unitModel);
            }



            if (state == PlayerState.inspect)
            {
                if (myControl)
                {
                    float minDis = float.PositiveInfinity;
                    int closestIndex = -1;
                    for (int i = 0; i < round.unitsPresent.Count; i++)
                    {
                        HumanAi ai = round.unitsPresent[i];
                        if (ai.rider != null) ai = ai.rider; //ѡ�����ֶ���������

                        if (ai.health.teamIndex == teamIndex)
                        {
                            if (ai == null) continue;
                            if (ai.canControl == false) continue;

                            Vector3 tarPos = ai.transform.position + Vector3.up * 0.5f;
                            float disDirect = Vector3.Distance(tarPos,
                                Camera.main.transform.position);
                            float disOnFocus = Vector3.Dot(tarPos - Camera.main.transform.position, Camera.main.transform.forward);
                            float dis = Mathf.Sqrt(disDirect * disDirect - disOnFocus * disOnFocus);
                            //�����õľ���dis�������������ƽ���ߵľ���

                            if (dis < minDis)
                            {
                                minDis = dis;
                                closestIndex = i;
                            }
                        }
                    }

                    if (closestIndex != -1)
                    {
                        HumanAi ai = round.unitsPresent[closestIndex];
                        if (ai.rider != null) ai = ai.rider;

                        //����select��Ч
                        if (Input.GetMouseButtonDown(1))
                        {
                            //print("play select audio");
                            if (selectAudio != null)
                            {
                                selectAudio.Play();
                            }
                        }


                        if (Input.GetMouseButton(1))
                        {
                            pointer.model.gameObject.SetActive(true);
                            pointer.root.position = pointer.rootPos.position;
                            pointer.root.forward =
                                ai.transform.position - pointer.rootPos.position;
                            pointer.tip.transform.position = ai.transform.position;

                            if (ai != lastOutlined )//˵��ѡ������һ��
                            {
                                if (lastOutlined != null)
                                    Algori.SetLayerOfChildren(lastOutlined, "0");
                                Algori.SetLayerOfChildren(ai.transform, "white"); //����
                                lastOutlined = ai.transform; //��¼�����ߵ�
                            }
                            if (Input.GetMouseButtonDown(0))
                            {
                                Algori.SetLayerOfChildren(ai.transform, "blue");
                                pointer.model.gameObject.SetActive(false);

                                //����ѡ��Ч��
                                GameObject g = Instantiate(selectEffect, ai.transform.position, ai.transform.rotation);
                                g.transform.SetParent(ai.transform);
                                g.GetComponent<MeshRenderer>().material = selectEffectMaterial[teamIndex];

                                PossessUnit(ai);
                                //������Ч
                                PlayDeleteSound pds = ai.transform.GetComponent<PlayDeleteSound>();
                                if (pds == null && ai.rider != null) pds = ai.rider.transform.GetComponent<PlayDeleteSound>();
                                if (pds != null) pds.Control();
                            }

                           
                        }
                        else
                        {
                            if (lastOutlined != null)
                                Algori.SetLayerOfChildren(lastOutlined, "0");
                            pointer.model.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        pointer.model.gameObject.SetActive(false);
                    }

                    //������ƶ�
                    Vector3 camForward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
                    Vector3 camRight = Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up);
                    float x = Input.GetAxis("Mouse X");
                    float y = Input.GetAxis("Mouse Y");
                    Vector3 dir = camRight.normalized * x + camForward.normalized * y;
                    //print(camForward);
                    if (dir.magnitude > 1) dir = dir.normalized;
                    Camera.main.transform.Translate(dir * Time.deltaTime * camMoveSpeed,Space.World);
                    float camMoveX = 0;
                    float camMoveY = 0;
                    if (Input.GetKey(KeyCode.D)) camMoveX += 6;
                    if (Input.GetKey(KeyCode.A)) camMoveX += -6;
                    if (Input.GetKey(KeyCode.W)) camMoveY += 6;
                    if (Input.GetKey(KeyCode.S)) camMoveY += -6;
                    Camera.main.transform.Translate(new Vector3(camMoveX, 0, camMoveY)*Time.deltaTime, Space.World);

                    Camera.main.transform.position = new Vector3(
                        Mathf.Clamp(Camera.main.transform.position.x, camRestrictX.x, camRestrictX.y), Camera.main.transform.position.y,
                        Mathf.Clamp(Camera.main.transform.position.z, camRestrictZ.x, camRestrictZ.y));
                }
                else
                {
                    
                }
            }

            if (state == PlayerState.battle)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    Algori.SetLayerOfChildren(controlling.transform, "0");

                    UnPossessUnit();

                    
                }
                if (controlling == null)
                {
                    state = PlayerState.inspect;
                }
            }
        }
        else if (isCpu)//AI
        {
            if (state == PlayerState.set && !troopDone)
            {
                AiTroop();

                troopIndex++;
                troopDone = true;
            }

            if (lastRoundIndex != RoundManager.gameIndex)
            {
                troopDone = false;
                lastRoundIndex = RoundManager.gameIndex;
            }
        }

        //��ʾ�ո񰴼�
        if (myControl)
        {
            if (uiSpaceCheckMe != null)
            if (state == PlayerState.set && RoundManager.gameIndex >= 2)
            {
                if (inspectIndex != -1)
                {
                        if (uiSpaceCheckMe != null) uiSpaceCheckMe.SetActive(true);
                    if (uiSpaceCheckOpponent != null)uiSpaceCheckOpponent.SetActive(false);
                }
                else
                {
                        if (uiSpaceCheckMe != null) uiSpaceCheckMe.SetActive(false);
                        if (uiSpaceCheckOpponent != null) uiSpaceCheckOpponent.SetActive(true);
                }
            }
            else
            {

                    if (uiSpaceCheckMe != null) uiSpaceCheckMe.SetActive(false);
                    if (uiSpaceCheckOpponent != null) uiSpaceCheckOpponent.SetActive(false);
            }
        }
    }

    public void SpawnExistedUnits()
    {
        shouldSpawn = true;
    }
    public void GetExistedUnits()
    {
        existedUnits.Clear();
        for (int i =0; i < round.unitsPresent.Count; i++)
        {

            bool sameTeam = false;
            sameTeam = round.unitsPresent[i].health != null ? round.unitsPresent[i].health.teamIndex
                == teamIndex : round.unitsPresent[i].GetComponent<HealthScript>().teamIndex == teamIndex;

            if (sameTeam)
            {
                HumanAi ai = GetDominantUnit(round.unitsPresent[i]);
                UnitInfo u = new UnitInfo();
                u.id = ai.GetComponent<IdentityScript>().id;
                u.position = ai.transform.position;
                existedUnits.Add(u);
            }
        }
    }

    HumanAi GetDominantUnit(HumanAi ai) //������������ȡ������
    {
        if (ai.rider != null) return ai.rider;
        return ai;
    }

    void PlaceUnit (Vector3 point, GameObject unit) //��point�±�
    {
        HealthScript health =
                      Instantiate(unit, point,
                          transform.rotation).GetComponent<HealthScript>();
        health.teamIndex = teamIndex;
        health.gameObject.SetActive(false);
        health.GetComponent<HumanAi>().inBattle = false;

        HumanAi ai = health.GetComponent<HumanAi>(); //�������

        round.unitsPresent.Add(GetDominantUnit(ai));
        //health.transform.SetParent(troop.transform.GetChild(troopIndex));
        currency -= health.cost;
    }
    void AiTroop()
    {
        if (troopIndex < troop.waves.Count)
        {
            for (int i = 0; i < troop.waves[troopIndex].Length; i++)
            {
                int id = troop.waves[troopIndex][i].GetComponent<IdentityScript>().id;
                float p = troopIndex > 0 ? GetUnitWinRate(id) : 1;

                if (Random.Range(0.0f, 1.0f) > p)
                {
                    Destroy(troop.waves[troopIndex][i].gameObject);
                }
                else
                {
                    troop.waves[troopIndex][i].GetComponent<HumanAi>().inBattle = false;

                    HumanAi ai = troop.waves[troopIndex][i].GetComponent<HumanAi>(); //�������

                    round.unitsPresent.Add(GetDominantUnit(ai));
                    troop.waves[troopIndex][i].gameObject.SetActive(false);
                    currency -= troop.waves[troopIndex][i].cost;
                }
            }
        }

        float[] rate = new float[troop.deck.Length];
        float totRate = 0;

        for (int i = 0; i < troop.deck.Length; i++)
        {
            rate[i] = GetUnitWinRate(troop.deck[i]);
            rate[i] = Mathf.Clamp(rate[i], 0.02f, 1);
            totRate += rate[i];
        }
        for (int i = 0; i < troop.deck.Length; i++)
        {
            rate[i] /= totRate;
        }

        int trial = 0;

        while (StillHaveMoney() && trial < 10000)
        {
            trial++;
            int index = Algori.SeedWeightedRandom(rate, 0);
            if (list.unitList[troop.deck[index]].GetComponent<HealthScript>().cost
                > currency) continue;

            bool spawnedThisRound = false;

            //������ã��ҷŻ��߹��ɷ�
            if (Random.Range(0.0f, 1.0f) < 0.7f)
            {//���ɷ���
                while (iy < border.y)
                {
                    while (ix < border.x)
                    {
                        Vector3 ori = transform.position - transform.right * border.x / 2
                            + transform.forward * border.y / 2 + transform.right - transform.forward;


                        Vector3 point = ori + transform.right * ix - transform.forward * iy;
                        point += Random.Range(-0.15f, 0.15f) * transform.right + Random.Range(-0.15f, 0.15f) * transform.forward; //add some randomness
                        if (PlaceVacant(point))
                        {
                            PlaceUnit(point, list.unitList[troop.deck[index]]);

                            spawnedThisRound = true;
                        }
                        ix++;
                        if (spawnedThisRound) break;
                    }
                    if (spawnedThisRound)
                    {
                        if (ix >= border.x)
                        {
                            ix = 0;
                            iy++;
                        }
                        break;
                    }
                    iy++;
                }
            }
            else
            {
                //���ҷ���
                //print("try random");
                for (int i = 0; i < 100; i++) //ÿ��unit��ೢ��100��
                {
                    Vector3 ori = transform.position - transform.right * border.x / 2
                          + transform.forward * border.y / 2;


                    Vector3 point = ori + transform.right * border.x * Random.Range(0.05f, 0.95f) - transform.forward*border.y*Random.Range(0.05f, 0.7f);
                    if (PlaceVacant(point))
                    {
                        //print("random success");
                        PlaceUnit(point, list.unitList[troop.deck[index]]);

                        break;
                    }
                }
            }
        }

        if (trial > 9999)
        {
            print("failed to place unit because no space!");
        }
    }
    bool PlaceVacant(Vector3 point)
    {
        RaycastHit hit;
        if (Physics.Raycast(point + Vector3.up*4, Vector3.down, out hit, 6f))
        {
            if (hit.transform.gameObject.tag == "Ground")
            {
                bool noCollapse = true;
                for (int i = 0; i < round.unitsPresent.Count; i++)
                {
                    HumanAi tarAi = round.unitsPresent[i];
                    if (tarAi == null) continue; //��ֹ����null
                    if (tarAi.horse != null) tarAi = tarAi.horse;
                    if (Mathf.Abs(hit.transform.position.x - tarAi.transform.position.x) < tarAi.size.x/2 &&
                        Mathf.Abs(hit.transform.position.z - tarAi.transform.position.z) < tarAi.size.y / 2)
                    {
                        noCollapse = false;
                        break;
                    }
                }
                if (noCollapse) return true;
            }
        }
        return false;
    }

    bool StillHaveMoney()
    {
        bool stillhave = false;
        for (int i = 0; i < troop.deck.Length; i++)
        {
            if (list.unitList[troop.deck[i]].GetComponent<HealthScript>().cost <= currency)
            {
                stillhave = true;
                break;
            }
        }
        return stillhave;
    }
    float GetUnitWinRate(int id, int k = 1) //k is laplace smoother
    {
        //float totCost = 0;
        float sum = 0;
        float c = 0;

        for (int j = 0; j < round.players.Length; j++)
        {
            if (round.players[j].teamIndex == teamIndex) continue;

            for (int i = 0; i < round.players[j].existedUnits.Count; i++)
            {
                HealthScript health = 
                    list.unitList[round.players[j].existedUnits[i].id].GetComponent<HealthScript>();
                int id2 = 
                    list.unitList[round.players[j].existedUnits[i].id].GetComponent<IdentityScript>().id;
                if (health.teamIndex == teamIndex) continue;

                //totCost += health.cost;
                //print("!");
                //print(txt.getString(id, id2));
                sum += Mathf.Log(((float.Parse(txt.getString(id, id2).Split('|')[0])) * 
                    (float.Parse(txt.getString(id, id2).Split('|')[1])) + k)/ (float.Parse(txt.getString(id, id2).Split('|')[1])));
                    //* (float)health.cost;
                c += 1.0f;
            }        
        }
        //return sum / (/*totCost **/ c);
        return sum;
    }

    bool CheckPlacable(Vector3 point, float mySizeX = 0.5f, float mySizeY = 0.5f)
    {
        //print("checking");
        //print(point);
        if (Mathf.Abs(point.x - transform.position.x) < border.x/2 &&
            Mathf.Abs(point.z - transform.position.z) < border.y/2)
        {
            bool noCollapse = true;
            for (int i = 0; i < round.unitsPresent.Count; i++)
            {
                HumanAi tarAi = round.unitsPresent[i];
                if (tarAi == null) continue; //��ֹ����null
                if (tarAi.horse != null) tarAi = tarAi.horse;
                if (Mathf.Abs(point.x - tarAi.transform.position.x) < (tarAi.size.x + mySizeX) / 2 &&
                    Mathf.Abs(point.z - tarAi.transform.position.z) < (tarAi.size.y + mySizeY) / 2)
                {
                    //print(tarAi.name);
                    noCollapse = false;
                    break;
                }
            }
            //print(noCollapse);
            return noCollapse;
        }
        else
        {
            return false;
        }
    }

    public void PossessUnit (HumanAi target)
    {
        target.inControl = true;
        controlling = target;
        buff.ApplyBuff(target);
        state = PlayerState.battle;
        cam.relativePos = controllerCameraPos.position - transform.position;
        cam.rotation = controllerCameraPos.rotation.eulerAngles;
        cam.SetTarget(target.transform);
    }

    public void UnPossessUnit()
    {
        if (controlling == null) return;

        //������Ч

            PlayDeleteSound pds = controlling.transform.GetComponent<PlayDeleteSound>();
            if (pds == null && controlling.rider != null) pds = controlling.rider.transform.GetComponent<PlayDeleteSound>();
            if (pds != null) pds.Control();

            print("Unpossessing: " + controlling.name);
        buff.RemoveBuff(controlling);
        controlling.inControl = false;
        if (controlling.horse != null) controlling.horse.inControl = false;
        if (controlling.rider != null) controlling.rider.inControl = false;
        state = PlayerState.inspect;
        controlling = null;
        cam.SetTarget(null);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(border.x, 0.1f, border.y));
    }

    public void GetButtons()
    {
        GameObject deckCanvas = GameObject.FindGameObjectWithTag("DeckCanvas");
        GameObject setCanvas = GameObject.FindGameObjectWithTag("SetCanvas");

        if (deckCanvas != null)
        {
            unitButtons = deckCanvas.GetComponentsInChildren<UnitButton>(true);
            for (int i = 0; i < unitButtons.Length; i++)
            {
                unitButtons[i].player = this;
                unitButtons[i].isDeckButton = true;
            }
        }

        if (setCanvas != null)
        {
            unitArrangeButtons = setCanvas.GetComponentsInChildren<UnitButton>(true);
            for (int i = 0; i < unitArrangeButtons.Length; i++)
            {
                unitArrangeButtons[i].player = this;
                unitArrangeButtons[i].isDeckButton = false;
            }
        }

        GameObject sandboxCanvas = GameObject.FindGameObjectWithTag("SandboxCanvas");

        if (sandboxCanvas != null)
        {
            unitSandboxButtons = sandboxCanvas.GetComponentsInChildren<UnitButton>(true);
            for (int i = 0; i < unitSandboxButtons.Length; i++)
            {
                unitSandboxButtons[i].player = this;
                unitSandboxButtons[i].isDeckButton = false;
            }
        }
    }
    public int GetSelectedCount()
    {
        if (unitButtons == null || unitButtons.Length <= 0) GetButtons();
        selectedCount = 0;
        for (int i = 0; i < unitButtons.Length; i++)
        {
            if (unitButtons[i].selected) selectedCount += 1;
        }
        return selectedCount;
    }

    public void SetDeckButtons()
    {
        for (int i = 0; i< unitButtons.Length; i++)
        {
            if (unitButtons[i].unitId >= list.unitList.Length) return;
            HealthScript h = 
                list.unitList[unitButtons[i].unitId].GetComponent<HealthScript>();
            unitButtons[i].costText.text = h.cost.ToString();
            unitButtons[i].cost = (int)h.cost;
        }
    }

    public void SetArrangeButtons()
    {

        int indexNow = 0;

        if (GetSelectedCount() == 0)
        {
            NoticePoper.PopNotice(3);
            for (int i = 0; i < 10; i++)
            {
                int r = Random.Range(0, unitButtons.Length);
                if (unitButtons[r].gameObject.active && unitButtons[r].enabled)
                {
                    UnitLock unitLock = unitButtons[r].GetComponent<UnitLock>();
                    if (unitLock != null && unitLock.locked) continue; //���˵Ĳ���
                    unitButtons[r].selected = true;
                }
            }
        }

        for (int i = 0; i < unitButtons.Length; i++)
        {
            if (unitButtons[i].selected)
            {
                unitArrangeButtons[indexNow].cost = unitButtons[i].cost;
                unitArrangeButtons[indexNow].player = unitButtons[i].player;
                unitArrangeButtons[indexNow].unitId = unitButtons[i].unitId;
                unitArrangeButtons[indexNow].GetComponent<Image>().sprite =
                    unitButtons[i].GetComponent<Image>().sprite;
                unitArrangeButtons[indexNow].costText.text = unitButtons[i].cost.ToString();
                indexNow++;
            }
        }
        for (; indexNow < unitArrangeButtons.Length; indexNow++)
        {
            unitArrangeButtons[indexNow].gameObject.SetActive(false);
        }
    }

    public void SelectedToSet (UnitButton unit)
    {
        if (unitArrangeButtons != null)
        {
            if (unitArrangeButtons.Length > 0)
            {
                for (int i = 0; i < unitArrangeButtons.Length; i++)
                {
                    if (unitArrangeButtons[i] == unit)
                    {
                        unitArrangeButtons[i].selected = true;
                    }
                    else
                    {
                        unitArrangeButtons[i].selected = false;
                    }
                }
            }
        }

        if (unitSandboxButtons != null)
        {
            if (unitSandboxButtons.Length > 0)
            {
                for (int i = 0; i < unitSandboxButtons.Length; i++)
                {
                    if (unitSandboxButtons[i] == unit)
                    {
                        unitSandboxButtons[i].selected = true;
                    }
                    else
                    {
                        unitSandboxButtons[i].selected = false;
                    }
                }
            }
        }
    }
}
