using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordPhoto : MonoBehaviour
{
    // Start is called before the first frame update
    bool useRelativeParent = false;//防止点不到的情况 //此功能暂时停用
    RectTransform myrect;
    RectTransform parentrect;

    Image image;
    bool initialized = false;
    bool imageEnabled = false;
   public Sprite bigSprite;

    Transform parent;
    Vector3 relativePos;

    void Initialize()
    {
        image = GetComponent<Image>();
        if (bigSprite == null) bigSprite = GetComponentInChildren<Image>().sprite;

        if (useRelativeParent) {
            myrect = GetComponent<RectTransform>();
            parentrect = transform.parent.GetComponent<RectTransform>();
            parent = transform.parent;
            relativePos = myrect.position - parentrect.position;
            transform.SetParent(transform.parent.parent.parent);
            
        }

        initialized = true;
    }
    void Start()
    {
        if (!initialized) Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) Initialize();

        if (imageEnabled && Input.GetMouseButton(0))
        {
            Image bigImage = GameObject.FindGameObjectWithTag("MemoBigPic").GetComponent<Image>();
            if (bigImage != null)
            {
                bigImage.sprite = null;
                bigImage.enabled = false;
                bigImage.transform.parent.GetComponent<Image>().enabled = false;
                imageEnabled = false;

                //播放音效
                AudioSource audio = GetComponent<AudioSource>();
                if (audio != null)
                {
                    print("photo closed");
                    audio.Play();
                }
            }
        }

    }

    void LateUpdate()
    {
        if (useRelativeParent)
        {
            myrect.position = parentrect.position + relativePos;
        }
    }

    public void OnClick()
    {
        if (!imageEnabled)
        {
            Image bigImage = GameObject.FindGameObjectWithTag("MemoBigPic").GetComponent<Image>();
            if (bigImage != null)
            {
                bigImage.sprite = bigSprite;
                bigImage.enabled = true;
                RectTransform r = bigImage.GetComponent<RectTransform>();

                r.sizeDelta = new Vector2(bigSprite.rect.width/ bigSprite.rect.height * 900f, 900f);
                
                bigImage.transform.parent.GetComponent<Image>().enabled = true;
                imageEnabled = true;

                //播放音效
                AudioSource audio = GetComponent<AudioSource>();
                if (audio != null)
                {
                    audio.Play();
                    print("photo opened");
                }
            }
            
        }
    }
}
