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
    [MenuItem("XSFTools/导出配置", false, (int)XSFMenuID.ExportSchema)]
    public static void ExportSchema()
    {
        XmlNodeList list = DoExport();

        Debug.Log("schema export done ...");

        DoCode(list);
    }

    private static XmlNodeList DoExport()
    {
        string sToolDir = Application.dataPath + "/../../config/tools/bin/";
        string sClientOutput = Application.dataPath + "/Scp/";
        string sServerOutput = Application.dataPath + "/../../server-cpp/bin/scp/";

        string sToolArg = $"ConfigSplit.py {sClientOutput} {sServerOutput}";
        Debug.Log(sToolArg);

        XSFEditorUtil.StartProcess("python", sToolArg, sToolDir);

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
                int nLoad = XMLReader.GetInt(ele, "cs");
                if(nLoad > 0)
                {
                    string sSrcXml = Application.dataPath + $"/../../config/{sName}.xml";
                    File.Copy(sSrcXml, sClientOutput + $"{sName}.xml", true);
                }
            }
            
        }

        AssetDatabase.Refresh();

        AASTools.UpdateAASGroup();

        AssetDatabase.Refresh();

        return nodeList;
    }

    static string sCSStructFile = Application.dataPath + "/Scripts/Schema/SchemaStructs.cs";
    static string sCSHelperFile = Application.dataPath + "/Scripts/Schema/GameSchemaHelper.cs";
    static string sCSIndexFile = Application.dataPath + "/Scripts/Schema/SchemaIndex.cs"; 

    private static void DoCode(XmlNodeList nodeList)
    {
        string sCSID = "";
        string sCSCreate = "";
        string sCSIndex = "";

        for (int i = 0; i < nodeList.Count; ++i)
        {
            XmlElement ele = nodeList[i] as XmlElement;

            int nID = XMLReader.GetInt(ele, "id");
            string sName = XMLReader.GetString(ele, "name");
            string sDesc = XMLReader.GetString(ele, "desc");

            int nCSLoad = XMLReader.GetInt(ele, "cs");
            int nLuaLoad = XMLReader.GetInt(ele, "Lua");
            int nGoLoad = XMLReader.GetInt(ele, "go");
            int nCppLoad = XMLReader.GetInt(ele, "cpp");
            SchemaType nType = (SchemaType)XMLReader.GetUInt(ele, "type");

            string sStructName = $"Scp{sName}";

            if(nCSLoad > 0)
            {
                sCSID += $"\t{sName} = {nID},\n";

                sCSCreate += $"\t\t\tcase SchemaID.{sName}: return new Schema{sName}();\n";

                XSFEditorUtil.ReplaceContentByTag(sCSHelperFile, "SCHEMA_ID_BEGIN", "SCHEMA_ID_END", sCSID);
                XSFEditorUtil.ReplaceContentByTag(sCSHelperFile, "SCHEMA_BEGIN", "SCHEMA_END", sCSCreate);

                string sStructHead = $"public class {sStructName}";
                string sStructContent = File.ReadAllText(sCSStructFile);
                if(!sStructContent.Contains(sStructHead)) 
                {
                    string sStructAdd = $"//{sDesc}\n" + sStructHead + "\n{\n" + $"//{sStructName}_START\n//{sStructName}_END\n" + "}\n";
                    XSFEditorUtil.AppendFileByTag(sCSStructFile, "SCHEMA_STRUCT", sStructAdd);
                }

                DoCSCode(sName, sStructName, sDesc, nType, ref sCSIndex);
            }
            else if(nLuaLoad > 0)
            {
                DoLuaCode(sName, nType);
            }
            else if(nCppLoad > 0)
            {
                DoCppCode(sName, nType);
            }
            else if(nGoLoad > 0)
            {
                DoGoCode(sName, nType);
            }

        }

        XSFEditorUtil.ReplaceContentByTag(sCSIndexFile, "CSV_INDEX_BEGIN", "CSV_INDEX_END", sCSIndex);
    }

    private static void DoCSCode(string name, string sStructName, string desc, SchemaType nType, ref string sIndex)
    {
        if(nType == SchemaType.CSV)
        {
            string sCSVTemplate = Application.dataPath + "/../../config/tools/template/SchemaCSV.cs";
            string sCSVFile = Application.dataPath + $"/Scp/{name}.csv";
            if(!File.Exists(sCSVFile))
            {
                Debug.LogError("csv file not exist, can not gen code. path=" + sCSVFile);
                return;
            }

            string [] lines = File.ReadAllLines(sCSVFile);
            string [] paramDesc = lines[0].Split(',');
            string [] paramType = lines[1].Split(',');
            string [] paramName = lines[2].Split(',');

            string sStructContent = "";
            string sListContent = "";
            for(int i = 0; i < paramDesc.Length; ++i)
            {
                CSVData data = CSVData.GetDataByName(paramType[i]);
                string newName = XSFEditorUtil.FirstLetterToUpper(paramName[i]);
                sStructContent += $"\tpublic {data.TypeName} {data.Prefix}{newName};\t//{paramDesc[i]}\n";

                sIndex += $"\t{sStructName}_{paramName[i]} = {i},\n";


                sListContent += $"\t\t\tscp.{data.Prefix}{newName} = (csv.GetData((int)CSVDataType.{data.DataType.ToString()}, i, (int)CSVIndex.{sStructName}_{paramName[i]}) as CSVData_{data.DataType.ToString()}).{data.ValueStr};\t// {paramDesc[i]}\n";
            }

            sIndex += "\n";

            XSFEditorUtil.ReplaceContentByTag(sCSStructFile, $"{sStructName}_START", $"{sStructName}_END", sStructContent);

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

            if(name == "Global")
            {
                string sGlobalID = "";
                for(int i = 3; i < lines.Length; ++i)
                {
                    string [] datas = lines[i].Split(',');
                    sGlobalID += $"\t{datas[(int)CSVIndex.ScpGlobal_enumDef]} = {datas[(int)CSVIndex.ScpGlobal_id]},\t\t//{datas[(int)CSVIndex.ScpGlobal_desc]}\n";
                }

                XSFEditorUtil.ReplaceContentByTag(sCSIndexFile, "GLOBAL_ID_START", "GLOBAL_ID_END", sGlobalID);
            }

        }
        else
        {
            string sCSVTemplate = Application.dataPath + "/../../config/tools/template/SchemaXML.cs";

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

    private static void DoLuaCode(string name, SchemaType nType)
    {

    }

    private static void DoCppCode(string name, SchemaType nType)
    {

    }

    private static void DoGoCode(string name, SchemaType nType)
    {

    }
}