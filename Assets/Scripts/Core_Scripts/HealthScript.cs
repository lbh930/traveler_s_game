using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rigid;
    public float maxHealth = 100;
    public float health = 100;
    public bool undying = false;

    public float pierceResis;
    public float cutResis;
    public float bluntResis;

    public int cost = 100;
    public float dieForceMultipler = 10;

    public float dropDeathThreshold = -16f; //y axis below this means death 

    Vector3 lastPos;
    Vector3 velocity = Vector3.zero;

    public bool isShield = false;

    [Header("Manually Add")]
    public Collider mainCollider;
    public GameObject ragdoll;
    public DropOnDeath drop;
    public SpawnOnDeath spawn;
    public GameObject hurtEffect;//受伤时放一个特效
    public GameObject hurtEffectSmall;
    

    public int teamIndex = 0;
    int lastTeamIndex = 0;

    public bool neverAsTarget = false;
    [HideInInspector] public Vector3 dieForce;
    [HideInInspector] public HealthScript owner;
    [HideInInspector] public HumanAi ai;

    [HideInInspector] public HealthScript parent;    
    

    [Header("Team Color")]
    public Material[] teamMaterials;
    public MeshRenderer[] teamMatObjects;
    public SkinnedMeshRenderer[] teamMatObjectsSkinned;



    //[HideInInspector] public Rigidbody[] drops = new Rigidbody[3];

    bool initialized = false;
    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        lastTeamIndex = teamIndex; //用于检测teamindex的改变

        rigid = GetComponent<Rigidbody>();
        ai = GetComponent<HumanAi>();
        lastPos = transform.position;
        if (drop == null) drop = GetComponent<DropOnDeath>();
        if (spawn == null) spawn = GetComponent<SpawnOnDeath>();
        if (mainCollider == null) mainCollider = GetComponent<Collider>();

        if (!neverAsTarget)
            TargetList.targets.Add(this);

        SetTeamColor();
    }

    void SetTeamColor()
    {
        //设置颜色

        if (teamMaterials.Length > 0 && teamIndex < teamMaterials.Length && teamIndex >= 0)
        {
            if (teamMatObjects.Length > 0)
            {
                for (int i = 0; i < teamMatObjects.Length; i++)
                {
                    if (teamMatObjects[i] != null)
                    {
                        teamMatObjects[i].material = teamMaterials[teamIndex];
                    }
                }
            }

            if (teamMatObjectsSkinned.Length > 0)
            {
                for (int i = 0; i < teamMatObjectsSkinned.Length; i++)
                {
                    if (teamMatObjectsSkinned[i] != null)
                    {
                        teamMatObjectsSkinned[i].material = teamMaterials[teamIndex];
                    }
                }
            }
        }
    }

    void Start()
    {
        Initialize();

    }

    public void AddToTargetList()
    {
        TargetList.targets.Add(this);
    }

    public void ApplyDamage(float damage)
    {
        if (damage > 0 && !undying)
        {
            health -= Mathf.RoundToInt(damage);
            if (hurtEffect != null)
            {
                if (damage >= 50)
                {
                    Instantiate(hurtEffect, transform.position, transform.rotation);
                }
                else if (hurtEffectSmall != null)
                {
                    Instantiate(hurtEffectSmall, transform.position, transform.rotation);
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector3 tempVelocity = (transform.position - lastPos) / Time.fixedDeltaTime;
        if (tempVelocity.magnitude < 1000)
        {
            velocity = (transform.position - lastPos) / Time.fixedDeltaTime;
        }
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < dropDeathThreshold && rigid != null && rigid.velocity.y < -1) health = 0;

        Initialize();

        if (teamIndex != lastTeamIndex) //teamIndex变过，重新设置颜色
        {
            lastTeamIndex = teamIndex;//表示已经更新
            SetTeamColor();
        }

        if (health <= 0)
        {
            HumanAi ai = GetComponent<HumanAi>(); //尝试获取humanai
            if (ai != null)
            {
                if (ai.rider != null)
                {
                    ai.SetRiderCollidersAsTrigger(false);
                    ai.rider.transform.SetParent(transform.parent); //死亡后释放骑手
                    Rigidbody riderRigid = ai.rider.GetComponent<Rigidbody>();
                    riderRigid.velocity = rigid.velocity;
                    
                }
            }

            Algori.SetLayerOfChildren(transform, "0"); //如果有描边就取消描边

            if (ragdoll != null)
            {  //有布娃娃则生成
                metgoragdoll();
                Destroy(gameObject);
            }
            if (drop != null)
            {
                drop.Drop();
            }
            if (spawn != null)
            {
                spawn.Spawn();
            }
        }
    }
    public void metgoragdoll()
    {
        Transform doll = Instantiate(ragdoll, transform.position, transform.rotation).transform;
        metcopytransforms(transform, doll, doll.GetComponent<RagdollScript>());
        doll.GetComponent<RagdollScript>().
            spineToAddForce.GetComponent<Rigidbody>().velocity += dieForce / rigid.mass * dieForceMultipler;
      
    }

    private void metcopytransforms(Transform varpsource, Transform varpdestination, RagdollScript ragdoll)
    {
        varpdestination.position = varpsource.position;
        varpdestination.rotation = varpsource.rotation;

        //保留物品
        if (varpdestination.childCount != varpsource.childCount)
        {
            for (int i = 0; i < varpsource.childCount; i++)
            {
                Transform source = varpsource.GetChild(i);
                bool same = false;
                for (int j = 0; j < varpdestination.childCount; j++)
                {
                    if (source.name == varpdestination.GetChild(j).name)
                    {
                        same = true;
                        break;
                    }
                }
                if (!same && source.gameObject.active)
                {
                    varpsource.GetChild(i).SetParent(varpdestination);

                    ClothesAnimation clothesAni = source.GetComponent<ClothesAnimation>();
                    if (clothesAni != null)
                    {
                        clothesAni.TargetMeshRenderer = ragdoll.skinnedRenderer;
                        clothesAni.Bind();
                    }
             
                    Rigidbody[] _rigid = source.GetComponentsInChildren<Rigidbody>();
                    if (_rigid.Length > 0)
                    {
                        foreach (Collider _collider in source.GetComponentsInChildren<Collider>())
                        {
                            if (!_collider.isTrigger) _collider.enabled = true;
                        }
                        foreach (Rigidbody __rigid in _rigid)
                        {
                            __rigid.isKinematic = false;
                            __rigid.velocity = velocity;
                            __rigid.transform.SetParent(null);
                        }
                    }
                    
                }
            }
        }

        Rigidbody dollRigid = varpdestination.GetComponent<Rigidbody>();
        if (dollRigid != null) dollRigid.velocity = velocity;

        foreach (Transform varchild in varpdestination)
        {
            Transform varcurrentsource = varpsource.Find(varchild.name);
            if (varcurrentsource)
                metcopytransforms(varcurrentsource, varchild, ragdoll);
        }
    }

    public void SetPhysicsActive(bool a)
    {
        if (rigid == null) rigid = GetComponent<Rigidbody>();
        rigid.isKinematic = !a;
        Collider[] colliders = GetComponents<Collider>();
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].enabled = a;
            }
        }
    }
}
