using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CraftScript : MonoBehaviour
{
    // Start is called before the first frame update
    int[] itemsCount;
    [HideInInspector]public TxtReader txt;
    InventoryScript inventory;
    IdentityList idList;
    TravelerController controller;
    ObjectSet objectSet;

    bool initialized = false;
    CraftFrame[] frames;
    public Transform frameParent;
    public Transform framePointer;

    public Vector2 maskTilting;
    public int frameCount = 9;
    int frameCountNow = 9;

    public float basicCraftTime = 2.0f;
    public GameObject[] frameTemplate;
    [HideInInspector]public int index = 0;
    int maxIndex = 20;

    float craftProgress = 0;
    bool craftStarted = false;
    float craftTime = 0;
    bool crafting = false;
    int shouldPoint;

    [Header("For Build Only")]
    public Transform buildParent;
    IdentityScript preShownBuilding;
    MapScript map;
    [HideInInspector]public bool constructing = false;
    [HideInInspector]public ConstructionSite site;

    void Initialize()
    {
        
        if (initialized) return;
        initialized = true;
        frames = new CraftFrame[frameCount];

        inventory = GetComponent<InventoryScript>();
        controller = GetComponent<TravelerController>();
        objectSet = GetComponent<ObjectSet>();

        txt = GetComponent<TxtReader>();
        txt.Read(Application.streamingAssetsPath,"1ab34c67d11878d.txt");

        maxIndex = txt.lineCount - 1;
        idList = GameObject.FindObjectOfType<IdentityList>();
        map = GameObject.FindObjectOfType<MapScript>();

        GenerateIcons(0, frameCount);

        UpdateFrames();

        CreateIcons();
    }

    void Start()
    {
        Initialize();
        
    }

    public void GenerateIcons(int type, int _frameCount = 9)
    {
        frameCountNow = _frameCount;

        for (int i = 0; i < frames.Length; i++)
        {
            if (frames[i]!=null)
                Destroy(frames[i].gameObject);
        }

        for (int i = 0; i < _frameCount; i++)
        {
            CraftFrame frame = GameObject.Instantiate(frameTemplate[type],
                transform.position,
                Quaternion.Euler(frameParent.rotation.eulerAngles
                )).GetComponent<CraftFrame>();

            frame.transform.position =
                frameParent.position + framePointer.forward * frame.frameGroupPadding
                * i * (-1) + framePointer.up * 0.03f;            

            if (type == 2)
            {
                frame.matGroupId = i;
                frame.site = site;
            }

            frames[i] = frame;
            frames[i].transform.SetParent(frameParent);
            frame.transform.localPosition += frame.firstFrameGroupTilting;
        }
    }

    void Build()
    {  
        if (controller.state != TravelerController.TravelerState.building) return;

        bool canBuild = false;

        //用射线找鼠标指的cell从而得到相应tile
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        //print(hit.transform.gameObject);
        //print(hit.point);
        
        Tilemap currentBuildingLayer = map.buildingLayers[map.currentLayer];
        Tilemap currentTerrainLayer = map.terrainLayers[map.currentLayer];
        Vector3Int pointedCellPos = Vector3Int.zero;

        if (Physics.Raycast(ray, out hit, 100f) && 
            hit.transform.GetComponent<Tilemap>().GetHashCode() == currentTerrainLayer.GetHashCode())
        {
            pointedCellPos = currentTerrainLayer.WorldToCell(hit.point);

            if (currentTerrainLayer.GetTile (pointedCellPos) != null
                && currentBuildingLayer.GetTile(pointedCellPos) == null)
            {
                canBuild = true;
            }
        }

        if (canBuild && !constructing)
        {
            //在所指的cell上预显示一个将要建造的建筑
            if ((preShownBuilding != null && preShownBuilding.id != frames[shouldPoint].craftID)
                || preShownBuilding == null)
            {
                if (preShownBuilding != null)
                {
                    Destroy(preShownBuilding.gameObject);
                }

                preShownBuilding = Instantiate(idList.bList[frames[shouldPoint].craftID],
                    buildParent).GetComponent<IdentityScript>();
                Component[] components = preShownBuilding.GetComponents<Component>();
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i].GetType() != typeof(MeshRenderer) &&
                        components[i].GetType() != typeof(MeshFilter) &&
                        components[i].GetType() != typeof(Transform) &&
                        components[i].GetType() != typeof (IdentityScript))
                    {
                        Destroy(components[i]);
                    }
                }               
            }
            preShownBuilding.transform.localPosition = Vector3.zero;

            buildParent.position = new Vector3(pointedCellPos.x, 0, pointedCellPos.y) + new Vector3 (0.5f,0,0.5f);

            //按下左键建造
            if (Input.GetMouseButtonDown(0))
            {
                //所指无tile则可直接建造
                if (canBuild)
                {       
                    IdentityScript built = Instantiate(idList.bList[frames[shouldPoint].craftID],
                    null).GetComponent<IdentityScript>();
                    built.transform.position = new Vector3(pointedCellPos.x,
                        map.buildingLayers[map.currentLayer].transform.position.y,
                        pointedCellPos.y) + new Vector3(0.5f, 0, 0.5f);

                    site = built.gameObject.GetComponent<ConstructionSite>();
                    site.placed = true;
                    int craftId = frames[shouldPoint].craftID;                   
                    site.craftId = craftId;
                    int matGroupCount = 0;
                    matGroupCount = txt.getInt(craftId, 2);
                    site.matId = new int[matGroupCount][]; //重建construction site的几个变量
                    site.craftTimes = new int[matGroupCount];
                    site.finisheds = new bool[matGroupCount];
                    site.completed = false;
                    for (int i = 0; i < site.finisheds.Length; i++)
                    {
                        site.finisheds[i] = false;
                    }
                    int index = 3;
                    for (int i = 0; i < matGroupCount; i++)
                    {
                        int matCount = txt.getInt(craftId, index);
                        site.matId[i] = new int[matCount];
                        for (int j = 0; j < matCount; j++)
                        {
                            index++;
                            site.matId[i][j] = txt.getInt(craftId, index);
                        }
                        index++;
                        site.craftTimes[i] = txt.getInt(craftId, index);

                        index++; //index+1后指向下一个matCount
                    }
                    StartConstructing(site);
                    //生成特效
                    Instantiate(objectSet.objects[0], site.transform.position, Quaternion.Euler(Vector3.zero));
                }
            }
        }
        else
        {
            if (preShownBuilding != null) Destroy(preShownBuilding.gameObject);
        }
    }

    public void StartConstructing(ConstructionSite newSite)
    {
        site = newSite;
        constructing = true;
        controller.state = TravelerController.TravelerState.building;
        GenerateIcons(2, site.matId.Length);
        CreateIcons(true);
    }

    public void UpdateFrames()
    {
        maxIndex = txt.lineCount - 1;

        shouldPoint = index % frameCountNow;

        if (constructing)
        {
            for (int i = 0; i < frameCountNow; i++)
            {
                if (i != shouldPoint)
                {
                    frames[i].transform.localScale = Vector3.one * frames[i].commonScale;
                }
                else
                {
                    frames[i].transform.localScale = Vector3.one * frames[i].selectedScale;
                }
            }
        }
        else
        {
            for (int i = 1; i <= frameCountNow / 2; i++)
            {
                int pointer = shouldPoint + i;
                pointer %= frameCountNow;
                frames[pointer].transform.position = frameParent.position +
                    framePointer.forward * ((float)i + frames[pointer].selectedScale * 0.1f)
                    * frames[pointer].frameGroupPadding * (-1);
                frames[pointer].craftID = index + i;
                frames[pointer].craftID %= maxIndex;
                frames[pointer].transform.localScale = Vector3.one * frames[pointer].commonScale;
            }

            for (int i = 1; i <= frameCountNow / 2; i++)
            {
                int pointer = shouldPoint - i;
                pointer = (pointer + frameCountNow) % frameCountNow;
                frames[pointer].transform.position = frameParent.position +
                    framePointer.forward * ((float)i + frames[pointer].selectedScale * 0.1f)
                    * frames[pointer].frameGroupPadding * (1);
                frames[pointer].craftID = index - i;
                frames[pointer].craftID = (frames[pointer].craftID + maxIndex * frameCountNow) % maxIndex;
                frames[pointer].transform.localScale = Vector3.one * frames[pointer].commonScale;
            }

            frames[shouldPoint].transform.position = frameParent.position;
            frames[shouldPoint].transform.localScale = Vector3.one * frames[shouldPoint].selectedScale;
            frames[shouldPoint].craftID = index;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        UpdateFrames();
        CheckAvailble();
        Craft();
        Build();

        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            craftStarted = true;
        }

        if (craftStarted && (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey (KeyCode.Space)))
        {
            bool canUse = true;
            for (int j = 0; j < frames[shouldPoint].matCount; j++)
            {
                int id = frames[shouldPoint].matId[j].id;
                if (itemsCount[id] <= 0)
                {
                    canUse = false;
                }
            }
            if (canUse) crafting = true;
        }

        if (Input.GetKeyUp(KeyCode.KeypadEnter) || Input.GetKeyUp(KeyCode.Space) || !craftStarted)
        {
            craftStarted = false;
            crafting = false;
        }
    }

    void Craft()
    {
        if (constructing && site.finisheds[shouldPoint]) return; //如果在建造填料且该项已经完成，不进行函数

        if (constructing || controller.state == TravelerController.TravelerState.checking)
        {
            Vector3 startPos = frames[shouldPoint].matFrames[frames[shouldPoint].matCount - 1].transform.localPosition
                + Vector3.right * maskTilting.x;
            startPos = new Vector3(startPos.x, 0, startPos.z);
            Vector3 finishPos = Vector3.right * maskTilting.y;

            float speed = 1.0f / (frames[shouldPoint].craftTime * basicCraftTime);
            craftProgress = Mathf.Clamp01(craftProgress);

            if (crafting)
            {
                craftProgress += speed * Time.deltaTime;

                if (craftProgress > 0.999f)
                {
                    if (constructing)
                    {
                        site.finisheds[shouldPoint] = true;

                        Instantiate(objectSet.objects[1], site.transform.position, Quaternion.Euler(Vector3.zero));

                        if (site.GetFinished())
                        {
                            constructing = false;
                            controller.state = TravelerController.TravelerState.walking;
                            site.completed = true;

                            Instantiate(objectSet.objects[2], site.transform.position, Quaternion.Euler(Vector3.zero));
                        }
                    }
                    else
                    {
                        FinishCraft(); //合成完成
                    }
                }
                frames[shouldPoint].maskTransform.localPosition = Vector3.Lerp(startPos,
                    finishPos, craftProgress);
            }
            else
            {
                craftProgress -= Time.deltaTime * 5;
                if (craftProgress < 0.001f) frames[shouldPoint].maskTransform.localPosition = Vector3.right * -10;
            }
        }
        //Debug.Log(craftProgress);                   
    }

    void FinishCraft()
    {
        int i = shouldPoint;
        for (int j = 0; j < frames[i].matCount; j++)
        {
            int id = frames[i].matId[j].id;
            DeleteInPack(id);
        }

        for (int j = 0; j < inventory.frames.Length; j++)
        {
            if (controller.CheckFrame(j, frames[shouldPoint].tarId, false) && inventory.frames[j].itemIdentity <= 0)
            {
                inventory.frames[j].itemIdentity = frames[shouldPoint].tarId.id;
                if (!inventory.frames[j].isQuickFrame)
                    controller.SetFrameOccupation(j, true, frames[shouldPoint].tarId);
                break;
            }
        }

        crafting = false;
        craftStarted = false;
    }

    void DeleteInPack(int id) //在背包中找到相应材料删除
    {
        for (int i = 0; i < inventory.frames.Length; i++)
        {
            if (inventory.frames[i].itemIdentity == id && inventory.frames[i].belonging == -1)
            {
                if (!inventory.frames[i].isQuickFrame)
                {
                    controller.SetFrameOccupation(i, false,
                        idList.list[id].GetComponent<IdentityScript>());
                }
                inventory.frames[i].itemIdentity = 0;
                break;
            }
        }
    }

    void CheckAvailble()
    {
        itemsCount = new int[idList.list.Length];
        for (int i = 0; i < itemsCount.Length; i++) itemsCount[i] = 0;

        for (int i = 0; i < inventory.frames.Length; i++)
        {
            if (inventory.frames[i].occupied && inventory.frames[i].belonging == -1)
            {
                itemsCount[inventory.frames[i].itemIdentity]++;
            }
        }

        for (int i = 0; i < frames.Length; i++)
        {
            bool canUse = true;
            for (int j = 0; j < frames[i].matId.Length; j++)
            {
                if (frames[i].matId[j] == null) break;

                int id = frames[i].matId[j].id;
                if (itemsCount[id] <= 0)
                {
                    frames[i].matId[j].meshRenderer.material.color = new Color(0.1f, 0.1f, 0.1f);
                    frames[i].matId[j].meshRenderer.material.DisableKeyword("Specular Highlights");
                    canUse = false;
                }
                else
                {
                    frames[i].matId[j].meshRenderer.material.color = frames[i].matId[j].originColor;
                    frames[i].matId[j].meshRenderer.material.EnableKeyword("Specular Highlights");
                }
            }

            if (!constructing && frames[i].tarId != null)
            {
                if (!canUse)
                {
                    frames[i].tarId.meshRenderer.material.color = new Color(0.1f, 0.1f, 0.1f);
                    frames[i].tarId.meshRenderer.material.DisableKeyword("Specular Highlights");
                }
                else
                {
                    frames[i].tarId.meshRenderer.material.color = frames[i].tarId.originColor;
                    frames[i].tarId.meshRenderer.material.EnableKeyword("Specular Highlights");
                }
            }
            else if (frames[i].renderer != null && site != null && constructing)
            {
                if (site.finisheds[i])
                {
                    frames[i].renderer.sprite = frames[i].imageSet.images[2];
                }
                else if (!canUse)
                {
                    frames[i].renderer.sprite = frames[i].imageSet.images[0];
                }
                else
                {
                    frames[i].renderer.sprite = frames[i].imageSet.images[1];
                }
            }
        }
    }

    public void CreateIcons(bool enforceCheck = false)
    {
        UpdateFrames();
        for (int i = 0; i < frameCountNow; i++)
        {
            frames[i].idList = controller.identityList;
            frames[i].txt = txt;
            if (controller.state == TravelerController.TravelerState.building && !constructing)
            {
                frames[i].Check(1, enforceCheck);
            }
            else if (controller.state == TravelerController.TravelerState.building && constructing)
            {
                frames[i].Check(2, enforceCheck);
            }
            else
            {
                frames[i].Check(0, enforceCheck);
            }
        }
    }

    public void Roll(int roll)
    {
        craftProgress = 0;
        frames[shouldPoint].maskTransform.localPosition = Vector3.right * -10;

        index -= roll;

        if (!constructing)
        {
            index = index < 0 ? (index + maxIndex * 10) % maxIndex : index;
            index = index > maxIndex ? index % maxIndex : index;
        }
        else
        {
            if (index >= maxIndex) index = maxIndex - 1;
            if (index <= 0) index = 0;
        }
        CreateIcons();
        
    }

    void Enable()
    {
        Initialize();
    }
}
