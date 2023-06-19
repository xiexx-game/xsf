//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\CSVData\CSVData_Bool.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 bool
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

public sealed class CSVData_Bool : CSVData
{
    public override CSVDataType DataType { get { return CSVDataType.Bool; } }

    public bool bValue;

    public override void Read(int nRow, int nCol, string sData)
    {
        if (string.IsNullOrEmpty(sData))
        {
            bValue = false;
            return;
        }


        if(sData == "1" || sData.ToLower() == "true")
            bValue = true;
        else
            bValue = false;

    }

#if UNITY_EDITOR
    public override string Prefix
    {
        get
        {
            return "b";
        }
    }

    public override string ValueStr
    {
        get
        {
            return "bValue";
        }
    }

    public override string TypeName
    {
        get
        {
            return "bool";
        }
    }
#endif
}