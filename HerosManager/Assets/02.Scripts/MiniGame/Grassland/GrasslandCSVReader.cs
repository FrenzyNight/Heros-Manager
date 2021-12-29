using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GrasslandCSVReader : MonoBehaviour
{
    //Hub
    public float standardHubSpeed;
    public float hubCoolTime;
    public float hubSpanTime;
    public float hubGetTime;
    

    public float hubGoldPer;
    public float hubGoldRes;
    public float hubNormalRes;

    public float hubCharSpeed;

    //Hunt
    public float standardHuntSpeed;
    public float huntArrowSpeed;
    public float huntArrowCoolTime;

    void Awake()
    {
        CSVRead();
        standardHubSpeed = 10f;
        standardHuntSpeed = 5f;
    }

    void Update()
    {
        
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

            if(row == 8) // 화살관련 변수
            {
                huntArrowSpeed = float.Parse(data_values[11]);
                huntArrowCoolTime = float.Parse(data_values[12]);
            }

            else if(row == 11) // 허브 환경변수
            {
                hubNormalRes = float.Parse(data_values[6]);
                hubCoolTime = float.Parse(data_values[12]);
                hubSpanTime = float.Parse(data_values[15]);
                hubGetTime = float.Parse(data_values[16]);

                hubCharSpeed = float.Parse(data_values[14]);
            }

            else if(row == 12) // 황금허브 확률
            {
                hubGoldRes = float.Parse(data_values[6]);
                hubGoldPer = float.Parse(data_values[8]) * 100;
            }

            
        }
    }
}
