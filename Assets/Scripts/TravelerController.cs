using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TravelerController : MonoBehaviour
{

    [System.Serializable]public struct CursorType
    {
        public Texture idle;
        public Texture attacking;
        public Texture hit;
        public Texture reloading;
    }
    public enum TravelerState
    {
        walking,
        building,
        checking,
    }
    
    [HideInInspector]public TravelerState state = TravelerState.walking;
    // Start is called before the first frame update
    [HideInInspector]public IdentityList identityList;
    Palette palette;

    public TravelerCamera cam;
    public Transform inventoryCamPos;
    public GameObject craftTable;
    HumanAi ai;

    IdentityScript lastPointedItem;

    public Transform handHold;
    InventoryScript inventory;
    IdentityScript lastInventoryHeld;
    InventoryFrame lastFrameHeld;
    bool lastHeldLifted = false;
    int parent; //last inventory object parent
    int holding = 0;

    bool initialized = false;
    Rigidbody rigid;
    Animator ani;
    Collider collider;
    ToolScript tool;
    [HideInInspector]public CraftScript craft;

    [HideInInspector]public float acceleration = 1000;
    [HideInInspector]public float maxSpeed = 3;
    float axisH;
    float axisV;
    Vector3 moveDirection;
    Vector3 blendPos;

   

    //鼠标部分
    [Header("Cursor")]
    public RawImage cursor;
    public Text cursorText;
    public Text cursorReloadText;
    public CursorType cutCursor;
    public CursorType pierceCursor;
    public CursorType rangedCursor;
    [HideInInspector]public float cursorHitTimer = -1;
    void Initialize()
    {
        if (initialized) return;
        initialized = true;
        rigid = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
        ani = GetComponent<Animator>();
        inventory = GetComponent<InventoryScript>();
        identityList = GameObject.FindObjectOfType<IdentityList>();
        palette = identityList.gameObject.GetComponent<Palette>();
        cursor = GameObject.FindGameObjectWithTag("Cursor").GetComponent<RawImage>();
        cursorText = GameObject.FindGameObjectWithTag("CursorText").GetComponent<Text>();
        cursorReloadText  = GameObject.FindGameObjectWithTag("CursorReloadText").GetComponent<Text>();
        ai = GetComponent<HumanAi>();
        craft = GetComponent<CraftScript>();
    }
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        cursorHitTimer -= Time.deltaTime;
        Initialize();
        Attack();
        SetCursor();
        /*Inventory();
        Collect();
        Craft();
        Build();

        if (state == TravelerState.building || state == TravelerState.checking)
        {
            craftTable.SetActive(true);
        }
        else
        {
            craftTable.SetActive(false);
        }*/
    }

    void Craft()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0.01f) craft.Roll(1);
        if (Input.GetAxis("Mouse ScrollWheel") < -0.01f) craft.Roll(-1);
    }

    void SetCursorTransform()
    {
        Vector2 charPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 1.3f);
        Vector2 dir = new Vector2(Input.mousePosition.x - charPos.x, Input.mousePosition.y - charPos.y);

        float angle = Vector3.Angle(Vector3.up, new Vector3(dir.x, dir.y, 0));
        if (Vector2.Dot(Vector2.right, dir) > 0)
        {
            angle *= -1;
        }

        cursor.rectTransform.eulerAngles = new Vector3(0, 0, angle);

        //放大效果
        float dis = Vector3.Distance(new Vector3(Screen.width/2, Screen.height/2, 0), Input.mousePosition);
        cursor.rectTransform.localScale = Vector3.one * 0.5f + dis / (Mathf.Sqrt((Screen.width * Screen.width + Screen.height * Screen.height) / 4)) * Vector3.one*0.5f;
    }
    void SetCursor()
    {
        if (PauseScript.paused)
        {
            return;
        }

        if (cursorText != null) cursorText.text = "";
        if (cursorReloadText != null) cursorReloadText.text = "";

        if (ai == null) return;

        if (!ai.inControl) return;

        if (cursor == null) return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        cursorText.text = "";
        cursorReloadText.text = "";
        cursor.color = Color.white;


        if (ai.tool == null)
        {

        }else if (ai.tool.type == ToolScript.ToolType.Melee || (ai.tool.roundNow > 0 && !Input.GetMouseButton(1)))
        {
            tool = ai.tool;

            if (ai.tool.animationIndex == 3 || ai.tool.animationIndex == 5 || ai.tool.animationIndex == 9)//说明是穿刺类
            {
                if (cursorHitTimer <= 0)
                {
                    cursor.texture = pierceCursor.idle;
                }
                else
                {
                    cursor.texture = pierceCursor.hit;            
                }
                cursor.rectTransform.position = Input.mousePosition;
                cursor.rectTransform.sizeDelta = new Vector2(150, 150);


            }
            else //说明是劈砍类
            {
                if (cursorHitTimer <= 0)
                {
                    cursor.texture = cutCursor.idle;
                    cursor.rectTransform.position = Input.mousePosition;
                }
                else
                {
                    cursor.texture = cutCursor.hit;
                    cursor.rectTransform.position = Input.mousePosition;
                }
                cursor.rectTransform.sizeDelta = new Vector2(256, 144);
            }

            SetCursorTransform();
        }
        else //说明是远程
        {
            tool = ai.tool; 
            if (tool.roundNow <= 0 && ai.tool.reloadTimer > 0)
            {//没子弹
                cursor.texture = rangedCursor.reloading;
                cursor.rectTransform.position = Input.mousePosition;
                cursor.rectTransform.sizeDelta = new Vector2(128, 128);
                cursor.rectTransform.Rotate(Vector3.forward * (ai.tool.reloadTimer * 80 + 200.0f)*Time.deltaTime);

                cursorReloadText.text = "";
                cursorReloadText.text += ((int)ai.tool.reloadTimer).ToString();
                cursorReloadText.text += ".";
                cursorReloadText.text += ((int)(((int)(ai.tool.reloadTimer * 10)) % 10)).ToString();

                cursorReloadText.rectTransform.eulerAngles = Vector3.zero;
                Vector3 temp = cursor.rectTransform.eulerAngles; //preserve
                cursor.rectTransform.eulerAngles = Vector3.zero; //so we can use localPos to set the text right down to cursor
                cursorReloadText.rectTransform.localPosition = new Vector3(0, -70, 0);
                cursorReloadText.transform.SetParent(cursor.transform.parent);
                cursor.rectTransform.eulerAngles = temp; //so reload text keep in place
                cursorReloadText.rectTransform.eulerAngles = Vector3.zero;
                cursorReloadText.transform.SetParent(cursor.transform);
                cursorReloadText.rectTransform.sizeDelta = new Vector3(128, 128);
                cursor.rectTransform.localScale = Vector3.one*0.66f;
               // cursorReloadText.rectTransform.sizeDelta *= 
            }
            else //有子弹
            {
                cursorReloadText.text = "";
                if (cursorHitTimer > 0)
                {
                    cursor.texture = rangedCursor.hit;
                }
                else if (Time.time < ai.tool.nextFireTime || ai.tool.roundNow <= 0)
                {
                    cursor.texture = rangedCursor.attacking;
                }
                else
                {
                    cursor.texture = rangedCursor.idle;
                }

                cursor.rectTransform.position = Input.mousePosition;
                cursor.rectTransform.sizeDelta = new Vector2(90, 90);
                cursorText.rectTransform.localScale = Vector3.one*0.5f;
                cursorReloadText.rectTransform.localScale = Vector3.one * 0.65f;

                if (tool.type == ToolScript.ToolType.Bow || tool.isShieldBow) //是弓箭类
                {
                    float bowDraw = ani.GetFloat("BowDraw");
                    if (bowDraw > 0.99f)
                    {
                        cursor.rectTransform.localScale *= 0.9f;
                        cursor.color = new Color(255, 165, 78);
                    }
                    else
                    {
                        cursorText.text = Mathf.RoundToInt((bowDraw * 100)).ToString() + "%";
                        cursorText.rectTransform.eulerAngles = Vector3.zero;
                        cursorText.rectTransform.localScale *= 1.5f - bowDraw * 0.35f;
                        cursor.rectTransform.sizeDelta *= 1.3f - bowDraw * 0.25f;
                    }
                }

                SetCursorTransform();
            }
        }
        //Cursor.SetCursor()
    }
    void Build()
    {
        //开关建造模式
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (state == TravelerState.walking)
            {
                state = TravelerState.building;
                craft.constructing = false;
                craft.GenerateIcons(1, craft.frameCount);
                craft.txt.Read(Application.streamingAssetsPath, "1ydf287gc.txt");
                craft.CreateIcons();
            }
            else if (state == TravelerState.building) state = TravelerState.walking;
        }

        //建造模式打开UI
        if (state == TravelerState.building)
        {

        }
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Collect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        IdentityScript hitID = null;
        if (Physics.Raycast(ray, out hit, 100))
        {
            hitID = hit.transform.GetComponent<IdentityScript>();
        }

        if (hitID != null && hitID.type != IdentityScript.IdType.item) return;

        if (lastPointedItem != null && (hitID == null || hitID.GetHashCode() != lastPointedItem.GetHashCode())) //鼠标移开，原来的指向物品取消高亮
        {
            if (lastPointedItem == null) return;
            MeshRenderer renderer;
            renderer = lastPointedItem.GetComponent<MeshRenderer>();
            renderer.material.DisableKeyword("_EMISSION");
            lastPointedItem = hitID;
        }
        if (hitID != null)//看看是不是指到了可以捡起的物品
        {
            MeshRenderer hitRenderer;
            hitRenderer = hitID.gameObject.GetComponent<MeshRenderer>();
            hitRenderer.material.EnableKeyword("_EMISSION");
            hitRenderer.material.SetColor("_EmissionColor", palette.itemHighlighted);
            lastPointedItem = hitID;
        }
        if (Input.GetKeyDown(KeyCode.E) && lastPointedItem != null) //按E捡起物品
        {
            /*bool storedByQuick = false;
            for (int i = 0; i < inventory.quickFrames.Length; i++) //先看能不能塞进快捷栏
            {
                 if (inventory.quickFrames[i].itemIdentity == 0)
                 {
                     inventory.quickFrames[i].itemIdentity = lastPointedItem.id;
                     Destroy(lastPointedItem.gameObject);
                     storedByQuick = true;
                     break;
                 }
            }
            if (!storedByQuick)//如果没塞进快捷栏，遍历物品栏
            {*/
            for (int i = 0; i < inventory.frames.Length; i++)
            {
                if (!inventory.frames[i].occupied)
                {
                    if (lastPointedItem != null && CheckFrame(i, lastPointedItem))//塞入背包
                    {
                        inventory.frames[i].itemIdentity = lastPointedItem.id;
                        Destroy(lastPointedItem.gameObject);
                        SetFrameOccupation(i, true, lastPointedItem);
                        break;
                    }
                }
            }
        }
    }

    void Inventory()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) holding = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) holding = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3)) holding = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4)) holding = 3;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (state != TravelerState.checking)
            {
                state = TravelerState.checking;
                craft.constructing = false;
                craft.txt.Read(Application.streamingAssetsPath, "1ab34c67d11878d.txt");
                craft.GenerateIcons(0, craft.frameCount);
                craft.CreateIcons();
            }
            else
            {
                state = TravelerState.walking;
                craft.constructing = false;
            }
        }
        
        IdentityScript handholdIdentity = handHold.GetComponentInChildren<IdentityScript>();
        if (handholdIdentity == null || handholdIdentity.id !=
            inventory.quickFrames[holding].itemIdentity){
            if (handholdIdentity != null) Destroy(handholdIdentity.gameObject);

            GameObject go = 
            GameObject.Instantiate(identityList.list[inventory.quickFrames[holding].itemIdentity],
                handHold.position, handHold.rotation);
            go.transform.SetParent(handHold);
            Rigidbody goRigid = go.GetComponent<Rigidbody>();
            Collider goCollider = go.GetComponent<Collider>();
            if (goRigid != null)
            {
                Destroy(goRigid);
                Destroy(goCollider);
                Collider goCollider2 = go.GetComponent<Collider>();
                if (goCollider2 != null) Destroy(goCollider2);
                Collider goCollider3 = go.GetComponent<CapsuleCollider>();
                if (goCollider3 != null) Destroy(goCollider3);
            }
        }

        if (state == TravelerState.checking)
        {
            inventory.quickFrames[0].transform.parent.gameObject.SetActive(true);
            cam.lockView = false;            
            cam.transform.position = inventoryCamPos.position;
            cam.transform.rotation = inventoryCamPos.rotation;
            /*cam.transform.position =
                Vector3.MoveTowards(cam.transform.position,
                inventoryCamPos.position, Time.deltaTime * 35);
            cam.transform.rotation = Quaternion.Euler(
                Vector3.MoveTowards(cam.transform.rotation.eulerAngles,
                inventoryCamPos.rotation.eulerAngles, Time.deltaTime * 1500));*/
            for (int i = 0; i < inventory.frames.Length; i++)  //遍历背包的每一个格子
            {
                if (inventory.frames[i].itemIdentity != 0)
                {
                    IdentityScript id = inventory.frames[i].GetComponentInChildren<IdentityScript>();
                    if (id == null || id.id != inventory.frames[i].itemIdentity)
                    {
                        if (id != null) Destroy(id.gameObject);
                        IdentityScript newid = GameObject.Instantiate(
                            identityList.list[inventory.frames[i].itemIdentity],
                            inventory.frames[i].transform.position,
                            Quaternion.Euler(inventory.frames[i].transform.rotation.eulerAngles
                            + new Vector3(180,90,90))).GetComponent<IdentityScript>();
                        newid.transform.SetParent(inventory.frames[i].transform);

                        newid.transform.rotation = Quaternion.Euler(newid.transform.rotation.eulerAngles 
                            - newid.pivotRotationInPack);
                        if (!inventory.frames[i].isQuickFrame)
                            newid.transform.localPosition -= newid.pivotInPack*10; //此和下行两个乘10晚点改掉
                        newid.transform.localScale = Vector3.one * (inventory.frames[i].isQuickFrame ? 10 : 10) * newid.scaleInPack;

                        Rigidbody goRigid = newid.GetComponent<Rigidbody>();
                        Collider goCollider = newid.GetComponent<Collider>();
                        if (goRigid != null)
                        {
                            Destroy(goRigid);
                            Destroy(goCollider);
                        }
                    }else if (id != null)
                    {
                        id.transform.position = inventory.frames[i].transform.position;
                        if (!inventory.frames[i].isQuickFrame)
                            id.transform.localPosition -= id.pivotInPack*10;
                    }
                }
                else
                {
                    IdentityScript id = inventory.frames[i].GetComponentInChildren<IdentityScript>();
                    if (id != null) Destroy(id.gameObject);
                }
            }

            /*for (int i = 0; i < inventory.quickFrames.Length; i++) //遍历快捷栏
            {
                if (inventory.quickFrames[i].itemIdentity != 0)
                {
                    IdentityScript id = inventory.quickFrames[i].GetComponentInChildren<IdentityScript>();
                    if (id == null || id.id != inventory.quickFrames[i].itemIdentity)
                    {
                        if (id != null) Destroy(id.gameObject);
                        IdentityScript newid = GameObject.Instantiate(
                            identityList.list[inventory.quickFrames[i].itemIdentity],
                            inventory.quickFrames[i].transform.position,
                            Quaternion.Euler(inventory.quickFrames[i].transform.rotation.eulerAngles 
                            + new Vector3(180, 90, 90))).GetComponent<IdentityScript>();
                        newid.transform.SetParent(inventory.quickFrames[i].transform);
                        //newid.transform.position -= newid.pivotInPack; 目前在快捷栏中物品定位在格子中间即可
                        newid.transform.localScale = Vector3.one * newid.scaleInPack * 10;

                        Rigidbody goRigid = newid.GetComponent<Rigidbody>();
                        Collider goCollider = newid.GetComponent<Collider>();
                        if (goRigid != null)
                        {
                            Destroy(goRigid);
                            Destroy(goCollider);
                        }
                    }
                }
            }*/

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //背包操作
            RaycastHit hit;
            InventoryFrame hitFrame = null;
            LayerMask mask = LayerMask.GetMask("Ui3D");

            if (Physics.Raycast(ray, out hit, 100, mask))
                 hitFrame = hit.transform.gameObject.GetComponent<InventoryFrame>();

            for (int i = 0; i < inventory.frames.Length; i++) //遍历格子着色(是否占用)
            {
                if (inventory.frames[i].occupied)
                {
                    inventory.framesMat[i].color = palette.frameOccupied;
                }
                else
                {
                    inventory.framesMat[i].color = palette.frameEmpty;
                }
            }

            if (hitFrame != null)
            {
                for (int i = 0; i < inventory.frames.Length; i++) //遍历格子着色（是否选中)
                {
                    if (inventory.frames[i].GetHashCode() == hitFrame.GetHashCode()) //遍历到被指到的
                    {
                        bool pressing = Input.GetMouseButton(0);
                        if (!inventory.frames[i].occupied)//空格子变完色走人或移动成功
                        {
                            inventory.framesMat[i].color = palette.framePointed;
                            if (Input.GetMouseButton(0))
                                inventory.framesMat[i].color = palette.framePressed;

                            if (lastHeldLifted && !pressing)///inv exchange
                            {
                                if (CheckFrame(i, lastInventoryHeld))
                                {
                                    inventory.frames[i].itemIdentity = lastFrameHeld.itemIdentity;
                                    SetFrameOccupation(parent, false, lastInventoryHeld);
                                    SetFrameOccupation(i, true, lastInventoryHeld);
                                    lastFrameHeld.itemIdentity = 0;
                                    Destroy(lastInventoryHeld.gameObject);
                                }
                                else
                                {
                                }
                                lastHeldLifted = false;
                                lastInventoryHeld = null;
                            }///
                        }
                        else
                        {
                            if (lastInventoryHeld == null && pressing)
                            {
                                parent = i;
                                if (inventory.frames[i].belonging != -1) parent = inventory.frames[i].belonging;
                                lastInventoryHeld = inventory.frames[parent].GetComponentInChildren<IdentityScript>();
                            }
                            else if (lastHeldLifted && !pressing && 
                                inventory.frames[i].belonging == parent)///inv exchange
                            {
                                if (CheckFrame(i, lastInventoryHeld, true)) //自身交换
                                {
                                    inventory.frames[i].itemIdentity = lastFrameHeld.itemIdentity;
                                    SetFrameOccupation(parent, false, lastInventoryHeld);
                                    SetFrameOccupation(i, true, lastInventoryHeld);
                                    lastFrameHeld.itemIdentity = 0;
                                    Destroy(lastInventoryHeld.gameObject);
                                }
                                else
                                {

                                }
                                lastHeldLifted = false;
                                lastInventoryHeld = null;
                            }
                            else if (lastHeldLifted && !pressing) { ///inv exchange
                                int newparent = i;//不空则完成上色
                                if (inventory.frames[i].belonging != -1) newparent = inventory.frames[i].belonging;
                                IdentityScript newHeld = inventory.frames[newparent].GetComponentInChildren<IdentityScript>();
                                if (CheckFrame(i, lastInventoryHeld)
                                    && CheckFrame(parent, newHeld)) //物品交换
                                {
                                    inventory.frames[i].itemIdentity = lastFrameHeld.itemIdentity;
                                    inventory.frames[parent].itemIdentity = newHeld.id;
                                    SetFrameOccupation(newparent, false, newHeld);
                                    SetFrameOccupation(parent, false, lastInventoryHeld);
                                    SetFrameOccupation(i, true, lastInventoryHeld);
                                    SetFrameOccupation(parent, true, newHeld);
                                    Destroy(lastInventoryHeld.gameObject);
                                    Destroy(newHeld.gameObject);
                                }
                                else
                                {

                                }
                                lastHeldLifted = false;
                                lastInventoryHeld = null;
                            }

                            for (int j = 0; 
                                j < (lastInventoryHeld != null? lastInventoryHeld.length : 1); j++)
                            {
                                if (!pressing)
                                    inventory.framesMat[parent + j].color = palette.framePointed;
                                else
                                    inventory.framesMat[parent + j].color = palette.framePressed;

                                for (int w = 0;
                                     w < (lastInventoryHeld != null ? lastInventoryHeld.height : 1); w++)
                                {
                                    if (!pressing)
                                        inventory.framesMat[parent + w * inventory.length + j].color
                                            = palette.framePointed;
                                    else
                                        inventory.framesMat[parent + w * inventory.length + j].color
                                            = palette.framePressed;
                                }
                            }
                        }

                        if (pressing && lastInventoryHeld != null)
                        {
                            lastInventoryHeld.transform.position = hit.point +
                                (Camera.main.transform.position - hit.point) * 0.06f;
                            lastHeldLifted = true;
                            lastFrameHeld = inventory.frames[parent];
                        }
                        else
                        {

                        }
                        break; //变色完成走人
                    }
                }
            }
            else
            {
                bool pressing = Input.GetMouseButton(0);
                if (lastInventoryHeld != null)
                {
                    Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (pressing)
                    {                       
                        lastInventoryHeld.transform.position =
                            Camera.main.transform.position + r.direction.normalized *
                            (Camera.main.transform.position.y - inventory.quickFrames[0].transform.position.y);
                        lastFrameHeld = inventory.frames[parent];
                    }
                    else if (lastHeldLifted)
                    {
                        Instantiate(identityList.list[inventory.frames[parent].itemIdentity],
                            lastInventoryHeld.transform.position + Vector3.up*-0.15f,
                            lastInventoryHeld.transform.rotation);
                        SetFrameOccupation(parent, false, lastInventoryHeld);
                        Destroy(lastInventoryHeld.gameObject);
                        inventory.frames[parent].itemIdentity = 0;
                        lastInventoryHeld = null;
                        lastHeldLifted = false;
                    }
                }

                if (!pressing)
                {
                    lastFrameHeld = null;
                    lastInventoryHeld = null;
                    lastHeldLifted = false;
                }
            }
        }
        else
        {
            inventory.quickFrames[0].transform.parent.gameObject.SetActive(false);
            cam.lockView = true;
        }
    }

    public bool CheckFrame(int p, IdentityScript id, bool selfExchange = true)
    {
        if (inventory.frames[p].isQuickFrame) return true;

        bool canUse = true;
        for (int j = 0; j < id.length; j++)
        {
            if (p + j >= inventory.frames.Length || 
                (p-inventory.quickCount)%inventory.length + j >= inventory.length)
            {
                canUse = false;
                break;
            }
            if (inventory.frames[p + j].occupied)
            {
                if (!selfExchange || (inventory.frames[p + j].belonging != parent
                    && inventory.frames[parent +j].occupied))
                {
                    //print("q " + inventory.frames[parent + j].belonging + " " + parent);
                    canUse = false;
                    break;
                }
            }

            for (int w = 0; w < id.height; w++)
            {
                if (p + w * inventory.length + j >= inventory.frames.Length ||
                    Mathf.Floor((p - inventory.quickCount)/inventory.length + 1 + w) > inventory.height)
                {
                    canUse = false;
                    break;
                }
                if (inventory.frames[p + w * inventory.length + j].occupied)
                {
                    if (!selfExchange ||
                        (inventory.frames[p + w * inventory.length + j].belonging != parent
                        && inventory.frames[p + w * inventory.length + j].occupied))
                    {
                        //print("p " + inventory.frames[parent + w * inventory.length].belonging + " " + parent);
                        canUse = false;
                        break;
                    }
                }
            }
        }      
        return canUse;
    }
    public void SetFrameOccupation(int p, bool occupied, IdentityScript id)
    {
        for (int j = 0; j < id.length; j++)
        {
            if (inventory.frames[p].isQuickFrame) break;

            inventory.frames[p + j].occupied = occupied;
            inventory.frames[p + j].belonging = p;
            for (int w = 0; w < id.height; w++)
            {
                inventory.frames[p + w * inventory.length + j].occupied = occupied;
                inventory.frames[p + w * inventory.length + j].belonging = p;
            }
        }
        inventory.frames[p].belonging = -1;
        inventory.frames[p].occupied = occupied;
    }

    void Attack()
    {
        if (state == TravelerState.checking) return;

        bool rightClicked = Input.GetMouseButton(1);
        ani.SetBool("RightClicked", rightClicked);

        if (!rightClicked)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ani.SetBool("Attack", true);
            }
        }
        else
        {
            if (ai.tool.type == ToolScript.ToolType.Firearm)
            {
                if (ai.tool.semiAuto)
                {
                    if (Input.GetMouseButtonDown(0) && Time.time > ai.tool.nextFireTime && ai.tool.roundNow > 0)
                    {
                        ani.SetBool("Attack", true);
                        ai.StartRangedDamage();
                    }
                }
                else
                {
                    if (Input.GetMouseButton(0) && Time.time > ai.tool.nextFireTime && ai.tool.roundNow > 0)
                    {
                        ani.SetBool("Attack", true);
                        ai.StartRangedDamage();
                    }
                }
            }
            else if (ai.tool.type == ToolScript.ToolType.Bow)
            {
                if (ai.tool.roundNow > 0)
                {
                    float bowDraw = ani.GetFloat("BowDraw");
                    if (Input.GetMouseButton(0))
                    {
                        ani.SetBool("Attack", true);
                        ai.tool.fireStart.position = Vector3.Lerp(ai.tool.stringPos[0].position,
                            ai.tool.stringPos[1].position, bowDraw);
                        ai.tool.fakeArrow.SetActive(true);
                    }
                    else
                    {
                        ani.SetBool("Attack", false);
                        if (bowDraw >= 0.99f && Time.time > ai.tool.nextFireTime)
                        {
                            ai.tool.fireStart.position = ai.tool.stringPos[0].position;
                            ani.SetBool("Attack", false);
                            ai.tool.fakeArrow.SetActive(false);
                            ai.StartRangedDamage();
                            ai.tool.nextFireTime = Time.time + 1 / ai.tool.fireRate;
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ani.SetBool("Attack", true);
                }
            }
        }
    }

    /*public void StartDamageDetection()
    {
        tool = handHold.GetComponentInChildren<ToolScript>();
        if (tool != null)
        {
            DamageDetection detector =
            (GameObject.Instantiate(tool.trailDetector,
                transform.position + transform.forward * tool.trailDistance
                + transform.up*1.3f,
                transform.rotation)).GetComponent<DamageDetection>();
            detector.damage = tool.damage;
            detector.knockForce = tool.knockForce;
            detector.transform.localScale = (1f / 15f) * tool.trailSize * new Vector3 (1, 2,1);
        }
    }*/
    public bool CheckMovable()
    {
        if (state == TravelerState.checking) return false;
        if (state == TravelerState.building && craft.constructing) return false;
        return true;
    }
    void Movement()
    {
        bool pressH = false;
        bool pressV = false;
        moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) //获取移动方向
        {
            if (!CheckMovable()) return;
            //if (Vector3.Project(rigid.velocity, transform.right * -1).magnitude < maxSpeed)
                moveDirection -= Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
            pressH = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (!CheckMovable()) return;
            //if (Vector3.Project(rigid.velocity, transform.right * 1).magnitude < maxSpeed)
                moveDirection += Vector3.ProjectOnPlane(Camera.main.transform.right, Vector3.up).normalized;
            pressH = true;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (!CheckMovable()) return;
            //if (Vector3.Project(rigid.velocity, transform.forward).magnitude < maxSpeed)
                moveDirection += Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            pressV = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (!CheckMovable()) return;
            //if (Vector3.Project(rigid.velocity, transform.forward * -1).magnitude < maxSpeed)
                moveDirection -= Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
            pressV = true;
        }
        moveDirection = moveDirection.normalized;

        ai.Movement(moveDirection);
        if (state != TravelerState.checking) //随着鼠标旋转
        {
            Vector3 mouseDir = Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                1)).direction.normalized;
            Vector3 mousePos = Camera.main.transform.position +
                mouseDir * ((Camera.main.transform.position.y - transform.position.y - 1) / Vector3.Project(mouseDir, Vector3.down).magnitude);

            Vector3 targetDir = mousePos - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);

            if (ai.horse == null)
                ai.RotateTowards(targetDir, 3);
            else
                ai.RotateBodyTowards(targetDir, 3);
        }
    }
}
