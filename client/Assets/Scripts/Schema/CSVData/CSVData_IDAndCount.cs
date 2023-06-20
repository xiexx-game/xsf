//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\CSVData\CSVData_IDAndCount.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 id count组
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

public struct CSVIdCount
{
    public uint id;
    public uint count;
}

public sealed class CSVData_IDAndCount : CSVData
{
    public override CSVDataType DataType { get { return CSVDataType.IDAndCount; } }

    public CSVIdCount[] icValue;

    public override void Read(int nRow, int nCol, string sData)
    {
        if (string.IsNullOrEmpty(sData))
        {
            icValue = null;
            return;
        }


        try
        {
            string[] item = sData.Split('|');
            icValue = new CSVIdCount[item.Length];
            for (int i = 0; i < item.Length; ++i)
            {
                string[] itemData = item[i].Split(':');
                if (itemData.Length != 2)
                {
                    throw new XSFSchemaLoadException($"CSVData_IDAndCount.Read error, should be id:count, str={sData}, row={nRow}, col={nCol}");
                }

                CSVIdCount ic;
                ic.id = Convert.ToUInt32(itemData[0]);
                ic.count = Convert.ToUInt32(itemData[1]);
                icValue[i] = ic;
            }
        }
        catch (Exception)
        {
            XSF.LogError($"CSVData_IDAndCount.Read error str={sData}, row={nRow}, col={nCol}");
            throw;
        }

    }

#if UNITY_EDITOR
    public override string Prefix
    {
        get
        {
            return "ic";
        }
    }

    public override string ValueStr
    {
        get
        {
            return "icValue";
        }
    }

    public override string TypeName
    {
        get
        {
            return "CSVIdCount[]";
        }
    }

    public override string GetLuaCode(string name) 
    {
        string result = name + " = {";

        if(string.IsNullOrEmpty(name))
        {
            result = "{";
        }
        

        if(icValue != null)
        {
            string icData = "";
            for(int i = 0; i < icValue.Length; i ++)
            {
                if(string.IsNullOrEmpty(icData))
                {
                    icData = "{" + $" id = {icValue[i].id}, count = {icValue[i].count}" + "}";
                }
                else 
                {
                    icData += " ," + "{" + $" id = {icValue[i].id}, count = {icValue[i].count}" + "}";
                }
            }

            result += icData;
        }

        result += "}";

        return result;
    }
#endif
}