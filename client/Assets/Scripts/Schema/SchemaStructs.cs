//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\SchemaStructs.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置结构定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

namespace XsfScp
{

//全局变量配值
public class ScpGlobal
{
//ScpGlobal_START
	public int iIntData;	//有符号整形
	public string sStringData;	//字符串数据
	public uint uUintData;	//uint整形
	public ulong ulUlongData;	//长整形数据
	public uint uEnergyItem;	//体力道具ID
//ScpGlobal_END
}

//物品配值
public class ScpItem
{
//ScpItem_START
	public uint uId;	//编号
	public int iType;	//类型
	public string sDesc;	//描述
	public float fParam1;	//参数1
	public ulong ulParam2;	//参数2
	public uint [] arParam3;	//参数3
	public CSVIdCount[] icParam4;	//参数4
	public bool bParam5;	//参数5
//ScpItem_END
}

//测试XML配值
public class ScpTestData
{
//ScpTestData_START
//ScpTestData_END
}

//多语言配置
public class ScpLanguage
{
//ScpLanguage_START
	public string sKey;	//键
	public string sValue;	//值
//ScpLanguage_END
}

//多语言配置
public class ScpLanguage_TW
{
//ScpLanguage_TW_START
	public string sKey;	//键
	public string sValue;	//值
//ScpLanguage_TW_END
}
// SCHEMA_STRUCT
// 不要删除上面的标签，自动生成代码需要用到

}
