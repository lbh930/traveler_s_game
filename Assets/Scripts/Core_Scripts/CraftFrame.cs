using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftFrame : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector]public IdentityList idList;
    bool initialized = false;
    public float frameGroupPadding = 0.5f;
    public float selectedScale = 1.2f;
    public float commonScale = 0.6f;
    public Vector3 firstFrameGroupTilting;
    
    [HideInInspector]public SpriteRenderer renderer;
    [HideInInspector] public int craftID;
    [HideInInspector] public int matGroupId = -1;
    [HideInInspector] public ConstructionSite site;
    [HideInInspector] public ImageSet imageSet;

    public float padding = 0.1f;
    public float sizeClamp = 0.4f;
    [HideInInspector] public TxtReader txt;
    [HideInInspector] public TxtReader txtCraft;
    [HideInInspector] public TxtReader txtBuild;
    public Transform tarPos;
    public Transform firstMaterialPos;
    public Transform pointer;
    public SpriteRenderer[] matFrames;
    public SpriteRenderer[] techFrames;
    public Transform maskTransform;
    [HideInInspector]public IdentityScript tarId;
    [HideInInspector]public IdentityScript[] matId;

    [HideInInspector] public float craftTime = 1;
    [HideInInspector] public int matCount;
    [HideInInspector] public int techCount;

    int lastID = -1;
    int lastType = 0;
    void Initialize()
    {
        
        if (initialized) return;
        renderer = GetComponent<SpriteRenderer>();
        imageSet = GetComponent<ImageSet>();
        initialized = true;
    }

    void Start()
    {
        Initialize();
    }

    public void Check(int type = 0, bool enforceCheck = false) //type,0为合成，1为建造
    {
        if (lastID != craftID || lastType != type || enforceCheck) Set(type);
        lastID = craftID;
        lastType = type;
    }
    void Set(int type)
    {
        if (tarId != null)
        {
            Destroy(tarId.gameObject);
            if (matId.Length > 0)
            {
                for (int i = 0; i < matId.Length; i++)
                {
                    if (matId[i] != null) Destroy(matId[i].gameObject);
                }
            }
        }

        switch (type)
        {
            case 0:
                tarId = GameObject.Instantiate(idList.list[txt.getInt(craftID, 0)],
                    tarPos.position, pointer.rotation).GetComponent<IdentityScript>();
                break;
            case 1:
                
                tarId = GameObject.Instantiate(idList.bList[txt.getInt(craftID, 0)],
                    tarPos.position, pointer.rotation).GetComponent<IdentityScript>();
                break;
            case 2: //construct
                break;
        }

        if (tarId != null)
            PutInFrame(tarId, 0.22f, false);

        if (type == 0 || type == 2) //处理制作材料的显示
        {
            if (type == 0)
                matCount = txt.getInt(craftID, 1);
            else if (type == 2)
                matCount = site.matId[matGroupId].Length;

            matId = new IdentityScript[matCount];

            for (int i = 0; i < matCount; i++)
            {
                switch (type)
                {
                    case 0:
                        matId[i] = GameObject.Instantiate(idList.list[txt.getInt(craftID, 2 + i)],
                            firstMaterialPos.position + firstMaterialPos.forward * padding * i * (-1),
                            pointer.rotation).GetComponent<IdentityScript>();
                            PutInFrame(matId[i], 0.125f, true, i);
                        break;
                    case 2:
                        matId[i] = matId[i] = GameObject.Instantiate(idList.list[site.matId[matGroupId][i]],
                            firstMaterialPos.position + firstMaterialPos.forward * padding * i * (-1),
                            pointer.rotation).GetComponent<IdentityScript>();
                            PutInFrame(matId[i], 0.125f, true, i);
                        break;
                }
            }

            for (int i = 0; i < matFrames.Length; i++)
            {
                if (i < matCount) matFrames[i].enabled = true;
                else matFrames[i].enabled = false;
            }      
        }

        switch (type) //处理科技相关显示
        {
            case 0:
                techCount = txt.getInt(craftID, 1 + matCount + 1);
                break;
            case 1:
                techCount = txt.getInt(craftID, 1);
                break;
            case 2:
                techCount = txt.getInt(craftID, 1);
                break;
        }

        for (int i = 0; i < techFrames.Length; i++)
        {
            if (i < techCount) techFrames[i].enabled = true;
            else techFrames[i].enabled = false;
        }

        switch (type) //处理合成时间
        {
            case 0:
                craftTime = txt.getFloat(craftID, 1 + matCount + 1 + techCount + 1);
                break;
            case 1:
                
                break;
            case 2:
                craftTime = site.craftTimes[matGroupId];
                break;
        }
    }

    void PutInFrame(IdentityScript target, float sizeOfFrame = 0.2f, bool isMat = false, int matIndex = 0)
    {
        MeshFilter mesh = target.GetComponent<MeshFilter>();
        target.transform.SetParent(transform);
        if (!isMat)
        {
            target.transform.localPosition = Vector3.zero;
        }
        else
        {
            target.transform.localPosition = firstMaterialPos.localPosition + Vector3.right * -1 * padding * matIndex;
        }

        target.ClearPhysics();

        if (mesh != null)
        {
            float scaleFactor = sizeOfFrame / mesh.mesh.bounds.size.y;
            if (scaleFactor > sizeClamp) scaleFactor = sizeClamp;
            if (isMat && scaleFactor > sizeClamp * 0.618f) scaleFactor = sizeClamp * 0.618f;
            target.transform.localPosition -= mesh.mesh.bounds.center * scaleFactor;
            target.transform.localScale = Vector3.one * scaleFactor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        //print(tarId.GetComponent<MeshFilter>().mesh.bounds.);
    }
}
