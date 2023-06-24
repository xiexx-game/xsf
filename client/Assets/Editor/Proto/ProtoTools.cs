//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Editor\Proto\ProtoTools.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：pb tools
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEditor;
using UnityEngine;
using System.IO;

public static class ProtoTools
{
#if UNITY_EDITOR_WIN
    static string PROTOC_DIR = Application.dataPath + "/../../proto/bin/win/";
    static string PROTOC = PROTOC_DIR + "protoc.exe";
#else
    static string protocDir = Application.dataPath + "/../../proto/bin/mac/";
    static string PROTOC = PROTOC_DIR + "protoc";
#endif

    [MenuItem("XSFTools/Proto/Export All", false, (int)XSFMenuID.PBExportAll)]
    public static void PBExportAll()
    {
        PBExportCS();
        PBExportLua();
    }

    [MenuItem("XSFTools/Proto/Export proto to csharp", false, (int)XSFMenuID.PBExportCS)]
    public static void PBExportCS()
    {
        string outputDir = Application.dataPath + "/Scripts/Net/Proto/";

        string [] csProto = new string[] { "ClientCS.proto", "CMessageID.proto"};

        for(int i = 0; i < csProto.Length; i ++)
        {
            XSFEditorUtil.StartProcess(PROTOC,
                $"--csharp_out={outputDir}  --proto_path=../../ {csProto[i]}", PROTOC_DIR);
        }

        Debug.Log("Export to csharp done...");
    }

    [MenuItem("XSFTools/Proto/Export proto to lua", false, (int)XSFMenuID.PBExportLua)]
    public static void PBExportLua()
    {
        string [] bytesProto = new string[] {"ClientLua.proto"};

        string outputDir = Application.dataPath + "/ProtoBytes/";

        for(int i = 0; i < bytesProto.Length; i ++)
        {
            XSFEditorUtil.StartProcess(PROTOC,
                $"--descriptor_set_out={outputDir}{bytesProto[i].Replace("proto", "bytes")}  --proto_path=../../ {bytesProto[i]}", PROTOC_DIR);
        }

        AssetDatabase.Refresh();

        string [] luaProto = new string[] { "CMessageID.proto", "Common.proto"};

        for(int i = 0; i < luaProto.Length; i ++)
        {
            string protoFile = Application.dataPath + "/../../proto/" + luaProto[i];
            string luaFile = Application.dataPath + "/../Lua/Proto/" + luaProto[i].Replace("proto", "lua");

            string content = "";
            string[] lines = File.ReadAllLines(protoFile);
            for(int J = 0; J < lines.Length; J ++)
            {
                if(lines[J].Contains("proto3") || lines[J].Contains("package")) {

                }
                else
                {
                    string line = lines[J];
                    line = line.Replace("//", "--");
                    line = line.Replace("{", "= {");
                    line = line.Replace(";", ",");
                    line = line.Replace("enum ", "XSF.");
                    content += line + "\n";
                }
            }

            File.WriteAllText(luaFile, content);
        }

        AASTools.UpdateAASGroup();

        Debug.Log("Export to lua done...");
    }

    [MenuItem("XSFTools/Proto/Export proto to cpp", false, (int)XSFMenuID.PBExportCpp)]
    public static void PBExportCpp()
    {
        string outputDir = Application.dataPath + "/Scripts/Net/Proto/";

        XSFEditorUtil.StartProcess(PROTOC,
            $"--cpp_out={outputDir}  --proto_path=../../ ClientCS.proto", PROTOC_DIR);
        XSFEditorUtil.StartProcess(PROTOC,
            $"--cpp_out={outputDir}  --proto_path=../../ CMessageID.proto", PROTOC_DIR);

        Debug.Log("Export to cpp done...");
    }
}