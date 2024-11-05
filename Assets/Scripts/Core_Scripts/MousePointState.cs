using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointState : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    public TravelerController controller;
    public float maxDistance = Mathf.Infinity;
    public bool pointing;
    public bool holding;
    public bool clicked;
    
    void Initialize()
    {       
        if (initialized) return;
        controller = GameObject.FindObjectOfType<TravelerController>();
        initialized = true;
    }

    void Start()
    {
        Initialize();
        
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast (ray, out hit, maxDistance))
        {
            clicked = false;
            holding = false;
            pointing = false;

            if (hit.collider.gameObject == gameObject)
            {
                pointing = true;
                if (Input.GetMouseButton(0))
                {
                    holding = true;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    clicked = true;
                }
            }
        }
    }
}
