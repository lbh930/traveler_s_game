using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticePoper : MonoBehaviour
{
    // Start is called before the first frame update
    bool initialized = false;
    TxtReader txt;
    AudioSource audio;
    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        if (initialized) return;
        txt = GetComponent<TxtReader>();
        txt.Read(Application.streamingAssetsPath, "notice.txt", ';');
        initialized = true;
    }

    // Update is called once per frame
    void Update()
    {
        Initialize();
    }

    public static void PopNotice(int line)
    {
        GameObject g = GameObject.FindGameObjectWithTag("NoticeCanvas");//������Ȼ��static����Ҳ����instance
        if (g != null)
        {
            Animator ani = g.GetComponent<Animator>();
            TxtReader reader = g.GetComponent<TxtReader>();
            AudioSource audio = g.GetComponent<AudioSource>();
            AudioList alist = g.GetComponent<AudioList>();

            string a = "��Ϣ"; //�õ���Ӧ���ı� //0����Ƶ���֣�1�����ģ�2��Ӣ��

            int language = PlayerPrefs.GetInt("language");

            if (language == 0 || language == 1)
            {
                //����
                a = reader.getString(line, 1);
            }
            else
            {
                //Ӣ��
                a = reader.getString(line, 2);
            }

            ani.SetTrigger("pop");
            Text text = GameObject.FindGameObjectWithTag("NoticeText").GetComponent<Text>();
            text.text = a;//��ֵ

            if (audio != null && alist != null && alist.clips.Length > 0)
            {
                for (int i = 0; i < alist.clips.Length; i++)
                {
                    if (alist.clips[i].name.Contains(reader.getString(line, 0)))
                    {
                        audio.clip = alist.clips[i];
                        audio.volume = PauseScript.mastervolume * PauseScript.soundfx;
                        audio.Play();
                        break;
                    }
                }
            }
        }
        
    }
}
