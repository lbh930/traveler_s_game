using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.EventSystems;
public class SandboxManager : MonoBehaviour
{
    public struct unitInfo
    {
        public Vector3 pos;
        public Quaternion rot;
        public int id;
        public int teamIndex;
        public HumanAi currentAi;
        public bool placedAfterRunning;
    }

    IdentityList list;
    public List<unitInfo> unitsPresent;

    bool initialized = false;

    bool isPausing = false;
    public bool battleRunning = false;

    bool spawning = false;
    int spawnIndex = 0;

    int oldCount = 0;

    public Button playButton;
    public Button pauseButton;
    public Button resumeButton;
    public Button stopButton;
    public Button deleteButton;
    public Button deleteConfirmButton;
    public Button deleteCancelButton;
    public Text deleteText;

    public SoundEffect selectSound;
    public SoundEffect deleteSound;
    public SoundEffect stopSound; 
    public SoundEffect battleSound;

    public GameObject stopHint;
    
    public GameObject[] allUI;

    public RectTransform mouseHint;
    bool enableUI = true;
    // Start is called before the first frame update

    void Initialize()
    {
        if (initialized) return;
        unitsPresent = new List<unitInfo>();
        list = GameObject.FindObjectOfType<IdentityList>();
        initialized = true;
    }
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();


        // Check if mouse is over any UI element
        bool isMouseOverUI = EventSystem.current.IsPointerOverGameObject();

        if (MenuManager.nextStoryCharacterId != "sandbox") return;

        if (isPausing)
        {
            Time.timeScale = 0;
        }

        if (battleRunning)
        {
            stopHint.gameObject.SetActive(true);
        }
        else
        {
            stopHint.gameObject.SetActive(false);
        }
        for (int i = 0; i < allUI.Length; i++)
        {
            if (enableUI)
            {
                allUI[i].gameObject.SetActive(true);
            }
            else
            {
                allUI[i].gameObject.SetActive(false);
            }
        }


        //if (!battleRunning)
        //{
        //    mouseHint.gameObject.SetActive(true);
        //}
        if (Input.GetKeyDown(KeyCode.F))
        {
            ClickStop();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            enableUI = !enableUI;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!battleRunning)
            {
                ClickPlay();
            }
            else
            {
                if (isPausing)
                {
                    ClickContinue();
                }
                else
                {
                    ClickPause();
                }
            }
        }

        //每帧生成一个
        if (spawning)
        {

            while (spawnIndex < oldCount)
            {
                if (spawnIndex < unitsPresent.Count && unitsPresent[spawnIndex].placedAfterRunning)
                {//跳过战斗中才加入的
                    spawnIndex++;
                }
                else
                {
                    break;
                }
            }

            if (spawnIndex < oldCount)
            {
                GameObject unitModel = (GameObject)Instantiate(list.unitList[unitsPresent[spawnIndex].id],
                    unitsPresent[spawnIndex].pos, unitsPresent[spawnIndex].rot);
                HealthScript h = unitModel.GetComponent<HealthScript>();
                h.Initialize();
                HumanAi ai = h.GetComponent<HumanAi>();

                h.neverAsTarget = true;

                if (ai.rider != null)
                {
                    ai.rider.GetComponent<HealthScript>().neverAsTarget = true;
                }

                h.GetComponent<HumanAi>().inBattle = false;

                h.teamIndex = unitsPresent[spawnIndex].teamIndex;

                unitModel.name = list.unitList[unitsPresent[spawnIndex].id].name;

                if (ai.unitTag == null)
                {
                    ai.Initialize();
                }
                if (ai.unitTag != null)
                {
                    ai.unitTag.Show();//启用tag
                    ai.canEdit = true;
                    ai.unitTag.ForceShowTag();
                }

                Add(Algori.GetDominantUnit(ai), unitsPresent[spawnIndex].id);

                spawnIndex++;
            }

            if (spawnIndex >= oldCount)
            {
                unitsPresent.RemoveRange(0, oldCount);
                spawning = false;
            }
        }

        if (battleRunning)
        {
            deleteButton.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);

            

            if (isPausing)
            {
                resumeButton.gameObject.SetActive(true);
                pauseButton.gameObject.SetActive(false);
            }
            else
            {
                resumeButton.gameObject.SetActive(false);
                pauseButton.gameObject.SetActive(true);
            }

            playButton.gameObject.SetActive(false);
        }
        else
        {
            deleteButton.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);
            resumeButton.gameObject.SetActive(false);
            pauseButton.gameObject.SetActive(false);

            playButton.gameObject.SetActive(true);
        }
    }

    public void Add(HumanAi ai, int id)
    {
        unitInfo info = new unitInfo();
        info.pos = ai.transform.position;
        info.rot = ai.transform.rotation;
        info.id = id;
        info.currentAi = ai;
        if (ai.health != null)
            info.teamIndex = ai.health.teamIndex;
        else
            info.teamIndex = ai.GetComponent<HealthScript>().teamIndex;

        info.placedAfterRunning = battleRunning;

        unitsPresent.Add(info);
    }

    public void Refresh()
    {//删掉废用的unitspresent
        for (int i = unitsPresent.Count-1; i >= 0; i--)
        {
            if (unitsPresent[i].currentAi == null)
            {
                unitsPresent.RemoveAt(i);
            }
        }
    }

    public void ClickPlay()
    {
        if (spawning) return;//防止bug
        if (stopSound != null) battleSound.PlayAudio();
        print("clickedPlay");
        Refresh();

        battleRunning = true;
        for (int i = unitsPresent.Count - 1; i >= 0; i--)
        {
            if (unitsPresent[i].currentAi != null)
            {
      
                unitsPresent[i].currentAi.GetInBattle();
            }
        }
    }

    public void ClickPause()
    {
        isPausing = true;
        if (stopSound != null) selectSound.PlayAudio();
        Time.timeScale = 0;
    }

    public void ClickContinue()
    {
        isPausing = false;
        Time.timeScale = 1;
        if (stopSound != null) selectSound.PlayAudio();
    }

    public void ClickDelete()
    {
        deleteCancelButton.gameObject.SetActive(true);
        deleteConfirmButton.gameObject.SetActive(true);
        deleteText.gameObject.SetActive(true);
        if (stopSound != null) selectSound.PlayAudio();
    }
    public void ClickDeleteConfirm()
    {
        deleteCancelButton.gameObject.SetActive(false);
        deleteConfirmButton.gameObject.SetActive(false);
        deleteText.gameObject.SetActive(false);
        for (int i = 0; i < unitsPresent.Count; i++)
        {
            if (unitsPresent[i].currentAi != null)
            {
                if (unitsPresent[i].currentAi.horse != null)
                {
                    Destroy(unitsPresent[i].currentAi.horse.gameObject);
                }
                else
                {
                    Destroy(unitsPresent[i].currentAi.gameObject);
                }
                
            }
        }
        if (stopSound != null) deleteSound.PlayAudio();
        unitsPresent.Clear();
    }

    public void ClickDeleteCancel()
    {
        deleteCancelButton.gameObject.SetActive(false);
        deleteConfirmButton.gameObject.SetActive(false);
        deleteText.gameObject.SetActive(false);

        if (stopSound != null) selectSound.PlayAudio();
    }
    public void ClickStop()
    {
        if (!battleRunning) return;
        ClearBattlefield();

        if (stopSound != null) stopSound.PlayAudio();

        oldCount = unitsPresent.Count;
        spawning = true;
        spawnIndex = 0;
        battleRunning = false;
        Time.timeScale = 1;
        isPausing = false;
    }

    public void ClearBattlefield()
    {

        RagdollScript[] bodies = FindObjectsOfType<RagdollScript>();
        ToolScript[] tools = FindObjectsOfType<ToolScript>();
        HumanAi[] ais = FindObjectsOfType<HumanAi>();
        DamageDetection[] damage = FindObjectsOfType<DamageDetection>();

        for (int i = 0; i < bodies.Length; i++)
        {
            BodyClear b = bodies[i].gameObject.AddComponent<BodyClear>();
            b.effect = null;
            b.timeLeft = Random.Range(0.0f, 0.5f);
        }
        /*for (int i = 0; i < unitsPresent.Count; i++){
            unitsPresent[i].inBattle = false;
            BodyClear b = unitsPresent[i].gameObject.AddComponent<BodyClear>();
            b.effect = objects.objects[0];
        }*/

        for (int i = 0; i < tools.Length; i++)
        {
            BodyClear b = tools[i].gameObject.AddComponent<BodyClear>();
            b.effect = null;
            b.timeLeft = Random.Range(0.0f, 0.5f);
        }
        for (int i = 0; i < ais.Length; i++)
        {
            BodyClear b = ais[i].gameObject.AddComponent<BodyClear>();
            b.effect = null;
            b.timeLeft = Random.Range(0.0f, 0.5f);
            ais[i].enabled = false;
            ais[i].target = null;
            ais[i].GetComponent<Animator>().speed = 0;
            
            if (ais[i].health == null)
            {
                ais[i].GetComponent<HealthScript>().SetPhysicsActive(false);
            }
            else
            {
                ais[i].health.SetPhysicsActive(false);
            }
        }

        for (int i = 0; i < damage.Length; i++)
        {
            damage[i].enabled = false;
            Destroy(damage[i].gameObject);
        }
    }
}
