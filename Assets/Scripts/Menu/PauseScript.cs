using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class PauseScript : MonoBehaviour
{
    public GameObject initialPage;
    public GameObject[] bg;
    public static bool paused = false;
    float originTimeScale = 1;

    public static int mastervolume;
    public static int soundfx;
    public static int voice;
    public static int bgm;

    public ScriptableRendererFeature ssaoRenderer;

    public GameObject[] UnpausableAt;

    Resolution[] availableRes;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();


    }

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        paused = false;

        if (PlayerPrefs.GetInt("DefaultSet") != 1)
        {
            PlayerPrefs.SetInt("DefaultSet", 1);
            PlayerPrefs.SetInt("fullscreen", 1);
            PlayerPrefs.SetInt("vsync", 0);
            PlayerPrefs.SetInt("resolution", 0);
            PlayerPrefs.SetInt("ssao", 1);
            PlayerPrefs.SetInt("mastervolume", 100);
            PlayerPrefs.SetInt("soundfx", 100);
            PlayerPrefs.SetInt("voice", 100);
            PlayerPrefs.SetInt("bgm", 100);
        }

        for (int i = 0; i < bg.Length; i++)
        {
            bg[i].SetActive(false);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        UpdateToggle("vsync");
        UpdateToggle("ssao");
        UpdateToggle("mastervolume");
        UpdateToggle("soundfx");
        UpdateToggle("voice");
        UpdateToggle("bgm");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused) Continue();
            else Pause();
        }

        if (paused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    void PlaySound()
    {
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null) audio.Play();
    }

    public void Pause()
    {
        for (int i = 0; i < UnpausableAt.Length; i++)
        {
            //Unpausable中只要有一个active就不可暂停
            if (UnpausableAt[i].activeSelf)
            {
                return;
            }
        }

        paused = true;
        originTimeScale = Time.timeScale;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (initialPage == transform.GetChild(i).gameObject)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < bg.Length; i++)
        {
            bg[i].SetActive(true);
        }
        PlaySound();
        Time.timeScale = 0;
    }

    public void Continue()
    {
        paused = false;
        for (int i = 0; i < bg.Length; i++)
        {
            bg[i].SetActive(false);
        }
        PlaySound();
        Time.timeScale = originTimeScale;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void UpdateToggle(string toggle)
    {
        if (toggle == "vsync")
        {
            int vsync = PlayerPrefs.GetInt("vsync");
            QualitySettings.vSyncCount = vsync;
        }else if (toggle == "ssao")
        {
            int ssao = PlayerPrefs.GetInt("ssao");
            if (ssao == 0) ssaoRenderer.SetActive(false);
            else ssaoRenderer.SetActive(true);
        }else if (toggle == "mastervolume" || toggle == "soundfx" || toggle == "bgm" || toggle == "voice")
        {
            mastervolume = PlayerPrefs.GetInt("mastervolume");
            soundfx = PlayerPrefs.GetInt("soundfx");
            bgm = PlayerPrefs.GetInt("bgm");
            voice = PlayerPrefs.GetInt("voice");
            
        }else if (toggle == "fullscreen")
        {
            int fullScreen = PlayerPrefs.GetInt("fullscreen");
            if (fullScreen == 0) Screen.fullScreen = false;
            else Screen.fullScreen = true;
        }else if (toggle == "resolution")
        {
            
            Resolution[] resolutions = Screen.resolutions;
            if (resolutions.Length <= 0)
            {
                return; //防止一些奇怪的错误
            }

            availableRes = new Resolution[Mathf.Clamp(resolutions.Length, -1, 9)];

            int index = 0;
            
            int biggestWidth = 640;
            
            for (int i = 0; i < availableRes.Length; i++)
            {
                int maxj = 0;
                for (int j = 0; j < resolutions.Length; j++)
                {
                    if (resolutions[j].width > biggestWidth && resolutions[j].width < (index - 1 >= 0 ? availableRes[index-1].width : resolutions[j].width + 1))
                    {
                        maxj = j;
                        biggestWidth = resolutions[j].width;
                    }
                }

                if (i == 0 && biggestWidth <= 640)
                {
                    return; //如果屏幕连640x360都支持不了，禁止调整分辨率
                }

                availableRes[index].width = biggestWidth;
                availableRes[index].height = biggestWidth * 9 / 16;
                index++;

                resolutions[maxj].width = -1;
                resolutions[maxj].height = -1;

                if (biggestWidth == 640)
                {
                    break;//640的分辨率出现一次就够了
                }

                biggestWidth = 640;

               
            }

            int reso = PlayerPrefs.GetInt("resolution");
            reso %= index;
            PlayerPrefs.SetInt("resolution", reso);
            print("resolution: " + availableRes[reso].width + " x " + availableRes[reso].height);

            Screen.SetResolution(availableRes[reso].width, availableRes[reso].height, Screen.fullScreen);
        }
    }
}
