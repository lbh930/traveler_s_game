using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]public struct SpeakerInfo
    {
        public string idName;
        public string displayName;
        public string[] avatarNames;
        public Sprite[] avatars;
        public DialogueSpeaker relatedSpeaker;
    }
    public Canvas canvas;
    public SpeakerInfo[]  speakerInfo;
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
