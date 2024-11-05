using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffIndicatorManager : MonoBehaviour
{
    public BuffIndicator[] indicators;
    public int[] winCount;

    bool initialized = false;
    TxtReader txt;
    Animator ani;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (initialized) return;
        initialized = true;

        txt = GetComponent<TxtReader>();
        ani = GetComponent<Animator>();
        txt.Read(Application.streamingAssetsPath, "Save.txt", ';');
        UpdateBuff();
    }

    void UpdateBuff()
    {
        int winNow = txt.getInt(0, 2);

        bool foundEnd = false;
        for (int i = 0; i < indicators.Length; i++)
        {
            if (!foundEnd)
            {  //还没有到解锁的结尾
                if (winNow >= winCount[i])
                {//解锁了
                    indicators[i].unlockedText.enabled = true;
                    indicators[i].lockedText.enabled = false;
                    indicators[i].dotImage.enabled = true;
                    indicators[i].lockImage.enabled = false;
                    indicators[i].progress.enabled = false;

                }
                else
                {//没解锁
                    indicators[i].unlockedText.enabled = false;
                    indicators[i].lockedText.enabled = true;
                    indicators[i].dotImage.enabled = false;
                    indicators[i].lockImage.enabled = true;
                    indicators[i].progress.enabled = true;
                    indicators[i].progress.text = winNow.ToString() + "/" + winCount[i].ToString();
                    foundEnd = true;
                }
            }
            else
            {
                indicators[i].unlockedText.enabled = false;
                indicators[i].lockedText.enabled = false;
                indicators[i].dotImage.enabled = false;
                indicators[i].lockImage.enabled = false;
                indicators[i].progress.enabled = false;
            }

            //9999为不启用的标记
            if (winCount[i] > 9999)
            {
                indicators[i].unlockedText.enabled = false;
                indicators[i].lockedText.enabled = false;
                indicators[i].dotImage.enabled = false;
                indicators[i].lockImage.enabled = false;
                indicators[i].progress.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
    }

    public void PlayApplyAnimation()
    {
        if (ani != null)
        {
            //print("apply");
            ani.SetTrigger("Apply");
        }
    }

    public void PlayShowAnimation()
    {

        //print("show");
        ani.ResetTrigger("Disable");
        ani.SetTrigger("Show");
    }

    public void PlayCloseAnimation()
    {
        //print("disable");
        ani.SetTrigger("Disable");
    }

    public void ApplyBuff(HumanAi ai)
    {
        int level = 0;
        int winNow = txt.getInt(0, 2);
        for (int i = 0; i < indicators.Length; i++)
        {
            if (winNow >= winCount[i])
            {
                level = i;
            }
            else
            {
                break;
            }
        }

        print("apply buff, level = " + level.ToString());
        if (level >= 0)
        { //减伤
            if (ai.health == null) ai.health = ai.GetComponent<HealthScript>();
            ai.health.maxHealth *= 2.5f;
            ai.health.health *= 2.5f;
        }
        if (level >= 1)
        { //近战攻击
            if (ai.tool != null)
            {
                ai.tool.damage = ai.tool.damage + ai.tool.damage / 2;
                ai.tool.knockForce *= 1.23f;
                ai.tool.hurtCount += 1;
            }
        }
        if (level >= 2)
        { //远程攻击
            ai.tool.rangedDamage = ai.tool.rangedDamage + ai.tool.rangedDamage / 2;
            ai.tool.rangedKnockForce *= 1.23f;
        }
        if (level >= 3)
        {//加速
            ai.maxSpeed *= 1.2f;
            ai.acceleration *= 1.5f;
        }
        if (level >= 4)
        {
            //马加速
            if (ai.horse != null)
            {
                print("horse buffed");
                //ai.horse.maxSpeed *= 1.15f;
                ai.horse.acceleration *= 2.3f;
            }
        }
        if (level >= 5)
        {//装填
            print(ai.tool);
            if (ai.tool != null)
            {

                ai.tool.reloadTime /= 2;
                
            }
        }
        if (level >= 6)
        {
            //精准
            if (ai.tool != null && ai.tool.playerSpreadAngle < 5)
            {
                ai.tool.playerSpreadAngle *= 0.1f;
            }
        }


    }

    public void RemoveBuff(HumanAi ai)
    {
        int level = 0;
        int winNow = txt.getInt(0, 2);
        for (int i = 0; i < indicators.Length; i++)
        {
            if (winNow >= winCount[i])
            {
                level = i;
            }
            else
            {
                break;
            }
        }

        print("remove buff, level = " + level.ToString());

        if (level >= 0)
        { //减伤
            if (ai.health == null) ai.health = ai.GetComponent<HealthScript>();
            ai.health.maxHealth = Mathf.RoundToInt(ai.health.maxHealth / 2.5f);
            ai.health.health = Mathf.RoundToInt(ai.health.health / 2.5f);
        }
        if (level >= 1)
        { //近战攻击
            if (ai.tool != null)
            {
                ai.tool.damage = Mathf.RoundToInt(ai.tool.damage / 1.5f);
                ai.tool.knockForce /= 1.23f;
                ai.tool.hurtCount -= 1;
            }
        }
        if (level >= 2)
        { //远程攻击
            ai.tool.rangedDamage = Mathf.RoundToInt(ai.tool.rangedDamage / 1.5f);
            ai.tool.rangedKnockForce /= 1.23f;
        }
        if (level >= 3)
        {//步行加速
            ai.maxSpeed /= 1.2f;
            ai.acceleration /= 1.5f;
        }
        if (level >= 4)
        {
            //马加速
            if (ai.horse != null)
            {
                //ai.horse.maxSpeed /= 1.15f;
                ai.horse.acceleration /= 2.3f;
            }
        }
        if (level >= 5)
        {//装填
            if (ai.tool != null)
            {
                ai.tool.reloadTime *= 2;
            }
        }
        if (level >= 6)
        {
            //精准
            if (ai.tool != null && ai.tool.playerSpreadAngle < 5) //霰弹不适用
            {
                ai.tool.playerSpreadAngle /= 0.1f;
            }
        }
    }
}
