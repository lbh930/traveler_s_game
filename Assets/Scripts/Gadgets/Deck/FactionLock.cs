using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactionLock : MonoBehaviour
{
    RawImage slot;
    public GameObject slotIcon;
    public int threshold;
    public int previousThreshold;
    public Text progressText;
    TxtReader txt;

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

        slot = transform.parent.GetComponent<RawImage>();
        txt = GetComponent<TxtReader>();
        txt.Read(Application.streamingAssetsPath, "Save.txt", ';');
        UpdateLock();
    }

    void UpdateLock()
    {
        if (txt == null) return;
        if (txt.getInt(0,2) < threshold)
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                Transform t = transform.parent.GetChild(i);
                if (t.gameObject == transform.parent.gameObject || t.gameObject == slotIcon.gameObject || t.gameObject == gameObject) continue;
                t.gameObject.SetActive(false); //关闭原slot的组件，只保留其icon
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }

            slot.enabled = false;
            //sandbox中，不改变chalk object
            //chalk一定放在第一个子级
            if (MenuManager.nextStoryCharacterId == "sandbox")
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                transform.parent.GetChild(0).gameObject.SetActive(true);
                slot.enabled = true;
            }
            
        }
        else
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                Transform t = transform.parent.GetChild(i);
                if (t.gameObject == transform.parent.gameObject || t.gameObject == slotIcon.gameObject || t.gameObject == gameObject) continue;
                t.gameObject.SetActive(true); //关闭原slot的组件，只保留其icon
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }

            slot.enabled = true; ;
        }

        if (txt.getInt(0, 2) >= previousThreshold)
        {
            progressText.text = txt.getInt(0, 2).ToString() + " / " + threshold.ToString();
        }
        else
        {
            progressText.text = txt.getInt(0, 2).ToString() + " /  ??";
        }
    }
    void OnEnable()
    {
        UpdateLock();
    }

    
    // Update is called once per frame
    void Update()
    {
        Initialize();
        if (MenuManager.nextStoryCharacterId == "sandbox")
        {
            if (Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.L))
            {
                if (threshold >= 0)
                {
                    threshold = -1;
                    UpdateLock();
                }
            }
        }
    }
}
