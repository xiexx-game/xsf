//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\CSVData\CSVData_Uint.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据 uint
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;
using System;

namespace XsfScp
{
    public sealed class CSVData_Uint : CSVData
    {
        public override CSVDataType DataType { get { return CSVDataType.Uint; } }

        public uint uValue;

        public override void Read(int nRow, int nCol, string sData)
        {
            if (string.IsNullOrEmpty(sData))
            {
                uValue = 0;
                return;
            }

            try
            {
                uValue = Convert.ToUInt32(sData);
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
                return "u";
            }
        }

        public override string ValueStr
        {
            get
            {
                return "uValue";
            }
        }

        public override string TypeName
        {
            get
            {
                return "uint";
            }
        }

        public override string GetLuaCode(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return uValue.ToString();
            }
            else
            {
                return $"{name} = {uValue}";
            }

        }
#endif
    }
}