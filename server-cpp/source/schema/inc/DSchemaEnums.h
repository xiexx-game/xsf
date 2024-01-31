//////////////////////////////////////////////////////////////////////////
//
// 文件：server-cpp/source/schema/inc/DSchemaEnums.h
// 作者：Xoen Xie
// 时间：2020/05/14
// 描述：配置枚举
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _D_SCHEMA_ENUMS_H_
#define _D_SCHEMA_ENUMS_H_


// 该文件不需要手动修改，请在Unity中使用工具导出代码
namespace xsf_scp
{

// SCHEMA_ENUM_START
// 物品类型
enum EMItemType
{
	ItemType_None = 0,	// 空
	ItemType_Normal = 1,	// 普通物品
	ItemType_Hero = 2,	// 英雄物品
	ItemType_Max = 3,	// 最大值
};

// 英雄类型
enum EMHeroType
{
	HeroType_None = 0,	// 空
	HeroType_Attack = 1,	// 攻击型
	HeroType_Defence = 2,	// 防御型
	HeroType_Max = 3,	// 最大值
};

// 英雄测试类型
enum EMTestType
{
	TestType_None = 0,	// 空
	TestType_Test0 = 1,	// 测试0
	TestType_Test1 = 10,	// 测试1
	TestType_Test2 = 11,	// 测试2
	TestType_Test3 = 12,	// 测试3
	TestType_Test4 = 100,	// 测试4
	TestType_Test5 = 101,	// 测试5
	TestType_Test6 = 102,	// 测试6
	TestType_Test7 = 30,	// 测试7
	TestType_Max = 31,	// 最大值
};

// SCHEMA_ENUM_END

}

    





#endif      // end of _D_SCHEMA_ENUMS_H_
