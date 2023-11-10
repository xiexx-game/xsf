//////////////////////////////////////////////////////////////////////////
// 
// 文件：source/xsf/inc/XSFQueue.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：无锁队列
// 说明：同一时刻下，只允许一个线程Pop，一个线程Push
//
//////////////////////////////////////////////////////////////////////////
#ifndef _XSF_QUEUE_H_
#define _XSF_QUEUE_H_

#include "XSFMallocHook.h"

namespace xsf
{
	template <typename T>
	class XSFQueue
	{
		struct _Node
		{
			T Data;
			_Node *  pNext;

			JEMALLOC_HOOK;
		};

	public:
		XSFQueue()
			: m_pHead(nullptr)
			, m_nPushCount(0)
			, m_nPopCount(0)
		{
			m_pHead = new _Node();
			m_pHead->pNext = nullptr;
			m_pTail = m_pHead;
		}

		~XSFQueue()
		{
			if ( m_pHead != nullptr)
			{
				delete m_pHead;
				m_pHead = nullptr;
				m_pTail = nullptr;
			}
		}

		// 压入一个数据
		void Push( T & data )	
		{		
			_Node * pNode = new _Node();
			pNode->Data		= data;
			pNode->pNext	= nullptr;
			m_pTail->pNext	= pNode;
			m_pTail			= pNode;
			
			++m_nPushCount;
		}

		// 取出一个数据
		bool Pop( T & data )
		{
			if ( m_pHead->pNext != nullptr )		
			{			
				++m_nPopCount;
				_Node * pNode = m_pHead;
				m_pHead = m_pHead->pNext;			
				data = m_pHead->Data;

				delete pNode;
				
				return true;		
			}		

			return false;
		}

		uint64 GetCount()
		{
			return m_nPushCount - m_nPopCount;
		}

	private:	
		_Node *  m_pHead;
		_Node *  m_pTail;
		
		volatile uint64 m_nPushCount;	
		volatile uint64 m_nPopCount;
	};

}


#endif