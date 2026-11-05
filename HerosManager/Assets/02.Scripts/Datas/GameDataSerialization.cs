using System;
using System.Globalization;
using UnityEngine;

public class JsonUtilityHelper
{
    public static T[] FromJson<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentException("JSON table is empty.", nameof(json));
        json = json.TrimStart('\uFEFF', ' ', '\t', '\r', '\n');
        if (json.Length == 0)
            throw new ArgumentException("JSON table is empty.", nameof(json));
        string newJson = "";
        if (json[0] == '{')
        {
            newJson = json;
        }
        else
        {
            newJson = "{ \"array\": " + json + "}";
        }
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        if (wrapper == null || wrapper.array == null)
            throw new FormatException("Expected a JSON array or an object containing an array field.");
        return wrapper.array;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.array = array;
        return JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}

public class CSVConvert
{
    public static int ToInt(object _obj)
    {
        string body = _obj.ToString();
        //body = Regex.Replace(body, @"[^0-9]", "");
        return int.Parse(body, CultureInfo.InvariantCulture);
    }

    public static float ToFloat(object _obj)
    {
        string body = _obj.ToString();
        //body = Regex.Replace(body, @"[^0-9]", "");
        return float.Parse(body, CultureInfo.InvariantCulture);
    }

    public static string ToString(object _obj)
    {
        return _obj.ToString();
    }
}
