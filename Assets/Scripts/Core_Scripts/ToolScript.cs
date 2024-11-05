using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(IdentityScript))]
public class ToolScript : MonoBehaviour
{
    public enum ToolType
    {
        Melee,
        Bow,
        Firearm,
        Shield,
    }
    public enum Hand
    {
        Default,
        Left,
        Right,
    }
    // Start is called before the first frame update
    bool initialized = false;
    public ToolType type = ToolType.Melee;
    public Hand hand = Hand.Default;
    public int damage;
    public GameObject trailDetector;
    public Vector3 offset;
    public float knockForce = 100;
    public int animationIndex = 0;
    public int hurtCount = 2; //能造成多少次伤害

    [Header("Melee Values")]
    public float trailSize = 1.5f;
    public float trailDistance = 1;
    public GameObject meleeEffect;
    [Header("General Ranged Values")]
    public int bulletMultipler = 1;
    public GameObject rangedDetector;
    public int rangedDamage;
    public float rangedKnockForce = 200;
    public float fireRate = 3;
    public int roundEachLoad = 10;
    public float reloadTime = 2;
    public bool canPenetrate = false;
    public bool stickOnObject = false;
    [HideInInspector] public int roundNow;
    public float bulletSpeed = 30;
    public float range = 20;
    public float rangeToAttack = 16;
    public Transform fireStart;
    public GameObject fireEffect;
    public GameObject hitEffect;
    public GameObject reloadSound;
    [Header("Firearm Values")]   
    public bool semiAuto = false;
    public float minSpreadAngle = 1;
    public float maxSpreadAngle = 2;
    public float playerSpreadAngle = 0;
    [HideInInspector]public float spreadNow;
    public float recoilMultipiler = 1.2f;
    [Header("Bow Values")]
    public Transform[] stringPos;
    public GameObject fakeArrow;
    public bool isShieldBow = false;

    [Header("Shield Values")]
    public float shieldRangedDefense = 0.4f;
    public float shieldMeleeDefense = 1.0f;


    [HideInInspector] public HealthScript owner;
    [HideInInspector]public float nextFireTime;
    [HideInInspector] public float reloadTimer = -1;
    

    void Initialize()
    {        
        if (initialized) return;
        spreadNow = minSpreadAngle;
        roundNow = roundEachLoad;
        nextFireTime = Time.time + Random.Range(1.0f, 2.0f);
        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        
    }
}
