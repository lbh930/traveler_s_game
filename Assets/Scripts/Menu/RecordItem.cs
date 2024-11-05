using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordItem : MonoBehaviour
{
    public string memoId;
    public RectTransform rectTransform;
    // Start is called before the first frame update
    bool initialized = false;

    public bool isNew = false;
    public UnityEngine.UI.Outline newOutline;
    public Text newText;

    public RectTransform recordPivot;

    void Start()
    {
        if (!initialized) Initialize();
        
    }

    void Initialize()
    {
        rectTransform = GetComponent<RectTransform>();

        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) Initialize();

        if (isNew)
        {
            if (newOutline != null)
            {
                newOutline.enabled = true;
                if (recordPivot != null)
                {
                    newOutline.effectDistance = (Vector2.one / recordPivot.localScale.x)*0.7f*5 + 0.3f*Vector2.one*5;
                }
            }
            if (newText != null)
            {
                newText.enabled = true;
                if (recordPivot != null)
                {
                    newText.transform.parent.localScale = (Vector3.one / recordPivot.localScale.x) * 0.7f + Vector3.one * 0.3f;
                }
            }
        }
        else
        {
            if (newOutline != null)
            {
                newOutline.enabled = false;
            }
            if (newText != null)
            {
                newText.enabled = false;
            }
        }
    }
}
