using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    // Start is called before the first frame update
    public RawImage fadeImage;
    [HideInInspector] public bool fade;
    [HideInInspector] public float fadeTime;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b,
                Mathf.MoveTowards(fadeImage.color.a, fade ? 1 : 0, Time.deltaTime * (1 / fadeTime)));
        }
    }
}
