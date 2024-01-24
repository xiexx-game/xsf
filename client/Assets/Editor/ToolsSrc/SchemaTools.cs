//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Editor\Schema\SchemaTools.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置工具
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8602, CS8618, CS8604, CS8603

using System.IO;
using System.Xml;
using System;
using System.Collections.Generic;

using NPOI.XSSF.UserModel;

namespace XSFTools
{
    public class ConfigData
    {
        public uint ID;
        public string Name;
        public string Desc;
        public int ColTable;
        public SchemaType nType;
        public int ClientLoad;
        public int ServerLoad;
    }

    public class SchemaTools
    {
        private const int CTABLE_MAX_COL = 6;
        private const int RTABLE_LAST_ROW = 4;
        public static void DoExport()
        {
            string sClientOutput = Helper.Instance.ClientDir + "/Assets/Scp";
            string sServerOutput = Helper.Instance.ServerDir + "/bin/scp";
            string sCppOutput = Helper.Instance.CppServerDir + "/bin/scp";

            Dictionary<string, int> Enums = EnumExport();

            string sLoadXml = Helper.Instance.ConfigDir + "/Load.xml";
            if(Directory.Exists(sClientOutput))
            {
                File.Copy(sLoadXml, sClientOutput + "/Load.xml", true);
            }
            
            if(Directory.Exists(sServerOutput)) {
                File.Copy(sLoadXml, sServerOutput + "/Load.xml", true);
            }

            if(Directory.Exists(sCppOutput)) {
                File.Copy(sLoadXml, sCppOutput + "/Load.xml", true);
            }

            string sLoadContent = File.ReadAllText(sLoadXml);

            XMLReader reader = new XMLReader();
            reader.Read(sLoadContent);

            Dictionary<string, ConfigData> Configs = new Dictionary<string, ConfigData>();
            
            XmlNodeList nodeList = reader.mRootNode.ChildNodes;
            for (int i = 0; i < nodeList.Count; ++i)
            {
                XmlElement ele = nodeList[i] as XmlElement;

                ConfigData cd = new ConfigData();

                cd.ID = XMLReader.GetUInt(ele, "id");
                cd.Name = XMLReader.GetString(ele, "name");

                cd.Desc = XMLReader.GetString(ele, "desc");

                cd.ClientLoad = XMLReader.GetInt(ele, "client");
                cd.ServerLoad = XMLReader.GetInt(ele, "server");
                cd.ColTable = XMLReader.GetInt(ele, "col_table");

                cd.nType = (SchemaType)XMLReader.GetUInt(ele, "type");
                if(cd.nType == SchemaType.XML) 
                {   
                    string sSrcXml = Helper.Instance.ConfigDir + $"/{cd.Name}.xml";

                    if(cd.ClientLoad > 0)
                    {
                        if(Directory.Exists(sClientOutput))
                        {
                            File.Copy(sSrcXml, sClientOutput + $"/{cd.Name}.xml", true);
                        }
                    }

                    if(cd.ServerLoad > 0)
                    {
                        if(Directory.Exists(sServerOutput))
                        {
                            File.Copy(sSrcXml, sServerOutput + $"/{cd.Name}.xml", true);
                        }

                        if(Directory.Exists(sCppOutput))
                        {
                            File.Copy(sSrcXml, sCppOutput + $"/{cd.Name}.xml", true);
                        }
                    }
                }

                Configs.Add(cd.Name, cd);
            }

            ExcelExport(Enums, Configs, sClientOutput, sServerOutput, sCppOutput);

            Helper.Instance.Logger.Log("configs export done ....");

            DoCode(Configs);
        }

        private static Dictionary<string, int> EnumExport()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            string CEnumPath = Helper.Instance.ClientDir + "/Assets/Scripts/Schema/SchemaEnums.cs";
            string SEnumPath = Helper.Instance.ServerDir + "/Schema/SchemaEnums.cs";
            string CppEnumPath = Helper.Instance.CppServerDir + "/source/schema/inc/DSchemaEnums.h";

            bool cExist = File.Exists(CEnumPath);
            bool sExist = File.Exists(SEnumPath);
            bool cppExist = File.Exists(CppEnumPath);

            string sEnumXml = Helper.Instance.ConfigDir + "/Enum.xml";
            string sEnumContent = File.ReadAllText(sEnumXml);
            XMLReader reader = new XMLReader();
            reader.Read(sEnumContent);

            XmlNodeList nodeList = reader.mRootNode.ChildNodes;

            string cStr = "";
            string sStr = "";
            string cppStr = "";
            for (int i = 0; i < nodeList.Count; ++i)
            {
                XmlElement ele = nodeList[i] as XmlElement;
                string name = XMLReader.GetString(ele, "name");
                string desc = XMLReader.GetString(ele, "desc");

                if(cExist)
                {
                    cStr += $"// {desc}\n";
                    cStr += $"public enum {name}\n" + "{\n";
                }

                if(sExist)
                {
                    sStr += $"// {desc}\n";
                    sStr += $"public enum {name}\n" + "{\n";
                }

                if(cppExist)
                {
                    cppStr += $"// {desc}\n";
                    cppStr += $"enum EM{name}\n" + "{\n";
                }

                var enums = ele.ChildNodes;
                int nValue = 0;
                for(int J = 0; J < enums.Count; J ++)
                {
                    XmlElement subE = enums[J] as XmlElement;
                    //<item name="None" desc="空" value="0" />
                    string ename = XMLReader.GetString(subE, "name");
                    string edesc = XMLReader.GetString(subE, "desc");
                    string evalue = XMLReader.GetString(subE, "value");

                    if(string.IsNullOrEmpty(evalue))
                    {
                        if(J < nValue)
                        {
                            nValue ++;
                        }
                        else
                        {
                            nValue = J;
                        }
                    }
                    else
                    {
                        nValue = System.Convert.ToInt32(evalue);
                    }

                    string key = $"{name}.{ename}";

                    //Helper.Instance.Logger.Log($"key={key}, v={nValue}");
                    result.Add(key, nValue);

                    if(cExist)
                    {
                        cStr += $"\t{ename} = {nValue},\t// {edesc}\n";
                    }

                    if(sExist)
                    {
                        sStr += $"\t{ename} = {nValue},\t// {edesc}\n";
                    }

                    if(cppExist)
                    {
                        cppStr += $"\t{name}_{ename} = {nValue},\t// {edesc}\n";
                    }
                }

                if(cExist)
                {
                    cStr += "}\n\n";
                }

                if(sExist)
                {
                    sStr += "}\n\n";
                }

                if(cppExist)
                {
                    cppStr += "};\n\n";
                }
            }

            if(cExist)
            {
                Helper.ReplaceContentByTag(CEnumPath, "SCHEMA_ENUM_START", "SCHEMA_ENUM_END", cStr);
            }

            if(sExist)
            {
                Helper.ReplaceContentByTag(SEnumPath, "SCHEMA_ENUM_START", "SCHEMA_ENUM_END", sStr);
            }

            if(cppExist)
            {
                Helper.ReplaceContentByTag(CppEnumPath, "SCHEMA_ENUM_START", "SCHEMA_ENUM_END", cppStr);
            }

            return result;
        }

        private static void ExcelExport(Dictionary<string, int> enums, Dictionary<string, ConfigData> configs, string sClientOutput, string sServerOutput, string sCppOutput)
        {
            DirectoryInfo di = new DirectoryInfo(Helper.Instance.ConfigDir);
            FileInfo[] files = di.GetFiles("*.xlsx");
            for(int i = 0; i < files.Length; i ++)
            {
                Helper.Instance.Logger.Log("准备导出配置:" + files[i].FullName);
                XSSFWorkbook workbook = new XSSFWorkbook(files[i].FullName);
                XSSFSheet sheet = (XSSFSheet)workbook.GetSheetAt(0);
                

                ConfigData cd = null;
                if(configs.TryGetValue(sheet.SheetName, out cd))
                {
                    if(cd.ClientLoad >= 0 || cd.ServerLoad >= 0)
                    {
                        if(cd.ColTable > 0)     // 列表
                        {
                            XSSFRow row = (XSSFRow)sheet.GetRow(0);
                            Helper.Instance.Logger.Log($"列表：表名{sheet.SheetName}, 总共 {sheet.LastRowNum+1}行，{row.LastCellNum}列");
                            if(row.LastCellNum != CTABLE_MAX_COL)
                            {
                                Helper.Instance.Logger.LogError($"行表：表名{sheet.SheetName}, 格式错误，表头不是6列");
                            }
                            else
                            {
                                ExportColTable(enums, cd, sheet, sheet.LastRowNum+1, sClientOutput, sServerOutput, sCppOutput);
                            }
                        }
                        else    // 行表
                        {
                            XSSFRow row = (XSSFRow)sheet.GetRow(2);
                            Helper.Instance.Logger.Log($"行表：表名{sheet.SheetName}, 总共 {sheet.LastRowNum+1}行，{row.LastCellNum}列");
                            if(sheet.LastRowNum < RTABLE_LAST_ROW)
                            {
                                Helper.Instance.Logger.LogError($"行表：表名{sheet.SheetName}, 格式错误，表头不足5行");
                            }
                            else
                            {
                                ExportTable(enums, cd, sheet, row.LastCellNum, sClientOutput, sServerOutput, sCppOutput);
                            }
                        }
                    }
                    else
                    {
                        Helper.Instance.Logger.Log($"表名：{sheet.SheetName} 未在Load.xml中配置导出项，跳过导出");
                    }   
                }
                else
                {
                    Helper.Instance.Logger.Log($"表名：{sheet.SheetName} 未在Load.xml中配置，按行表导出");
                    XSSFRow row = (XSSFRow)sheet.GetRow(2);
                    Helper.Instance.Logger.Log($"行表：表名{sheet.SheetName}, 总共 {sheet.LastRowNum+1}行，{row.LastCellNum}列");
                    if(sheet.LastRowNum < RTABLE_LAST_ROW)
                    {
                        Helper.Instance.Logger.LogError($"行表：表名{sheet.SheetName}, 格式错误，表头不足5行");
                    }
                    else
                    {
                        ExportTable(enums, cd, sheet, row.LastCellNum, sClientOutput, sServerOutput, sCppOutput);
                    }
                }
            }
        }

        // 列表
        private static void ExportColTable(Dictionary<string, int> enums, ConfigData cd, XSSFSheet sheet, int TotalRow, string sClientOutput, string sServerOutput, string sCppOutput)
        {
            string clientStr = "";
            string serverStr = "";

            for(int r = 1; r < TotalRow; r ++)
            {
                var row = sheet.GetRow(r);
                
                bool ClientExport = row.GetCell(1).StringCellValue.Contains("*");
                bool ServerExport = row.GetCell(0).StringCellValue.Contains("*");
                
                string clientLine = "";
                string serverLine = "";
                for(int c = 2; c < CTABLE_MAX_COL; c ++)
                {   
                    var cell = row.GetCell(c);
                    string cellStr = cell.ToString();

                    if(cellStr.Contains("."))
                    {
                        int v = 0;
                        if(enums.TryGetValue(cellStr, out v))
                        {
                            cellStr = v.ToString();
                        }
                    }

                    if(ClientExport)
                    {
                        clientLine += cellStr + ",";
                    }

                    if(ServerExport)
                    {
                        serverLine += cellStr + ",";
                    }
                }

                if(clientLine.Length > 0)
                {
                    clientStr += clientLine.Substring(0, clientLine.Length-1) + "\n";
                }
                
                if(serverLine.Length > 0)
                {
                    serverStr += serverLine.Substring(0, serverLine.Length-1) + "\n";
                }
            }

            string sConfigName = cd != null ? cd.Name : sheet.SheetName;
            if(clientStr.Length > 0)
            {
                if(Directory.Exists(sClientOutput))
                {
                    clientStr = clientStr.Substring(0, clientStr.Length-1);
                    File.WriteAllText(sClientOutput + $"/{sConfigName}.csv", clientStr);
                }
            }

            if(serverStr.Length > 0)
            {
                serverStr = serverStr.Substring(0, serverStr.Length-1);

                if(Directory.Exists(sServerOutput))
                {
                    File.WriteAllText(sServerOutput +$"/{sConfigName}.csv", serverStr);
                }

                if(Directory.Exists(sCppOutput))
                {
                    File.WriteAllText(sCppOutput +$"/{sConfigName}.csv", serverStr);
                }
            }
        }

        // 行表
        private static void ExportTable(Dictionary<string, int> enums, ConfigData cd, XSSFSheet sheet, int TotalCol, string sClientOutput, string sServerOutput, string sCppOutput)
        {
            bool []ClientExport = new bool[TotalCol];
            bool []ServerExport = new bool[TotalCol];

            XSSFRow row0 = (XSSFRow)sheet.GetRow(0);
            for(int c = 0; c < row0.LastCellNum; c ++)
            {
                if(c >= TotalCol)
                    break;

                XSSFCell cell = (XSSFCell)row0.GetCell(c);
                if(cell.StringCellValue.Contains("*"))
                {
                    ServerExport[c] = true;
                }
            }

            XSSFRow row1 = (XSSFRow)sheet.GetRow(1);
            for(int c = 0; c < row1.LastCellNum; c ++)
            {
                if(c >= TotalCol)
                    break;

                XSSFCell cell = (XSSFCell)row1.GetCell(c);
                if(cell.StringCellValue.Contains("*"))
                {
                    ClientExport[c] = true;
                }
            }

            string clientStr = "";
            string serverStr = "";
            for(int r = 2; r <= sheet.LastRowNum; r ++)
            {
                string clientLine = "";
                string serverLine = "";
                XSSFRow row = (XSSFRow)sheet.GetRow(r);
                if(r <= 4)
                {
                    
                }
                else
                {
                    var cell = row.GetCell(0);
                    if(!cell.StringCellValue.Contains("*"))
                    {
                        continue;
                    }
                }

                for(int c = 1; c < TotalCol; c++)
                {
                    var cell = row.GetCell(c);
                    if(cell == null)
                    {
                        if(ClientExport[c])
                        {
                            clientLine += ",";
                        }

                        if(ServerExport[c])
                        {
                            serverLine += ",";
                        }
                    }
                    else
                    {
                        string cellStr = cell.ToString();
                        if(cellStr.Contains("."))
                        {
                            int v = 0;
                            if(enums.TryGetValue(cellStr, out v))
                            {
                                cellStr = v.ToString();
                            }
                        }

                        if(ClientExport[c])
                        {
                            clientLine += cellStr + ",";
                        }

                        if(ServerExport[c])
                        {
                            serverLine += cellStr + ",";
                        }
                    }
                }

                if(clientLine.Length > 0)
                {
                    clientStr += clientLine.Substring(0, clientLine.Length-1) + "\n";
                }
                
                if(serverLine.Length > 0)
                {
                    serverStr += serverLine.Substring(0, serverLine.Length-1) + "\n";
                }
            }

            string sConfigName = cd != null ? cd.Name : sheet.SheetName;
            if(clientStr.Length > 0)
            {
                if(Directory.Exists(sClientOutput))
                {
                    clientStr = clientStr.Substring(0, clientStr.Length-1);
                    File.WriteAllText(sClientOutput + $"/{sConfigName}.csv", clientStr);
                }
            }

            if(serverStr.Length > 0)
            {
                serverStr = serverStr.Substring(0, serverStr.Length-1);
                if(Directory.Exists(sServerOutput))
                {
                    File.WriteAllText(sServerOutput +$"/{sConfigName}.csv", serverStr);
                }

                if(Directory.Exists(sCppOutput))
                {
                    File.WriteAllText(sCppOutput +$"/{sConfigName}.csv", serverStr);
                }
            }
        }

        static string CCSStructFile = Helper.Instance.ClientDir + "/Assets/Scripts/Schema/SchemaStructs.cs";
        static string CCSHelperFile = Helper.Instance.ClientDir + "/Assets/Scripts/Schema/GameSchemaHelper.cs";
        static string CCSIndexFile = Helper.Instance.ClientDir + "/Assets/Scripts/Schema/SchemaIndex.cs"; 

        static string SCSStructFile = Helper.Instance.ServerDir + "/Schema/SchemaStructs.cs";
        static string SCSHelperFile = Helper.Instance.ServerDir + "/Schema/GameSchemaHelper.cs";
        static string SCSIndexFile = Helper.Instance.ServerDir + "/Schema/SchemaIndex.cs"; 

        static string SCppDefFile = Helper.Instance.CppServerDir + "/source/schema/inc/DSchemaDef.h";
        static string SCppIDFile = Helper.Instance.CppServerDir + "/source/schema/inc/DSchemaID.h";

        static string SCppIndexFile = Helper.Instance.CppServerDir + "/source/schema/src/schemas/SchemaIndex.h";
        static string SCppHeadersFile = Helper.Instance.CppServerDir + "/source/schema/src/schemas/SchemaHeaders.h";

        private static void DoCode(Dictionary<string, ConfigData> configs)
        {
            string CCSID = "";
            string CCSCreate = "";
            string CCSIndex = "";

            string SCSID = "";
            string SCSCreate = "";
            string SCSIndex = "";

            string SCppID = "";
            string SCppCreate = "#define SCHEMA_CREATE				\\\n";
            string SCppIndex = "";
            string SCppHeaders = "";

            bool IsClientDirExist = File.Exists(CCSStructFile);
            bool IsServerDirExist = File.Exists(SCSStructFile);
            bool IsCppServerExist = File.Exists(SCppDefFile);

            foreach( KeyValuePair<string, ConfigData> kv in configs)
            {
                ConfigData cd = kv.Value;
                string sStructName = $"Scp{cd.Name}";

                if(cd.ClientLoad > 0 && IsClientDirExist)
                {
                    CCSID += $"\t{cd.Name} = {cd.ID},\n";

                    CCSCreate += $"\t\t\tcase SchemaID.{cd.Name}: return new Schema{cd.Name}();\n";

                    string sStructHead = $"public class {sStructName}";
                    string sStructContent = File.ReadAllText(CCSStructFile);
                    if(!sStructContent.Contains(sStructHead)) 
                    {
                        string sStructAdd = $"//{cd.Desc}\n" + sStructHead + "\n{\n" + $"//{sStructName}_START\n//{sStructName}_END\n" + "}\n";
                        Helper.AppendFileByTag(CCSStructFile, "SCHEMA_STRUCT", sStructAdd);
                    }

                    DoCSCode(cd.ColTable, cd.Name, sStructName, cd.Desc, cd.nType, ref CCSIndex);
                }
                
                if(cd.ServerLoad > 0)
                {
                    if(IsServerDirExist)
                    {
                        SCSID += $"\t\t{cd.Name} = {cd.ID},\n";

                        SCSCreate += $"\t\t\t\tcase SchemaID.{cd.Name}: return new Schema{cd.Name}();\n";

                        string sStructHead = $"public class {sStructName}";
                        string sStructContent = File.ReadAllText(SCSStructFile);
                        if(!sStructContent.Contains(sStructHead)) 
                        {
                            string sStructAdd = $"//{cd.Desc}\n" + sStructHead + "\n{\n" + $"//{sStructName}_START\n//{sStructName}_END\n" + "}\n";
                            Helper.AppendFileByTag(SCSStructFile, "SCHEMA_STRUCT", sStructAdd);
                        }

                        DoServerCode(cd.ColTable, cd.Name, sStructName, cd.Desc, cd.nType, ref SCSIndex);
                    }

                    if(IsCppServerExist)
                    {
                        SCppID += $"\tSchemaID_{cd.Name} = {cd.ID},\n";

                        SCppCreate += $"\tcase SchemaID_{cd.Name}:		m_SchemaList[nSchemaID] = new Schema{cd.Name}();	break;			\\\n";

                        SCppHeaders += $"#include \"Schema{cd.Name}.h\"\n";

                        string sStructHead = $"struct {sStructName}";
                        string sStructContent = File.ReadAllText(SCppDefFile);
                        if(!sStructContent.Contains(sStructHead))
                        {
                            string sStructAdd = $"//{cd.Desc}\n" + sStructHead + "\n{\n" + $"//{sStructName}_START\n//{sStructName}_END\n" + "};\n";
                            Helper.AppendFileByTag(SCppDefFile, "SCHEMA_STRUCT", sStructAdd);
                        }

                        DoCppCode(cd.ColTable, cd.Name, sStructName, cd.Desc, cd.nType, ref SCppIndex);
                    }
                }
            }

            if(IsClientDirExist)
            {
                Helper.ReplaceContentByTag(CCSHelperFile, "SCHEMA_ID_BEGIN", "SCHEMA_ID_END", CCSID);
                Helper.ReplaceContentByTag(CCSHelperFile, "SCHEMA_BEGIN", "SCHEMA_END", CCSCreate);
                Helper.ReplaceContentByTag(CCSIndexFile, "CSV_INDEX_BEGIN", "CSV_INDEX_END", CCSIndex);
            }

            if(IsServerDirExist)
            {
                Helper.ReplaceContentByTag(SCSHelperFile, "SCHEMA_ID_BEGIN", "SCHEMA_ID_END", SCSID);
                Helper.ReplaceContentByTag(SCSHelperFile, "SCHEMA_BEGIN", "SCHEMA_END", SCSCreate);
                Helper.ReplaceContentByTag(SCSIndexFile, "CSV_INDEX_BEGIN", "CSV_INDEX_END", SCSIndex);
            }

            if(IsCppServerExist)
            {
                Helper.ReplaceContentByTag(SCppIDFile, "SCHEMA_ID_BEGIN", "SCHEMA_ID_END", SCppID);
                Helper.ReplaceContentByTag(SCppIDFile, "SCHEMA_CREATE_BEGIN", "SCHEMA_CREATE_END", SCppCreate);
                Helper.ReplaceContentByTag(SCppIndexFile, "CSV_INDEX_BEGIN", "CSV_INDEX_END", SCppIndex);
                Helper.ReplaceContentByTag(SCppHeadersFile, "SCHEMA_BEGIN", "SCHEMA_END", SCppHeaders);
            }
        }

        private static void DoCSCode(int nColTable, string name, string sStructName, string desc, SchemaType nType, ref string sIndex)
        {
            if(nType == SchemaType.CSV)
            {
                string sCSVTemplate = Helper.Instance.ConfigDir + "/template/SchemaCSV.cs";
                string sCSVFile = Helper.Instance.ClientDir + $"/Assets/Scp/{name}.csv";
                if(!File.Exists(sCSVFile))
                {
                    Helper.Instance.Logger.LogError("csv file not exist, can not gen C# code. path=" + sCSVFile);
                    return;
                }

                string [] lines = File.ReadAllLines(sCSVFile);
                
                string sStructContent = "";
                string sListContent = "";

                if(nColTable > 0)
                {
                    for(int i = 0; i < lines.Length; i ++)
                    {
                        string[] lineData = lines[i].Split(",");

                        var ca = Helper.Instance.GetConfigAttribute(lineData[1]);
                        if(ca == null)
                        {
                            Helper.Instance.Logger.LogError($"ConfigAttribute is null, name={lineData[1]}, please config in CodeConfig.xml");
                        }

                        string newName = Helper.FirstLetterToUpper(lineData[2]);
                        sStructContent += $"\tpublic {ca.CsTypeName} {ca.Prefix}{newName};\t//{lineData[0]}\n";

                        string nRowIndex = $"{sStructName}_{newName}";
                        sIndex += $"\t{sStructName}_{newName} = {i},\n";

                        sListContent += $"\t\t\t{name}Data.{ca.Prefix}{newName} = (csv.GetData((int)CSVDataType.{ca.Suffix}, (int)CSVIndex.{nRowIndex}, 3) as CSVData_{ca.Suffix}).{ca.ValueStr};\t// {lineData[0]}\n";
                    }
                }
                else
                {
                    string [] paramDesc = lines[0].Split(',');
                    string [] paramType = lines[1].Split(',');
                    string [] paramName = lines[2].Split(',');

                    for(int i = 0; i < paramDesc.Length; ++i)
                    {
                        var ca = Helper.Instance.GetConfigAttribute(paramType[i]);
                        if(ca == null)
                        {
                            Helper.Instance.Logger.LogError($"ConfigAttribute is null, name={paramType[i]}, please config in CodeConfig.xml");
                        }

                        string newName = Helper.FirstLetterToUpper(paramName[i]);
                        sStructContent += $"\tpublic {ca.CsTypeName} {ca.Prefix}{newName};\t//{paramDesc[i]}\n";

                        sIndex += $"\t{sStructName}_{paramName[i]} = {i},\n";


                        sListContent += $"\t\t\tscp.{ca.Prefix}{newName} = (csv.GetData((int)CSVDataType.{ca.Suffix}, i, (int)CSVIndex.{sStructName}_{paramName[i]}) as CSVData_{ca.Suffix}).{ca.ValueStr};\t// {paramDesc[i]}\n";
                    }
                }
                

                sIndex += "\n";

                Helper.ReplaceContentByTag(CCSStructFile, $"{sStructName}_START", $"{sStructName}_END", sStructContent);

                string sSchemaName = $"Schema{name}";
                string sCSVCodeFile = Helper.Instance.ClientDir + $"/Assets/Scripts/Schema/{sSchemaName}.cs";
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
                    Helper.Instance.Logger.Log("gen code file, name=" + sCSVCodeFile);
                }

                Helper.ReplaceContentByTag(sCSVCodeFile, "_CSV_LIST_BEGIN_", "_CSV_LIST_END_", sListContent);
            }
            else
            {
                string sCSVTemplate = Helper.Instance.ConfigDir + "/template/SchemaXML.cs";

                string sSchemaName = $"Schema{name}";
                string sXMLCodeFile = Helper.Instance.ClientDir + $"/Assets/Scripts/Schema/{sSchemaName}.cs";
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
                    Helper.Instance.Logger.Log("gen code file, name=" + sXMLCodeFile);
                }
            }
        }

        private static void DoServerCode(int nColTable, string name, string sStructName, string desc, SchemaType nType, ref string sIndex)
        {
            if(nType == SchemaType.CSV)
            {
                string sCSVTemplate = Helper.Instance.ConfigDir + "/template/server/SchemaCSV.cs";
                string sCSVFile = Helper.Instance.ServerDir + $"/bin/scp/{name}.csv";
                if(!File.Exists(sCSVFile))
                {
                    Helper.Instance.Logger.LogError("DoServerCode csv file not exist, can not gen C# code. path=" + sCSVFile);
                    return;
                }

                string [] lines = File.ReadAllLines(sCSVFile);
                
                string sStructContent = "";
                string sListContent = "";

                if(nColTable > 0)
                {   
                    // 类型， 值， 名称， 注释， client server
                    for(int i = 0; i < lines.Length; i ++)
                    {
                        string[] lineData = lines[i].Split(",");

                        var ca = Helper.Instance.GetConfigAttribute(lineData[1]);
                        if(ca == null)
                        {
                            Helper.Instance.Logger.LogError($"ConfigAttribute is null, name={lineData[1]}, please config in CodeConfig.xml");
                        }

                        string newName = Helper.FirstLetterToUpper(lineData[2]);
                        sStructContent += $"\tpublic {ca.CsTypeName} {ca.Prefix}{newName};\t//{lineData[0]}\n";

                        string nRowIndex = $"{sStructName}_{newName}";
                        sIndex += $"\t{sStructName}_{newName} = {i},\n";

                        sListContent += $"\t\t\t{name}Data.{ca.Prefix}{newName} = (csv.GetData((int)CSVDataType.{ca.Suffix}, (int)CSVIndex.{nRowIndex}, 3) as CSVData_{ca.Suffix}).{ca.ValueStr};\t// {lineData[0]}\n";
                    }
                }
                else
                {
                    string [] paramDesc = lines[0].Split(',');
                    string [] paramType = lines[1].Split(',');
                    string [] paramName = lines[2].Split(',');

                    for(int i = 0; i < paramDesc.Length; ++i)
                    {
                        var ca = Helper.Instance.GetConfigAttribute(paramType[i]);
                        if(ca == null)
                        {
                            Helper.Instance.Logger.LogError($"ConfigAttribute is null, name={paramType[i]}, please config in CodeConfig.xml");
                        }

                        string newName = Helper.FirstLetterToUpper(paramName[i]);
                        sStructContent += $"\tpublic {ca.CsTypeName} {ca.Prefix}{newName};\t//{paramDesc[i]}\n";

                        sIndex += $"\t{sStructName}_{paramName[i]} = {i},\n";


                        sListContent += $"\t\t\t\tscp.{ca.Prefix}{newName} = (csv.GetData((int)CSVDataType.{ca.Suffix}, i, (int)CSVIndex.{sStructName}_{paramName[i]}) as CSVData_{ca.Suffix}).{ca.ValueStr};\t// {paramDesc[i]}\n";
                    }
                }
                

                sIndex += "\n";

                Helper.ReplaceContentByTag(SCSStructFile, $"{sStructName}_START", $"{sStructName}_END", sStructContent);

                string sSchemaName = $"Schema{name}";
                string sCSVCodeFile = Helper.Instance.ServerDir + $"/Schema/{sSchemaName}.cs";
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
                    Helper.Instance.Logger.Log("gen code file, name=" + sCSVCodeFile);
                }

                Helper.ReplaceContentByTag(sCSVCodeFile, "_CSV_LIST_BEGIN_", "_CSV_LIST_END_", sListContent);
            }
            else
            {
                string sCSVTemplate = Helper.Instance.ConfigDir + "/template/server/SchemaXML.cs";

                string sSchemaName = $"Schema{name}";
                string sXMLCodeFile = Helper.Instance.ServerDir + $"/Schema/{sSchemaName}.cs";
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
                    Helper.Instance.Logger.Log("gen code file, name=" + sXMLCodeFile);
                }
            }
        }

        private static void DoCppCode(int nColTable, string name, string sStructName, string desc, SchemaType nType, ref string sIndex)
        {
            //*
            if(nType == SchemaType.CSV)
            {
                string sHTemplate = Helper.Instance.ConfigDir + "/template/server-cpp/SchemaCSV.h";
                string sCppTemplate = Helper.Instance.ConfigDir + "/template/server-cpp/SchemaCSV.cpp";

                string sSchemaName = $"Schema{name}";

                string sHCodeFile = Helper.Instance.CppServerDir + $"/source/schema/src/schemas/{sSchemaName}.h";
                string sCppCodeFile = Helper.Instance.CppServerDir + $"/source/schema/src/schemas/{sSchemaName}.cpp";

                DateTime currentDateTime = DateTime.Now;
                string date = currentDateTime.ToShortDateString();

                string sCSVFile = Helper.Instance.CppServerDir + $"/bin/scp/{name}.csv";
                if(!File.Exists(sCSVFile))
                {
                    Helper.Instance.Logger.LogError("DoCppCode csv file not exist, can not gen C# code. path=" + sCSVFile);
                    return;
                }

                string [] lines = File.ReadAllLines(sCSVFile);

                string sStructContent = "";
                string sListContent = "";

                if(nColTable > 0)
                {   
                    // 类型， 值， 名称， 注释， client server
                    for(int i = 0; i < lines.Length; i ++)
                    {
                        string[] lineData = lines[i].Split(",");

                        var ca = Helper.Instance.GetConfigAttribute(lineData[1]);
                        if(ca == null)
                        {
                            Helper.Instance.Logger.LogError($"ConfigAttribute is null, name={lineData[1]}, please config in CodeConfig.xml");
                        }

                        string newName = Helper.FirstLetterToUpper(lineData[2]);

                        sStructContent += $"\t{ca.CppTypeName} {ca.Prefix}{newName};\t//{lineData[0]}\n";

                        string nRowIndex = $"CSVIndex_{sStructName}_{newName}";
                        sIndex += $"\tCSVIndex_{sStructName}_{newName} = {i},\n";

                        sListContent += $"\t\t{name}Data.{ca.Prefix}{newName} = GET_DATA(CSVData_{ca.Suffix}, CSVDataType_{ca.Suffix}, {nRowIndex}, 3)->{ca.ValueStr};\t// {lineData[0]}\n";
                    }
                }
                else
                {
                    string [] paramDesc = lines[0].Split(',');
                    string [] paramType = lines[1].Split(',');
                    string [] paramName = lines[2].Split(',');

                    for(int i = 0; i < paramDesc.Length; ++i)
                    {
                        var ca = Helper.Instance.GetConfigAttribute(paramType[i]);
                        if(ca == null)
                        {
                            Helper.Instance.Logger.LogError($"ConfigAttribute is null, name={paramType[i]}, please config in CodeConfig.xml");
                        }

                        string newName = Helper.FirstLetterToUpper(paramName[i]);
                        sStructContent += $"\t{ca.CppTypeName} {ca.Prefix}{newName};\t//{paramDesc[i]}\n";

                        string nColIndex = $"CSVIndex_{sStructName}_{paramName[i]}";
                        sIndex += $"\tCSVIndex_{sStructName}_{paramName[i]} = {i},\n";

                        sListContent += $"\t\t\tscp->{ca.Prefix}{newName} = GET_DATA(CSVData_{ca.Suffix}, CSVDataType_{ca.Suffix}, i, {nColIndex})->{ca.ValueStr};;\t// {paramDesc[i]}\n";
                    }
                }
                

                sIndex += "\n";

                Helper.ReplaceContentByTag(SCppDefFile, $"{sStructName}_START", $"{sStructName}_END", sStructContent);

                if(!File.Exists(sCppCodeFile)) 
                {
                    string sContent = File.ReadAllText(sHTemplate);
                    sContent = sContent.Replace("_SCHEMA_NAME_", sSchemaName);
                    sContent = sContent.Replace("_SCP_NAME_", sStructName);
                    sContent = sContent.Replace("_SCHEMA_DATE_", date);
                    sContent = sContent.Replace("_SCHEMA_AUTHOR_", "Xoen Xie");
                    sContent = sContent.Replace("_SCHEMA_DESC_", desc);
                    File.WriteAllText(sHCodeFile, sContent);
                    Helper.Instance.Logger.Log("gen code file, name=" + sHCodeFile);

                    sContent = File.ReadAllText(sCppTemplate);
                    sContent = sContent.Replace("_SCHEMA_NAME_", sSchemaName);
                    sContent = sContent.Replace("_SCP_NAME_", sStructName);
                    sContent = sContent.Replace("_SCHEMA_DATE_", date);
                    sContent = sContent.Replace("_SCHEMA_AUTHOR_", "Xoen Xie");
                    sContent = sContent.Replace("_SCHEMA_DESC_", desc);
                    File.WriteAllText(sCppCodeFile, sContent);
                    Helper.Instance.Logger.Log("gen code file, name=" + sCppCodeFile);
                }

                Helper.ReplaceContentByTag(sCppCodeFile, "_CSV_LIST_BEGIN_", "_CSV_LIST_END_", sListContent);
            }
            else
            {
                string sHTemplate = Helper.Instance.ConfigDir + "/template/server-cpp/SchemaXML.h";
                string sCppTemplate = Helper.Instance.ConfigDir + "/template/server-cpp/SchemaXML.cpp";

                string sSchemaName = $"Schema{name}";

                string sHCodeFile = Helper.Instance.CppServerDir + $"/source/schema/src/schemas/{sSchemaName}.h";
                string sCppCodeFile = Helper.Instance.CppServerDir + $"/source/schema/src/schemas/{sSchemaName}.cpp";

                DateTime currentDateTime = DateTime.Now;
                string date = currentDateTime.ToShortDateString();

                if(!File.Exists(sHCodeFile)) 
                {
                    string sContent = File.ReadAllText(sHTemplate);
                    sContent = sContent.Replace("_SCHEMA_NAME_", sSchemaName);
                    sContent = sContent.Replace("_SCP_NAME_", sStructName);
                    sContent = sContent.Replace("_SCHEMA_DATE_", date);
                    sContent = sContent.Replace("_SCHEMA_AUTHOR_", "Xoen Xie");
                    sContent = sContent.Replace("_SCHEMA_DESC_", desc);

                    File.WriteAllText(sHCodeFile, sContent);
                    Helper.Instance.Logger.Log("gen code file, name=" + sHCodeFile);

                    sContent = File.ReadAllText(sCppTemplate);
                    sContent = sContent.Replace("_SCHEMA_NAME_", sSchemaName);
                    sContent = sContent.Replace("_SCP_NAME_", sStructName);
                    sContent = sContent.Replace("_SCHEMA_DATE_", date);
                    sContent = sContent.Replace("_SCHEMA_AUTHOR_", "Xoen Xie");
                    sContent = sContent.Replace("_SCHEMA_DESC_", desc);

                    File.WriteAllText(sCppCodeFile, sContent);
                    Helper.Instance.Logger.Log("gen code file, name=" + sCppCodeFile);
                }
            }
            //*/
        }
    }

}