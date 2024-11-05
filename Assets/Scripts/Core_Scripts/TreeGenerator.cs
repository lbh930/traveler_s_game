using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    [HideInInspector]public SingleTrunk root;
    public GameObject[] trunkObject;
    public GameObject[] leaveObject;
    public GameObject rootObject;

    [Header ("Trunk Properties")]
    public float[] trunkBottomAverageRadius;
    public float[] trunkRadiusRandomRange;
    public float[] averageHeight;
    public float[] heightRandomRange;
    public float[] trunkAverageLength;
    public float[] trunkLengthRandomRange;
    public float[] trunkMaxJiggle;
    public float[] trunkMaxTwist;
    public float[] chanceOfGrow;
    public int branchPerTrunk = 3;
    public float minAngle = -10f;
    public float maxAngle = 20f;
    public float[] minbranchGrowthHeight;

    [Header("Leaves Properties")]
    public float[] leavesAverageSize;
    public float[] leavesSizeRandomRange;

    void Initialize()
    {
        
        if (initialized) return;
        root = GenerateTree(0);
        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }

    SingleTrunk GenerateTree(int level = 0, float scaleMuiltipler = 1.0f)
    {
        if (level >= 3) return null;

        float treeHeight = averageHeight[level] + Random.Range((heightRandomRange[level] / 2) * -1,
            heightRandomRange[level] / 2);
        float totalHeight = 0;
        SingleTrunk previousTrunk = null;
        SingleTrunk firstTrunk = null;
        while (true)
        {
            SingleTrunk trunk = GameObject.Instantiate(trunkObject[level], 
                gameObject.transform).GetComponent<SingleTrunk>();

            //设置新旧树干的nxt和prev
            if (previousTrunk != null) {
                previousTrunk.nextTrunk = trunk;
                trunk.transform.SetParent(previousTrunk.transform);
            }
            else
            {
                firstTrunk = trunk;
            }

            trunk.previousTrunk = previousTrunk;           

            //设置新树干外形和位置（第一轮）

            trunk.startBone.localScale = Vector3.one * 2
                * Algori.RandomInRange(trunkBottomAverageRadius[level], trunkRadiusRandomRange[level]);

            trunk.endBone.localScale = Vector3.one * 2
                * Algori.RandomInRange(trunkBottomAverageRadius[level], trunkRadiusRandomRange[level]);

            if (trunk.previousTrunk != null) trunk.startBone.localScale = trunk.previousTrunk.endBone.localScale;

            if (previousTrunk != null)
                trunk.startBone.transform.localPosition = Algori.Jiggle(Vector3.zero,
                    Random.Range(0.0f, trunkMaxJiggle[level]));
            trunk.endBone.transform.localPosition = Algori.Jiggle(Vector3.zero,
                Random.Range(0.0f, trunkMaxJiggle[level]));

            trunk.endBone.rotation = 
                Quaternion.Euler(Vector3.up * Random.Range(0.0f, trunkMaxTwist[level]));

            float trunkHeight = trunkAverageLength[level] + Random.Range((trunkLengthRandomRange[level] / 2) * -1,
                trunkLengthRandomRange[level] / 2);
            if (trunkHeight + totalHeight > treeHeight)
            {
                //如果达到预期高度则停止while
                trunkHeight = treeHeight - totalHeight;
                trunk.height = trunkHeight;
                trunk.endBone.transform.position =
                    new Vector3(trunk.endBone.position.x,
                    trunk.startBone.position.y + trunkHeight, trunk.endBone.position.z);
                trunk.endBone.localScale = Vector3.one * 0.001f;
                trunk.DisableCollider();

                //并且种树叶
                Transform leaves = Instantiate(leaveObject[level], transform).transform;

                leaves.localScale = Vector3.one * Algori.RandomInRange(leavesAverageSize[level], leavesSizeRandomRange[level]);
                leaves.rotation = Quaternion.Euler(new Vector3(
                    Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f)));
                leaves.SetParent(trunk.transform);
                leaves.localPosition = trunk.endBone.localPosition;                
                break;
            }

            float scalePercent = (treeHeight - totalHeight) / treeHeight;
            trunk.endBone.transform.position =
                   new Vector3(trunk.endBone.position.x,
                   trunk.startBone.position.y + trunkHeight, trunk.endBone.position.z);
            trunk.endBone.localScale *= scalePercent * scaleMuiltipler;
            if (previousTrunk == null) trunk.startBone.localScale *= scaleMuiltipler;
            totalHeight += trunkHeight;
            trunk.height = trunkHeight;

            //生成树根
            if (level == 0 && previousTrunk == null)
            {
                SingleTrunk bottomTrunk = Instantiate(rootObject, trunk.transform.position, trunk.transform.rotation).GetComponent<SingleTrunk>();
                bottomTrunk.SetCollider();
                bottomTrunk.transform.SetParent(transform);
                Algori.CopyTransform(bottomTrunk.startBone, trunk.startBone);
                Algori.CopyTransform(bottomTrunk.endBone, trunk.endBone);

                GetComponent<TreeFall>().bottomTrunk = bottomTrunk.transform;
                GetComponent<TreeFall>().rootTrunk = trunk;
                GetComponent<TreeFall>().rootHealth = trunk.gameObject.GetComponent<HealthScript>();
            }

            previousTrunk = trunk;

            //设置新生成的trunk的碰撞
            if (level == 0)
                trunk.SetCollider();
            else
                trunk.DisableCollider();

            //如果没有因为是最后一干被break掉，才生长树枝。
            trunk.branches = new SingleTrunk[branchPerTrunk];
            for (int i = 0; i < branchPerTrunk; i++)
            {
                if ((totalHeight - trunkHeight / 2) < minbranchGrowthHeight[level]) break;

                float m = 1; //越接近顶部越可能枝条
                if (scalePercent < 0.3f) m = 1.5f;
                if (scalePercent < 0.15f) m = 2.5f;
                if (scalePercent > 0.66f) m = 0.7f; 

                if (Random.Range(0.00f, 1.00f) < chanceOfGrow[level] * m)
                {
                    SingleTrunk branchTrunk = GenerateTree(level + 1, scalePercent*scaleMuiltipler);
                    if (branchTrunk == null) break;

                    float panAngle = Random.Range(360.0f / branchPerTrunk * (i - 1), 360.0f / branchPerTrunk);
                    float hAngle = Random.Range(minAngle, maxAngle);
                    Vector3 dir = new Vector3(Mathf.Sin(panAngle), Mathf.Tan (hAngle/360.0f * 2 * Mathf.PI),
                        Mathf.Cos(panAngle));
                    
                    AlignTrunk(branchTrunk);
                    branchTrunk.transform.up = dir;
                    branchTrunk.transform.localPosition = branchTrunk.transform.position;

                    branchTrunk.transform.SetParent(trunk.transform);
                    branchTrunk.transform.localPosition
                        = (trunk.endBone.localPosition + trunk.startBone.localPosition) / 2;
                }
            }
        }

        if (level == 0) AlignTrunk(firstTrunk);

        return firstTrunk;
    }
    void AlignTrunk(SingleTrunk trunkNow)
    {
        if (trunkNow.nextTrunk != null)
        {
            trunkNow.nextTrunk.transform.position +=
                trunkNow.endBone.position - trunkNow.nextTrunk.startBone.position;
            trunkNow.nextTrunk.startBone.rotation = trunkNow.endBone.rotation;
            AlignTrunk(trunkNow.nextTrunk);
        }
        else
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        
        /*if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                if (child.gameObject != gameObject)
                Destroy(child.gameObject);
            }

            root = null;
            GenerateTree(0);
        }*/

    }
}
