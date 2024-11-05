using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DamageType
{
    pierce,
    cut,
    blunt,
}
public class DamageDetection : MonoBehaviour
{
    List<HealthScript> detected = new List<HealthScript>();
    public DamageType type;
    public int damage;
    public float knockForce;
    public float availableTime = 0.2f;
    public bool canPenetrate = false;
    public int teamIndex = -1;
    public HealthScript attacker;
    public bool stickOnObject = false;

    public GameObject hitEffect;

    float time = 0;
    [HideInInspector] public bool autoFly = false;
    [HideInInspector] public float flySpeed = 0;
    [HideInInspector] public float range = 20;
    [HideInInspector] public Vector3 startPoint;
    [HideInInspector] public int hurtCount = 1; //可以一次性对多少人造成伤害
    public float startOffset = 0;
    Vector3 endPoint;
    HealthScript targetHealth;
    ToolScript targetTool;
    Transform targetRagdoll = null;
    // Start is called before the first frame update
    bool initialized = false;

    bool used = false;
    bool disable = false;

    public GameObject hitFeedback;
    public GameObject killFeedback;
    public void Initialize()
    {
        
        if (initialized) return;
        startPoint = transform.position;
        endPoint = startPoint + range * new Vector3(transform.forward.x, 0, transform.forward.z);
        transform.forward = endPoint - startPoint;

        //gameObject.AddComponent<AutoDestroy>();
        //gameObject.GetComponent<AutoDestroy>().timeToDestruct = 10f;
        initialized = true;
    }

    void Start()
    {
        Initialize();       
    }

    bool SelfHit(HealthScript h)
    {
        if (attacker == null) return false;

        if (attacker == h) 
        {
            return true;
        }


        if (attacker.ai != null && attacker.ai.horse != null && attacker.ai.horse == h.ai)
        {
    
            return true;
        }

        return false;
    }

    // Update is called once per frame

    void StickWall(Transform wall)
    {

        flySpeed = 0;
        if (wall != null) transform.SetParent(wall);
        if (wall != null && wall.tag == "Ragdoll")
        {
            //transform.position = wall.transform.position;
            transform.position += Random.Range(-0.4f, -0.2f) * transform.forward;
            transform.position += Random.Range(-0.05f, -0.05f) * transform.up;
        }
        else
        {
            transform.position = endPoint;
            transform.position += Random.Range(-0.7f, -0.58f) * transform.forward;
            transform.position += Random.Range(-0.3f, -0f) * transform.up;
        }
        //print(wall);
        Destroy(this);
    }

    void RayTest()
    {
        targetHealth = null;
        targetTool = null;
        endPoint = startPoint + transform.forward * range;

        Ray rayLow = new Ray();
        rayLow.origin = transform.position - Vector3.up;
        rayLow.direction = endPoint - startPoint;

        RaycastHit hit;
        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = endPoint - startPoint;



        //print(transform.position);
        //print(rayLow.origin);

        float rayRange = Vector3.Distance(startPoint + transform.forward * range,
            transform.position);

        /*
        print(ray.origin);
        print(ray.direction);
        print(rayRange);*/
        //hitRagdoll = false;
        if (Physics.Raycast(ray, out hit, rayRange))
        {
            targetHealth = hit.transform.GetComponent<HealthScript>();

            targetTool = hit.transform.GetComponent<ToolScript>();

            targetRagdoll = hit.transform;
            //print(hit.transform.name);
            if (targetTool != null && targetTool.owner != null && targetTool.owner.teamIndex == attacker.teamIndex) targetTool = null; //不会击中队友的盾牌

            if (targetHealth == null && hit.transform.parent != null)
            {
                targetHealth = hit.transform.parent.GetComponent<HealthScript>();
            }

            if (targetHealth == null)
            {
                endPoint = hit.point;
                if (hit.transform.tag == "Ragdoll")
                {
                    //hitRagdoll = true;
                }
            }
            else if (!SelfHit(targetHealth))
            {
                //print(hit.transform.name + "ok");
                //print(hit.transform.name + "   **");
                endPoint = hit.point;
                //print(endPoint);
            }



        }

        /*
        if (Physics.Raycast(rayLow, out hit, rayRange)) //低位检测只适用于生物
        {
            targetHealth = hit.transform.GetComponent<HealthScript>();
            targetTool = hit.transform.GetComponent<ToolScript>(); //排除盾牌的可能性

            if (targetTool != null && targetTool.owner.teamIndex == attacker.teamIndex) targetTool = null; //不会击中队友的盾牌

            if (targetHealth == null && hit.transform.parent != null)
            {
                targetHealth = hit.transform.parent.GetComponent<HealthScript>();
            }

            if (targetHealth != null && !SelfHit(targetHealth))//如果发现检测到的是自己，取消
            {
                endPoint = hit.point;
            }
            else
            {
                targetHealth = null;
                endPoint = startPoint + transform.forward * range;
            }
        }*/

    }
    void Update()
    {
       // Time.timeScale = 0.07f;
        Initialize();
        time += Time.deltaTime;

        if (Vector3.Distance(startPoint, endPoint) <=
            Vector3.Distance(startPoint, transform.position))
        {
            //if (!(targetHealth != null && targetHealth.owner == attacker))
            //{
            transform.position = endPoint;

            if (canPenetrate && targetTool != null && targetTool.shieldRangedDefense < 0.99f && (targetTool.type == ToolScript.ToolType.Shield || targetTool.isShieldBow))//可穿透并且击中了盾牌？
            {
                for (int i = 0; i < 100; i++)
                {
                    transform.position += transform.forward * 0.01f; //一点点的穿过盾牌，直到不能raycast到盾牌
                    RaycastHit shieldHit;
                    Ray shieldRay = new Ray();
                    shieldRay.origin = transform.position;
                    shieldRay.direction = transform.forward;
                   

                    float shieldRayRange = Vector3.Distance(startPoint + transform.forward * range,
                        transform.position);

                    if (Physics.Raycast(shieldRay, out shieldHit, shieldRayRange))
                    {
                        //print(shieldHit.transform.name);
                        ToolScript shieldAgain = shieldHit.transform.GetComponent<ToolScript>();
                        //print(shieldHit.transform.name);
                        //print(shieldAgain);
                        if (shieldAgain != null && shieldAgain == targetTool) //还能检测到原来的盾牌，说明偏移不够
                        {
                            continue;
                        }
                        else
                        {//说明已经穿过盾牌
                            endPoint = startPoint + transform.forward * range;
                            damage = Mathf.RoundToInt((float)damage *(1.0f - targetTool.shieldRangedDefense));
                            targetTool = null;

                            /*print(shieldHit.transform.name + "!!!!!!!!!!!!!!!!");
                            print(transform.position);
                            print(shieldRay.origin);
                            print(shieldRay.direction);
                            print(shieldRayRange);*/
                            if (targetTool != null && targetTool.GetComponentInChildren<HealthScript>() != null)
                            ApplyDamage(targetTool.GetComponentInChildren<HealthScript>());
                            RayTest();
                            break;
                        }
                    }
                }
            }else if (!canPenetrate && targetTool != null && stickOnObject){
                //说明箭插盾牌上
                if (Mathf.Abs(Vector3.Distance(transform.position, startPoint) - range) > 0.5f)
                {
                    StickWall(targetTool.transform);
                }
                else
                {
                    Destroy(gameObject);
                }
                return;
            }

            if (targetHealth == null && stickOnObject) //击中了墙
            {
                //print(Vector3.Distance(transform.position, startPoint).ToString() + "  " + range.ToString());
                if (Mathf.Abs(Vector3.Distance(transform.position, startPoint) - range) > 0.5f)
                {
                    if (targetRagdoll != null)
                        StickWall(targetRagdoll);
                    else
                        Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
                return;
            }

            if (targetHealth != null && targetHealth != attacker)
            {
                ApplyDamage(targetHealth);
            }
            if (hitEffect != null)
            {

                GameObject g = Instantiate(hitEffect, transform.position, transform.rotation);
                g.transform.forward = transform.forward * -1;
            }

            //保留轨迹
            TrailRenderer tr = GetComponentInChildren<TrailRenderer>();
            if (tr != null) tr.transform.SetParent(null);
            Destroy(gameObject);
            //}
        }

        RayTest();
        
        transform.position += transform.forward * flySpeed * Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (used) disable = true;
    }

    /*private void OnCollisionEnter(Collision colli)
    {
        if (time > availableTime) return;

        if (disable) return;
        used = true;
        HealthScript health = colli.transform.GetComponent<HealthScript>();
        if (health == null && colli.transform.parent != null)
        {
            health = colli.transform.parent.GetComponent<HealthScript>();
        }
        if (health != null && health != attacker)
        {
            float m = health.teamIndex != teamIndex ? 1.0f : 0.2f;

            if (type == DamageType.blunt)
                health.ApplyDamage(damage * (1.0f - health.bluntResis) * m);
            if (type == DamageType.cut)
                health.ApplyDamage(damage * (1.0f - health.cutResis) * m);
            if (type == DamageType.pierce)
                health.ApplyDamage(damage * (1.0f - health.pierceResis) * m);
            health.dieForce = health.transform.position - (transform.position - transform.forward);
            health.dieForce = new Vector3(health.dieForce.x, 0, health.dieForce.z);
            health.dieForce = health.dieForce.normalized * knockForce;
        }
    }*/

    void ApplyDamage(HealthScript health)


    {
        if (SelfHit(health))
        {
            return;
        }

        if (health.parent != null && health.parent == attacker) return; //防止击中自己的盾牌

        if (hurtCount <= 0) return;//如果不再能伤害

        if (detected.Count > 0)
        {
            foreach (HealthScript detectedHealth in detected)
            {
                if (detectedHealth == health) return;
            }
        }
        float m = health.teamIndex != teamIndex ? 1.0f : 0.2f;

        if (type == DamageType.blunt)
            health.ApplyDamage(damage * (1.0f - health.bluntResis) * m);
        if (type == DamageType.cut)
            health.ApplyDamage(damage * (1.0f - health.cutResis) * m);
        if (type == DamageType.pierce)
            health.ApplyDamage(damage * (1.0f - health.pierceResis) * m);
        if (attacker != null)
        {
            health.dieForce = health.transform.position - attacker.transform.position;
        }
        else
        {
            health.dieForce = health.transform.position - (transform.position - transform.forward*1.5f);
        }
        
        health.dieForce = new Vector3(health.dieForce.x, 0, health.dieForce.z);
        health.dieForce = health.dieForce.normalized * knockForce;
        detected.Add(health);

        if (!health.isShield)
        {
            hurtCount -= 1; //减少一次可伤害次数
        }

        if (attacker != null)
        {
            HumanAi attackerAi = attacker.gameObject.GetComponent<HumanAi>();
            if (attackerAi != null)
            {
                attackerAi.OnHit(0.4f);

                if (attackerAi.inControl && attackerAi.health != null)
                {
                    if (health.health > 0 && hitFeedback != null)
                    {
                        Instantiate(hitFeedback, transform.position, transform.rotation);
                    }
                    else if (killFeedback != null)
                    {
                        Instantiate(killFeedback, transform.position, transform.rotation);
                    }
                    
                }
            }
        }
    }
    void OnTriggerEnter(Collider colli)
    {
        

        if (time > availableTime) return;

        if (disable) return;

        used = true;
        HealthScript health = colli.GetComponent<HealthScript>();
        if (health == null && colli.transform.parent != null)
        {
            health = colli.transform.parent.GetComponent<HealthScript>();
        }
        if (attacker != null && health != null && health != attacker) //可能可以造成伤害
        {

            if (attacker.ai != null && attacker.ai.horse != null && attacker.ai.horse == health)
            {//防止击中自己的马
                return;
            }

            Ray ray = new Ray();
            ray.origin = attacker.transform.position + Vector3.up*1.3f;
            ray.direction = colli.bounds.center - ray.origin;
            //print(ray.origin);
            //print(ray.direction);
            

            RaycastHit[] hits = Physics.RaycastAll(ray);
            float closestShieldDistance = int.MaxValue - 100;
            ShieldTrigger closestShield = null;
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {             
                    ShieldTrigger shield = hits[i].transform.GetComponentInChildren<ShieldTrigger>();

                    if (shield == null) continue;

                    if (shield.transform != hits[i].transform && (shield.transform.parent != null && shield.transform.parent != hits[i].transform))
                    {//防止击中人却从手中获取shield component
                        continue;
                    }

                    if (shield != null && shield.health.teamIndex != teamIndex &&
                        Vector3.Distance(attacker.transform.position, shield.transform.position) < closestShieldDistance)
                    {
                        //print("2");
                        closestShield = shield;
                        closestShieldDistance =
                            Vector3.Distance(attacker.transform.position, new Vector3(shield.transform.position.x, 
                            attacker.transform.position.y, shield.transform.position.z));
                    }
                }
            }

            float healthDistance = Vector3.Distance(attacker.transform.position, new Vector3(health.transform.position.x, attacker.transform.position.y, health.transform.position.z));
            //消除y轴的影响


            if (closestShield != null && closestShieldDistance < healthDistance) //检测是否触盾
            {
                //ApplyDamage(closestShield.health);
                damage = Mathf.RoundToInt(damage*closestShield.damageMultipiler);
            }

            ApplyDamage(health);

        }
    }
}
