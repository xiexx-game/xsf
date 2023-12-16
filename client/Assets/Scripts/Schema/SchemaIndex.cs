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
	RowScore = 1,		//一行消除的分数
	ScoreAddition = 2,		//分数加成
	MaxLifeCount = 3,		//最大生命值
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

	ScpTetris_id = 0,
	ScpTetris_ChangeData = 1,
	ScpTetris_rows = 2,
	ScpTetris_cols = 3,

	ScpTetrisLevels_id = 0,
	ScpTetrisLevels_Tetris = 1,
	ScpTetrisLevels_DownInterval = 2,
	ScpTetrisLevels_LevelScore = 3,

	ScpSnakeLevels_id = 0,
	ScpSnakeLevels_MoveInterval = 1,
	ScpSnakeLevels_LevelScore = 2,
	ScpSnakeLevels_FoodScore = 3,

	ScpPacManLevels_id = 0,
	ScpPacManLevels_MoveInterval = 1,

	ScpPacManMap_Row = 0,
	ScpPacManMap_Col = 1,
	ScpPacManMap_Sprite = 2,
	ScpPacManMap_XRota = 3,
	ScpPacManMap_YRota = 4,
	ScpPacManMap_ZRota = 5,
	ScpPacManMap_ConnectUp = 6,
	ScpPacManMap_ConnectRight = 7,
	ScpPacManMap_ConnectDown = 8,
	ScpPacManMap_ConnectLeft = 9,
	ScpPacManMap_BlockType = 10,

//CSV_INDEX_END
}




