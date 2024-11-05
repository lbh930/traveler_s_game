using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxFactionSlot : MonoBehaviour
{
    // Start is called before the first frame update

    public float downY;
    public float upY;
    RectTransform rect;

    public bool show = false;
    bool initialized = false;

    public SoundEffect clickSound;

    public float slideSpeed = 100;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (initialized) return;
        initialized = true;

        rect = transform.parent.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();

        if (!show)
        {
            rect.anchoredPosition = new Vector3(rect.anchoredPosition.x,
                Mathf.Lerp(rect.anchoredPosition.y, upY, Time.unscaledDeltaTime * slideSpeed));
        }
        else
        {
            rect.anchoredPosition = new Vector3(rect.anchoredPosition.x,
                Mathf.Lerp(rect.anchoredPosition.y, downY, Time.unscaledDeltaTime * slideSpeed));
        }
    }

    public void OnClick()
    {
        show = !show;
        if (clickSound != null)
        {
            clickSound.PlayAudio();
        }
    }
}
