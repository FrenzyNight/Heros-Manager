using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ForestCSVReader : MonoBehaviour
{
    // Cave

    public float varSpeed;
    public float coolTime;

    public float downStoneStunTime;
    public float upStoneStunTime;
    public float downStonePer;
    public float upStonePer;
    
    public float jemResource;
    public float jemPer;

    public float charJump;

    void Awake()
    {
        CSVRead();
    }

    void CSVRead()
    {
        StreamReader sr = new StreamReader(Application.dataPath + "/" + "Resources/CSVData/MiniGameData.csv");

        int row = 0;
        bool endOfFile = false;
        while(!endOfFile)
        {
            row ++;
            string data_String = sr.ReadLine();
            if(data_String == null)
            {
                endOfFile = true;
                break;
            }

            var data_values = data_String.Split(',');

           
            if(row == 2) // 속도와 쿨타임
            {
                varSpeed = float.Parse(data_values[11]);
                coolTime = float.Parse(data_values[12]);

                charJump = float.Parse(data_values[14]);
            }
            else if(row == 3) // 하단부 바위
            {
                downStonePer = float.Parse(data_values[8]) * 100;
                downStoneStunTime = float.Parse(data_values[9]);
            }
            else if(row == 4) // 상단부 바위
            {
                upStonePer = float.Parse(data_values[8]) * 100;
                upStoneStunTime = float.Parse(data_values[9]);
            }
            else if(row == 5) // 보석
            {
                jemResource = float.Parse(data_values[7]);
                jemPer = float.Parse(data_values[8]) * 100;
            }
            
        }
    }
}
