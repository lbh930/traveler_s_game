using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    bool initialized = false;
    public TravelerController controller;
    InventoryScript inventory;
    public CanvasManager canvas;
    CraftScript craft;
    [HideInInspector] public Animator animator;

    public bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }

    void Initialize()
    {
        if (controller == null) controller = GameObject.FindObjectOfType<TravelerController>();
        canvas = GameObject.FindObjectOfType<CanvasManager>();
        inventory = controller.GetComponent<InventoryScript>();
        craft = controller.GetComponent<CraftScript>();
        animator = controller.GetComponent<Animator>();

        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) Initialize();

        if (!canMove)
        {
            animator.SetFloat("WalkX", Mathf.MoveTowards(animator.GetFloat("WalkX"), 0, Time.deltaTime * 3.5f));
            animator.SetFloat("WalkY", Mathf.MoveTowards(animator.GetFloat("WalkY"), 0, Time.deltaTime * 3.5f));
            animator.SetBool("Attack", false);
        }
    }

    public void SetMovable(bool movable)
    {
        canMove = movable;
        controller.enabled = movable;
        inventory.enabled = movable;
        craft.enabled = movable;
    }
}
