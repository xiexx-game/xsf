//////////////////////////////////////////////////////////////////////////
// 
// 文件：source/xsf/src/net/XSFNetDef.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：网络相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#ifndef _XSF_NET_DEF_H_
#define _XSF_NET_DEF_H_

#include <sys/socket.h>
#include <netinet/in.h>
#include "XSFDef.h"
#include "XSFMallocHook.h"

namespace xsf
{
	#if (EAGAIN != EWOULDBLOCK)
	#define AGAIN_WOULDBLOCK EAGAIN : case EWOULDBLOCK
	#else
	#define AGAIN_WOULDBLOCK EAGAIN
	#endif

	#define MAX_NET_BUFFER			32768			// 网络默认缓存大小，32K
	#define MIN_NET_BUFFER			1024			// 默认最小接受大小，1K

	enum EMNetStatus
	{
		NetStatus_None = 0,
		NetStatus_Accept,		// 等待连接连入
		NetStatus_Work,
		NetStatus_Connecting,
		NetStatus_Error,
		NetStatus_Release,
	};


	enum EMConnectResult
	{
		ConnectResult_OK = 0,
		ConnectResult_Wait,
		ConnectResult_Error,
	};

	enum EMDispatchResult
	{
		DispatchResult_None = 0,
		DispatchResult_Release,
		DispatchResult_Timeout,
	};

	enum EMSendResult
	{
		SendResult_Run = 0,		// 还需要继续发送
		SendResult_Done = 1,	// 已经发送完毕了
		SendResult_Error,		// 发送出错
	};

	union sockaddr_all
	{
		struct sockaddr s;
		struct sockaddr_in v4;
		struct sockaddr_in6 v6;
	};

	class XSFNet;

	struct INetOperation
	{
		virtual ~INetOperation(void) {}

		JEMALLOC_HOOK;

		virtual void Execute( XSFNet * pNet ) = 0;
	};


	struct INetEvent
	{
		virtual ~INetEvent(void) {}

		JEMALLOC_HOOK;

		virtual void OnEvent(void) = 0;

        virtual void Release(void) { delete this; }
	};


	struct NetBuffer
	{
		byte * m_pBuffer = nullptr;
		uint32 m_nSize = 0;

		uint32 m_nHead = 0;
        uint32 m_nTail = 0;

		void Malloc( uint32 nSize )
		{
            XSF_FREE(m_pBuffer);

			m_nSize = nSize;
			m_pBuffer = (byte*)xsf_malloc(m_nSize);
            m_nHead = 0;
            m_nTail = 0;
		}

		void Free(void)
		{
			XSF_FREE(m_pBuffer);
		}

		void Push(byte * pData, uint32 nLength)
		{
			memcpy(m_pBuffer + m_nTail, pData, nLength);
			m_nTail += nLength;
		}

		byte * EmptyBuffer(uint32 & nEmptySize)
		{
			nEmptySize = m_nSize - m_nTail;
			return m_pBuffer + m_nTail;
		}

		byte * HeadBuffer(uint32 & nSize)
		{
			nSize = m_nSize - m_nHead;
			return m_pBuffer + m_nHead;
		}

		void AddHead(uint32 nSize)
		{
			m_nHead += nSize;
		}

		//void Clear(void)
		void Reset(void)
		{
			m_nHead = 0;
            m_nTail = 0;
		}

		//void Reset(void)
		void Clear(void)
		{
			m_pBuffer = nullptr;
			m_nSize = 0;
			m_nHead = 0;
            m_nTail = 0;
		}
	};


} // namespace xsf

#endif

