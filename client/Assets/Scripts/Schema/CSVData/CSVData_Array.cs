//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\CSVData\CSVData_Array.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 uint数组
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;

namespace XsfScp
{
    public sealed class CSVData_Array : CSVData
    {
        public override CSVDataType DataType { get { return CSVDataType.Array; } }

        public uint[] arValue;

        public override void Read(int nRow, int nCol, string sData)
        {
            if (string.IsNullOrEmpty(sData))
            {
                arValue = null;
                return;
            }

            try
            {
                string[] datas = sData.Split(':');
                arValue = new uint[datas.Length];
                for (int i = 0; i < datas.Length; ++i)
                {
                    arValue[i] = Convert.ToUInt32(datas[i]);
                }
            }
            catch (Exception)
            {
                Debug.LogError($"CSVData_Array.Read error str={sData}, row={nRow}, col={nCol}");
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
                return "uint []";
            }
        }

        public override string CppTypeName
        {
            get
            {
                return "CSVArray";
            }
        }

#endif
    }
}