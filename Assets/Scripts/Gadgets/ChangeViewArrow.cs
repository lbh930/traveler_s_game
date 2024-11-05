using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeViewArrow : MonoBehaviour
{
    public GameObject arrowStateReceiver; //用这个gameobject接收状态
    public SpriteRenderer sprite;
    public Sprite forward;
    public Sprite backward;
    public Transform frame;
    public float maxScale = 0.8f;
    public float minScale = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
        transform.localScale = Vector3.one * 0.49f;
    }

    // Update is called once per frame
    void Update()
    {
        if (arrowStateReceiver.active)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(sprite.color.a, 1, Time.deltaTime * 8));
            transform.localScale = Vector3.Slerp(transform.localScale, Vector3.one * maxScale, Time.deltaTime * 6);
        }
        else
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.Lerp(sprite.color.a, 0, Time.deltaTime * 8));
            transform.localScale = Vector3.Slerp(transform.localScale, Vector3.one * minScale, Time.deltaTime * 6);
        }

        if (Vector3.Dot(Camera.main.transform.forward, transform.up) > 0)
        {
            sprite.sprite = forward;

        }
        else
        {
            sprite.sprite = backward;
        }

        if (frame != null)
        {
            transform.position = frame.transform.position + (frame.forward * (frame.localScale.z/2 + 0.1f)) + Vector3.up * 0.1f;
        }
    }
}
