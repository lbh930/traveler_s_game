using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCanvasAnimatorEvent : MonoBehaviour
{
    // Start is called before the first frame update
    IdentityList id;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GeneratePotraits()
    {
        GameObject.FindObjectOfType<MenuManager>().GeneratePotraits();
    }

    public void DestroyIcons()
    {
        GameObject.FindObjectOfType<MenuManager>().DestoryPotraits();
    }

    public void PreLoadLevel()
    {
        GetComponent<Animator>().SetTrigger("LoadDone");
    }

    public void LoadLevel()
    {
        id = GameObject.FindObjectOfType<IdentityList>();
        int i = Random.Range(0, id.sceneList.Length);
        i = Random.Range(0, id.sceneList.Length);
        i = Random.Range(0, id.sceneList.Length);

        SceneManager.LoadScene(id.sceneList[i]);

    }
}
