using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoReader : MonoBehaviour
{
    // Start is called before the first frame update
    public TxtReader save;
    public TxtReader story;
    bool initialized = false;

    RecordItem[] records;

    public RectTransform recordPivot;

    void Initialize(){
        records = GetComponentsInChildren<RecordItem>();
        ReadMemo();
        initialized = true;
    }
    void Start()
    {
        if (!initialized)Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
    }

    public void ReadMemo()
    {
        save.Read(Application.streamingAssetsPath, "Save.txt");
        story.Read(Application.streamingAssetsPath, "Storyline.txt");
        for (int j = 0; j < records.Length; j++)
        {
            records[j].gameObject.SetActive(false);
            //print(records[j].name + " disabled");
        }

        //print(save.m_ArrayData.Count);
        int day = save.getInt(0, 0);
        int lastDay = 0;
        for (int i = 0; i < story.m_ArrayData.Count; i++)
        {
            if (story.getString(i,0) == "day")
            {
                if (story.getInt(i, 1) >= day)
                {
                    break; //说明过了，取消循环
                }
                else
                {   //memo的记录是和day同行的
                    for (int j = 2; j < story.m_ArrayData[i].Length; j++)
                    {
                        if (story.m_ArrayData[i].Length <= 2)
                        {
                            break;
                        }
                        string memoName = story.getString(i, j);
                        for (int k = 0; k < records.Length; k++)
                        {
                            if (records[k].memoId == memoName)
                            {

                                records[k].gameObject.SetActive(true);
                                if (recordPivot != null)
                                {
                                    records[k].recordPivot = recordPivot;
                                }

                                //print(records[j].name + " enabled");
                            }
                        }
                    }
                    lastDay = story.getInt(i, 1);
                }
            }
        }

        //标记新增的
        for (int i = 0; i < story.m_ArrayData.Count; i++)
        {
            if (story.getString(i, 0) == "day")
            {
                if (story.getInt(i, 1) == lastDay)
                {//memo的记录是和day同行的
                    for (int j = 2; j < story.m_ArrayData[i].Length; j++)
                    {
                        if (story.m_ArrayData[i].Length <= 2)
                        {
                            break;
                        }
                        string memoName = story.getString(i, j);
                        for (int k = 0; k < records.Length; k++)
                        {
                            if (records[k].memoId == memoName)
                            {

                                records[k].isNew = true;
                                print(records[j].name + " is new");
                            }
                        }
                    }
                }
            }
        }
    }
}
