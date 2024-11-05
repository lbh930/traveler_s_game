using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollideToEvent : MonoBehaviour
{
    // Start is called before the first frame update
    public EventMachine eventMachine;
    bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (other.transform.GetComponent<TravelerController>() != null)
        {
            if (eventMachine == null) eventMachine = GetComponent<EventMachine>();
            if (eventMachine != null)
            {
                eventMachine.working = true;
            }
        }
    }
}
