using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDeleteSound : MonoBehaviour
{
    public AudioSource deleteAudio;
    public AudioSource placeAudio;
    public AudioSource controlAudio;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Delete()
    {
        deleteAudio.transform.parent = null;
        deleteAudio.Play();
        deleteAudio.transform.GetComponent<AutoDestroy>().timeToDestruct = 4;
        print("playdeletesound");
    }

    public void Place()
    {
        placeAudio.Play();
        placeAudio.transform.SetParent(null);
        placeAudio.transform.GetComponent<AutoDestroy>().timeToDestruct = 4;
    }

    public void Control()
    {
        controlAudio.Play();
    }
}
