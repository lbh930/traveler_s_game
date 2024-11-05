using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MenuCalendar : MonoBehaviour
{
    public Animator ani;
    public Animator canvasAni;
    public int yearBase = 2023;
    public int monthBase = 5;
    public int dayBase = 2;

    [HideInInspector]public int tempYear;
    [HideInInspector]public int tempMonth;
    [HideInInspector]public int tempDay;
    public TextMeshPro[] oldDateText = new TextMeshPro[3];
    public TextMeshPro[] newDateText = new TextMeshPro[3];
    // Start is called before the first frame update
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    public void SetCalendar(int day)
    {
        //print(day);
        CalculateDate(day);

        oldDateText[0].text = tempYear.ToString();
        if (tempMonth < 10)
        {//如果个位数自动加0
            oldDateText[1].text = '0' + tempMonth.ToString();
        }
        else
        {
            oldDateText[1].text = tempMonth.ToString();
        }

        if (tempDay < 10)
        {
            oldDateText[2].text = '0' + tempDay.ToString();
        }
        else
        {
            oldDateText[2].text = tempDay.ToString();
        }

    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeDate(0, 3);
        }*/
    }
    public void ChangeDate(int olddate, int newdate){
        CalculateDate(olddate);//算出旧日期，一个个赋值
        oldDateText[0].text = tempYear.ToString();
        if (tempMonth < 10)
        {
            oldDateText[1].text = '0' + tempMonth.ToString();
        }
        else
        {
            oldDateText[1].text = tempMonth.ToString();
        }

        if (tempDay < 10)
        {
            oldDateText[2].text = '0' + tempDay.ToString();
        }
        else
        {
            oldDateText[2].text = tempDay.ToString();
        }

        //算出新日期，一个个赋值
        CalculateDate(newdate);

        newDateText[0].text = tempYear.ToString();
        if (tempMonth < 10)
        {//如果个位数自动加0
            newDateText[1].text = '0' + tempMonth.ToString();
        }
        else
        {
            newDateText[1].text = tempMonth.ToString();
        }

        if (tempDay < 10)
        {
            newDateText[2].text = '0' + tempDay.ToString();
        }
        else
        {
            newDateText[2].text = tempDay.ToString();
        }

        //启动动画
        if (newDateText[0].text != oldDateText[0].text)
        {
            ani.SetTrigger("Year");
        }else if (newDateText[1].text != oldDateText[1].text)
        {
            ani.SetTrigger("Month");
        }
        else
        {
            ani.SetTrigger("Day");
        }
    }
    void CalculateDate (int day)
    {
        tempDay = dayBase;
        tempMonth = monthBase;
        tempYear = yearBase;
        tempDay += day;
        tempMonth = monthBase;
        tempYear = yearBase;
        while (true)
        {
            if (tempMonth % 2 == 1)
            {
                if (tempDay > 31)
                {
                    tempDay -= 31;
                    tempMonth++;
                    continue;
                }
            }else if (tempMonth == 2)
            {
                if (tempDay > 28)
                {
                    tempDay -= 28;
                    tempMonth++;
                    continue;
                }
            }
            else
            {
                if (tempDay > 30)
                {
                    tempDay -= 30;
                    tempMonth++;
                    continue;
                }
            }

            if (tempMonth > 12)
            {
                tempYear++;
                tempMonth -= 12;
                continue;
            }
            break;
        }
    }
    public void EndAnimation()
    {
        if (canvasAni == null)
        {
            print("Canvas Animator Not Assigned");
        }
        else
        {
            canvasAni.SetTrigger("ForceStartProgram");
            canvasAni.SetTrigger("MemoUpdated"); 
        }
    }
}
