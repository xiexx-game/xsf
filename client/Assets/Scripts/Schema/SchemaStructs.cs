//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\Scripts\Schema\SchemaStructs.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置结构定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////


/*ScpGlobal_START
	public uint uId;	//编号
	public string sType;	//类型
	public string sValue;	//值
	public string sEnumDef;	//枚举定义
	public string sDesc;	//描述
ScpGlobal_END*/

//全局变量配值
public class ScpGlobal
{
    public uint uId;	//编号
    public CSVData data;
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
// SCHEMA_STRUCT
// 不要删除上面的标签，自动生成代码需要用到
