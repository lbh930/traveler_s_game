using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algori : MonoBehaviour
{
    public static int SeedRandom(int a, int b, int seed)
    {
        return Random.Range(a, b);
    }

    public static int STS(int a, int b)
    {
        return a * b;
    }

    public static float SeedRandom(float a, float b, int seed)
    {
        return Random.Range(a, b);
    }

    public static Vector3 GetRelativeVector(Vector3 forward, Vector3 right, Vector3 vector)
    {
        forward = forward.normalized;
        right = right.normalized;
        return new Vector3(
            Vector3.Dot(vector, right),
            Vector3.Dot(vector, Vector3.Cross(forward, right)),
            Vector3.Dot(vector, forward));
    }

    public static Vector3 Jiggle(Vector3 originPos, float jiggleDistance)
    {
        Vector3 newPos = Vector3.zero;

        float angle = Random.Range(0.0f, 360.0f);
        Vector3 dir = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
        newPos = dir.normalized * jiggleDistance + originPos;

        return newPos;
    }

    public static float RandomInRange(float average, float range)
    {
        return Random.Range(average - range / 2, average + range / 2);
    }

    public static HumanAi GetDominantUnit(HumanAi ai) //如果有骑手则获取到骑手
    {
        if (ai.rider != null) return ai.rider;
        return ai;
    }

    public static void CopyTransform(Transform target, Transform template)
    {
        Transform oldParent = target.parent;
        target.SetParent(template.parent);
        target.transform.position = template.transform.position;
        target.transform.rotation = template.transform.rotation;
        target.transform.localScale = template.transform.localScale;
        target.SetParent(oldParent);
    }

    public static Vector3Int GetNearby(Vector3Int origin, int i, int step = 1)
    {
        switch (i)
        {
            case 0:
                return new Vector3Int(origin.x, origin.y + step, origin.z);
                break;
            case 1:
                return new Vector3Int(origin.x + step, origin.y, origin.z);
                break;
            case 2:
                return new Vector3Int(origin.x, origin.y - step, origin.z);
                break;
            case 3:
                return new Vector3Int(origin.x - step, origin.y, origin.z);
                break;
        }
        return origin;
    }

    public static Vector3 GetNearbyPos(Vector3 origin, int i, int step = 2)
    {
        switch (i)
        {
            case 0:
                return new Vector3(origin.x, origin.y, origin.z + step);
                break;
            case 1:
                return new Vector3(origin.x + step, origin.y, origin.z);
                break;
            case 2:
                return new Vector3(origin.x, origin.y, origin.z - step);
                break;
            case 3:
                return new Vector3(origin.x - step, origin.y, origin.z);
                break;
        }
        return origin;
    }

    public static int SeedWeightedRandom(float[] p, int seed)
    {
        //print("a");

        float tot = 0;
        float point = SeedRandom(0.0f, 1.0f, seed);
        for (int i = 0; i < p.Length; i++)
        {
            tot += p[i];
            if (point + 0.000001f < tot) return i;
        }
        return p.Length - 1;
    }

    public static void SetLayerOfChildren(Transform p, string layerName)
    {
        int layerId = 0;

        if (layerName == "red")
        {
            layerId = 7;
        }
        else if (layerName == "blue")
        {
            layerId = 8;
        }
        else if (layerName == "white")
        {
            layerId = 9;
        }

        for (int i = 0; i < p.childCount; i++)
        {
            p.GetChild(i).gameObject.layer = layerId;
            SetLayerOfChildren(p.GetChild(i), layerId);
        }
    }

    public static void SetLayerOfChildren(Transform p, int layer)
    {
        for (int i = 0; i < p.childCount; i++)
        {
            p.GetChild(i).gameObject.layer = layer;
            SetLayerOfChildren(p.GetChild(i), layer); //递归
        }
    }
}
