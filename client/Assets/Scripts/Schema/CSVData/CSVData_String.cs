//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\CSVData\CSVData_String.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 string
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;

namespace XsfScp
{
    public sealed class CSVData_String : CSVData
    {
        public override CSVDataType DataType { get { return CSVDataType.String; } }

        public string sValue;

        public override void Read(int nRow, int nCol, string sData)
        {
            sValue = sData;
        }
    }
}