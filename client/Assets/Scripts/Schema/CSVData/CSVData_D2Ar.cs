//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Schema/CSVData/CSVData_D2Ar.cs
// 作者：Xoen Xie
// 时间：2023/08/26
// 描述：CSV数据 2维uint数组
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

public struct ArrayData
{
    public uint[] data;
}

public sealed class CSVData_D2Ar : CSVData
{
    public override CSVDataType DataType { get { return CSVDataType.D2Ar; } }

    public ArrayData[] arValue;

    public override void Read(int nRow, int nCol, string sData)
    {
        if (string.IsNullOrEmpty(sData))
        {
            arValue = null;
            return;
        }

        try
        {
            string[] item = sData.Split('|');
            arValue = new ArrayData[item.Length];
            for (int i = 0; i < item.Length; ++i)
            {
                string[] itemData = item[i].Split(':');

                arValue[i] = new ArrayData();
                arValue[i].data = new uint[itemData.Length];
                for(int j = 0; j < itemData.Length; ++j)
                {
                    arValue[i].data[j] = Convert.ToUInt32(itemData[j]);
                }
            }
        }
        catch (Exception)
        {
            Debug.LogError($"CSVData_2Array.Read error str={sData}, row={nRow}, col={nCol}");
            throw;
        }
    }

#if UNITY_EDITOR
    public override string Prefix
    {
        get
        {
            return "ar";
        }
    }

    public override string ValueStr
    {
        get
        {
            return "arValue";
        }
    }

    public override string TypeName
    {
        get
        {
            return "ArrayData []";
        }
    }
#endif
}