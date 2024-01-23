//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\Schema\SchemaReader.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置读取器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8625, CS8618, CS8601
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;

namespace XSFTools
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

    #region XML读取器

    public sealed class XMLReader : ISchemaReader
    {
        private const string XML_ROOT = "root";

        private XmlDocument m_Document;

        public XmlNode mRootNode { get; private set; }

        public XMLReader()
        {
            m_Document = new XmlDocument();
        }

        public override void Read(string sContent)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;

            XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(sContent)), settings);
            m_Document.Load(reader);

            mRootNode = m_Document.SelectSingleNode(XML_ROOT);
        }

        public static int GetInt(XmlElement e, string attribute = null)
        {
            string tmp = "";
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
            string tmp = "";
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
            string tmp = "";
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
            string tmp = "";
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
            string tmp = "";
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
            string tmp = "";
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


