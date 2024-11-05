using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{
    public enum ButtonType{
        transition,
        toggle,
        endpause,
        quit,
        language,
        slider,
        credit,
        reset,
    }

    MenuManager menu;
    public enum ToggleType
    {
        other,
        resolution,
        fullscreen,
        vsync,
        ssao,

    }

    public ButtonType type;
    //public ToggleType toggleType;


    [Header ("For Transition")]
    public GameObject transitTo;

    [Header("For Toggle")]
    public GameObject[] toggleList = new GameObject[1];
    public int toggleIndex = 0;
    public Text resolutionText;

    public string prefName;

    [Header("For Language")]
    public int languageIndex = 0;

    [Header("For Slider")]
    public Slider slider;

    [Header("For Credit")]
    public GameObject credit;

    PauseScript ps;

    void Start()
    {
        toggleIndex %= toggleList.Length;
    }
    void Update()
    {
       
        if (type == PauseButton.ButtonType.slider)
        {
            PlayerPrefs.SetInt(prefName, Mathf.RoundToInt(slider.value));
            UpdateToggle(prefName);
        }else if (prefName == "resolution")
        {
            resolutionText.text = "[ " + Screen.width.ToString() + " x " + Screen.height.ToString() + " ]";
        }
        else
        {
            toggleIndex %= toggleList.Length;
        }
    }

    void OnEnable()
    {
        toggleIndex = PlayerPrefs.GetInt(prefName);

        if (type == ButtonType.toggle && prefName != "resolution")
        {
            for (int i = 0; i < toggleList.Length; i++)
            {
                if (i == toggleIndex) toggleList[i].SetActive(true);
                else toggleList[i].SetActive(false);
            }
        }else if (prefName == "resolution")
        {
            resolutionText.gameObject.SetActive(true);
            resolutionText.text = "[ " + Screen.width.ToString() + " x " + Screen.height.ToString() + " ]";
        }

        if (type == ButtonType.slider)
        {
            slider.gameObject.SetActive(true);
            int v = PlayerPrefs.GetInt(prefName);

            v = Mathf.Clamp(v, 0, 101);
            GetComponent<Button>().enabled = false;
            GetComponent<Image>().raycastTarget = false;
            slider.value = v;
        }

        if (type == ButtonType.credit)
        {
            credit.SetActive(false);
        }
    }
    public void OnClick()
    {
        //播放音频
        GetComponent<SoundEffect>().PlayAudio();

        if (type == ButtonType.transition)
        {
            transitTo.SetActive(true);
            AudioSource audio = transitTo.GetComponent<AudioSource>();
            if (audio != null) audio.Play(); //播放音频
            gameObject.transform.parent.gameObject.SetActive(false);
        }
        else if (type == ButtonType.toggle)
        {
            if (prefName != "resolution")
            {
                toggleIndex = PlayerPrefs.GetInt(prefName);
                toggleIndex++;
                toggleIndex %= toggleList.Length;
                PlayerPrefs.SetInt(prefName, toggleIndex);

                for (int i = 0; i < toggleList.Length; i++)
                {
                    if (i == toggleIndex) toggleList[i].SetActive(true);
                    else toggleList[i].SetActive(false);
                }

                UpdateToggle(prefName);
            }
            else
            {
                //分辨率特殊 单独处理
                toggleIndex = PlayerPrefs.GetInt(prefName);
                toggleIndex++;
                PlayerPrefs.SetInt(prefName, toggleIndex);
                UpdateToggle(prefName);
                resolutionText.text = "[ " + Screen.width.ToString() + " x " + Screen.height.ToString() + " ]";
            }

        }
        else if (type == ButtonType.endpause)
        {
            GameObject.FindObjectOfType<PauseScript>().Continue();
        }
        else if (type == ButtonType.quit)
        {
            menu = FindObjectOfType<MenuManager>();
            if (menu != null && menu.state == MenuManager.menuState.single)
            {
                SceneManager.LoadScene("Menu"); //从程序界面退回到主菜单
            }
            else if (SceneManager.GetActiveScene().name == "Menu")
            {
                Application.Quit();
                print("quit");
            }
            else
                SceneManager.LoadScene("Menu");
        }
        else if (type == ButtonType.language)
        {
            PlayerPrefs.SetInt("language", languageIndex);


            TextLanguageChanger[] tlc = FindObjectsOfType<TextLanguageChanger>(true);
            for (int i = 0; i < tlc.Length; i++)
            {
                if (tlc.Length == 0) break;
                tlc[i].ChangeText();
            }

            ChineseFontChanger cfc = GameObject.FindObjectOfType<ChineseFontChanger>(true);
            if (cfc != null)
            {
                cfc.ChangeFont();
            }

            MenuManager menu = GameObject.FindObjectOfType<MenuManager>();
            if (menu != null)
            {
                menu.SetDayText();
            }


        }
        else if (type == ButtonType.slider)
        {
            PlayerPrefs.SetInt(prefName, Mathf.RoundToInt(slider.value));
        }
        else if (type == ButtonType.credit)
        {
            credit.SetActive(true);
        }
        else if (type == ButtonType.reset)
        {

            GetComponent<TxtReader>().ResetStory();
        }
    }

    public void UpdateToggle(string toggleName)
    {
        if (ps == null) ps = FindObjectOfType<PauseScript>();
        ps.UpdateToggle(toggleName);
    }
}
