//////////////////////////////////////////////////////////////////////////
//
// 文件：client/Assets/Scripts/Schema/SchemaEnums.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：配置枚举定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

namespace XsfScp
{
// SCHEMA_ENUM_START
// 物品类型
public enum ItemType
{
	None = 0,	// 空
	Normal = 1,	// 普通物品
	Hero = 2,	// 英雄物品
	Max = 3,	// 最大值
}

// 英雄类型
public enum HeroType
{
	None = 0,	// 空
	Attack = 1,	// 攻击型
	Defence = 2,	// 防御型
	Max = 3,	// 最大值
}

// 英雄类型
public enum TestType
{
	None = 0,	// 空
	Test0 = 1,	// 测试0
	Test1 = 10,	// 测试1
	Test2 = 11,	// 测试2
	Test3 = 12,	// 测试3
	Test4 = 100,	// 测试4
	Test5 = 101,	// 测试5
	Test6 = 102,	// 测试6
	Test7 = 30,	// 测试7
	Max = 31,	// 最大值
}

// SCHEMA_ENUM_END
}
