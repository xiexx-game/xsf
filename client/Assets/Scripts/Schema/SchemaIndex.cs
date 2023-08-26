//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Schema\SchemaIndex.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置索引定义
// 说明：由工具自动化生成，禁止使用手动修改
//
//////////////////////////////////////////////////////////////////////////

public enum GlobalID
{
//GLOBAL_ID_START
	intData = 1,		//有符号整形
	stringData = 2,		//字符串数据
	uintData = 3,		//uint整形
	ulongData = 4,		//长整形数据
	floatData = 5,		//浮点数
	uintArrayData = 6,		//无符号整形数组
	boolData = 7,		//布尔数据
	iacData = 8,		//id和count数据
//GLOBAL_ID_END
}

public enum CSVIndex
{
//CSV_INDEX_BEGIN
	ScpGlobal_id = 0,
	ScpGlobal_type = 1,
	ScpGlobal_value = 2,
	ScpGlobal_enumDef = 3,
	ScpGlobal_desc = 4,

	ScpItem_id = 0,
	ScpItem_type = 1,
	ScpItem_desc = 2,
	ScpItem_param1 = 3,
	ScpItem_param2 = 4,
	ScpItem_param3 = 5,
	ScpItem_param4 = 6,
	ScpItem_param5 = 7,

	ScpLanguage_key = 0,
	ScpLanguage_value = 1,

	ScpLevelGame_id = 0,
	ScpLevelGame_Name = 1,
	ScpLevelGame_ShowUIs = 2,
	ScpLevelGame_SceneObjects = 3,

//CSV_INDEX_END
}




