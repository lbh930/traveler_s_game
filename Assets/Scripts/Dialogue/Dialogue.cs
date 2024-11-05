using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TxtReader))]
public class Dialogue : MonoBehaviour
{
    bool initialized = false;
    bool dialogueStarted = false;
    public GameObject boxPrefab;
    DialogueBox currentBox;
    public Transform speakerMappings;
    public string fileName;
    public int startLine;
    public EventMachine triggerAfterDialogue;
    string desiredText;
    string currentText;
    bool atEnd = false;
    float lastTypeTime;
    float secPerLetter = 0.1f;
    
    int currentLine = 0;
    Transform currentSpeaker;
    DialogueManager manager;
    TxtReader reader;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
        reader = GetComponent<TxtReader>();
        manager = GameObject.FindObjectOfType<DialogueManager>();
    }
    void Initialize()
    {
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)Initialize();

        if (dialogueStarted) CheckDialogue();

        if (Input.GetKeyDown(KeyCode.T)) StartDialogue();
    }
    void LateUpdate()
    {
        if (currentSpeaker != null && currentBox != null)
        {
            Vector2 center = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 pos = Camera.main.WorldToScreenPoint(currentSpeaker.position);

            Vector2 desiredPos = pos;
            
            if (pos.x < Screen.width * 0.3f) desiredPos += new Vector2(Screen.width * currentBox.xm, 0);
            else desiredPos += new Vector2(-1 * Screen.width * currentBox.xm, 0);
            if (pos.y > Screen.height * 0.7f) desiredPos += new Vector2(0, -1*Screen.height * currentBox.ym);
            else desiredPos += new Vector2(0, 1 * Screen.height * currentBox.ym);
            //print(desiredPos);

            currentBox.GetComponent<RectTransform>().position = Vector2.Lerp(currentBox.transform.position,
                desiredPos, Time.deltaTime * 10);
        }
    }
    void CheckDialogue()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentText.Length < desiredText.Length)
            {
                currentText = desiredText;
            }
            else
            {
                currentLine++;
                ReadLine();
            }
        }

        if (currentText.Length < desiredText.Length)
        {
            int toAdd = Mathf.RoundToInt((Time.time - lastTypeTime) / secPerLetter);
            for (int i = 0; i < toAdd; i++)
            {
                if (toAdd <= 0) break;
                currentText += desiredText[currentText.Length];
                if (currentText.Length == desiredText.Length) break;
                lastTypeTime = Time.time;
            }
        }

        if (currentBox != null)
            currentBox.dialogueText.text = currentText;
    }
    public void StartDialogue()
    {
        currentBox = Instantiate(boxPrefab, manager.canvas.transform).GetComponent<DialogueBox>();
        currentBox.transform.SetParent(manager.canvas.transform);
        currentLine = startLine;

        int language = PlayerPrefs.GetInt("language");

        if (language == 2)
        { //英语？
            reader.Read(Application.streamingAssetsPath, fileName + "_en");
        }
        else
        {
            reader.Read(Application.streamingAssetsPath, fileName);
        }

        if (reader.m_ArrayData.Count <= 0)
        {//如果英文读取失败
            reader.Read(Application.streamingAssetsPath, fileName);
        }

        ReadLine();
        dialogueStarted = true;
    }

    public void ReadLine()
    {
        if (atEnd)
        {
            atEnd = false;
            dialogueStarted = false;
            if (triggerAfterDialogue != null)
                triggerAfterDialogue.working = true;
            Destroy(currentBox.gameObject);
            return;
        }

        currentText = "";
        desiredText = reader.getString(currentLine - 1, 2);
        //print(desiredText);
        desiredText += "      ";

        //检测是否结尾
        if (desiredText.Contains("[end]"))
        {
            atEnd = true;
            desiredText = desiredText.Replace("[end]", "");
        }

        float letterPerSec = reader.getFloat(currentLine - 1, 3);
        secPerLetter = letterPerSec > 0 ? 1 / reader.getFloat(currentLine - 1, 3) : 10;
        lastTypeTime = Time.time;

        //找到speaker并换头像
        string idName = reader.getString(currentLine - 1, 0);
        string avatarName = reader.getString(currentLine - 1, 1);
        
        for (int i = 0; i < manager.speakerInfo.Length; i++)
        {
            if (manager.speakerInfo[i].idName == idName)
            {
                currentSpeaker = manager.speakerInfo[i].relatedSpeaker.transform;
                currentBox.nameText.text = manager.speakerInfo[i].displayName;
                for (int j = 0; j < manager.speakerInfo[i].avatarNames.Length; j++)
                {
                    if (avatarName == manager.speakerInfo[i].avatarNames[j]) {
                        currentBox.avatar.image.sprite = manager.speakerInfo[i].avatars[j];
                    }
                }            
            }
        }
    }
}
