//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\CSVData\CSVData_Float.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 float
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

namespace XsfScp
{
    public sealed class CSVData_Float : CSVData
    {
        public override CSVDataType DataType { get { return CSVDataType.Float; } }

        public float fValue;

        public override void Read(int nRow, int nCol, string sData)
        {
            if (string.IsNullOrEmpty(sData))
            {
                fValue = 0;
                return;
            }

            try
            {
                fValue = Convert.ToSingle(sData);
            }
            catch (Exception)
            {
                Debug.LogError($"CSVData_Uint.Read error str={sData}, row={nRow}, col={nCol}");
                throw;
            }
        }

#if UNITY_EDITOR
        public override string Prefix
        {
            get
            {
                return "f";
            }
        }

        public override string ValueStr
        {
            get
            {
                return "fValue";
            }
        }

        public override string TypeName
        {
            get
            {
                return "float";
            }
        }

        public override string CppTypeName
        {
            get
            {
                return "float";
            }
        }
#endif
    }
}