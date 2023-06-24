//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\_SCHEMA_NAME_.cs
// 作者：_SCHEMA_AUTHOR_
// 时间：_SCHEMA_DATE_
// 描述：_SCHEMA_DESC_
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class _SCHEMA_NAME_ : ISchema
{
    //private Dictionary<uint, _SCP_NAME_> m_Datas;
    //private _SCP_NAME_ [] m_Datas;

    public string GetSchemaName(string name)
    {
        return name;
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        //m_Datas = new Dictionary<uint, _SCP_NAME_>();
        CSVReader csv = reader as CSVReader;
        //m_Datas = new _SCP_NAME_[csv.mRowCount + 1];

        for (int i = 0; i < csv.mRowCount; ++i)
        {
            _SCP_NAME_ scp = new _SCP_NAME_();

//_CSV_LIST_BEGIN_
//_CSV_LIST_END_

            /* 使用字典
            if (m_Datas.ContainsKey(scp.nID))
            {
                XSF.LogError("_SCHEMA_NAME_.OnSchemaLoad key exist, id=" + scp.nID);
                return false;
            }

            m_Datas.Add(scp.nID, scp);
            //*/

            /* 使用数组
            m_Datas[scp.nIndex] = scp;
            //*/
        }

        return true;
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
            XSF.LogError(string.Format("_SCHEMA_NAME_.Get nIndex[{0}] >= m_Datas.Length[{1}]", nIndex, m_Datas.Length));
            return null;
        }

        return m_Datas[nIndex];
    }
    //*/
}