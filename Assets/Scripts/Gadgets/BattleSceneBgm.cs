using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSceneBgm : MonoBehaviour
{
    // Start is called before the first frame update
    public BgmVolume idleSource;
    public BgmVolume fightSource;
    public BgmVolume winSource;
    public BgmVolume loseSource;
    public BgmVolume drumstickSource;
    public BgmVolume victorySource;
    public BgmVolume failureSource;
    public BgmVolume closureSource;
    

    public float idleVolume = 0.5f;
    public float fightVolume = 0.9f;
    public float winVolume = 1.1f;
    public float drumVolume = 1.0f;
    public float victoryVolume = 1.1f;

    
    bool initialized = false;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        
    }

    void Initialize()
    {
        if (initialized) return;
        initialized = true;

        idleSource.basicVolume = idleVolume;
        fightSource.basicVolume = 0;
        winSource.basicVolume = winVolume;
        loseSource.basicVolume = winVolume;
        victorySource.basicVolume = victoryVolume;
        failureSource.basicVolume = victoryVolume;
        closureSource.basicVolume = victoryVolume;
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();

        if (RoundManager.gameIndex == 4 || RoundManager.gameIndex == 8 || RoundManager.gameIndex == 12 || RoundManager.gameIndex == 16)
        {//这说明在战斗状态
            idleSource.basicVolume = Mathf.MoveTowards(idleSource.basicVolume, idleVolume * 0.2f, Time.deltaTime);
            if (fightSource.basicVolume < 0.01f)//刚进入战斗，音乐从头播放
            {
                fightSource.GetComponent<AudioSource>().Play();
                fightSource.basicVolume = 0.011f;//确保play单次触发
            }

            fightSource.basicVolume = Mathf.MoveTowards(fightSource.basicVolume, fightVolume, Time.deltaTime * 2);
            
        }
        else
        {//说明idle了
            idleSource.basicVolume = Mathf.MoveTowards(idleSource.basicVolume, idleVolume, Time.deltaTime);
            fightSource.basicVolume = Mathf.MoveTowards(fightSource.basicVolume, 0, Time.deltaTime * 3);
        }
    }

    public void PlayDrum()
    {
        drumstickSource.GetComponent<AudioSource>().Play();
    }

    public void PlayWin()
    {
        winSource.GetComponent<AudioSource>().Play();
    }

    public void PlayLose()
    {
        loseSource.GetComponent<AudioSource>().Play();
    }

    public void PlayVictory()
    {
        victorySource.GetComponent<AudioSource>().Play();
    }

    public void PlayFailure()
    {
        failureSource.GetComponent<AudioSource>().Play();
    }

    public void PlayClosure()
    {
        closureSource.GetComponent<AudioSource>().Play();
    }
}
