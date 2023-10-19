//////////////////////////////////////////////////////////////////////////
// 
// 文件：Schema/_SCHEMA_NAME_.cs
// 作者：_SCHEMA_AUTHOR_
// 时间：_SCHEMA_DATE_
// 描述：_SCHEMA_DESC_
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8603, CS8618
using XSF;

namespace XsfScp
{
    public class _SCHEMA_NAME_ : ISchema
    {
        //private Dictionary<uint, _SCP_NAME_> m_Datas;
        //private _SCP_NAME_ [] m_Datas;

        public string GetSchemaName(string name)
        {
            return name;
        }

        public void OnSchemaLoad(ISchemaReader reader)
        {
            //m_Datas = new Dictionary<uint, _SCP_NAME_>();
            //m_Datas = new _SCP_NAME_[csv.mRowCount + 1];
            CSVReader csv = reader as CSVReader;

            for (int i = 0; i < csv.mRowCount; ++i)
            {
                ScpItem scp = new ScpItem();

//_CSV_LIST_BEGIN_
//_CSV_LIST_END_

                /* 使用字典
                if (m_Datas.ContainsKey(scp.uId))
                {
                    throw new XSFSchemaLoadException("_SCHEMA_NAME_.OnSchemaLoad key exist, id=" + scp.uId);
                }

                m_Datas.Add(scp.uId, scp);
                //*/

                /* 使用数组
                m_Datas[scp.nIndex] = scp;
                //*/
            }
        }

        /* 使用字典 根据ID获取
        public _SCP_NAME_ Get(uint nID)
        {
            _SCP_NAME_ scp = null;
            m_Datas.TryGetValue(nID, out scp);
            return scp;
        }
        //*/

        /* 使用数组 根据索引获取
        public _SCP_NAME_ Get(uint nIndex)
        {
            if (nIndex >= m_Datas.Length)
            {
                return null;
            }

            return m_Datas[nIndex];
        }
        //*/
    }
}

