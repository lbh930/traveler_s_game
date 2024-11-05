using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TxtReader : MonoBehaviour
{
    public List<string[]> m_ArrayData = new List<string[]>();

    public StreamReader sr = null;
    [HideInInspector]public int lineCount;

    void Update()
    {

    }
    public string getString(int row, int col)
    {
        if (row >= m_ArrayData.Count || col >= m_ArrayData[row].Length)
        {
            return "";
        }
        return m_ArrayData[row][col].Replace("\\n", "\n");
    }

    public int getInt(int row, int col)
    {

        if (m_ArrayData[row].Length <= col) return 0;
        else return int.Parse(m_ArrayData[row][col]);
    }

    public float getFloat(int row, int col)
    {
        if (col < m_ArrayData[row].Length)
        {
            return float.Parse(m_ArrayData[row][col]);
        }
        else
        {
            return -1;
        }
    }

    public void ResetStory()
    {
        int winNow = 0;
        Read(Application.streamingAssetsPath, "Save.txt", ';');
        winNow = getInt(0, 2);//记录下来

        File.Delete(Application.persistentDataPath + "/Save.txt");//删除
        File.Copy(Application.streamingAssetsPath + "/Save.txt", Application.persistentDataPath + "/Save.txt");//新建

        print("file reseted");

        Read(Application.streamingAssetsPath, "Save.txt", ';');
        Write(0, 2, winNow.ToString(), "Save.txt", ';', false);

        SceneManager.LoadScene("Menu");
    }
    public void Read(string path, string fileName, char split = ';')
    {
        
        if (fileName == "Save.txt") { //存档位置改掉

            if (File.Exists(Application.persistentDataPath
                   + "/Save.txt"))
            {
            }
            else //创建
            {
                File.Copy(Application.streamingAssetsPath + "/Save.txt", Application.persistentDataPath + "/Save.txt");
                print("file created");
            }
            path = Application.persistentDataPath;
        }

        m_ArrayData.Clear();      
        try
        {
           // print("TextReader_Reading: " + path + "/" + fileName);
            sr = File.OpenText(path + "/" + fileName);       
        }
        catch
        {
            return;
        }
        string line;
        lineCount = 0;
        while ((line = sr.ReadLine()) != null)
        {
            m_ArrayData.Add(line.Split(split));
            lineCount++;
        }
        sr.Close();
        sr.Dispose();
    }

    public void Write (int line, int col, string content, string fileName, char separation = ',', bool writeToConvert = false)
    {
        if (fileName == "Save.txt")
        { //存档位置改掉

            if (File.Exists(Application.persistentDataPath
                   + "/Save.txt"))
            {
            }
            else //创建
            {
                File.Copy(Application.streamingAssetsPath + "/Save.txt", Application.persistentDataPath + "/Save.txt");
                print("file created on write");
            }
        }

        string path = Application.persistentDataPath;
        if (writeToConvert)
        {
            path = Application.streamingAssetsPath;
        }

        Read(path, fileName, separation);
        StreamWriter file;
        file = File.CreateText(path + "/" + fileName);

        for (int i = 0; i < Mathf.Max(m_ArrayData.Count, line+1); i++)
        {
            string toWrite = "";
            for (int j = 0; j < Mathf.Max(i < m_ArrayData.Count ? m_ArrayData[i].Length:col+1, col+1); j++)
            {
                
                if (i == line && j == col)
                {
                    toWrite += content;
                    toWrite += separation;

                }
                else
                {
                    if (i < m_ArrayData.Count && j < m_ArrayData[i].Length)
                    {
                        toWrite += m_ArrayData[i][j];
                        if (toWrite.Length > 0 && toWrite[toWrite.Length-1] != separation)//防止分隔符重叠
                            toWrite += separation;

                    }
                    else
                    {
                        toWrite += "";
                        toWrite += separation;

                    }
                }
            }

            file.WriteLine(toWrite);
        }

        file.Close();
            //file.Write()
    }
}


