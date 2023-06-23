//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaLanguage.cs
// 作者：Xoen Xie
// 时间：2023/6/23
// 描述：多语言配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

public class SchemaLanguage : ISchema
{
    private Dictionary<string, ScpLanguage> m_Datas;

    public string GetSchemaName(string name)
    {
        if(string.IsNullOrEmpty(XSFLocalization.Instance.LanSchemaExt))
        {
            return name;
        }
        else
        {
            return $"{name}_{XSFLocalization.Instance.LanSchemaExt}";
        }
    }

    public bool OnSchemaLoad(ISchemaReader reader)
    {
        m_Datas = new Dictionary<string, ScpLanguage>();
        CSVReader csv = reader as CSVReader;

        for (int i = 0; i < csv.mRowCount; ++i)
        {
            ScpLanguage scp = new ScpLanguage();

//_CSV_LIST_BEGIN_
			scp.sKey = (csv.GetData((int)CSVDataType.String, i, (int)CSVIndex.ScpLanguage_key) as CSVData_String).sValue;	// 键
			scp.sValue = (csv.GetData((int)CSVDataType.String, i, (int)CSVIndex.ScpLanguage_value) as CSVData_String).sValue;	// 值
//_CSV_LIST_END_

            scp.sValue = scp.sValue.Replace("[comma]", ",");

            //* 使用字典
            if (m_Datas.ContainsKey(scp.sKey))
            {
                XSF.LogError("SchemaLanguage.OnSchemaLoad key exist, id=" + scp.sKey);
                return false;
            }

            m_Datas.Add(scp.sKey, scp);
            //*/
        }

        return true;
    }

    public string Get(string sKey)
    {
        ScpLanguage scp = null;
        if(m_Datas.TryGetValue(sKey, out scp))
        {
            return scp.sValue;
        }
        else
        {
#if UNITY_EDITOR
            if(UnityEngine.Application.isPlaying) {
                XSF.LogError("SchemaLanguage.Get can not get local text, key=" + sKey);
            } else {
                UnityEngine.Debug.LogError("SchemaLanguage.Get can not get local text, key=" + sKey);
            }
            
#else
            XSF.LogError("SchemaLanguage.Get can not get local text, key=" + sKey);
#endif
            return sKey;
        }
        
    }
}
