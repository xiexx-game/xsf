//////////////////////////////////////////////////////////////////////////
// 
// 文件：source/xsf/inc/StreamWriter.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：自定义二进制写
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#ifndef _STREAM_WRITER_H_
#define _STREAM_WRITER_H_

#include "XSFDef.h"
#include "XSFMallocHook.h"
#include "XSFLog.h"
#include "XSFBytesOps.h"

namespace xsf
{
	#define BUFF_DEFAULT_SIZE	256

	class StreamWriter
	{
	public:
		StreamWriter(void)
			: m_nCurPos(0)
			, m_nSize(BUFF_DEFAULT_SIZE)
		{
			m_pBuffer = (byte*)xsf_malloc(BUFF_DEFAULT_SIZE);
		}
		
		~StreamWriter(void)
		{
			if( m_pBuffer != nullptr )
			{
				xsf_free(m_pBuffer);
				m_pBuffer = nullptr;
			}
		}
		
		void Clear()
		{
			m_nCurPos = 0;
			memset( m_pBuffer, 0, m_nSize );
		};

		byte * Buffer(void) { return m_pBuffer; };
		uint32 Size(void) { return m_nCurPos; };

		void WriteUint8( uint8 value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U8 >= m_nSize )
			{
				Resize();
			}

			xsf_byte::WriteUint8( m_pBuffer+m_nCurPos, value ); 
			m_nCurPos += xsf_byte::SIZE_U8;
		};

		void WriteInt8( int8 value )
		{
			WriteUint8(value);
		};

		void WriteUint16( uint16 value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U16 >= m_nSize )
			{
				Resize();
			}

			xsf_byte::WriteUint16(m_pBuffer + m_nCurPos, value);
			m_nCurPos += xsf_byte::SIZE_U16;
		};

		void WriteInt16( int16 value )
		{	
			WriteUint16(value);
		};

		void WriteUint32( uint32 value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U32 >= m_nSize )
			{
				Resize();
			}

			xsf_byte::WriteUint32( m_pBuffer+m_nCurPos, value );
			m_nCurPos += xsf_byte::SIZE_U32;
		};

		void WriteInt( int32 value )
		{
			WriteUint32(value);
		};

		void WriteUint64( uint64 value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U64 >= m_nSize )
			{
				Resize();
			}

			xsf_byte::WriteUint64( m_pBuffer+m_nCurPos, value );
			m_nCurPos += xsf_byte::SIZE_U64;
		};

		void WriteInt64( int64 value )
		{
			WriteUint64(value);
		};

		void WriteBool( bool value ) 
		{
			if( m_nCurPos + xsf_byte::SIZE_BOOL >= m_nSize )
			{
				Resize();
			}

			xsf_byte::WriteBool( m_pBuffer+m_nCurPos, value ); 
			m_nCurPos += xsf_byte::SIZE_BOOL;
		};

		void WriteFloat( float value ) 
		{
			if( m_nCurPos + xsf_byte::SIZE_FLOAT >= m_nSize )
			{
				Resize();
			}

			xsf_byte::WriteFloat( m_pBuffer+m_nCurPos, value ); 
			m_nCurPos += xsf_byte::SIZE_FLOAT; 
		};

		void WriteDouble( double value )
		{
			if( m_nCurPos + xsf_byte::SIZE_DOUBLE >= m_nSize )
			{
				Resize();
			}

			xsf_byte::WriteDouble( m_pBuffer+m_nCurPos, value ); 
			m_nCurPos += xsf_byte::SIZE_DOUBLE; 
		};

		void WriteString( const char * str )
		{
			uint32 nLen = 0;
			if( nullptr != str )
				nLen = (uint32)strlen(str);
			
			if( (m_nCurPos + nLen) >= m_nSize )
			{
				Resize(nLen);
			}

			if( nLen > 0 )
			{
				memcpy( m_pBuffer+m_nCurPos, str, nLen );
				m_nCurPos += nLen;
			}
		};

		// 写入一个字符串，如果为空，或者长度为0，至少写入1个字节长度
		void WriteU8String( const char * str )
		{
			uint8 nLen = 0;
			if( nullptr != str )
				nLen = (uint8)strlen(str);
			
			if( (m_nCurPos + nLen + xsf_byte::SIZE_U8) >= m_nSize )
			{
				Resize(nLen);
			}

			xsf_byte::WriteUint8( m_pBuffer+m_nCurPos, nLen );
			m_nCurPos += xsf_byte::SIZE_U8;

			if( nLen > 0 )
			{
				memcpy( m_pBuffer+m_nCurPos, str, nLen );
				m_nCurPos += nLen;
			}
		};

		// 写入一个字符串，如果为空，或者长度为0，至少写入2个字节长度
		void WriteU16String( const char * str )
		{
			uint16 nLen = 0;

			if( nullptr != str )
				nLen = (uint16)strlen(str);

			if( (m_nCurPos + nLen + xsf_byte::SIZE_U16) >= m_nSize )
			{
				Resize(nLen);
			}

			xsf_byte::WriteUint16( m_pBuffer+m_nCurPos, nLen );
			m_nCurPos += xsf_byte::SIZE_U16;

			if( nLen > 0 )
			{
				memcpy( m_pBuffer+m_nCurPos, str, nLen );
				m_nCurPos += nLen;
			}

		};

		// 写入一个Buff数据，如果为空，或者长度为0，至少会写入1个字节长度
		void WriteU8Buffer( const byte * buff, uint8 len )
		{
			if( (m_nCurPos+len+xsf_byte::SIZE_U8) >= m_nSize )
			{
				Resize(len);
			}

			if( nullptr != buff && len > 0 )
			{
				xsf_byte::WriteUint8( m_pBuffer+m_nCurPos, len );
				m_nCurPos += xsf_byte::SIZE_U8;

				memcpy( m_pBuffer+m_nCurPos, buff, len );
				m_nCurPos += len;
			}
			else
			{
				xsf_byte::WriteUint8( m_pBuffer+m_nCurPos, 0 );
				m_nCurPos += xsf_byte::SIZE_U8;
			}
			
		}

		// 写入一个Buff数据，如果为空，或者长度为0，至少会写入2个字节长度
		void WriteU16Buffer( const byte * buff, uint16 len )
		{
			if( (m_nCurPos+len+xsf_byte::SIZE_U16) >= m_nSize )
			{
				Resize(len);
			}

			if( nullptr != buff && len > 0 )
			{
				xsf_byte::WriteUint16( m_pBuffer+m_nCurPos, len );			// 至少写入长度数据
				m_nCurPos += xsf_byte::SIZE_U16;

				memcpy( m_pBuffer+m_nCurPos, buff, len );
				m_nCurPos += len;
			}
			else
			{
				xsf_byte::WriteUint16( m_pBuffer+m_nCurPos, 0 );			// 至少写入长度数据
				m_nCurPos += xsf_byte::SIZE_U16;
			}
		}

		// 写入一个Buff数据，如果为空，或者长度为0，则不写数据
		void WriteBuffer( const byte * buff, uint32 len )
		{
			if( nullptr == buff || 0 == len )
			{
				return;
			}

			if( (m_nCurPos+len) >= m_nSize )
			{
				Resize(len);
			}

			memcpy( m_pBuffer+m_nCurPos, buff, len );
			m_nCurPos += len;
		}

		byte * BeginWriteBuffer(uint32 len)
		{
			if ((m_nCurPos + len) >= m_nSize)
			{
				Resize(len);
			}

			m_nLastWriteBufferLength = len;

			return m_pBuffer + m_nCurPos;
		}

		void EndWriteBuffer(void)
		{
			m_nCurPos += m_nLastWriteBufferLength;
			m_nLastWriteBufferLength = 0;
		}

		void Save( const char * sFilename )
		{
			FILE* fp = 0;
			fp = fopen(sFilename, "wb");
			if (!fp)
			{
				XSF_ERROR("StreamWriter can't open file, filename=%s", sFilename);
				return;
			}

			fwrite(m_pBuffer, 1, m_nCurPos, fp);
			fclose(fp);
		}

	private:
		void Resize( uint32 nLength = 0 )
		{
            uint32 nLenAdd = BUFF_DEFAULT_SIZE*2;
            if( nLength > nLenAdd  )
            {
                nLenAdd = nLength + BUFF_DEFAULT_SIZE*2;
            }

			uint32 nNewSize = m_nSize + nLenAdd;

	#ifdef _ONE_DEBUG_
			//XSF_WARN("StreamWriter::Resize new size is [%u], old size=%u, nLenAdd=%u", nNewSize, m_nSize, nLenAdd );
	#endif

			// 重新分配空间
			byte * pNewBuff = (byte*)xsf_malloc(nNewSize);
			memset( pNewBuff, 0, nNewSize );

			// 把旧数据拷贝到新的空间里
			memcpy( pNewBuff, m_pBuffer, m_nSize );

            //XSF_ERROR("StreamWriter::Resize pNewBuff=%lu, nNewSize=%u, m_pBuffer=%lu, m_nSize=%u", pNewBuff, nNewSize, m_pBuffer, m_nSize);

			// 释放旧的空间
			xsf_free(m_pBuffer);

			// 重新赋值地址和大小
			m_pBuffer = pNewBuff;
			m_nSize = nNewSize;
		}

	private:
		uint32 m_nCurPos;

		uint32 m_nSize;
		byte * m_pBuffer;

		uint32 m_nLastWriteBufferLength;
	};
}






#endif	// _ONE_STREAM_WRITER_H_