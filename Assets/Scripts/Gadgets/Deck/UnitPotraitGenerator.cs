using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitPotraitGenerator : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject statue;
    public int id = -1;
    void Start()
    {
        
    }

    public void MakeItStatic()
    {
        foreach (Collider c in statue.GetComponentsInChildren<Collider>())
        {
            Destroy(c);
        }

        foreach (Rigidbody c in statue.GetComponentsInChildren<Rigidbody>())
        {
            c.useGravity = false;
        }
        foreach (HumanAi c in statue.GetComponentsInChildren<HumanAi>())
        {
            c.inBattle = false;

            c.GetComponent<HealthScript>().neverAsTarget = true;

        }
        foreach (TravelerController c in statue.GetComponentsInChildren<TravelerController>())
        {
            Destroy(c);
        }
        foreach (Animator c in statue.GetComponentsInChildren<Animator>())
        {
            c.SetBool("RightClicked", true);
            c.Update(1000);
        }
    }

    public void ClearStatue()
    {
        if (statue != null) Destroy(statue);
        id = -1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
