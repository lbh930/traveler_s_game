using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public class ChineseFontChanger : MonoBehaviour
{
    private const int LOCALE_SYSTEM_DEFAULT = 0x0800;
    private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
    private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

    public Font simplified;
    public Font tranditional;

    //Text[] texts;

    bool initialized = false;

    [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);

    /// <summary>
    /// 讲字符转换为繁体中文
    /// </summary>
    /// <param name="source">输入要转换的字符串</param>
    /// <returns>转换完成后的字符串</returns>
    public static string ToTraditional(string source)
    {
        String target = new String(' ', source.Length);
        int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, source, source.Length, target, source.Length);
        return target;
    }

    public static string ToSimplified(string source)
    {
        String target = new String(' ', source.Length);
        int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_SIMPLIFIED_CHINESE, source, source.Length, target, source.Length);
        return target;
    }

    void Initialize()
    {
        if (initialized) return;

        ChangeFont();

        initialized = true;
    }

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        Initialize();
    }

    public void ChangeFont()
    {
        int language = PlayerPrefs.GetInt("language");
        Text[] texts = FindObjectsOfType<Text>(true);

        for (int i = 0; i < texts.Length; i++)
        {
            if (language == 0)
            {
                texts[i].font = simplified;
                texts[i].text = ToSimplified(texts[i].text);
            }
            else if (language == 1)
            {
                texts[i].font = tranditional;
                texts[i].text = ToTraditional(texts[i].text);
            }
            else
            {
                texts[i].font = simplified;
            }
            //if (PauseScript.)
            //texts[i].
        }
    }
}


