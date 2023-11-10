//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/XSFDef.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：通用定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#ifndef _XSF_DEF_H_
#define _XSF_DEF_H_

#include <inttypes.h>
#include <memory.h>
#include <stdlib.h>
#include <string.h>
#include <sys/time.h>
#include <time.h>
#include <stdio.h>
#include <math.h>
#include <unistd.h>
#include <string>
#include <iostream>

using namespace std;

namespace xsf
{
	// 二进制类型定义
	using byte = unsigned char;

	// 整类型定义
	using int8 = char;
	using int16 = short;
	using int32 = int;
	using int64 = int64_t;

	using uint8 = unsigned char;
	using uint16 = unsigned short;
	using uint32 = unsigned int;
	using uint64 = uint64_t;

	// 长度定义
	#define BUFFER_SIZE_16		16
	#define BUFFER_SIZE_32		32
	#define BUFFER_SIZE_64		64
	#define BUFFER_SIZE_128		128
	#define BUFFER_SIZE_256		256
	#define BUFFER_SIZE_512		512
	#define BUFFER_SIZE_1024	1024

	// IP长度定义
	#define MAX_IP_SIZE 		BUFFER_SIZE_256

	// 无效的文件描述符
	#define INVALID_FD		(-1)

	#define MKDIR_OK				0
    #define MKDIR_ERROR_PARAM		1
    #define MKDIR_ERROR_DIR_NAME	2

	// 检测一个位标记
	#define XSF_CHECK_FLAG(_d, _f)	((_d & _f) == _f)

	// 设置一个位标记
	#define XSF_SET_FLAG( _d, _f, _set )		\
	{											\
		if (_set)								\
		{										\
			_d |= (uint32)_f;					\
		}										\
		else									\
		{										\
			_d = _d & (~(uint32)_f);			\
		}										\
	}

	#define MESSAGE_MODULE_ID   0
	#define SCHEMA_MODULE_ID	1
	#define DEFAULT_START_MODULE_ID	2

	#define SERVER_DISPATCH_TIME	200

	// ID组合
	#define UINT64_ID(id1, id2)		((uint64(id1))<<32 | (uint64(id2)))
	#define FIRST_UINT64_ID(id)		((uint32)((uint64(id))>>32))
	#define SECOND_UINT64_ID(id)	((uint32)((uint64(id))&0xFFFFFFFF))

	#define UINT32_ID(id1, id2)		((uint32(id1))<<16 | (uint32(id2)))
	#define FIRST_UINT32_ID(id)		((uint16)((uint32(id))>>16))
	#define SECOND_UINT32_ID(id)	((uint16)((uint32(id))&0xFFFF))

	#define UINT16_ID(id1, id2)		((uint16(id1))<<8 | (uint16(id2)))
	#define FIRST_UINT16_ID(id)		((uint8)((uint16(id))>>8))
	#define SECOND_UINT16_ID(id)	((uint8)((uint16(id))&0xFF))


	#define XSF_RELEASE(p)						\
	if (nullptr != (p))							\
	{											\
		p->Release();							\
		p = nullptr;							\
	}											\


	#define XSF_DELETE(p)						\
	if (nullptr != (p))							\
	{											\
		delete p;								\
		p = nullptr;							\
	}											\


	#define XSF_DELETE_ARRAY(p)					\
	if (nullptr != (p))							\
	{											\
		delete [] p;							\
		p = nullptr;							\
	}											\


	#define XSF_FREE(p)							\
	if (nullptr != (p))							\
	{											\
		xsf_free(p);							\
		p = nullptr;							\
	}											\


    // 这个宏会定义一个临时变量，禁止使用该宏对成员变量赋值，避免覆盖
	#define XSF_CAST(_DEST, _P, _CLASS)		_CLASS * _DEST = static_cast<_CLASS*>(_P)


	union SID
	{
		uint32 ID;

		struct s
		{
			uint32 index		: 8;		// 索引
			uint32 type			: 8;		// 服务器endpoint类型
			uint32 server		: 16;		// 16位服务器编号，一般一个硬件服（或VPS）对应一个编号
		} S;

		// 客户端ID，
		struct c
		{
			uint32 id			: 16;		// 当前登录索引号
            uint32 key       	: 8;        // 唯一序列号
			uint32 gate			: 8;		// 当前网关序号
		} C;
	};
	

	struct XSF_TM
	{
		int32 sec;
        int32 min;
        int32 hour;
        int32 mday;
        int32 mon;
        int32 year;
        int32 wday;
        int32 yday;
	};
}

#endif	// end of _XSF_DEF_H_
