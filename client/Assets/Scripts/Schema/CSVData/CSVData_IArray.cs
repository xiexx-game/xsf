//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Schema/CSVData/CSVData_IArray.cs
// 作者：Xoen Xie
// 时间：2023/08/27
// 描述：CSV数据 int数组
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

public sealed class CSVData_IArray : CSVData
{
    public override CSVDataType DataType { get { return CSVDataType.IArray; } }

    public int[] arValue;

    public override void Read(int nRow, int nCol, string sData)
    {
        if (string.IsNullOrEmpty(sData))
        {
            arValue = null;
            return;
        }

        try
        {
            string [] datas = sData.Split(':');
            arValue = new int[datas.Length];
            for(int i = 0; i < datas.Length; ++i)
            {
                arValue[i] = Convert.ToInt32(datas[i]);
            }
        }
        catch (Exception)
        {
            Debug.LogError($"CSVData_IArray.Read error str={sData}, row={nRow}, col={nCol}");
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
            return "int []";
        }
    }
#endif
}