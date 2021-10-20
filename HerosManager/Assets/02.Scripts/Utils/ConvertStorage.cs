using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class ConvertStorageBase<T>
{
    public virtual void Setup(string stageName) { }

    public virtual void ConvertCSVToClass(string csvPath) { }

    public virtual void ConvertCSVToClass(string csvPath, string key) { }

    protected int ConvertToInt(object obj)
    {
        string _body = obj.ToString();
        _body = Regex.Replace(_body, @"[^0-9]", "");
        return int.Parse(_body);
    }

    protected string ConvertToString(object obj)
    {
        return obj.ToString();
    }

    protected float ConvertToFloat(object obj)
    {
        string _body = obj.ToString();
        //_body = Regex.Replace(_body, @"[^0-9]", "");
        return float.Parse(_body);
    }
}
