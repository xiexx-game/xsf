//////////////////////////////////////////////////////////////////////////
// 
// 文件：source/xsf/inc/StreamReader.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：自定义二进制读取器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#ifndef _STREAM_READER_H_
#define _STREAM_READER_H_

#include "XSFDef.h"
#include "XSF.h"
#include "XSFLog.h"
#include "XSFBytesOps.h"

namespace xsf
{
	class StreamReader
	{
    private:
        char m_sName[BUFFER_SIZE_64] = {0};

	public:
		StreamReader( const char * sName ) : m_nCurPos(0), m_pBuffer(NULL), m_nSize(0) 
        {
            strcpy( m_sName, sName );
        }

		~StreamReader(void) {};

		void Init( byte * pBuffer, uint32 len )
		{
			m_nCurPos = 0;
			m_nSize = len;
			m_pBuffer = pBuffer;
		};

		byte * Buffer(void) { return m_pBuffer + m_nCurPos; };
		uint32 Size(void) { return m_nSize - m_nCurPos; };

		byte * RawBuffer(void) { return m_pBuffer; };
		uint32 RawSize(void) { return m_nSize; };

		void Jump( uint32 nSize )
		{
			if( m_nCurPos + nSize <= m_nSize )
			{
				m_nCurPos += nSize;
			}
			else
			{
				m_nCurPos = m_nSize;
			}
		}

		void ReadUint8( uint8 & value ) 
		{
			if( m_nCurPos + xsf_byte::SIZE_U8 <= m_nSize )
			{
				value = xsf_byte::ReadUint8( m_pBuffer+m_nCurPos );
				m_nCurPos += xsf_byte::SIZE_U8;
				return;
			}

            XSF_ERROR("StreamReader::ReadUint8 [%s] out of size", m_sName);

			value = 0;
		};

		void ReadInt8( int8 & value ) 
		{
			if( m_nCurPos + xsf_byte::SIZE_U8 <= m_nSize )
			{
				value = xsf_byte::ReadUint8( m_pBuffer+m_nCurPos );
				m_nCurPos += xsf_byte::SIZE_U8;
				return;
			}

            XSF_ERROR("StreamReader::ReadInt8 [%s] out of size", m_sName);

			value = 0;
		};

		void ReadUint16( uint16 & value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U16 <= m_nSize )
			{
				value = xsf_byte::ReadUint16( m_pBuffer+m_nCurPos );
				m_nCurPos += xsf_byte::SIZE_U16;
				return;
			}

            XSF_ERROR("StreamReader::ReadUint16 [%s] out of size: %u m_nCurPos:%u", m_sName, m_nSize, m_nCurPos);

			value = 0;
		};

		void ReadInt16( int16 & value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U16 <= m_nSize )
			{
				value = xsf_byte::ReadUint16( m_pBuffer+m_nCurPos );
				m_nCurPos += xsf_byte::SIZE_U16;
				return;
			}

            XSF_ERROR("StreamReader::ReadInt16 [%s] out of size", m_sName);

			value = 0;
		};

		void ReadUint32( uint32 & value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U32 <= m_nSize )
			{
				value = xsf_byte::ReadUint32( m_pBuffer+m_nCurPos );
				m_nCurPos += xsf_byte::SIZE_U32;
				return;
			}

            XSF_ERROR("StreamReader::ReadUint32 [%s] out of size", m_sName);

			value = 0;
		};

		void ReadInt32( int32 & value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U32 <= m_nSize )
			{
				value = xsf_byte::ReadUint32( m_pBuffer+m_nCurPos );
				m_nCurPos += xsf_byte::SIZE_U32;
				return;
			}

            XSF_ERROR("StreamReader::ReadInt32 [%s] out of size", m_sName);

			value = -1;
		};

		void ReadUint64( uint64 & value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U64 <= m_nSize )
			{
				value = xsf_byte::ReadUint64( m_pBuffer+m_nCurPos );
				m_nCurPos += xsf_byte::SIZE_U64;
				return;
			}

            XSF_ERROR("StreamReader::ReadUint64 [%s] out of size:%u m_nCurPos:%u", m_sName, m_nSize, m_nCurPos);

			value = 0;
		};

		void ReadInt64( int64 & value )
		{
			if( m_nCurPos + xsf_byte::SIZE_U64 <= m_nSize )
			{
				value = xsf_byte::ReadUint64( m_pBuffer+m_nCurPos );
				m_nCurPos += xsf_byte::SIZE_U64;
				return;
			}

            XSF_ERROR("StreamReader::ReadInt64 [%s] out of size", m_sName);

			value = -1;
		};

		void ReadBool( bool & value ) 
		{
			if( m_nCurPos + xsf_byte::SIZE_BOOL <= m_nSize )
			{
				value = xsf_byte::ReadBool( m_pBuffer+m_nCurPos ); 
				m_nCurPos += xsf_byte::SIZE_BOOL; 
				return;
			}

            XSF_ERROR("StreamReader::ReadBool [%s] out of size", m_sName);

			value = false;
		};

		void ReadFloat( float & value ) 
		{
			if( m_nCurPos + xsf_byte::SIZE_FLOAT <= m_nSize )
			{
				value = xsf_byte::ReadFloat( m_pBuffer+m_nCurPos ); 
				m_nCurPos += xsf_byte::SIZE_FLOAT;
				return;
			}

            XSF_ERROR("StreamReader::ReadFloat [%s] out of size", m_sName);

			value = 0.0f;
		};

		void ReadDouble( double & value )
		{
			if( m_nCurPos + xsf_byte::SIZE_DOUBLE <= m_nSize )
			{
				value = xsf_byte::ReadDouble( m_pBuffer+m_nCurPos ); 
				m_nCurPos += xsf_byte::SIZE_DOUBLE;
				return;
			}

            XSF_ERROR("StreamReader::ReadDouble [%s] out of size", m_sName);

			value = 0.0;
		};

		bool ReadString(uint32 nReadLen, char * out, uint32 len )
		{
			if( (m_nCurPos + nReadLen) > m_nSize )
			{
				nReadLen = m_nSize - m_nCurPos;
			}

			if( nReadLen > 0 )
			{
				if( nullptr != out && len > 0 )
				{
					uint32 nCopyLen = (nReadLen + 1) > len ? (len - 1) : nReadLen;
					memcpy( (void*)out, m_pBuffer+m_nCurPos, nCopyLen );
					out[nCopyLen] = '\0';
				}

				m_nCurPos += nReadLen;
			}
			else
			{
				if (nullptr != out && len > 0)
					out[0] = '\0';
			}

			return true;
		};

		bool ReadBuffer(uint32 nReadLen, byte * out, uint32 len )
		{
			if( (m_nCurPos + nReadLen) > m_nSize )
			{
				nReadLen = m_nSize - m_nCurPos;
			}

			if( nReadLen > 0 )
			{
				if( nullptr != out && len > 0 )
				{
					uint32 nCopyLen = nReadLen > len ? len : nReadLen;
					memcpy( out, m_pBuffer+m_nCurPos, nCopyLen );
				}

				m_nCurPos += nReadLen;
			}

			return true;
		}

	private:
		uint32 m_nCurPos;
		byte * m_pBuffer;
		uint32 m_nSize;
	};
}



#endif	// _STREAM_READER_H_