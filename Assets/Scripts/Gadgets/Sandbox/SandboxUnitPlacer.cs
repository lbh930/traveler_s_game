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

        

        mouseHint.position = new Vector3(-10000, 0, 0); //Ĭ�ϲ���ʾ
        // Check if mouse is over any UI element

        if (MenuManager.nextStoryCharacterId != "sandbox") return;

        bool isMouseOverUI = EventSystem.current.IsPointerOverGameObject();

        if (Input.GetMouseButtonDown(2)) //ȡ��
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
                posSave = unitModel.transform.position; //���ƿ�unitmodel����ֹ���߼�⵽�Լ���
                unitModel.transform.position = Vector3.down * 10000;
            }

            if (Physics.Raycast(ray, out hit, 100, ~(1 << 10)) &&
                hit.transform.gameObject.tag == "Ground")
            {


                if (unitModel != null)
                {
                    unitModel.transform.position = posSave;//���߼������ԷŻ�unitmodel
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
                        pointingUnit.ai.unitTag.PointerExit();//ԭ��ָ�ŵ�ȡ����
                        pointingUnit = null;
                    }

                    //���hint
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
                            ai.rider.GetComponent<HealthScript>().SetPhysicsActive(false); //����Ҳ�ص���ײ
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

                    //���teamindex = 0, �Ҽ�teamindex = 1
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
                            HealthScript riderHealth;
                            riderHealth = ai.rider.GetComponent<HealthScript>();
                            riderHealth.SetPhysicsActive(true); //����Ҳ������ײ
                            riderHealth.teamIndex = chosenTeamIndex;
                        }

                        sandbox.Add(Algori.GetDominantUnit(ai), index);
                        unitModel = null;

                        //currency -= (int)list.unitList[index].GetComponent<HealthScript>().cost;

                        //���Ŵ�����Ч
                        PlayDeleteSound pds = ai.transform.GetComponent<PlayDeleteSound>();
                        if (pds == null && ai.rider != null) pds = ai.rider.transform.GetComponent<PlayDeleteSound>();
                        if (pds != null) pds.Place();
                        //}
                        //else
                        //{
                        //    NoticePoper.PopNotice(6);
                        //}

                        //�������ģ�⣬ֱ����Ϊս����λ����
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
                //����ɾ��unit
                if (hit.transform != null)
                {
                   // print(hit.transform.name);
                    HealthScript h = hit.transform.GetComponent<HealthScript>();

                    if (h != null && !sandbox.battleRunning)
                    {
                        HumanAi ai = h.GetComponent<HumanAi>(); //��ȥɾ������

                        if (ai != null)
                        {
                            if (h != pointingUnit && ai.unitTag != null) //�޸�tag
                            {
                                if (pointingUnit != null)
                                    pointingUnit.ai.unitTag.PointerExit();//ԭ��ָ�ŵ�ȡ����
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
                                    deleteAudio.Play();//����ɾ����Ч
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
                            pointingUnit.ai.unitTag.PointerExit();//ԭ��ָ�ŵ�ȡ����
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
                else
                {

                }
            }
        }
        //print(noCollapse);
        return noCollapse;
    }

}
