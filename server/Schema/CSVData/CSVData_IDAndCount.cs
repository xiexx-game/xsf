//////////////////////////////////////////////////////////////////////////
//
// 文件：Schema/CSVData/CSVData_IDAndCount.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 id count组
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using XSF;

namespace XsfScp
{
    public struct CSVIdCount
    {
        public uint id;
        public uint count;
    }

    public sealed class CSVData_IDAndCount : CSVData
    {
        public override CSVDataType DataType { get { return CSVDataType.IDAndCount; } }

        public CSVIdCount[]? icValue;

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
                        throw new XSFSchemaLoadException ($"CSVData_IDAndCount.Read error, should be id:count, str={sData}, row={nRow}, col={nCol}");
                    }

                    CSVIdCount ic;
                    ic.id = Convert.ToUInt32(itemData[0]);
                    ic.count = Convert.ToUInt32(itemData[1]);
                    icValue[i] = ic;
                }
            }
            catch (Exception e)
            {
                Serilog.Log.Error($"CSVData_IDAndCount.Read error str={sData}, row={nRow}, col={nCol}, message={e.Message}");
                throw;
            }
        }

    }
}
