//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Schema/CSVData/CSVData_SArray.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 字符串数组
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

public sealed class CSVData_SArray : CSVData
{
    public override CSVDataType DataType { get { return CSVDataType.SArray; } }

    public string[] sarValue;

    public override void Read(int nRow, int nCol, string sData)
    {
        if (string.IsNullOrEmpty(sData))
        {
            sarValue = null;
            return;
        }

        try
        {
            sarValue = sData.Split(':');
        }
        catch (Exception)
        {
            Debug.LogError($"CSVData_SArray.Read error str={sData}, row={nRow}, col={nCol}");
            throw;
        }
    }

#if UNITY_EDITOR
    public override string Prefix
    {
        get
        {
            return "sar";
        }
    }

    public override string ValueStr
    {
        get
        {
            return "sarValue";
        }
    }

    public override string TypeName
    {
        get
        {
            return "string []";
        }
    }

    public override string GetLuaCode(string name) 
    {
        string result = name + " = {";
        if(string.IsNullOrEmpty(name))
        {
            result = "{";
        }

        if(sarValue != null)
        {
            string arData = "";
            for(int i = 0; i < sarValue.Length; i ++)
            {
                if(string.IsNullOrEmpty(arData))
                {
                    arData = sarValue[i].ToString();
                }
                else 
                {
                    arData += " ," + sarValue[i].ToString();
                }
            }

            result += arData;
        }

        result += "}";

        return result;
    }
#endif
}