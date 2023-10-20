//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Editor\Schema\SchemaTools.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置工具
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Xml;
using System;

public static class SchemaTools
{
    [MenuItem("XSFTools/导出配置 (Export Schema)", false, (int)XSFMenuID.ExportSchema)]
    public static void ExportSchema()
    {
        XmlNodeList list = DoExport();

        Debug.Log("schema export done ...");

        DoCode(list);
    }

    
    [MenuItem("XSFTools/设置本地化 (Set Localization)/清除 (Clear)", false, (int)XSFMenuID.Localization_Clear)]
    public static void Localization_Clear()
    {
        PlayerPrefs.SetString(XSFLocalization.Instance.LOCAL_PREFS, "");
        Debug.Log("Clear Localization");
        XSFLocalization.Instance.Init();
    }

    [MenuItem("XSFTools/设置本地化 (Set Localization)/英文 (English)", false, (int)XSFMenuID.Localization_En)]
    public static void Localization_En()
    {
        PlayerPrefs.SetString(XSFLocalization.Instance.LOCAL_PREFS, "English");
        Debug.Log("Set Localization: English");
        XSFLocalization.Instance.Init();
    }

    [MenuItem("XSFTools/设置本地化 (Set Localization)/中文简体 (Chinese)", false, (int)XSFMenuID.Localization_Ch)]
    public static void Localization_Ch()
    {
        PlayerPrefs.SetString(XSFLocalization.Instance.LOCAL_PREFS, "Chinese");
        Debug.Log("Set Localization: Chinese");
        XSFLocalization.Instance.Init();
    }

    private static XmlNodeList DoExport()
    {
        string sToolDir = Application.dataPath + "/../../config/bin/";
        string sClientOutput = Application.dataPath + "/Scp/";
        string sServerOutput = Application.dataPath + "/../../server/bin/scp/";

        string sToolArg = $"ConfigSplit.py {sClientOutput} {sServerOutput}";
        Debug.Log(sToolArg);

        XSFEditorUtil.StartProcess("python3", sToolArg, sToolDir);

        string sConfigDir = Application.dataPath + "/../../config/";

        string sLoadXml = Application.dataPath + "/../../config/Load.xml";
        File.Copy(sLoadXml, sClientOutput + "Load.xml", true);

        if(Directory.Exists(sServerOutput)) {
            File.Copy(sLoadXml, sServerOutput + "Load.xml", true);
        }

        string sLoadContent = File.ReadAllText(Application.dataPath + "/../../config/Load.xml");
        XMLReader reader = new XMLReader();
        reader.Read(sLoadContent);
        
        XmlNodeList nodeList = reader.mRootNode.ChildNodes;
        for (int i = 0; i < nodeList.Count; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;

            int nID = XMLReader.GetInt(ele, "id");
            string sName = XMLReader.GetString(ele, "name");
            SchemaType nType = (SchemaType)XMLReader.GetUInt(ele, "type");
            if(nType == SchemaType.XML) 
            {   
                string sSrcXml = Application.dataPath + $"/../../config/{sName}.xml";

                int nLoad = XMLReader.GetInt(ele, "client");
                if(nLoad > 0)
                {
                    File.Copy(sSrcXml, sClientOutput + $"{sName}.xml", true);
                }

                int nServerLoad = XMLReader.GetInt(ele, "server");
                if(nServerLoad > 0 && Directory.Exists(sServerOutput))
                {
                    File.Copy(sSrcXml, sServerOutput + $"{sName}.xml", true);
                }
            }
            
        }

        AssetDatabase.Refresh();

        AASTools.UpdateAASGroup();

        AssetDatabase.Refresh();

        return nodeList;
    }

    static string CCSStructFile = Application.dataPath + "/Scripts/Schema/SchemaStructs.cs";
    static string CCSHelperFile = Application.dataPath + "/Scripts/Schema/GameSchemaHelper.cs";
    static string CCSIndexFile = Application.dataPath + "/Scripts/Schema/SchemaIndex.cs"; 

    static string SCSStructFile = Application.dataPath + "/../../server/Schema/SchemaStructs.cs";
    static string SCSHelperFile = Application.dataPath + "/../../server/Schema/GameSchemaHelper.cs";
    static string SCSIndexFile = Application.dataPath + "/../../server/Schema/SchemaIndex.cs"; 

    private static void DoCode(XmlNodeList nodeList)
    {
        string CCSID = "";
        string CCSCreate = "";
        string CCSIndex = "";

        string SCSID = "";
        string SCSCreate = "";
        string SCSIndex = "";

        bool IsServerDirExist = File.Exists(SCSStructFile);

        for (int i = 0; i < nodeList.Count; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;

            int nID = XMLReader.GetInt(ele, "id");
            string sName = XMLReader.GetString(ele, "name");
            string sDesc = XMLReader.GetString(ele, "desc");

            int nClientLoad = XMLReader.GetInt(ele, "client");
            int nServerLoad = XMLReader.GetInt(ele, "server");

            SchemaType nType = (SchemaType)XMLReader.GetUInt(ele, "type");

            string sStructName = $"Scp{sName}";

            if(nClientLoad > 0)
            {
                CCSID += $"\t{sName} = {nID},\n";

                CCSCreate += $"\t\t\tcase SchemaID.{sName}: return new Schema{sName}();\n";

                string sStructHead = $"public class {sStructName}";
                string sStructContent = File.ReadAllText(CCSStructFile);
                if(!sStructContent.Contains(sStructHead)) 
                {
                    string sStructAdd = $"//{sDesc}\n" + sStructHead + "\n{\n" + $"//{sStructName}_START\n//{sStructName}_END\n" + "}\n";
                    XSFEditorUtil.AppendFileByTag(CCSStructFile, "SCHEMA_STRUCT", sStructAdd);
                }

                DoCSCode(sName, sStructName, sDesc, nType, ref CCSIndex);
            }
            
            if(nServerLoad > 0 && IsServerDirExist)
            {
                SCSID += $"\t\t{sName} = {nID},\n";

                SCSCreate += $"\t\t\t\tcase SchemaID.{sName}: return new Schema{sName}();\n";

                string sStructHead = $"public class {sStructName}";
                string sStructContent = File.ReadAllText(SCSStructFile);
                if(!sStructContent.Contains(sStructHead)) 
                {
                    string sStructAdd = $"//{sDesc}\n" + sStructHead + "\n{\n" + $"//{sStructName}_START\n//{sStructName}_END\n" + "}\n";
                    XSFEditorUtil.AppendFileByTag(SCSStructFile, "SCHEMA_STRUCT", sStructAdd);
                }

                DoServerCode(sName, sStructName, sDesc, nType, ref SCSIndex);
            }
        }

        XSFEditorUtil.ReplaceContentByTag(CCSHelperFile, "SCHEMA_ID_BEGIN", "SCHEMA_ID_END", CCSID);
        XSFEditorUtil.ReplaceContentByTag(CCSHelperFile, "SCHEMA_BEGIN", "SCHEMA_END", CCSCreate);
        XSFEditorUtil.ReplaceContentByTag(CCSIndexFile, "CSV_INDEX_BEGIN", "CSV_INDEX_END", CCSIndex);

        if(IsServerDirExist)
        {
            XSFEditorUtil.ReplaceContentByTag(SCSHelperFile, "SCHEMA_ID_BEGIN", "SCHEMA_ID_END", SCSID);
            XSFEditorUtil.ReplaceContentByTag(SCSHelperFile, "SCHEMA_BEGIN", "SCHEMA_END", SCSCreate);
            XSFEditorUtil.ReplaceContentByTag(SCSIndexFile, "CSV_INDEX_BEGIN", "CSV_INDEX_END", SCSIndex);
        }
    }

    private static void DoCSCode(string name, string sStructName, string desc, SchemaType nType, ref string sIndex)
    {
        if(nType == SchemaType.CSV)
        {
            string sCSVTemplate = Application.dataPath + "/../../config/template/SchemaCSV.cs";
            string sCSVFile = Application.dataPath + $"/Scp/{name}.csv";
            if(!File.Exists(sCSVFile))
            {
                Debug.LogError("csv file not exist, can not gen C# code. path=" + sCSVFile);
                return;
            }

            string [] lines = File.ReadAllLines(sCSVFile);
            string [] paramDesc = lines[0].Split(',');
            string [] paramType = lines[1].Split(',');
            string [] paramName = lines[2].Split(',');

            string sStructContent = "";
            string sListContent = "";

            if(name == "Global")
            {   
                // 类型， 值， 名称， 注释， client server
                for(int i = 3; i < lines.Length; i ++)
                {
                    string[] lineData = lines[i].Split(",");

                    if(lineData[4].Contains("*"))
                    {
                        CSVData data = CSVData.GetDataByName(lineData[0]);
                        string newName = XSFEditorUtil.FirstLetterToUpper(lineData[2]);
                        sStructContent += $"\tpublic {data.TypeName} {data.Prefix}{newName};\t//{lineData[3]}\n";

                        sListContent += $"\t\tGlobalData.{data.Prefix}{newName} = (csv.GetData((int)CSVDataType.{data.DataType.ToString()}, nRowIndex++, 1) as CSVData_{data.DataType.ToString()}).{data.ValueStr};\t// {lineData[3]}\n";
                    }
                }
            }
            else
            {
                for(int i = 0; i < paramDesc.Length; ++i)
                {
                    CSVData data = CSVData.GetDataByName(paramType[i]);
                    string newName = XSFEditorUtil.FirstLetterToUpper(paramName[i]);
                    sStructContent += $"\tpublic {data.TypeName} {data.Prefix}{newName};\t//{paramDesc[i]}\n";

                    sIndex += $"\t{sStructName}_{paramName[i]} = {i},\n";


                    sListContent += $"\t\t\tscp.{data.Prefix}{newName} = (csv.GetData((int)CSVDataType.{data.DataType.ToString()}, i, (int)CSVIndex.{sStructName}_{paramName[i]}) as CSVData_{data.DataType.ToString()}).{data.ValueStr};\t// {paramDesc[i]}\n";
                }
            }
            

            sIndex += "\n";

            XSFEditorUtil.ReplaceContentByTag(CCSStructFile, $"{sStructName}_START", $"{sStructName}_END", sStructContent);

            string sSchemaName = $"Schema{name}";
            string sCSVCodeFile = Application.dataPath + $"/Scripts/Schema/{sSchemaName}.cs";
            if(!File.Exists(sCSVCodeFile)) 
            {
                string sContent = File.ReadAllText(sCSVTemplate);
                sContent = sContent.Replace("_SCHEMA_NAME_", sSchemaName);
                sContent = sContent.Replace("_SCP_NAME_", sStructName);

                DateTime currentDateTime = DateTime.Now;
                string date = currentDateTime.ToShortDateString();
                sContent = sContent.Replace("_SCHEMA_DATE_", date);
                sContent = sContent.Replace("_SCHEMA_AUTHOR_", "Xoen Xie");
                sContent = sContent.Replace("_SCHEMA_DESC_", desc);

                File.WriteAllText(sCSVCodeFile, sContent);
                Debug.Log("gen code file, name=" + sCSVCodeFile);
            }

            XSFEditorUtil.ReplaceContentByTag(sCSVCodeFile, "_CSV_LIST_BEGIN_", "_CSV_LIST_END_", sListContent);
        }
        else
        {
            string sCSVTemplate = Application.dataPath + "/../../config/template/SchemaXML.cs";

            string sSchemaName = $"Schema{name}";
            string sXMLCodeFile = Application.dataPath + $"/Scripts/Schema/{sSchemaName}.cs";
            if(!File.Exists(sXMLCodeFile)) 
            {
                string sContent = File.ReadAllText(sCSVTemplate);
                sContent = sContent.Replace("_SCHEMA_NAME_", sSchemaName);
                sContent = sContent.Replace("_SCP_NAME_", sStructName);

                DateTime currentDateTime = DateTime.Now;
                string date = currentDateTime.ToShortDateString();
                sContent = sContent.Replace("_SCHEMA_DATE_", date);
                sContent = sContent.Replace("_SCHEMA_AUTHOR_", "Xoen Xie");
                sContent = sContent.Replace("_SCHEMA_DESC_", desc);

                File.WriteAllText(sXMLCodeFile, sContent);
                Debug.Log("gen code file, name=" + sXMLCodeFile);
            }
        }
    }

    private static void DoServerCode(string name, string sStructName, string desc, SchemaType nType, ref string sIndex)
    {
        if(nType == SchemaType.CSV)
        {
            string sCSVTemplate = Application.dataPath + "/../../config/template/server/SchemaCSV.cs";
            string sCSVFile = Application.dataPath + $"/../../server/bin/scp/{name}.csv";
            if(!File.Exists(sCSVFile))
            {
                Debug.LogError("DoServerCode csv file not exist, can not gen C# code. path=" + sCSVFile);
                return;
            }

            string [] lines = File.ReadAllLines(sCSVFile);
            string [] paramDesc = lines[0].Split(',');
            string [] paramType = lines[1].Split(',');
            string [] paramName = lines[2].Split(',');

            string sStructContent = "";
            string sListContent = "";

            if(name == "Global")
            {   
                // 类型， 值， 名称， 注释， client server
                for(int i = 3; i < lines.Length; i ++)
                {
                    string[] lineData = lines[i].Split(",");

                    if(lineData[4].Contains("*"))
                    {
                        CSVData data = CSVData.GetDataByName(lineData[0]);
                        string newName = XSFEditorUtil.FirstLetterToUpper(lineData[2]);
                        sStructContent += $"\tpublic {data.TypeName} {data.Prefix}{newName};\t//{lineData[3]}\n";

                        sListContent += $"\t\t\tGlobalData.{data.Prefix}{newName} = (csv.GetData((int)CSVDataType.{data.DataType.ToString()}, nRowIndex++, 1) as CSVData_{data.DataType.ToString()}).{data.ValueStr};\t// {lineData[3]}\n";
                    }
                }
            }
            else
            {
                for(int i = 0; i < paramDesc.Length; ++i)
                {
                    CSVData data = CSVData.GetDataByName(paramType[i]);
                    string newName = XSFEditorUtil.FirstLetterToUpper(paramName[i]);
                    sStructContent += $"\tpublic {data.TypeName} {data.Prefix}{newName};\t//{paramDesc[i]}\n";

                    sIndex += $"\t{sStructName}_{paramName[i]} = {i},\n";


                    sListContent += $"\t\t\t\tscp.{data.Prefix}{newName} = (csv.GetData((int)CSVDataType.{data.DataType.ToString()}, i, (int)CSVIndex.{sStructName}_{paramName[i]}) as CSVData_{data.DataType.ToString()}).{data.ValueStr};\t// {paramDesc[i]}\n";
                }
            }
            

            sIndex += "\n";

            XSFEditorUtil.ReplaceContentByTag(SCSStructFile, $"{sStructName}_START", $"{sStructName}_END", sStructContent);

            string sSchemaName = $"Schema{name}";
            string sCSVCodeFile = Application.dataPath + $"/../../server/Schema/{sSchemaName}.cs";
            if(!File.Exists(sCSVCodeFile)) 
            {
                string sContent = File.ReadAllText(sCSVTemplate);
                sContent = sContent.Replace("_SCHEMA_NAME_", sSchemaName);
                sContent = sContent.Replace("_SCP_NAME_", sStructName);

                DateTime currentDateTime = DateTime.Now;
                string date = currentDateTime.ToShortDateString();
                sContent = sContent.Replace("_SCHEMA_DATE_", date);
                sContent = sContent.Replace("_SCHEMA_AUTHOR_", "Xoen Xie");
                sContent = sContent.Replace("_SCHEMA_DESC_", desc);

                File.WriteAllText(sCSVCodeFile, sContent);
                Debug.Log("gen code file, name=" + sCSVCodeFile);
            }

            XSFEditorUtil.ReplaceContentByTag(sCSVCodeFile, "_CSV_LIST_BEGIN_", "_CSV_LIST_END_", sListContent);
        }
        else
        {
            string sCSVTemplate = Application.dataPath + "/../../config/template/server/SchemaXML.cs";

            string sSchemaName = $"Schema{name}";
            string sXMLCodeFile = Application.dataPath + $"/../../server/Schema/{sSchemaName}.cs";
            if(!File.Exists(sXMLCodeFile)) 
            {
                string sContent = File.ReadAllText(sCSVTemplate);
                sContent = sContent.Replace("_SCHEMA_NAME_", sSchemaName);
                sContent = sContent.Replace("_SCP_NAME_", sStructName);

                DateTime currentDateTime = DateTime.Now;
                string date = currentDateTime.ToShortDateString();
                sContent = sContent.Replace("_SCHEMA_DATE_", date);
                sContent = sContent.Replace("_SCHEMA_AUTHOR_", "Xoen Xie");
                sContent = sContent.Replace("_SCHEMA_DESC_", desc);

                File.WriteAllText(sXMLCodeFile, sContent);
                Debug.Log("gen code file, name=" + sXMLCodeFile);
            }
        }
    }
}