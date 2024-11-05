using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetList : MonoBehaviour
{
    public static List<HealthScript> targets = new List<HealthScript>();
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {

    }

    public static HealthScript FindCloseEnemy(Vector3 origin, float radius, int teamIndex)
    {
        if (targets == null) return null;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null || targets[i].neverAsTarget || !targets[i].gameObject.activeSelf)
            {
                targets.RemoveAt(i);
                i--;
            }
        }
        if (targets.Count == 0) return null;

        int index = -1;
        float minDis = float.PositiveInfinity;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i].teamIndex == teamIndex) continue;   
            float dis = Vector3.Distance(origin, targets[i].transform.position);
            if (dis > radius) continue;

            if (dis < minDis)
            {
                minDis = dis;
                index = i;
            }
        }
        if (index >= 0)
        {
            return targets[index];
        }
        else
        {
            return null;
        }
    }

    public static HealthScript FindEnemy (Vector3 origin, int teamIndex)
    {
        if (targets == null) return null;
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] == null || targets[i].neverAsTarget || !targets[i].gameObject.activeSelf)
            {
                targets.RemoveAt(i);
                i--;
            }
        }
        if (targets.Count == 0) return null;

        float[] chances = new float[targets.Count];
        float distanceSum = 0;
        foreach (HealthScript target in targets) //获取长度总和
        {
            if (target == null) break;
            if (target.teamIndex == teamIndex) continue;
            distanceSum += 1/Vector3.Distance(origin, target.transform.position);
        }
        float percentSum = 0;
        //print(targets.Count);
        for (int a = 0; a < targets.Count; a++) //算概率
        {
            //print(teamIndex + " e " + targets[a].teamIndex + );
            if (targets[a].teamIndex == teamIndex)
            {
                chances[a] = 0;
                continue;
            }
            //print(teamIndex + "  " + a);
            chances[a] = 1/Vector3.Distance(origin, targets[a].transform.position) / distanceSum;
            percentSum += chances[a];
        }

        for (int i=0; i < chances.Length; i++)
        {
            chances[i] *= 1/percentSum;
            //print("c " + chances[i] + " team: " + teamIndex );
        }

        int b = Algori.SeedWeightedRandom(chances, 0);
        
        if (b >= 0 && targets[b].teamIndex != teamIndex)
        {
            return targets[b];
        }
        else
        {
            return null;
        }
    }
}
