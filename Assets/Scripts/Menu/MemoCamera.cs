using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoCamera : MonoBehaviour
{
    RectTransform rect;
    public float minx = float.PositiveInfinity;
    public float maxx = float.NegativeInfinity;
    public float miny = float.PositiveInfinity;
    public float maxy = float.NegativeInfinity;

    public float zoomSpeed = 50;
    public float moveSpeed = -1000;
    // Start is called before the first frame update
    bool initialized = false;
    void Start()
    {
        if (!initialized) Initialize();
        
    }

    void Initialize()
    {
        rect = GetComponent<RectTransform>();
        rect.position = Vector3.zero;
        initialized = true;
    }
    void CalculateBoundary()
    {
        Vector3 originPos = rect.anchoredPosition;
        //print(rect.anchoredPosition);
        //print(rect.position);
        //print(rect.localPosition);
        rect.anchoredPosition = Vector3.zero;
        minx = float.PositiveInfinity;
        maxx = float.NegativeInfinity;
        miny = float.PositiveInfinity;
        maxy = float.NegativeInfinity;
        RecordItem[] ris = GetComponentsInChildren<RecordItem>();
        //print(ris.Length);
        //print(rect.localScale.x);
        foreach (RecordItem ri in ris)
        {        
            if (ri.gameObject.active)
            {
                minx = Mathf.Min(ri.rectTransform.localPosition.x * -1, minx);
                miny = Mathf.Min(ri.rectTransform.localPosition.y * -1, miny);
                maxx = Mathf.Max(ri.rectTransform.localPosition.x * -1, maxx);
                maxy = Mathf.Max(ri.rectTransform.localPosition.y * -1, maxy);
            }
        }

        if (minx > 1000000)
        {
            minx = -300;
            maxx = 200;
            miny = -300;
            maxy = 200;
        }

        minx *= rect.localScale.x;
        maxx *= rect.localScale.x;
        miny *= rect.localScale.y;
        maxy *= rect.localScale.y;

         rect.anchoredPosition = originPos;
    }
    // Update is called once per frame
    void Update()
    {
        if (!initialized) Initialize();


        //位移部分
        Vector2 mouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector2 key = Vector2.zero;
        if (Input.GetKey(KeyCode.A)) key -= Vector2.right;
        if (Input.GetKey(KeyCode.D)) key += Vector2.right;
        if (Input.GetKey(KeyCode.W)) key += Vector2.up;
        if (Input.GetKey(KeyCode.S)) key -= Vector2.up;

        if (Mathf.Abs(mouse.x) < 1000000)
        {
            if (!(rect.position.x < 1000000))
            {
                rect.position = Vector3.zero;
            }
            else
            {

            }
            rect.Translate(mouse * moveSpeed * Time.deltaTime);
            rect.Translate(key * moveSpeed * Time.deltaTime);
        }

        //缩放部分
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        float k1 = rect.localScale.x;
        rect.localScale = new Vector3(rect.localScale.x + zoom * Time.deltaTime*zoomSpeed,
            rect.localScale.x + zoom * Time.deltaTime * zoomSpeed, 1);
        rect.localScale = new Vector3(Mathf.Clamp(rect.localScale.x, 0.25f, 1.8f),
           Mathf.Clamp(rect.localScale.x, 0.25f, 1.8f) , 1);
        float k2 = rect.localScale.x;
        if (Mathf.Abs(k1) > 0.01f)
        {
            float k = k2 / k1;

            //缩放《-》位移补正
            float a = rect.localPosition.x;
            float b = rect.localPosition.y;

            if (Mathf.Abs(a) < 1000000 && Mathf.Abs(b) < 1000000)
            {
                rect.localPosition += new Vector3((k - 1f) * a, (k - 1f) * b, 0);
            }
        }
        CalculateBoundary();
        rect.anchoredPosition = new Vector3(Mathf.Clamp(rect.anchoredPosition.x, minx, maxx),
            Mathf.Clamp(rect.anchoredPosition.y, miny, maxy));
    }

}
