using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HuntManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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

            if(row == 2) // 속도와 쿨타임
            {
                
            }
            
        }
    }
}
