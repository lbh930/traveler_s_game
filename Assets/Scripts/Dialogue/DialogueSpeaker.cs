using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueSpeaker : MonoBehaviour
{
    bool initialized = false;
    public string idName;
    DialogueManager manager;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();

    }
    void Initialize()
    {
        initialized = true;
        manager = FindObjectOfType<DialogueManager>();
        for (int i = 0; i < manager.speakerInfo.Length; i++)
        {
            if (manager.speakerInfo[i].idName == idName)
            {
                manager.speakerInfo[i].relatedSpeaker = this;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();
    }
}
