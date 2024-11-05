using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPhoto : MonoBehaviour
{
    public Animator ani;
    public Texture[] photos;
    public RawImage image;

    bool timerSet = false;

    float photoTimer = float.PositiveInfinity;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        photoTimer -= Time.deltaTime;   
        if (photoTimer < 0)
        {
            timerSet = false;
            image.texture = photos[index];
            if (index == 2 || index == 3)
            {
                image.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 600);
            }
            //print(index);
        } 
    }

    public void SetPhoto (int indexx)
    {
        print("photo set to: " + indexx.ToString());
        index = indexx;
        if (!timerSet)
        {
            timerSet = true;
            photoTimer = 0.2f;
        }
        ani.SetTrigger("ShowPhoto");
    }

    public void RemovePhoto()
    {
        ani.SetTrigger("RemovePhoto");
    }
}
