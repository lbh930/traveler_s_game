using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame 
    public Vector3 relativePos;
    public Vector3 rotation;
    Transform target;
    Vector3 lastPos;
    FlashMoveTowards move;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
        
    {
        if (move == null) move = GetComponent<FlashMoveTowards>();
        if (target != null)
        {
            
            if (!move.reached)
            {
                move.targetPos = target.position + relativePos;
            }
            else
            {
                transform.position = target.position + relativePos;
            }
        }

        if (MenuManager.nextStoryCharacterId != "sandbox")
        {
            RaycastHit hit;
            // for (int i = 0; i < 256; i++)//·Â·ð³¢ÊÔÀë¿ªÊÓÒ°ÕÚ¸ÇÎï
            //{
            if (Physics.Raycast(transform.position, transform.forward, out hit, 16))
            {
                if (hit.transform.tag == "ViewObstacle")
                {
                    transform.position = hit.point + transform.forward * 5f;
                }
            }
        }
        //}
    }

    public void SetTarget(Transform tar) 
    {
        target = tar;
        if (tar != null)
        {
            lastPos = tar.position;
            move.SetNewDestination(tar.position + relativePos, rotation, 0.4f);
        }
    }
}
