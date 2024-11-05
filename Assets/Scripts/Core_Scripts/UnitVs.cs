using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitVs : MonoBehaviour
{
    // Start is called before the first frame update
    List<HealthScript> allUnits = new List<HealthScript>();
    TxtReader csv;

    IdentityList list;
    public int row = 0;
    public int col = 0;
    int cost;
    int minCostGap = 100;
    float minUnitGap = 1;
    public bool finished = true;

    public float timeScale = 1.0f;


    public bool fixrow = false;
    public bool fixcol = false;
    
    float timer = 100;
    void Start()
    {
        list = FindObjectOfType<IdentityList>();
        csv = GetComponent<TxtReader>();
        
    }
    void Update()
    {
        Time.timeScale = timeScale;
        Spawn();
        Check();
    }
    void Spawn()
    {
        if (finished)
        {
            timer = 100; //重设计时器
            finished = false;

            int originalcol = col;
            int originalrow = row;

            col++;
            if (col >= list.unitList.Length)
            {
                col %= list.unitList.Length;
                row++;
            }

            row %= list.unitList.Length;

            if (fixrow) row = originalrow;
            if (fixcol) col = originalcol;

            while (list.unitList[row] == null)
            {
                row++;
                row %= list.unitList.Length;
            }
            while (list.unitList[col] == null)
            {
                col++;
                if (col >= list.unitList.Length)
                {
                    col %= list.unitList.Length;
                    row++;
                }
            }


            
            if (col == row)
            {
                csv.Read(Application.streamingAssetsPath, "UnitVs.csv", ';');
                csv.Write(row, col, "0.5|1000", "UnitVs.csv", ';');
                finished = true;
                return;
            }
            //初始化
            cost = 2000;
            if (allUnits.Count>0)
                allUnits.Clear();
            //
            print("row is " + row.ToString() + " col is " + col.ToString());
                
            HealthScript h1 = list.unitList[row].GetComponent<HealthScript>();
            HealthScript h2 = list.unitList[col].GetComponent<HealthScript>();
            
            for (int i = 0; i < 50; i++)
            {
                int _n1 = Mathf.FloorToInt(cost / (int)h1.cost);
                int _n2 = Mathf.FloorToInt(cost / (int)h2.cost);
                if (Mathf.Abs(_n1 * h1.cost - _n2 * h2.cost) <= minCostGap)
                {
                    break;
                }
                cost += minCostGap;
            }

            int n1 = Mathf.FloorToInt(cost / (int)h1.cost);
            int n2 = Mathf.FloorToInt(cost / (int)h2.cost);

            int maxCount = Mathf.Max(n1, n2);
            float gap1 = minUnitGap * ((float)maxCount/(float)n1);
            float gap2 = minUnitGap * ((float)maxCount / (float)n2);


            Vector3 ini1 = Vector3.zero;
            Vector3 ini2 = Vector3.zero;

            if ((row != 2 && row != 8 && row != 16 && row != 26 && row != 25 && row !=31 && row != 32)&&
                (col != 2 && col != 8 && col != 16 && col != 26 && col != 25 && col != 31 && col != 32))
            {//不是远程放近一点
                ini1 = new Vector3(transform.position.x, -8, 5);
                ini2 = new Vector3(transform.position.x, -8, -5);
            }
            else
            {
               ini1 = new Vector3(transform.position.x, -8, 20);
                ini2 = new Vector3(transform.position.x, -8, -20);
            }
         

            for (int i = 0; i <= n1; i++)
            {
                CreateUnit(row, ini1 + Vector3.right * gap1 * i, 1);
            }
            for (int i = 0; i <= n2; i++)
            {
                CreateUnit(col, ini2 + Vector3.right * gap2 * i, 2);
            }
        }
    }

    void Check()
    {
        timer -= Time.deltaTime;
        bool team1alive = false;
        bool team2alive = false;
        for (int i = 0; i < allUnits.Count; i++)
        {
            if (allUnits[i] == null) continue;
            if (allUnits[i].neverAsTarget) continue;
            if (allUnits[i].teamIndex == 1) team1alive = true;
            if (allUnits[i].teamIndex == 2) team2alive = true;
        }

        if (timer < 0)
        {
            print("time out");
            //计时器耗尽，随机胜负
            if (Random.Range (0,2) == 0)
            {
                team1alive = true;
                team2alive = false;
            }
            else
            {
                team1alive = false;
                team2alive = true;
            }
        }

        if (team1alive && !team2alive)
        {
            print("team1win");
            csv.Read(Application.persistentDataPath, "UnitVs.csv", ';');
            if (row >= csv.m_ArrayData.Count || col >= csv.m_ArrayData[row].Length ||
                csv.getString(row, col).Length <= 0)
            {
                csv.Write(row, col, "0.5|2", "UnitVs.csv", ';');
                //csv.Write(col, row, "0|1", "UnitVs.csv", ';');

            }
            else
            {
                string s = csv.getString(row, col);
                float a = float.Parse(s.Split('|')[0]);
                float b = float.Parse(s.Split('|')[1]);
                float rate = ((a * b) + 1) / (b + 1);
                float rate2 = 1 - rate;
                csv.Write(row, col, rate.ToString() + "|"
                    + Mathf.RoundToInt((b + 1)).ToString(), "UnitVs.csv", ';');
                //csv.Write(col, row, rate2.ToString() + "|"
                    //+ Mathf.RoundToInt((b + 1)).ToString(), "UnitVs.csv", ';');
            }

            ClearBattlefield();
            finished = true;
        }
        else if (!team1alive && team2alive)
        {
            print("team2win");
            csv.Read(Application.persistentDataPath, "UnitVs.csv", ';');
            if (row >= csv.m_ArrayData.Count || col >= csv.m_ArrayData[row].Length ||
                csv.getString(row, col).Length <= 0)
            {
                csv.Write(row, col, "0.5|2", "UnitVs.csv", ';');
                //csv.Write(col, row, "1|1", "UnitVs.csv", ';');
            }
            else
            {
                string s = csv.getString(row, col);
                float a = float.Parse(s.Split('|')[0]);
                float b = float.Parse(s.Split('|')[1]);
                float rate = ((a * b)) / (b + 1);
                float rate2 = 1 - rate;
                csv.Write(row, col, rate.ToString() + "|"
                    + Mathf.RoundToInt((b + 1)).ToString(), "UnitVs.csv", ';');
                //csv.Write(col, row, rate2.ToString() + "|"
                    //+ Mathf.RoundToInt((b + 1)).ToString(), "UnitVs.csv", ';');
            }

            ClearBattlefield();
            finished = true;
        }else if (!team1alive && !team2alive){
            ClearBattlefield();
            finished = true;
        }   
    }

    public void ClearBattlefield()
    {
        RagdollScript[] bodies = FindObjectsOfType<RagdollScript>();
        ToolScript[] tools = FindObjectsOfType<ToolScript>();
        for (int i = 0; i < bodies.Length; i++)
        {
            Destroy(bodies[i].gameObject);
        }
        for (int i = 0; i < allUnits.Count; i++)
        {
            if (allUnits[i] != null)
                Destroy(allUnits[i].gameObject);
        }
        for (int i = 0; i < tools.Length; i++)
        {
            Destroy(tools[i].gameObject);
        }
        allUnits.Clear();
    }

    void CreateUnit(int index, Vector3 pos, int teamIndex)
    {
        HealthScript h = Instantiate(list.unitList[index], pos, transform.rotation).GetComponent<HealthScript>();
        h.teamIndex = teamIndex;
        HumanAi ai = h.ai;
        if (ai == null) ai = h.GetComponent<HumanAi>();

        if (ai.rider != null)
        {
            allUnits.Add(ai.rider.GetComponent<HealthScript>());
            ai.rider.GetComponent<HealthScript>().teamIndex = teamIndex;
        }
        else
        {
            allUnits.Add(h);
        }
        
    }
}
