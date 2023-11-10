//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/XSFLog.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：日志定义接口
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _XSF_LOG_API_H_
#define _XSF_LOG_API_H_

namespace xsf
{
	enum EMLogType
	{
		LogType_Info = 0,		// 普通
		LogType_Warning,		// 警告
		LogType_Urgent,			// 紧急
		LogType_Error,		    // 错误
	};

	// 日志数据
	void XSFLogFile( unsigned char nLogType, const char * sFormat, ... );

	#define XSF_INFO( _FORMAT, ... )		XSFLogFile( LogType_Info, _FORMAT, ##__VA_ARGS__ )
	#define XSF_WARN( _FORMAT, ... )	    XSFLogFile( LogType_Warning, _FORMAT, ##__VA_ARGS__ )
	#define XSF_URGENT( _FORMAT, ... )	    XSFLogFile( LogType_Urgent, _FORMAT, ##__VA_ARGS__ )
	#define XSF_ERROR( _FORMAT, ... )		XSFLogFile( LogType_Error, _FORMAT, ##__VA_ARGS__ )

#ifdef _XSF_DEBUG_
    #define FUNCTION_ENTER(_N)            XSF_INFO("NORMAL:[%s] --> enter[%d]", __PRETTY_FUNCTION__, _N)
    #define FUNCTION_EXIT(_N)            XSF_INFO("NORMAL:[%s] exit[%d] -->", __PRETTY_FUNCTION__, _N)
#else
    #define FUNCTION_ENTER(_N)
    #define FUNCTION_EXIT(_N)
#endif


#ifdef _XSF_NET_DEBUG_
	#define NET_FUNCTION_ENTER(_N, _SID)		XSF_WARN("NET:[%s] --> enter[%d], socket id:[%u]", __PRETTY_FUNCTION__, _N, _SID)
	#define NET_FUNCTION_EXIT(_N, _SID)			XSF_WARN("NET:[%s] exit[%d], socket id:[%u]", __PRETTY_FUNCTION__, _N, _SID)
#else
	#define NET_FUNCTION_ENTER(_N, _SID)
	#define NET_FUNCTION_EXIT(_N, _SID)
#endif


    
}


#endif      // end of _XSF_LOG_API_H_