using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class UnitTag : MonoBehaviour
{
    bool initialized = false;
    SpriteRenderer renderer;
    LookAtConstraint lookat;
    HumanAi ai;
    public Sprite tag;
    public Sprite tagDeletable;
    public Sprite tagForbidden;
    public Sprite blank;

    public bool show = false;

    public Color team0Color = Color.blue;
    public Color team1Color = Color.red;

    [HideInInspector] public bool dontShow = false;
    void Initialize()
    {
        initialized = true;
        dontShow = false;
        renderer = GetComponent<SpriteRenderer>();
        lookat = gameObject.GetComponent<LookAtConstraint>();
        ai = transform.parent.GetComponent<HumanAi>();
        transform.localScale = Vector3.zero;
        lookat.weight = 1;

        ConstraintSource source = new ConstraintSource();
        source.sourceTransform = Camera.main.transform;
        source.weight = 1;
        lookat.AddSource(source);
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) Initialize();

        if (show && !dontShow)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one*0.35f, Time.unscaledDeltaTime * 6);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.unscaledDeltaTime * 6);
        }

        if (ai != null)
        {
            if (ai.health != null)
            {
                if (ai.health.teamIndex == 0)
                {
                    renderer.color = team0Color;
                    renderer.flipX = false;
                }
                else
                {
                    renderer.color = team1Color;
                    renderer.flipX = true;
                }
            }
        }
    }

    public void PointerEnter()
    {
        if (ai == null) return;

        dontShow = false;

        if (transform.localScale.magnitude < 0.01f)
        {
            transform.localScale = Vector3.one * 0.35f;
        }

        if (ai.canEdit)
        {
            renderer.sprite = tagDeletable;
        }
        else
        {
            renderer.sprite = tagForbidden;
        }
    }

    public void Show()
    {
        show = true;
    }

    public void Close()
    {
        show = false;
    }

    public void ForceShowTag()
    {
        if (renderer == null) Initialize();
        renderer.sprite = tag;
    }

    public void PointerExit()
    {
        if (ai == null) Initialize();
        if (ai == null) return;
        
        if (ai.canEdit)
        {
            renderer.sprite = tag;
        }
        else
        {
            renderer.sprite = blank;
        }
    }
}
