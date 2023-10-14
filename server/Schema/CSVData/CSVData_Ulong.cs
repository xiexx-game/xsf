//////////////////////////////////////////////////////////////////////////
//
// 文件：Schema/CSVData/CSVData_Ulong.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 ulong
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

namespace XsfScp
{
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
                Serilog.Log.Error($"CSVData_Uint.Read error str={sData}, row={nRow}, col={nCol}");
                throw;
            }
        }
    }
}

