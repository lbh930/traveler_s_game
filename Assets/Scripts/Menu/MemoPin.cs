using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoPin : MonoBehaviour
{
    public Transform parent;

    RectTransform rect;
    public RectTransform[] connected = new RectTransform[1];
    public RawImage[] connectedImage;
    public RectTransform[] strings;
    public GameObject stringObject;

    RawImage image;
    // Start is called before the first frame update
    bool initialized = false;
    public void Initialize() {
        if (initialized) return;
        initialized = true;

        parent = transform.parent;//原父级，用于判断是否enable
        rect = GetComponent<RectTransform>();

        connectedImage = new RawImage[connected.Length];
        if (connectedImage.Length > 0) {
            for (int i = 0; i < connectedImage.Length; i++)
            {
                if (connected[i]!=null)
                    connectedImage[i] = connected[i].GetComponent<RawImage>();
            }
        }

        rect = GetComponent<RectTransform>();

        stringObject.SetActive(true);
        strings = new RectTransform[connected.Length];
        image = GetComponent<RawImage>();

        parent = transform.parent;//原父级，用于判断是否enable

        Transform pp = GameObject.FindGameObjectWithTag("MemoPinParent").transform;
        Transform sp = GameObject.FindGameObjectWithTag("MemoStringParent").transform;

        transform.SetParent(pp);
        
        for (int i = 0; i < strings.Length; i++)
        {
            if (connected[i] == null) continue;

            MemoPin mp = connected[i].GetComponent<MemoPin>();
            if (mp != null)
            {
                mp.Initialize();
            }
            connected[i].SetParent(pp);
            //strings[i].SetParent(transform.parent);

            strings[i] = Instantiate(stringObject, rect.position, rect.rotation).GetComponent<RectTransform>();
            strings[i].SetParent(sp);
            strings[i].gameObject.SetActive(false);
            strings[i].right = connected[i].position - rect.position;
            float dis = Vector3.Distance(connected[i].anchoredPosition, rect.anchoredPosition);        
            strings[i].localScale = new Vector3(dis / 100f, 1, 1);
            strings[i].GetComponent<RawImage>().uvRect.Set(0, 0, dis / 100, 1);
        }
        stringObject.SetActive(false);


        

        
    }
    void Start()
    {
        if (!initialized)Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (parent != null)
        {
            if (!parent.gameObject.active)
            {
                if (Mathf.Abs(rect.position.x) < 1000000)
                {
                    rect.position = new Vector3(rect.position.x, rect.position.y, 0);
                }
                else
                {
                    //rect.position = Vector3.zero;
                }
                image.enabled = false;
            }
            else
            {
                if (Mathf.Abs(rect.position.x) < 1000000)
                {
                    rect.position = new Vector3(rect.position.x, rect.position.y, 0);
                }
                else
                {
                    //rect.position = Vector3.zero;
                }
                image.enabled = true;
            }
        }

        

        if (!initialized)Initialize();
        stringObject.SetActive(false);
        for (int i = 0; i < connected.Length; i++)
        {
            if (connected.Length <= 0) break;
            //print(strings.Length);
            if (strings.Length == 0 || strings[i] == null) continue;
            if (connectedImage[i].enabled && connected[i].position.z > -1)
            {
                strings[i].gameObject.SetActive(true);
            }
            else
            {
                strings[i].gameObject.SetActive(false);
            }
        }
        //if (parent.gameObject.active == false) gameObject.SetActive(false);
        //print(rect.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < connected.Length; i++) {
            if (connected[i] == null || connected[i].gameObject.active == false) continue;
            Gizmos.DrawLine(transform.position, connected[i].position);
        }
    }
}
