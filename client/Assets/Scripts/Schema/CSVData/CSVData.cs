//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\CSVData\CSVData.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：CSV数据基类
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using XSF;

namespace XsfScp
{
    public enum CSVDataType
    {
        None = 0,
        Int,        // 有符号32位整形，配置为空是为0
        Uint,       // 无符号32位整形，配置为空是为0
        Ulong,      // 无符号64位整形，配置为空是为0
        Float,      // 浮点，配置为空是为0
        String,     // 字符串，配置为空是为空串
        Bool,       // 布尔值，配置为空为false， 1或者true时为true
        Array,      // 整形数组类型，配置为空无数据 num1:num2:num3
        IDAndCount, // ID和count组合，id1:count1|id2:count2|id3:count3
        Max,
    }

    public class CSVData : ICSVData
    {
        public virtual CSVDataType DataType { get; }

        public virtual void Read(int nRow, int nCol, string sData)
        {

        }

        private static CSVData[] m_Datas;
        public static CSVData GetData(int nType)
        {
            if (m_Datas == null)
            {
                m_Datas = new CSVData[(int)CSVDataType.Max];
            }

            if (m_Datas[nType] == null)
            {
                switch ((CSVDataType)nType)
                {
                    case CSVDataType.Int: m_Datas[nType] = new CSVData_Int(); break;
                    case CSVDataType.Uint: m_Datas[nType] = new CSVData_Uint(); break;
                    case CSVDataType.Ulong: m_Datas[nType] = new CSVData_Ulong(); break;
                    case CSVDataType.Bool: m_Datas[nType] = new CSVData_Bool(); break;
                    case CSVDataType.Float: m_Datas[nType] = new CSVData_Float(); break;
                    case CSVDataType.Array: m_Datas[nType] = new CSVData_Array(); break;
                    case CSVDataType.String: m_Datas[nType] = new CSVData_String(); break;
                    case CSVDataType.IDAndCount: m_Datas[nType] = new CSVData_IDAndCount(); break;

                    default: throw new XSFSchemaLoadException($"CSVData.GetData type error, type={nType}");
                }
            }

            return m_Datas[nType];
        }
    }
}