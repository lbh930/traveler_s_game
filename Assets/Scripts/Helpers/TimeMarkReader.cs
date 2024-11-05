using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMarkReader : MonoBehaviour
{
    // Start is called before the first frame update
    public TxtReader csv_read;
    public TxtReader csv_write;
    public TxtReader csv_plot;
    public string name;
    public string name2;
    public string plotfile;
    public string plotname;
    public string folderName;
    public string otherName = "lzq";
    public double[] times;
    public double[] times2;
    public int pointer = 0;
    public int pointer2 = 0;
    public int p = 0;

    public int linesAfterPlotId = 2;  
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            pointer = 0;
            times = new double[1000];
            times2 = new double[1000];

            csv_read.Read(Application.streamingAssetsPath + "/convert/" + folderName, name);

            for (int i = 1; i < csv_read.m_ArrayData.Count; i++)
            {
                string a = csv_read.getString(i, 0);
                string b = "标记 ";
                /*if (i < 10)
                {
                    b += "0";
                }else if (i < 100)
                {
                   
                }else if (i < 1000)
                {

                }
                b += i.ToString();*/

                //b += " ";

                a = a.Replace(b, "");
                for (int j = 0; j < a.Length; j++)
                {
                    if (a[j] - '0' < 0 || a[j] - '9' > 0)
                    {
                        a = a.Remove(0, j + 1);
                        break;
                    }
                }

                //int index = 0;
                int minute = 0;
                int second = 0;
                //int unit = 0;

                string c = a.Split(':')[0];
                string d = a.Split(':')[1];
                d = d.Remove(6);
                d = d.Remove(2, 1);

                minute = int.Parse(c);
                second = int.Parse(d);

                double time = (double)minute* 60 + (double)second/1000;

                times[pointer++] = time;
            }

            //////////////////////////////////////////////////////////////////

            csv_read.Read(Application.streamingAssetsPath + "/convert/" + folderName, name2);
            for (int i = 1; i < csv_read.m_ArrayData.Count; i++)
            {
                string a = csv_read.getString(i, 0);
                string b = "标记 ";
                /*if (i < 10)
                {
                    b += "0";
                }else if (i < 100)
                {
                   
                }else if (i < 1000)
                {

                }
                b += i.ToString();*/

                //b += " ";

                a = a.Replace(b, "");
                for (int j = 0; j < a.Length; j++)
                {
                    if (a[j] - '0' < 0 || a[j] - '9' > 0)
                    {
                        a = a.Remove(0, j + 1);
                        break;
                    }
                }

                //int index = 0;
                int minute = 0;
                int second = 0;
                //int unit = 0;

                string c = a.Split(':')[0];
                string d = a.Split(':')[1];
                d = d.Remove(6);
                d = d.Remove(2, 1);

                print(c);
                minute = int.Parse(c);
                second = int.Parse(d);

                double time = (double)minute * 60 + (double)second / 1000;

                times2[pointer2++] = time;
            }
            p = 0;

            for (int i = 0; i < pointer2-1; i++)
            {
                for (int j = i+1; j < pointer2; j++)
                {
                    if (times2[i] > times2[j])
                    {
                        double temp = times2[i];
                        times2[i] = times2[j];
                        times2[j] = temp;
                    }
                }
            }

            csv_plot.Read(Application.streamingAssetsPath, plotfile);

            print("ok1");

            for (p=0; p < csv_plot.m_ArrayData.Count; p++)
            {
                if (csv_plot.getString(p, 0) == "*plot")
                {
                    print(csv_plot.getString(p, 1));
                    print(plotname);
                }
                if (csv_plot.getString(p,0) == "*plot" && csv_plot.getString(p,1) == plotname)
                {
                    print("ok1.5");
                    break;
                }
            }

            print("ok2");
            p += 1;

            pointer = 0;
            pointer2 = 0;
            int pointer3 = 0; //for written file
            p += linesAfterPlotId;


            for (p = p; p<csv_plot.m_ArrayData.Count; p++)
            {
                if (csv_plot.getString(p, 0) == "zrg")
                {
                    string toWrite = times[pointer].ToString() + ";" + times[pointer + 1].ToString() + "\n";
                    print("zrg " + toWrite);
                    csv_write.Write((pointer3++), 0, toWrite, "convert/" + folderName + "/" + name + "_con.csv", ';', true);
                    pointer+=2;

                }else if (csv_plot.getString(p, 0) == "*plot")
                {
                    break;
                }
                else if (csv_plot.getString(p,0) == otherName)
                {
                    string toWrite = times2[pointer2].ToString() + ";" + times2[pointer2 + 1].ToString() + "\n";
                    print("other " + toWrite);
                    csv_write.Write((pointer3++), 0, toWrite, "convert/" + folderName + "/" + name + "_con.csv", ';', true);
                    pointer2+=2;
                }
                else //如果这一行是指令行
                {
                    pointer3++;
                }
            }

            for (int i = 0; i < pointer; i+=2)
            {/*
                if (i % 2 == 0)
                {
                    csv_write.Write((i) / 2, 0, times[i].ToString() + ",", "convert/" + name + "_con.csv");
                }
                else
                {
                    csv_write.Write((i) / 2, 1, times[i].ToString(), "convert/" + name + "_con.csv");
                }*/
                
            }
        }
    }
}
