//////////////////////////////////////////////////////////////////////////
//
// 文件：Schema/CSVData/CSVData_SArray.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 string数组
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;

namespace XsfScp
{
    public sealed class CSVData_SArray : CSVData
    {
        public override CSVDataType DataType { get { return CSVDataType.SArray; } }

        public string[]? sarValue;

        public override void Read(int nRow, int nCol, string sData)
        {
            if (string.IsNullOrEmpty(sData))
            {
                sarValue = null;
                return;
            }

            sarValue = sData.Split(':');
        }
    }
}
