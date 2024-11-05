using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UnitLock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool initialized = false;
    public int unlockWin = 0;
    public Text lockText;
    public Button button;
    public RawImage lockImage;
    public Text costText;
    TxtReader txt;

    [HideInInspector] public bool locked = false;
    void Initialize()
    {
        if (initialized) return;
        txt = GetComponent<TxtReader>();
        txt.Read(Application.streamingAssetsPath, "Save.txt", ';');
        button = GetComponent<Button>();
        lockImage.enabled = false;
        lockText.enabled = false;
        initialized = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        Initialize();   
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();

        if (txt != null)
        {
            int winCount = txt.getInt(0, 2);
            if (winCount < unlockWin) //Ëø×¡
            {
                //lockText.enabled = true;
                lockImage.enabled = true;
                lockText.text = winCount.ToString() + " / " + unlockWin.ToString();
                button.interactable = false;
                costText.enabled = false;
                locked = true;
            }
            else
            {
                lockText.enabled = false;
                lockImage.enabled = false;
                //lockText.text = winCount.ToString() + " / " + unlockWin.ToString();
                button.interactable = true;
                costText.enabled = true;
            }
        }

        if (MenuManager.nextStoryCharacterId == "sandbox")
        {
            if (Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.L))
            {
                unlockWin = 0;
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        lockText.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        lockText.enabled = false;
    }
}
