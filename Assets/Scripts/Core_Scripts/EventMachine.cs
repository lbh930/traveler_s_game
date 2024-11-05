using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventMachine : MonoBehaviour
{
    bool initialized = false;
    EventManager eventManager;

    public bool working = false;
    public Transform[] references;
    [TextArea(50, 20)]
    public string script;

    int pointer = 0;
    float timer;
    string[] commands;

    Vector3 savedPosition;
    Quaternion savedRotaion;
    // Start is called before the first frame update
    void Start()
    {
        if (!initialized) Initialize();
    }
    void Initialize()
    {
        initialized = true;
        commands = script.Split('\n');
        if (eventManager == null)
        {
            eventManager = transform.parent.GetComponent<EventManager>();
        }
        savedPosition = eventManager.controller.transform.position;
        savedRotaion = eventManager.controller.transform.rotation;
    }

    void Work()
    {
        while (true && timer <= 0)
        {
            if (pointer >= commands.Length)
            {
                working = false;
                break;
            }

            if (commands[pointer].Contains("no move"))
            {
                eventManager.SetMovable(false);
            }
            else if (commands[pointer].Contains("move"))
            {
                eventManager.SetMovable(true);
            }
            else if (commands[pointer].Contains("wait"))
            {
                string[] command = commands[pointer].Split(' ');
                timer = float.Parse(command[1]);
            }
            else if (commands[pointer].Contains("ani"))
            {
                
                string[] command = commands[pointer].Split(' ');
                print(command);
                if (command[2][0] == 't')
                {
                    eventManager.animator.SetBool(command[1], true);
                }
                else if (command[2][0] == 'f')
                {
                    eventManager.animator.SetBool(command[1], false);
                }
                else if (command[2].Contains ("."))
                {
                    eventManager.animator.SetFloat(command[1], float.Parse(command[2]));
                }
                else
                {
                    eventManager.animator.SetFloat(command[1], int.Parse(command[2]));
                }
            }
            else if (commands[pointer].Contains("saveTrans"))
            {
                savedPosition = eventManager.controller.transform.position;
                savedRotaion = eventManager.controller.transform.rotation;
            }
            else if (commands[pointer].Contains ("revertTrans"))
            {
                eventManager.controller.transform.position = savedPosition;
                eventManager.controller.transform.rotation = savedRotaion;
            }
            else if (commands[pointer].Contains("mouse"))
            {
            }
            else if (commands[pointer].Contains ("cam no force"))
            {
                Camera.main.GetComponent<TravelerCamera>().forceFollow = null;
            }
            else if (commands[pointer].Contains("cam force"))
            {
                string[] command = commands[pointer].Split(' ');
                Camera.main.GetComponent<TravelerCamera>().forceFollow
                    = references[int.Parse(command[2])];
            }
            else if (commands[pointer].Contains("fade"))
            {
                string[] command = commands[pointer].Split(' ');
                int fade = int.Parse(command[1]);
                if (fade == 1)
                    eventManager.canvas.fade = true;
                else
                    eventManager.canvas.fade = false;
                eventManager.canvas.fadeTime = float.Parse(command[2]);
            }else if (commands[pointer].Contains("dialogue"))
            {
                Dialogue dia = GetComponent<Dialogue>();
                if (dia != null) dia.StartDialogue();
            }

            pointer++;
        }
        timer -= Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized) Initialize();

        if (working) Work();
    }
}
