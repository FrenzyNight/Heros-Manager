using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectData : Singleton<ObjectData>
{
    public ConvertStorageFenceData<FenceData> convertStorage_FenceData;
    private string sz_pathCSV_Fence = "CSVData/";

    public void Setup()
    {
        if (convertStorage_FenceData == null)
            convertStorage_FenceData = new ConvertStorageFenceData<FenceData>();
        convertStorage_FenceData.ConvertCSVToClass(sz_pathCSV_Fence);


    }
}

public class ConvertStorageFenceData<T> : ConvertStorageBase<T>
{
    public List<FenceData> fanceDataList = new List<FenceData>();

    public override void Setup(string stageName)
    {
        //base.Setup(stageName);
    }

    public override void ConvertCSVToClass(string csvPath)
    {
        //base.ConvertCSVToClass(csvPath);

        List<Dictionary<string, object>> data = CSVReader.Read(csvPath);

        for (int i = 0; i < data.Count; i++)
        {
            FenceData fenceData = new FenceData();
            fenceData.temp1 = ConvertToInt(data[i][""]);
            fenceData.temp2 = ConvertToString(data[i][""]);
            fenceData.temp3 = ConvertToFloat(data[i][""]);

            fanceDataList.Add(fenceData);
        }
    }
}

[Serializable]
public class FenceData
{
    public int temp1;
    public string temp2;
    public float temp3;
}