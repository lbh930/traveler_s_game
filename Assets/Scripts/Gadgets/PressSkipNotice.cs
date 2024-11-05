using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PressSkipNotice : MonoBehaviour
{
    public Text sub;
    public Text text;
    public RawImage q;

    float timer = float.PositiveInfinity;

    public static bool noticeShowed = false;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!noticeShowed && timer > 999999 && sub.text.Length > 0)
        {//µ¥´Î´¥·¢
            timer = 6;
            noticeShowed = true;
        }   

        if (timer < 999999 && timer >= 0)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.MoveTowards(text.color.a, 1, Time.deltaTime));
            q.color = new Color(q.color.r, q.color.g, q.color.b, Mathf.MoveTowards(q.color.a, 1, Time.deltaTime));
        }
        else
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.MoveTowards(text.color.a, 0, Time.deltaTime));
            q.color = new Color(q.color.r, q.color.g, q.color.b, Mathf.MoveTowards(q.color.a, 0, Time.deltaTime));
        }

        timer -= Time.deltaTime;


    }
}
