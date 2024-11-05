using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SandboxButton : MonoBehaviour
{
    // Start is called before the first frame update
    public string sceneName = "Suzhou_Garden";
    public SoundEffect sound;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        if (sound != null) sound.PlayAudio();
        MenuManager.nextStoryCharacterId = "sandbox";
        MenuManager.actualCharaterId = "sandbox";
        SceneManager.LoadScene(sceneName);
    }
}
