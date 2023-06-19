//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\CSVData\CSVData_Ulong.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 ulong
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

public sealed class CSVData_Ulong : CSVData
{
    public override CSVDataType DataType { get { return CSVDataType.Ulong; } }

    public ulong ulValue;

    public override void Read(int nRow, int nCol, string sData)
    {
        if (string.IsNullOrEmpty(sData))
        {
            ulValue = 0;
            return;
        }

        try
        {
            ulValue = Convert.ToUInt64(sData);
        }
        catch (Exception)
        {
            XSF.LogError($"CSVData_Uint.Read error str={sData}, row={nRow}, col={nCol}");
            throw;
        }
    }

#if UNITY_EDITOR
    public override string Prefix
    {
        get
        {
            return "ul";
        }
    }

    public override string ValueStr
    {
        get
        {
            return "ulValue";
        }
    }

    public override string TypeName
    {
        get
        {
            return "ulong";
        }
    }
#endif
}