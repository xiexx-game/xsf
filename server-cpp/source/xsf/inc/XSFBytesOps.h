//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/xsf.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：二进制操作相关
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#ifndef _XSF_BYTES_OPS_H_
#define _XSF_BYTES_OPS_H_

#include "XSFDef.h"
using namespace xsf;

namespace xsf_byte
{
	enum
	{
		SIZE_U8 = 1,
		SIZE_U16 = 2,
		SIZE_U32 = 4,
		SIZE_U64 = 8,
		SIZE_BOOL = 1,
		SIZE_FLOAT = 4,
		SIZE_DOUBLE = 8,
	};

	inline void WriteUint8(byte *buff, uint8 value)
	{
		buff[0] = value;
	}

	inline uint8 ReadUint8(byte *buff)
	{
		return buff[0];
	}

	inline void WriteUint16(byte *buff, uint16 value)
	{
		byte *dest = reinterpret_cast<byte *>(&value);
		buff[0] = dest[0];
		buff[1] = dest[1];
	}

	inline uint16 ReadUint16(byte *buff)
	{
		uint16 value = 0;
		byte *dest = reinterpret_cast<byte *>(&value);
		dest[0] = buff[0];
		dest[1] = buff[1];

		return value;
	}

	inline void WriteUint32(byte *buff, uint32 value)
	{
		byte *dest = reinterpret_cast<byte *>(&value);
		buff[0] = dest[0];
		buff[1] = dest[1];
		buff[2] = dest[2];
		buff[3] = dest[3];
	}

	inline uint32 ReadUint32(byte *buff)
	{
		uint32 value = 0;
		byte *dest = reinterpret_cast<byte *>(&value);

		dest[0] = buff[0];
		dest[1] = buff[1];
		dest[2] = buff[2];
		dest[3] = buff[3];

		return value;
	}

	inline uint64 ReadUint64(byte *buff)
	{
		uint64 value = 0;
		byte *dest = reinterpret_cast<byte *>(&value);
		dest[0] = buff[0];
		dest[1] = buff[1];
		dest[2] = buff[2];
		dest[3] = buff[3];
		dest[4] = buff[4];
		dest[5] = buff[5];
		dest[6] = buff[6];
		dest[7] = buff[7];

		return value;
	}

	inline void WriteUint64(byte *buff, uint64 value)
	{
		byte *dest = reinterpret_cast<byte *>(&value);

		buff[0] = dest[0];
		buff[1] = dest[1];
		buff[2] = dest[2];
		buff[3] = dest[3];
		buff[4] = dest[4];
		buff[5] = dest[5];
		buff[6] = dest[6];
		buff[7] = dest[7];
	}

	inline float ReadFloat(byte *buff)
	{
		float value = 0;
		byte *dest = reinterpret_cast<byte *>(&value);

		dest[0] = buff[0];
		dest[1] = buff[1];
		dest[2] = buff[2];
		dest[3] = buff[3];

		return value;
	}

	inline void WriteFloat(byte *buff, float value)
	{
		byte *dest = reinterpret_cast<byte *>(&value);

		buff[0] = dest[0];
		buff[1] = dest[1];
		buff[2] = dest[2];
		buff[3] = dest[3];
	}

	inline bool ReadBool(byte *buff)
	{
		if (buff[0] > 0)
		{
			return true;
		}

		return false;
	}

	inline void WriteBool(byte *buff, bool value)
	{
		if (value)
			buff[0] = 1;
		else
			buff[0] = 0;
	}

	inline double ReadDouble(byte *buff)
	{
		double value = 0;
		byte *dest = reinterpret_cast<byte *>(&value);
		dest[0] = buff[0];
		dest[1] = buff[1];
		dest[2] = buff[2];
		dest[3] = buff[3];
		dest[4] = buff[4];
		dest[5] = buff[5];
		dest[6] = buff[6];
		dest[7] = buff[7];

		return value;
	}

	inline void WriteDouble(byte *buff, double value)
	{
		byte *dest = reinterpret_cast<byte *>(&value);

		buff[0] = dest[0];
		buff[1] = dest[1];
		buff[2] = dest[2];
		buff[3] = dest[3];
		buff[4] = dest[4];
		buff[5] = dest[5];
		buff[6] = dest[6];
		buff[7] = dest[7];
	}
}

#endif // end of _XSF_BYTES_OPS_H_
