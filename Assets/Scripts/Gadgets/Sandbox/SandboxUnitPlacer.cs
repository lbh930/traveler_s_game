using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.EventSystems;

public class SandboxUnitPlacer : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject unitModel;
    int index;

    GameObject sandboxCanvas;

    UnitButton[] unitSandboxButtons;

    IdentityList list;
    bool initialized = false;

    HealthScript pointingUnit;

    SandboxManager sandbox;

    public AudioSource selectAudio;
    public AudioSource deleteAudio;

    public RectTransform mouseHint;

    public float unitRotation = 0;

    int lastTeamIndex = 0;
    void Start()
    {
        Initialize();

    }

    void Initialize()
    {
        if (initialized) return;

        GameObject sandboxCanvas = GameObject.FindGameObjectWithTag("SandboxCanvas");

        if (sandboxCanvas != null)
        {
            unitSandboxButtons = sandboxCanvas.GetComponentsInChildren<UnitButton>(true);
        }

        list = GameObject.FindObjectOfType<IdentityList>();

        sandbox = FindObjectOfType<SandboxManager>(true);

        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();

        

        mouseHint.position = new Vector3(-10000, 0, 0); //默认不显示
        // Check if mouse is over any UI element

        if (MenuManager.nextStoryCharacterId != "sandbox") return;

        bool isMouseOverUI = EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButtonDown(2)) //取消
        {
            for (int i = 0; i < unitSandboxButtons.Length; i++)
            {
                unitSandboxButtons[i].selected = false;
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            unitRotation += Time.unscaledDeltaTime * 300;
        }

        if (!isMouseOverUI)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Vector3 posSave = Vector3.zero;
            if (unitModel != null)
            {
                posSave = unitModel.transform.position; //先移开unitmodel，防止射线检测到自己。
                unitModel.transform.position = Vector3.down * 10000;
            }

            if (Physics.Raycast(ray, out hit, 100, ~(1 << 10)) &&
                hit.transform.gameObject.tag == "Ground")
            {


                if (unitModel != null)
                {
                    unitModel.transform.position = posSave;//射线检测完可以放回unitmodel
                }

                int index = 0;
                bool haveSelected = false;
                for (int i = 0; i < unitSandboxButtons.Length; i++)
                {
                    if (unitSandboxButtons[i] != null && unitSandboxButtons[i].selected)
                    {
                        index = unitSandboxButtons[i].unitId;
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


                if (haveSelected && CheckPlacable(hit.point, mySize.x, mySize.y))
                {
                    if (pointingUnit != null)
                    {
                        pointingUnit.ai.unitTag.PointerExit();//原来指着的取消掉
                        pointingUnit = null;
                    }

                    //鼠标hint
                    RectTransform cursorRect = GameObject.FindGameObjectWithTag("Cursor").GetComponent<RectTransform>();
                    if (cursorRect != null)
                    {
                        if (!isMouseOverUI)
                            mouseHint.position = cursorRect.position;
                        else
                            mouseHint.position = new Vector3(-10000, 0, 0);
                    }

                    if (unitModel == null || list.unitList[index].name != unitModel.name)
                    {
                        if (unitModel != null) Destroy(unitModel);
                        unitModel = Instantiate(list.unitList[index], hit.point,
                            Quaternion.Euler(new Vector3(0, unitRotation, 0)));
                        HealthScript h = unitModel.GetComponent<HealthScript>();
                        h.Initialize();
                        h.SetPhysicsActive(false);

                        HumanAi ai = h.GetComponent<HumanAi>();
                        if (ai != null && ai.rider != null)
                        {
                            ai.rider.GetComponent<HealthScript>().SetPhysicsActive(false); //骑手也关掉碰撞
                        }

                        h.neverAsTarget = true;
                        if (ai.rider != null)
                        {
                            ai.rider.GetComponent<HealthScript>().neverAsTarget = true;
                        }
                        h.teamIndex = lastTeamIndex;
                        h.GetComponent<HumanAi>().inBattle = false;

                        unitModel.name = list.unitList[index].name;
                    }
                    unitModel.transform.position = hit.point;
                    unitModel.transform.rotation = Quaternion.Euler(new Vector3(0, unitRotation, 0));

                    //左键teamindex = 0, 右键teamindex = 1
                    bool pressedMouse = false;
                    int chosenTeamIndex = 0;
                    if (Input.GetMouseButtonDown(0))
                    {
                        pressedMouse = true;
                        chosenTeamIndex = 0;
                        lastTeamIndex = 0;
                    }
                    if (Input.GetMouseButtonDown(1))
                    {
                        pressedMouse = true;
                        chosenTeamIndex = 1;
                        lastTeamIndex = 1;
                    }
                    if (pressedMouse)
                    {
                        //if (currency >= list.unitList[index].GetComponent<HealthScript>().cost)
                        //{
                        HealthScript h = unitModel.GetComponent<HealthScript>();

                        if (h != null)
                        {
                            h.teamIndex = chosenTeamIndex;
                        }

                        h.SetPhysicsActive(true);
                        HumanAi ai = h.GetComponent<HumanAi>();

                        if (ai.unitTag != null)
                        {
                            ai.unitTag.Show();//启用tag
                            ai.unitTag.PointerEnter();
                        }

                        if (pointingUnit != null)//强制把pointingUnit设置为刚放下的
                        {
                            pointingUnit.ai.unitTag.PointerExit();//原来指着的取消掉                                    
                        }
                        pointingUnit = h; //强制指新建的

                        
                        if (ai != null && ai.rider != null)
                        {
                            HealthScript riderHealth;
                            riderHealth = ai.rider.GetComponent<HealthScript>();
                            riderHealth.SetPhysicsActive(true); //骑手也开启碰撞
                            riderHealth.teamIndex = chosenTeamIndex;
                        }

                        sandbox.Add(Algori.GetDominantUnit(ai), index);
                        unitModel = null;

                        //currency -= (int)list.unitList[index].GetComponent<HealthScript>().cost;

                        //播放创建音效
                        PlayDeleteSound pds = ai.transform.GetComponent<PlayDeleteSound>();
                        if (pds == null && ai.rider != null) pds = ai.rider.transform.GetComponent<PlayDeleteSound>();
                        if (pds != null) pds.Place();
                        //}
                        //else
                        //{
                        //    NoticePoper.PopNotice(6);
                        //}

                        //如果正在模拟，直接作为战斗单位加入
                        if (sandbox.battleRunning)
                        {
                            Algori.GetDominantUnit(ai).GetInBattle();
        
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
                //可以删除unit
                if (hit.transform != null)
                {
                   // print(hit.transform.name);
                    HealthScript h = hit.transform.GetComponent<HealthScript>();

                    if (h != null && !sandbox.battleRunning)
                    {
                        HumanAi ai = h.GetComponent<HumanAi>(); //用去删除坐骑

                        if (ai != null)
                        {
                            if (h != pointingUnit && ai.unitTag != null) //修改tag
                            {
                                if (pointingUnit != null)
                                    pointingUnit.ai.unitTag.PointerExit();//原来指着的取消掉
                                pointingUnit = h;
                                ai.unitTag.PointerEnter();
                            }
                        }

                        bool canDelete = false;
                        if (h.teamIndex == 0 && Input.GetMouseButton(1)) canDelete = true;
                        if (h.teamIndex == 1 && Input.GetMouseButton(0)) canDelete = true;

                        if (canDelete)
                        {
                            if (ai.canEdit && (ai.rider == null || ai.rider.canEdit))
                            {
                                if (deleteAudio != null)
                                {
                                    deleteAudio.Play();//播放删除音效
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
                        if (pointingUnit != null)
                        {
                            pointingUnit.ai.unitTag.PointerExit();//原来指着的取消掉
                            pointingUnit = null;
                        }
                    }
                }
            }
        }
        else
        {
            if (unitModel != null) Destroy(unitModel);
        }
    }

    bool CheckPlacable(Vector3 point, float mySizeX = 0.5f, float mySizeY = 0.5f)
    {
        //print("checking");
        //print(point);
        bool noCollapse = true;
        if (sandbox.unitsPresent != null)
        {
            for (int i = 0; i < sandbox.unitsPresent.Count; i++)
            {
                if (sandbox.unitsPresent[i].currentAi != null)
                {
                    HumanAi tarAi = sandbox.unitsPresent[i].currentAi;
                    if (tarAi == null) continue; //防止出现null
                    if (tarAi.horse != null) tarAi = tarAi.horse;
                    if (Mathf.Abs(point.x - tarAi.transform.position.x) < (tarAi.size.x + mySizeX) / 2 &&
                        Mathf.Abs(point.z - tarAi.transform.position.z) < (tarAi.size.y + mySizeY) / 2)
                    {
                        //print(tarAi.name);
                        noCollapse = false;
                        break;
                    }
                }
                else
                {

                }
            }
        }
        //print(noCollapse);
        return noCollapse;
    }

}
