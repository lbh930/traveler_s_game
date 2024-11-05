using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuScroll : MonoBehaviour
{
    public RectTransform trans;
    public Slider slider;
    public RectTransform originPoint;
    public float min = float.PositiveInfinity;
    public float max = float.NegativeInfinity;

    bool initialized = false;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (initialized) return;
        initialized = true;

        min = float.PositiveInfinity;
        max = float.NegativeInfinity;

        originPoint.position = trans.position;

    }

    // Update is called once per frame
    void Update()
    {
        Initialize();

        RectTransform rect = GetComponent<RectTransform>();

        trans.position = originPoint.position;//�ȳ�ʼ���ټ���min��max

        for (int i = 0; i < trans.childCount; i++)
        {
            RectTransform rt = trans.GetChild(i).GetComponent<RectTransform>();
            if (rt == rect || rt == trans || rt == originPoint) continue;

            if (rt.position.y < min)
            {
                min = rt.position.y;
            }

            if (rt.position.y > max)
            {
                max = rt.position.y;
            }
        }

        if (Mathf.Abs(min - max) > 10000000) return; //˵��min��max��û�б����ã�����

        trans.position = new Vector3(originPoint.position.x,
            originPoint.position.y - (slider.value / 100) * (min - max), originPoint.position.z);

    }
}
