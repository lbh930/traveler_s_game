using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ending_One_Trigger : MonoBehaviour
{
    public int triggerDayNumber = 57;
    public int triggerDayNumber2 = 56;
    public int triggerStoryIndex = 2;
    public int dayAfterwards = 58;
    public int dayNow;
    TxtReader save;
    bool initialized = false;
    bool endingStarted = false;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
        EndingStarter();
    }

    void Initialize()
    {
        save = GetComponent<TxtReader>();
        save.Read(Application.streamingAssetsPath, "Save.txt", ';');


        initialized = true;
    }

    void EndingStarter()
    {
        if (endingStarted) return;
        int storyIndexNow = save.getInt(0, 0);//先读出来现在的index
        int storyIndexNow2 = save.getInt(0, 1);
        dayNow = storyIndexNow;
        if (storyIndexNow == triggerDayNumber || (storyIndexNow == triggerDayNumber2 && storyIndexNow2 == triggerStoryIndex))
        {
            print("starting ending 1");
            endingStarted = true;
            save.Write(0, 0, (dayAfterwards).ToString(), "Save.txt", ';');
            SceneManager.LoadScene("Ending1");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) Initialize();
        EndingStarter();
    }
}
