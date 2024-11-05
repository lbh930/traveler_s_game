using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseCredit : MonoBehaviour
{
    public float minY;
    public float maxY;
    public RectTransform rect;
    float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, Mathf.MoveTowards(rect.anchoredPosition.y, maxY,Time.unscaledDeltaTime*(maxY-minY)/30*speed));
        if (Input.anyKey)
        {
            speed = 4;
        }
        else
        {
            speed = 1;
        }

        if (Mathf.Abs(rect.anchoredPosition.y - maxY) < 0.01f || Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false); //put self to sleep
        }
    }

    void OnEnable()
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, minY);
    }
}
