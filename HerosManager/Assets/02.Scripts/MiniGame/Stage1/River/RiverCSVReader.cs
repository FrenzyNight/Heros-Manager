using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class RiverCSVReader : MonoBehaviour
{
    //fish
    public float standardFishSpeed;

    public float fishLogSpeed;

    public float fishCoolTime;
    public float fishVarSpeed;

    public float fishGoldPer;
    public float fishGoldRes;

    public float fishNormalRes;

    //water
    public float standardWaterSpeed;

    public float waterRes;
    public float waterGetTime;

    public float waterRockSpeed;
    public float waterRockCoolTime;
    public float waterRockMaxNum;

    public float waterRockInvTime;

    

    void Awake()
    {
        CSVRead();
        standardFishSpeed = 1f;
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

            if(row == 13) //water
            {
                waterRes = float.Parse(data_values[4]);

                waterGetTime = float.Parse(data_values[14]);
            }

            else if(row == 14) //rock
            {
                waterRockCoolTime = float.Parse(data_values[12]);
                waterRockInvTime = float.Parse(data_values[10]);
                waterRockMaxNum = float.Parse(data_values[15]);
                waterRockSpeed = float.Parse(data_values[11]);
            }

            else if(row == 15) // fish 공용, 일반
            {
                fishNormalRes = float.Parse(data_values[5]);

                fishVarSpeed = float.Parse(data_values[11]);
                fishCoolTime = float.Parse(data_values[12]);
            }
            else if(row == 16) // goldfish
            {
                fishGoldRes = float.Parse(data_values[5]);
                fishGoldPer = float.Parse(data_values[8]) * 100;
            }

            else if(row == 17) //Log
            {
                fishLogSpeed = float.Parse(data_values[11]);
            }
                

            
        }
    }
}
