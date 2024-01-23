//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\Schema\SchemaReader.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置读取器
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace XSF
{

public enum SchemaType
{
    None = 0,
    CSV,
    XML,
}

public abstract  class ISchemaReader
{
    public abstract void Read(string sContent);
}



#region CSV读取器
public sealed class CSVReader : ISchemaReader
{
    private const char COLUMN_SPLIT = ',';
    private const char LINE_SPLIT = '\n';
    private const int START_INDEX = 3;          // CSV表头占用3行
    private const int CTABLE_MAX_COL = 4;

    private List<string[]> m_SchemaList;

    public int mRowCount { get; private set; }
    public int mColCount { get; private set; }

    private bool m_bIsColTable;

    public CSVReader(bool IsColTable)
    {
        m_bIsColTable = IsColTable;
    }

    public override void Read(string sContent)
    {
        string[] lines = sContent.Split(LINE_SPLIT);
        mRowCount = 0;
        int nStartIndex = 0;
        if(m_bIsColTable)
        {
            mColCount = CTABLE_MAX_COL;
        }
        else
        {
            nStartIndex = START_INDEX;
            string[] propertyNames = lines[START_INDEX - 1].Split(COLUMN_SPLIT);
            mColCount = propertyNames.Length;
        }

        m_SchemaList = new List<string[]>();

        for (int i = nStartIndex; i < lines.Length; ++i)
        {
            if (string.IsNullOrEmpty(lines[i]))
                continue;

            string sLine = lines[i].Replace("\r", "");

            string[] values = sLine.Split(COLUMN_SPLIT);
            if (values.Length != mColCount)
            {
                throw new XSFSchemaLoadException($"CSVReader.LoadFromContent, row={i}, [ColCount:{mColCount} != CurColCount:{values.Length}]");
            }

            m_SchemaList.Add(values);
            mRowCount++;
        }
    }

    public ICSVData GetData(int nType, int nRow, int nCol)
    {
        if(nRow >= mRowCount)
        {
            throw new XSFSchemaLoadException($"CSVReader.GetData, nRow:{nRow} >= mRowCount:{mRowCount}");
        }   

        if(nCol >= mColCount)
        {
            throw new XSFSchemaLoadException($"CSVReader.GetData, nCol:{nCol} >= mColCount:{mColCount}");
        }

        ICSVData d = XSFSchema.Instance.Helper.GetData(nType);
        if(d == null) 
        {
            throw new XSFSchemaLoadException($"CSVReader.GetData, csv data type:{nType} error");
        }

        d.Read(nRow, nCol, m_SchemaList[nRow][nCol]);
        
        return d;
    }
}

#endregion

#region XML读取器

public sealed class XMLReader : ISchemaReader
{
    private const string XML_ROOT = "root";

    private XmlDocument m_Document;

    public XmlNode mRootNode { get; private set; }

    public override void Read(string sContent)
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true;

        XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(sContent)), settings);
        m_Document = new XmlDocument();
        m_Document.Load(reader);

        mRootNode = m_Document.SelectSingleNode(XML_ROOT);
        if (mRootNode == null)
        {
            throw new XSFSchemaLoadException($"XMLReader.LoadFromContent need root element");
        }
    }

    public static int GetInt(XmlElement e, string attribute = null)
    {
        string tmp = null;
        if (string.IsNullOrEmpty(attribute))
            tmp = e.InnerText;
        else
            tmp = e.GetAttribute(attribute);

        if (string.IsNullOrEmpty(tmp))
        {
            return 0;
        }

        try
        {
            return Convert.ToInt32(tmp);
        }
        catch(Exception)
        {
            throw;
        }
    }

    public static uint GetUInt(XmlElement e, string attribute = null)
    {
        string tmp = null;
        if (string.IsNullOrEmpty(attribute))
            tmp = e.InnerText;
        else
            tmp = e.GetAttribute(attribute);

        if (string.IsNullOrEmpty(tmp))
        {
            return 0;
        }

        try
        {
            return Convert.ToUInt32(tmp);
        }
        catch(Exception)
        {
            throw;
        }
    }

    public static long GetLong(XmlElement e, string attribute = null)
    {
        string tmp = null;
        if (string.IsNullOrEmpty(attribute))
            tmp = e.InnerText;
        else
            tmp = e.GetAttribute(attribute);

        if (string.IsNullOrEmpty(tmp))
        {
            return 0;
        }

        try
        {
            return Convert.ToInt64(tmp);
        }
        catch(Exception)
        {
            throw;
        }
    }

    public static ulong GetULong(XmlElement e, string attribute = null)
    {
        string tmp = null;
        if (string.IsNullOrEmpty(attribute))
            tmp = e.InnerText;
        else
            tmp = e.GetAttribute(attribute);

        if (string.IsNullOrEmpty(tmp))
        {
            return 0;
        }

        try
        {
            return Convert.ToUInt64(tmp);
        }
        catch(Exception)
        {
            throw;
        }
    }

    public static float GetFloat(XmlElement e, string attribute = null)
    {
        string tmp = null;
        if (string.IsNullOrEmpty(attribute))
            tmp = e.InnerText;
        else
            tmp = e.GetAttribute(attribute);

        if (string.IsNullOrEmpty(tmp))
        {
            return 0;
        }

        try
        {
            return Convert.ToSingle(tmp);
        }
        catch(Exception)
        {
            throw;
        }
    }

    public static string GetString(XmlElement e, string attribute = null)
    {
        if (string.IsNullOrEmpty(attribute))
            return e.InnerText;

        return e.GetAttribute(attribute);
    }


    public static bool GetBoolean(XmlElement e, string attribute = null)
    {
        string tmp = null;
        if (string.IsNullOrEmpty(attribute))
            tmp = e.InnerText;
        else
            tmp = e.GetAttribute(attribute);

        if (string.IsNullOrEmpty(tmp))
            return false;

        tmp = tmp.ToLower();
        if (tmp.Equals("true") || tmp.Equals("1"))
            return true;

        return false;
    }
}

#endregion

}

