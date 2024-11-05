using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(IdentityScript))]
public class HumanAi : MonoBehaviour
{
    public enum AiType
    {
        Human,
        Fourfoot,
        Horse,
    }
    public enum CreatureState
    {
        Idle,
        Alert,
        Follow,
        Attack,
        Defend,
        Escape,
        Focus,
    }
    // Start is called before the first frame update
    bool initialized = false;
    NavMeshAgent agent;
    Rigidbody rigid;
    Animator ani;
    public Transform target;
    Collider targetCollider;
    Vector3 targetLastPos;

    public bool inBattle = true;
    public float acceleration = 9f;
    public CreatureState state;
    public AiType type;
    public Transform handhold;
    public Transform secondaryHandhold;
    public int[] toolSet;
    public int[] secondaryToolSet;

    public Vector2 size = new Vector2(0.4f, 0.4f);
 
    [HideInInspector]public ToolScript tool;
    [HideInInspector]public ToolScript secondaryTool;
    IdentityScript toolId;
    IdentityScript secondaryToolId;
    IdentityList identityList;
    [HideInInspector]public TravelerController controller;
    [HideInInspector]public HealthScript health;

    [HideInInspector] public bool inControl = false;
    [HideInInspector] public bool canEdit = true;
    [HideInInspector] public UnitTag unitTag;

    public float[] thinkTime = { 5, 4, 2.5f };
    float timeToThink = 0;

    Vector3 blendPos = Vector3.zero;
    public float maxSpeed = 2.5f;
    public float rotateSpeedMultipler = 3;
    public bool canControl = true; //设置该角色可否被possess


    float avoidAttackBugTimer = 0;

    //for ranged weapon only
    int fireCounter = 1;
    float fireRelaxTime = 0;
    bool targetInAim = true;
    bool justEndedAttack = false;

    float meleeDistance = 1;
    float rangedDistance = 5;

    //用来设置骑手之类的
    [Header("For Rider And Horse")]
    public HumanAi rider;
    public HumanAi horse;
    public Transform riderParent;
    public Transform[] spines;
    public Transform lookatPointer;
    public void Initialize()
    {
        
        if (initialized) return;
        agent = GetComponent<NavMeshAgent>();
        rigid = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
        controller = GetComponent<TravelerController>();
        if (controller != null) controller.enabled = false;
        agent.updatePosition = false;
        identityList = GameObject.FindObjectOfType<IdentityList>();
        health = GetComponent<HealthScript>();
        unitTag = GetComponentInChildren<UnitTag>();



        //骑马的tag高一点
        if (unitTag != null && horse != null)
        {
            unitTag.transform.position += Vector3.up * 0.3f;
        }

        if (rider != null)
        {
            rider.GetComponent<HealthScript>().teamIndex = health.teamIndex;
            SetRiderCollidersAsTrigger(true); //关闭骑手的碰撞器防止bug
        }
        if (horse != null)
        {
            HorsebackMovementUpdate(); //关闭刚体
        }

        if (secondaryToolSet.Length > 0) //装备副武器
        {
            EquipWeapon(secondaryToolSet[0], true);
        }
        EquipWeapon(toolSet[1]);

        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }

    public void OnHit(float hitTime = 0.5f)
    {
        if (controller != null)
        {
            if (controller.cursorHitTimer < 0) controller.cursorHitTimer = 0;
            controller.cursorHitTimer += hitTime;
            if (controller.cursorHitTimer > hitTime * 3) controller.cursorHitTimer = hitTime * 3;       
        }
    }

    // Update is called once per frame
    public void StartRangedDamage()
    {
        Vector3 offset = GetAttackBase().forward * tool.offset.z + GetAttackBase().right * tool.offset.x +
        GetAttackBase().up * tool.offset.y;

        if (tool.fireEffect != null)
            GameObject.Instantiate(tool.fireEffect, tool.fireStart.position, tool.fireStart.rotation);//生成枪口效果

        //循环生成子弹
        for (int i = 0; i < tool.bulletMultipler; i++)
        {
            DamageDetection detector =
            (GameObject.Instantiate(tool.rangedDetector,
                transform.position + GetAttackBase().forward * tool.trailDistance
                + transform.up * 1.3f + offset,
                GetAttackRotation())).GetComponent<DamageDetection>();

            detector.damage = tool.rangedDamage;
            detector.knockForce = tool.rangedKnockForce;
            detector.teamIndex = health.teamIndex;
            detector.attacker = horse == null ? health : horse.health;
            detector.canPenetrate = tool.canPenetrate;
            detector.hitEffect = tool.hitEffect;

            detector.range = tool.range;
            detector.hurtCount = tool.hurtCount;
            detector.flySpeed = tool.bulletSpeed;
            detector.transform.position = tool.fireStart.position;
            detector.transform.position += detector.transform.forward * detector.startOffset;

            if (!inControl)
            {
                Vector3 targetPos = target.position;
                if (targetCollider != null) targetPos = targetCollider.bounds.center;
                targetPos = new Vector3(targetPos.x, tool.fireStart.position.y,
                    targetPos.z);
                Vector3 targetVelocity = (target.position - targetLastPos) / Time.deltaTime;
                float estTime = Vector3.Distance(targetPos, tool.fireStart.position) / tool.bulletSpeed;

                targetPos = targetPos + estTime * targetVelocity; //计算瞄准坐标

                detector.transform.forward = targetPos - detector.transform.position;
                if (Vector3.Angle(detector.transform.forward, GetAttackBase().forward) > 40)
                {
                    detector.transform.forward = GetAttackBase().forward;
                }
            }
            else
            {
                detector.transform.forward = GetAttackBase().forward;
            }


            detector.transform.rotation = Quaternion.Euler(
                detector.transform.rotation.eulerAngles +
                Vector3.up *
                Random.Range(tool.spreadNow * -1, tool.spreadNow));

            if (!inControl)
            {
                tool.spreadNow = tool.spreadNow * tool.recoilMultipiler;
                tool.spreadNow = Mathf.Clamp(tool.spreadNow, tool.minSpreadAngle, tool.maxSpreadAngle);
            }
            else
            {
                tool.spreadNow = tool.playerSpreadAngle;
            }


            tool.roundNow -= 1;
            tool.nextFireTime = Time.time + (1 / tool.fireRate) * Random.Range(0.9f, 1.1f);

            detector.Initialize();
        }
    }
    public void StartDamageDetection()
    {
        if (tool != null)
        {
            Vector3 offset = GetAttackBase().forward * tool.offset.z + GetAttackBase().right * tool.offset.x +
                GetAttackBase().up * tool.offset.y;
            DamageDetection detector =
            (GameObject.Instantiate(tool.trailDetector,
                transform.position + GetAttackBase().forward * tool.trailDistance
                + transform.up * 1.3f + offset,
                GetAttackRotation())).GetComponent<DamageDetection>();
            detector.damage = tool.damage;
            detector.hurtCount = tool.hurtCount;
            detector.knockForce = tool.knockForce;          
            detector.teamIndex = health.teamIndex;
            detector.attacker = health;
            detector.stickOnObject = tool.stickOnObject;
            detector.transform.localScale *= (1f / 15f) * tool.trailSize;  
            if (tool.meleeEffect != null)
            {
                Instantiate(tool.meleeEffect, transform.position, transform.rotation);
            }
        }
    }

    Quaternion GetAttackRotation()
    {
        return horse != null ? spines[spines.Length - 1].rotation : transform.rotation;
    }
    Transform GetAttackBase()
    {
        return (horse != null ? spines[spines.Length - 1].transform : transform);
    }

    public void EndAttackAnimation()
    {
        ani.SetBool("Attack", false);
        justEndedAttack = true;
    }

    public bool InBattle() //如果有坐骑，取决于坐骑是否参战
    {
        if (horse != null) return horse.inBattle;
        return inBattle;
    }

    void MovementUpdate() //在动画中表现movement
    {
        ani.SetBool("Riding", false);
        rigid.isKinematic = false;
        blendPos = Vector3.MoveTowards(blendPos,
        Algori.GetRelativeVector(transform.forward, transform.right, rigid.velocity.normalized),
             acceleration * Time.fixedDeltaTime * 2.5f / maxSpeed);

        ani.SetFloat("WalkX", blendPos.x * Vector3.Project(rigid.velocity, transform.right).magnitude);
        ani.SetFloat("WalkY", blendPos.z * Vector3.Project(rigid.velocity, transform.forward).magnitude);
    }

    void HorsebackMovementUpdate()
    {
        ani.SetBool("Riding", true);
        rigid.isKinematic = true; //不再受rigidbody影响，跟着坐骑走
    }

    public Vector3 EstimateDestination(Rigidbody me, Rigidbody tar, float spead, Vector3 tarOffset, float distanceThreshold)
    {
        float dis = Vector3.Distance(me.position, tar.position + tarOffset);

        Vector3 desPos = tar.position + tarOffset;
     
        //print(gameObject.name + " " + agent.desiredVelocity.magnitude + " " + rigid.velocity.magnitude);
        if (tar != null)
        {
            float t = dis / (maxSpeed -
                Vector3.Dot(me.velocity.normalized * maxSpeed, tar.velocity));
            desPos += tar.velocity * t * 0.99f;//用于设置目的地的预判。
            if (Vector3.Distance(desPos, transform.position) < distanceThreshold) //防止目的点过于近而走不动
            {
                desPos = target.position;
            }
            //print(desPos + " " + targetRigid.velocity * t);
        }

        return desPos;
    }

    void Update()
    {
        Initialize();

        //防止animator中attack一直是true导致动画不能复原的bug
        if (ani.GetBool("Attack"))
        {
            avoidAttackBugTimer += Time.deltaTime;
            if (avoidAttackBugTimer > 1.8f)
            {
                ani.SetBool("Attack", false);
            }
        }
        else
        {
            avoidAttackBugTimer = 0;
        }

        if (horse != null)
        {
            inBattle = horse.inBattle; //骑手的参战状态取决于坐骑
        }
        if (rider != null)
        {
            rider.health.teamIndex = health.teamIndex; //骑手的id和坐骑一致
        }
        //换弹
        if (tool != null && tool.roundNow <= 0 && !ani.GetBool("Attack"))
        {
            if (tool.reloadTimer < -0.001) tool.reloadTimer = tool.reloadTime;
            tool.reloadTimer -= Time.deltaTime;
            if (tool.reloadTimer <= 0)
            {
                tool.reloadTimer = -1;
                tool.roundNow = tool.roundEachLoad;
                if (tool.reloadSound != null && inControl)
                {
                    Instantiate(tool.reloadSound, tool.transform.position, tool.transform.rotation);
                }
                fireRelaxTime = Random.Range(0.22f, 0.46f);
            }
        }

        if (horse == null)
            MovementUpdate(); //更新movement
        else
            HorsebackMovementUpdate(); //更新有坐骑时的movement

        if (horse != null) horse.inControl = inControl; //控制了骑手也要设置马的incontrol
        if (inControl)
        {
            agent.enabled = false;
            if (controller != null)
                controller.enabled = true;
            return; //如果被玩家控制，return
        }
        else
        {
            if (controller != null)
            {
                if (horse != null)
                {
                    horse.agent.enabled = true;
                }
                agent.enabled = true;
                controller.enabled = false;
            }
        }

        if (target == null && InBattle() && rider == null)//如果检测到是坐骑则不寻找target
        {
            HealthScript targetHealth = TargetList.FindEnemy(transform.position, health.teamIndex);
            SetTarget(targetHealth);
        }
   
        agent.speed = maxSpeed;
        agent.nextPosition = transform.position;
        rigid.velocity = Vector3.MoveTowards(rigid.velocity, 
            new Vector3(agent.desiredVelocity.x, rigid.velocity.y, agent.desiredVelocity.z), 
            acceleration*Time.deltaTime);

        //检测是否需要实时更新目的地以及面向以及攻击

        if (tool != null && !tool.semiAuto)
        {
            ani.SetBool("Attack", false);
        }

        Vector3 lookatPos = Vector3.zero;
        if (target != null && (state == CreatureState.Focus || state == CreatureState.Defend || state == CreatureState.Attack))
        {
            lookatPos = target.position;
            if (tool != null && tool.type != ToolScript.ToolType.Melee)
            {
                Vector3 targetVelocity = target.position - targetLastPos;
                float estTime = Vector3.Distance(target.position, transform.position) / tool.bulletSpeed;
                lookatPos = target.position + estTime * targetVelocity;
            }


            Vector3 targetDir = new Vector3(lookatPos.x, transform.position.y, lookatPos.z)
                - transform.position;

            if (horse == null) //有马则转动身体，无马转动全身
                RotateTowards(targetDir, 2);
            else
                RotateBodyTowards(targetDir, 3);
        }
        else
        {
            if (agent.desiredVelocity.magnitude > 1)
            {
                lookatPos = transform.position + agent.desiredVelocity;
            }
            else
            {
                lookatPos = transform.position + transform.forward * 1f;
            }
            Vector3 targetDir = new Vector3(lookatPos.x, transform.position.y, lookatPos.z)
                - transform.position;

            if (horse == null)
            { //有马则转动身体，无马转动全身
                RotateTowards(targetDir);
            }
            else
            {
                RotateBodyTowards(targetDir,1.5f);
            }
        }

        if (state == CreatureState.Attack && target != null)
        {
            float dis = Vector3.Distance(target.position, transform.position);
            
            Vector3 desPos = target.position;

            Rigidbody targetRigid = target.GetComponent<Rigidbody>();
            //print(gameObject.name + " " + agent.desiredVelocity.magnitude + " " + rigid.velocity.magnitude);
            if (targetRigid != null)
            {
                if (horse == null) { //步兵骑兵分开
                    //desPos = EstimateDestination(rigid, targetRigid, maxSpeed, Vector3.zero, agent.stoppingDistance);
                }
                else
                {
                    if (tool != null) //骑枪正冲，砍刀走侧边
                    {
                        if (tool.trailDistance > 1.5f)
                        {
                            desPos += (targetRigid.position - transform.position).normalized * 4;
                           // desPos = EstimateDestination(rigid, targetRigid, maxSpeed,
                           //     (targetRigid.position - transform.position).normalized*2, agent.stoppingDistance);
                        }
                        else
                        {
                            //desPos = EstimateDestination(rigid, targetRigid, maxSpeed,
                            //    rigid.transform.right*-1*tool.trailDistance, agent.stoppingDistance);
                            desPos += rigid.transform.right * -0.85f * tool.trailDistance;
                        }
                    }
                }
            }
            if (horse == null)
            {
                if (rider == null)
                { //对于攻击范围大的，自动保持距离
                    if (tool != null && Vector3.Distance(transform.position, desPos) < tool.trailDistance * 0.7f)
                    {
                        desPos += Vector3.ProjectOnPlane(transform.position - desPos, Vector3.up).normalized * (tool.trailDistance*0.9f + agent.stoppingDistance);
                        //print((Vector3.ProjectOnPlane(transform.position - desPos, Vector3.up).normalized * (tool.trailDistance + agent.stoppingDistance)).ToString() + "  " + transform.name); 
                    }
                    //print(target.transform.position + "   " + desPos);
                    agent.SetDestination(desPos); //无马且非坐骑则将敌人位置设为目的地
                }
            }
            else
            {
                //如果骑马而且是远程，despos选择逃避性的算法
                if (tool.type == ToolScript.ToolType.Firearm || tool.type == ToolScript.ToolType.Bow)
                {
                    if (target != null && tool.roundNow <= 0)
                    {
                        desPos = target.position + (transform.position - target.position).normalized * tool.rangeToAttack;
                        Vector3 biasDir = Vector3.Cross((transform.position - target.position).normalized, Vector3.up).normalized;
                        desPos += biasDir * 1.5f;
                    }
                }
                if (horse.agent.isOnNavMesh)
                {
                    horse.agent.SetDestination(desPos);//如果骑着马，则设置马的目的地。
                }
            }

            lookatPos = target.position;
            float distance = targetCollider == null ? Vector3.Distance(transform.position, target.position) :
                Vector3.Distance(targetCollider.ClosestPointOnBounds(transform.position), transform.position);

            bool rangedUnitMelee = false;
            if (tool != null && (tool.type == ToolScript.ToolType.Bow || tool.type == ToolScript.ToolType.Firearm) &&
                tool.roundNow <= 0) rangedUnitMelee = true;

            if (rangedUnitMelee)
            {
                if (distance < meleeDistance * Random.Range(1.0f, 1.3f))
                {
                    if (!justEndedAttack)
                    {
                        ani.SetBool("RightClicked", false);
                        ani.SetBool("Attack", true);
                    }
                    else
                        justEndedAttack = false;
                }
            }
            else if (tool != null && tool.type == ToolScript.ToolType.Melee)
            {
                if (distance < agent.stoppingDistance * 1.2f)
                {
                    if (!justEndedAttack)
                        ani.SetBool("Attack", true);
                    else
                        justEndedAttack = false;
                }
            }
            else if (tool != null && tool.type == ToolScript.ToolType.Firearm && (distance < tool.rangeToAttack && targetInAim &&
                tool.nextFireTime <= Time.time && fireRelaxTime <= 0) && tool.roundNow > 0)
            {
                StartRangedDamage();
                ani.SetBool("RightClicked", true);
                ani.SetBool("Attack", true);        
                fireCounter -= 1;
                if (fireCounter <= 0)
                {
                    if (tool.semiAuto)
                    {
                        fireCounter = 1;
                    }
                    else
                    {
                        fireCounter = Mathf.RoundToInt(tool.roundEachLoad - 1 *
                            tool.roundEachLoad / tool.rangeToAttack * distance);
                    }

                    if (fireCounter < 1) fireCounter = 1;
                    fireRelaxTime = Mathf.Pow(1 / tool.rangeToAttack * distance, 3);
                }
            }
            else if (tool != null && tool.type == ToolScript.ToolType.Bow && (distance < tool.rangeToAttack && targetInAim &&
                tool.nextFireTime <= Time.time && fireRelaxTime <= 0) && tool.roundNow > 0)
            {
                ani.SetBool("RightClicked", true);
                ani.SetBool("Attack", true);
                float bowDraw = ani.GetFloat("BowDraw");
                tool.fireStart.position = Vector3.Lerp(tool.stringPos[0].position,
                    tool.stringPos[1].position, bowDraw);
                if (tool.fakeArrow != null)
                    tool.fakeArrow.SetActive(true);
                if (bowDraw >= 0.99f)
                {
                    tool.fireStart.position = tool.stringPos[0].position;
                    fireRelaxTime = (1 / tool.rangeToAttack * distance);
                    ani.SetBool("Attack", false);
                    
                    if (tool.fakeArrow != null)
                        tool.fakeArrow.SetActive(false);

                    StartRangedDamage();
                }
            }
            else if (tool != null)
            {
                tool.spreadNow = Mathf.MoveTowards(tool.spreadNow, inControl ? tool.playerSpreadAngle : tool.minSpreadAngle, 
                    Mathf.Abs((tool.fireRate*tool.minSpreadAngle*(1-tool.recoilMultipiler))*0.5f*Time.deltaTime));
            }
        }
        if (state == CreatureState.Escape)
        {
            if (type == AiType.Horse && rider == null)
            { //当马的骑手死亡后，马离开战场
                HealthScript fearSource = TargetList.FindCloseEnemy(transform.position, 100, health.teamIndex);
                if (fearSource != null)
                {
                    agent.SetDestination(transform.position + (transform.position - fearSource.transform.position).normalized * 25);
                }
            }
        }
        if (state == CreatureState.Defend)
        {
            if (target != null)
            {
                lookatPos = target.position;
            }
        }

        TryThink();

        fireRelaxTime -= Time.deltaTime;

        if (target != null) targetLastPos = target.transform.position; //放在最后面
    }

    bool CheckMovable() //临时使用
    {
        return true;
    }
    public void Movement(Vector3 moveDirection)
    {
        if (horse != null) //如果有马则传递给马
        {
            horse.Movement(moveDirection);
            return;
        }

        float realMaxspeed = maxSpeed; //用于实现前进速度大于后退速度
        realMaxspeed += (Vector3.Dot(moveDirection.normalized, transform.forward.normalized) - 1f)*0.11f*maxSpeed;

        float y = rigid.velocity.y;
        rigid.velocity = Vector3.MoveTowards(rigid.velocity,
        moveDirection.magnitude > 0.0001 ? moveDirection.normalized * realMaxspeed :
        Vector3.zero, acceleration*1.3f * Time.fixedDeltaTime);

        rigid.velocity = new Vector3(rigid.velocity.x,
            y, rigid.velocity.z);

        blendPos = Vector3.MoveTowards(blendPos,
            Algori.GetRelativeVector(transform.forward, transform.right, moveDirection.normalized),
            acceleration * Time.fixedDeltaTime / realMaxspeed);
        ani.SetFloat("WalkX", blendPos.x);
        ani.SetFloat("WalkY", blendPos.z);

        if (type == HumanAi.AiType.Horse) //马需要自动转向
        {
            RotateTowards(moveDirection, 1);
        }

        Vector3 velocityH = Vector3.Project(rigid.velocity,
                transform.right);
        Vector3 velocityV = Vector3.Project(rigid.velocity,
               transform.forward);
    }

    public void SetRiderCollidersAsTrigger(bool toSet)
    {
        if (rider != null)
        {
            Collider[] colli = rider.GetComponents<Collider>();
            for (int i = 0; i < colli.Length; i++)
            {
                colli[i].isTrigger = toSet;
            }
        }
    }

    void SetTarget(HealthScript health)
    {
        if (health != null)
        {
            target = health.transform;
            targetCollider = health.mainCollider;
            targetLastPos = target.position;
        }
    }

    public void TryThink()
    {
        if (rider != null) return; //是坐骑则跳过思考

        if (Time.time < timeToThink) return;

        if (InBattle())
        {
            float radius = 2.5f;
            if (tool != null)
            {
                if (tool.type == ToolScript.ToolType.Melee)
                {
                    radius = Mathf.Max(radius, tool.trailDistance * 1.5f);
                }
                else if (tool.type == ToolScript.ToolType.Bow || tool.type == ToolScript.ToolType.Firearm)
                {
                    radius = Mathf.Max(radius, tool.rangeToAttack * 0.75f);
                }
            }
            HealthScript newTarget = TargetList.FindCloseEnemy(transform.position, radius, health.teamIndex);
            SetTarget(newTarget);
        }

        float m = Random.Range(0.7f, 1.3f);
        if (state == CreatureState.Attack || state == CreatureState.Escape || state == CreatureState.Defend) timeToThink = Time.time + thinkTime[2] * m;
        else if (state == CreatureState.Alert) timeToThink = Time.time + thinkTime[1] * m;
        else timeToThink = Time.time + thinkTime[0] * m;

        if (state == CreatureState.Attack && tool != null)
        {
            //副手持盾或弓盾时自动举盾
            if ((secondaryTool!=null && secondaryTool.type == ToolScript.ToolType.Shield) 
                || (tool != null && tool.type == ToolScript.ToolType.Shield) || tool.isShieldBow)
            {
                ani.SetBool("RightClicked", true);
            }
            //持枪或持弓自动瞄准
            if (tool != null && 
                (tool.type == ToolScript.ToolType.Bow || tool.type == ToolScript.ToolType.Firearm))
            {
                ani.SetBool("RightClicked", true);
            }

            EquipWeapon(toolSet[2]);
            float p = Random.Range(0.00f, 1.00f);
            if (p < 1f)
            {
                if (tool != null)
                {
                    float toolDistance = tool.trailDistance;
                    rangedDistance = tool.rangeToAttack;
                    meleeDistance = tool.trailDistance;
                    if (tool != null && (tool.type == ToolScript.ToolType.Firearm ||
                        tool.type == ToolScript.ToolType.Bow))
                    {
                        toolDistance = tool.rangeToAttack * Random.Range(0.5f, 0.66f);
                    }
                    agent.stoppingDistance = toolDistance * Random.Range(0.9f, 1.2f);
                }
            }else 
            {
                //一定几率攻转防
                state = CreatureState.Defend;
                timeToThink = Time.time;
                TryThink();
                return;
            }
        }

        if (state == CreatureState.Defend)
        {
            EquipWeapon(toolSet[2]);
            float p = Random.Range(0.00f, 1.00f);
            if (p < 0.4f)
            {
                float toolDistance = tool != null ? tool.trailDistance : 1f;
                if (tool != null && (tool.type == ToolScript.ToolType.Firearm || 
                   tool.type == ToolScript.ToolType.Bow))
                {
                    toolDistance = tool.rangeToAttack * Random.Range(0.5f, 0.66f);
                }

                agent.SetDestination(target.position + (transform.position - target.position).normalized 
                    * toolDistance * Random.Range(1.0f, 1.7f)
                    + transform.right * Random.Range (-1.0f, 1.0f) * Mathf.Sqrt(toolDistance));
            }
            else
            {
                //一定几率防转攻
                state = CreatureState.Attack;
                timeToThink = Time.time;
                TryThink();
                return;
            }
        }

        if (state == CreatureState.Idle || state == CreatureState.Focus)
        {

        }

        if (tool != null && type == AiType.Human) ani.SetFloat("WeaponType", Mathf.Round(tool.animationIndex));
    }

    void LateUpdate()
    {
        if (horse != null) //如果有马，则转动身体
        {
            Vector3 topSpineEuler = spines[spines.Length - 1].rotation.eulerAngles;

            topSpineEuler = new Vector3(0, topSpineEuler.y, 0);
            float a = lookatPointer.rotation.eulerAngles.y;
            if (a > 180) a = a - 360;
            float b = topSpineEuler.y;
            if (b > 180) b = b - 360; //用a和b把角度转为0->180、-180->0制


            float deltaAngle = a - b;
            float deltaA = a - b;
            if (deltaA < -180) deltaA += 360;
            float deltaB = a - 180 + (-180) - b;
            if (deltaB < -180) deltaB += 360;


            if (Mathf.Abs(deltaA) < Mathf.Abs(deltaB))
            {
                deltaAngle = deltaA;
            }
            else
            {
                deltaAngle = deltaB;
            }

            //print(gameObject.name + " : " + lookatPointer.rotation.eulerAngles.y + " " + topSpineEuler.y + " " + deltaAngle);
            for (int i = 0; i < spines.Length; i++)
            {
                spines[i].Rotate((Vector3.up * deltaAngle / spines.Length), Space.World);
            }
        }
    }
    public void RotateBodyTowards (Vector3 targetDir, float speed = 1) //用于转动脊椎骨骼

    {
        speed *= rotateSpeedMultipler;

        float angle = Vector3.Angle(targetDir, lookatPointer.forward); //用lookatPointer做渐变式的转动，并用于设置脊椎转动

        if (tool != null && tool.type != ToolScript.ToolType.Melee)
        {
            if (angle <= Mathf.Max(tool.minSpreadAngle, 5)) targetInAim = true;
            else targetInAim = false;
        }

        if (Vector3.Dot(targetDir, lookatPointer.right) > 0)
        {
            if (angle * speed * Time.deltaTime < angle)
                lookatPointer.Rotate(0, angle * speed * Time.deltaTime, 0);
            else
                lookatPointer.Rotate(0, angle, 0);
        }
        else
        {
            if (angle * speed * Time.deltaTime < angle)
                lookatPointer.Rotate(0, angle * -1 * speed * Time.deltaTime, 0);
            else
                lookatPointer.Rotate(0, -1 * angle, 0);
        }
        //之后在lateupdate中调用lookatpointer的方向
    }
    public void RotateTowards(Vector3 targetDir, float speed = 1)
    {
        speed *= rotateSpeedMultipler;
        float angle = Vector3.Angle(targetDir, transform.forward);

        if (tool!= null && tool.type != ToolScript.ToolType.Melee)
        {
            if (angle <= Mathf.Max(tool.minSpreadAngle, 5)) targetInAim = true;
            else targetInAim = false;
        }

        if (Vector3.Dot(targetDir, transform.right) > 0)
        {
            if (angle * speed * Time.deltaTime < angle)
                transform.Rotate(0, angle * speed * Time.deltaTime, 0);
            else
                transform.Rotate(0, angle * speed, 0);
        }
        else
        {
            if (angle * 3 * Time.deltaTime < angle)
                transform.Rotate(0, angle * -1*speed * Time.deltaTime, 0);
            else
                transform.Rotate(0, -1* speed * angle, 0);
        }
    }

    public void GetInBattle()
    {
        inBattle = true;

        if (unitTag != null)
        {
            unitTag.Close();
        }

        if (horse != null)
        {
            horse.inBattle = true; //别忘了坐骑
        }

        if (health == null) health = GetComponent<HealthScript>();
        health.neverAsTarget = false;
        health.AddToTargetList();
    }
    public void EquipWeapon(int id, bool isSecondary = false)
    {
        if (id < 0) return;

        IdentityScript idToCompare;
        Transform targetTransform;

        if (!isSecondary) idToCompare = toolId;
        else idToCompare = secondaryToolId;

        if ((idToCompare == null || id != idToCompare.id))
        {
            GameObject go =
            GameObject.Instantiate(identityList.list[id], transform);
            ToolScript goTool = go.GetComponentInChildren<ToolScript>();

            HealthScript shieldHealth = go.GetComponentInChildren<HealthScript>();
            if (shieldHealth != null)
            {
                shieldHealth.parent = health;
            }

            if (goTool.hand == ToolScript.Hand.Default)
            {
                targetTransform = isSecondary ? secondaryHandhold : handhold;
            }
            else
            {
                targetTransform = goTool.hand == ToolScript.Hand.Left ? secondaryHandhold : handhold;
            }

            go.transform.SetParent(targetTransform);
            go.transform.localPosition = Vector3.zero;
            go.transform.rotation = targetTransform.rotation;
            Rigidbody goRigid = go.GetComponent<Rigidbody>();
            Collider[] goCollider = go.GetComponents<Collider>();
            if (goRigid != null)
            {
                goRigid.isKinematic = true;
                for (int i = 0; i < goCollider.Length; i++)
                {
                    goCollider[i].enabled = false;
                }  
            }

            HealthScript toolHealth = go.GetComponentInChildren<HealthScript>();
            if (toolHealth != null)
            {
                toolHealth.teamIndex = health.teamIndex;
                toolHealth.owner = health;
            }

            if (idToCompare != null) Destroy(idToCompare.gameObject);
            idToCompare = targetTransform.GetComponentInChildren<IdentityScript>();

            if (!isSecondary)
            {
                tool = targetTransform.GetComponentInChildren<ToolScript>();
                tool.owner = health;
                toolId = go.GetComponent<IdentityScript>();
            }
            else
            {
                secondaryTool = targetTransform.GetComponentInChildren<ToolScript>();
                secondaryTool.owner = health;
                secondaryToolId = go.GetComponent<IdentityScript>();
            }


        }
    }
}
