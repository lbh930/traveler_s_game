using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashMoveTowards : MonoBehaviour
{
    // Start is called before the first frame update
    public bool reached = true;
    public Vector3 targetPos;
    public Vector3 targetRot;
    Vector3 originPos;
    Vector3 originRot;
    public float progress = 0;
    public float totalTime = 1;

    public float minDistanceToJump = 10;

    Transform targetTrans;
    bool copyTransRot = false;

    bool started = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!reached)
        {
            if (targetTrans != null)
            {
                targetPos = targetTrans.position;
                targetRot = targetTrans.rotation.eulerAngles;
            }

            if (!started)
            {
                originPos = transform.position;
                originRot = transform.rotation.eulerAngles;
            }
            started = true;

            progress += Time.deltaTime / totalTime;
            float totDis = Vector3.Distance(originPos, targetPos);
            double m = 0;

            float mid1 = 0.5f;
            /*if (totDis > minDistanceToJump)
            {
                mid1 = (minDistanceToJump / 2) / (totDis / 2) * 0.5f;
            }*/
            if (progress < 0.5f)
            {
                m = (progress * progress * progress) * mid1 / (0.5 * 0.5 * 0.5);
            }
            else
            {
                double a = (0.5 - (progress - 0.5));
                m = 1-((a*a*a) * mid1 / (0.5 * 0.5 * 0.5));
            }

            transform.position = originPos + (targetPos - originPos) * (float)m;
            transform.rotation = Quaternion.Euler(originRot + (targetRot - originRot) * (float)m);
            if (progress >= 1.0f)
            {
                progress = 0;
                reached = true;
                transform.position = targetPos;
                started = false;
                targetTrans = null;
            }
        }
    }

    public void SetNewDestination(Vector3 pos, Vector3 rot, float totTime)
    {
        originPos = transform.position;
        originRot = transform.rotation.eulerAngles;
        targetRot = rot;
        targetPos = pos;
        reached = false;
        totalTime = totTime;
        started = true;
        progress = 0;
    }

    public void SetNewDestination(Transform target, float totTime)
    {
        originPos = transform.position;
        originRot = transform.rotation.eulerAngles;
        targetTrans = target;
        reached = false;
        totalTime = totTime;
        started = true;
        progress = 0;
    }
}
