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

//关卡游戏
public class ScpLevelGame
{
//ScpLevelGame_START
	public uint uId;	//编号
	public string sName;	//名称
	public uint [] arShowUIs;	//显示UI
	public string [] sarSceneObjects;	//场景资源
//ScpLevelGame_END
}

//俄罗斯方块配置
public class ScpTetris
{
//ScpTetris_START
	public uint uId;	//编号
	public ArrayData [] arChangeData;	//变体数据
	public int [] arRows;	//出生的行
	public int [] arCols;	//出生的列
//ScpTetris_END
}

//俄罗斯方块游戏关卡配置
public class ScpTetrisLevels
{
//ScpTetrisLevels_START
	public uint uId;	//编号
	public uint [] arTetris;	//俄罗斯方块
	public float fDownInterval;	//掉落时间间隔
	public uint uLevelScore;	//等级分数
//ScpTetrisLevels_END
}

//贪吃蛇游戏关卡配置
public class ScpSnakeLevels
{
//ScpSnakeLevels_START
	public uint uId;	//编号
	public float fMoveInterval;	//移动时间间隔
	public uint uLevelScore;	//等级分数
	public uint uFoodScore;	//食物分数
//ScpSnakeLevels_END
}

//吃豆人游戏关卡配置
public class ScpPacManLevels
{
//ScpPacManLevels_START
	public uint uId;	//编号
	public float fMoveInterval;	//移动时间间隔
//ScpPacManLevels_END
}

//吃豆人游戏地图配置
public class ScpPacManMap
{
//ScpPacManMap_START
	public int iRow;	//行
	public int iCol;	//列
	public string sSprite;	//图片
	public float fXRota;	//X旋转
	public float fYRota;	//Y旋转
	public float fZRota;	//z旋转
	public uint uBlockType;	//块类型
//ScpPacManMap_END
}
// SCHEMA_STRUCT
// 不要删除上面的标签，自动生成代码需要用到
