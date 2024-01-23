//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\CSVData\CSVData_Int.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 int
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;

namespace XsfScp
{
    public sealed class CSVData_Int : CSVData
    {
        public override CSVDataType DataType { get { return CSVDataType.Int; } }

        public int iValue;

        public override void Read(int nRow, int nCol, string sData)
        {
            if (string.IsNullOrEmpty(sData))
            {
                iValue = 0;
                return;
            }

            try
            {
                iValue = Convert.ToInt32(sData);
            }
            catch (Exception)
            {
                Debug.LogError($"CSVData_Int.Read error str={sData}, row={nRow}, col={nCol}");
                throw;
            }
        }
    }
}