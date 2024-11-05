using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueBox : MonoBehaviour
{
    public Button avatar;
    public RawImage avatarFrame;
    public Button textFrame;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public float xm = 0.1f;
    public float ym = 0.1f;
    bool initialized = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }
    void Initialize()
    {
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
    }
}
