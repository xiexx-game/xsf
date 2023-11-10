//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/logger/XSFLogger.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：日志功能
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#include "XSFLogger.h"
#include "XSF.h"
#include "XSFLog.h"
#include "XSFServerDef.h"
#include "XSFServer.h"

namespace xsf
{
    static const char* colorPrint[] =
    {
        "\033[0m[%s] [%s-%u] %s\n",					// Info     默认颜色
        "\033[32m[%s] [%s-%u] %s\n\033[0m",			// Warning	深绿
        "\033[35m[%s] [%s-%u] %s\n\033[0m",
        "\033[31m[%s] [%s-%u] %s\n\033[0m",			// Error    红色
    };

	bool XSFLogger::Create(ServerInit * pInit)
	{
		m_bOutputConsole = pInit->OutputConsole;
        m_nLogLevel = pInit->nLogLevel;

		cout << m_bOutputConsole << endl;

		if( !OpenLogFile() )
			return false;

		StartThread();

		return true;
	}

	void XSFLogger::Release(void)
	{
		StopThread();

		delete this;
	}

	void XSFLogger::Push(uint8 nType, const char *format, va_list ap)
	{
		if( !IsWorking() || nType < m_nLogLevel )
			return;

		static thread_local char m_sFormatBuffer[MAX_LOG_SIZE] = { 0 };
        memset( m_sFormatBuffer, 0, sizeof(m_sFormatBuffer) );
		vsnprintf(m_sFormatBuffer, MAX_LOG_SIZE-1, format, ap);
		time_t nCurTime = XSFCore::Time();

		// 如果需要在终端中显示
		if (m_bOutputConsole)
		{
			const SID * pSID = XSFCore::GetServer()->GetSID();
            const char * sTypeName = EP2CNName(pSID->S.type);
            
			char sTime[BUFFER_SIZE_64] = {0};
			XSF_TM sttime;
			XSFCore::GetUTCTM(nCurTime, sttime);
			sprintf(sTime, "%d/%d/%d %d:%d:%d", sttime.year, sttime.mon, sttime.mday, sttime.hour, sttime.min, sttime.sec);

			fprintf(stdout, colorPrint[nType], sTime, sTypeName, pSID->S.index, m_sFormatBuffer);
		}

		LogData * pData	= new LogData();
		pData->m_nTime		= nCurTime;
		pData->m_nType	    = nType;
		pData->Set(m_sFormatBuffer);

		// 因为日志输入会有不同的线程访问，所以在这里要加上一个自旋锁
		XSF_SPINLOCK_LOCK
		m_LogQueue.Push(pData);
		XSF_SPINLOCK_UNLOCK

		ThreadWakeup();
	}

	// 线程退出前被调用
	void XSFLogger::OnThreadExit(void)
	{
		// 即使服务器关闭了，最后的日志也需要写入到文件中
        Pop();

		CloseLogFile();
	}

	void XSFLogger::OnThreadLoop(void)
	{
		Pop();
	}

    bool XSFLogger::IsAllDone(void)
    {
        return m_LogQueue.GetCount() <= 0;
    }


	void XSFLogger::Pop()
	{
        static const char* colorFormat[] =
		{
			"[Info] ",			// Info
			"[Warn] ",			// Warning
			"[Urgen] ",
			"[Error] ",			// Error
		};

		LogData * pData = nullptr;
		while (m_LogQueue.Pop(pData))
		{
            const SID * pSID = XSFCore::GetServer()->GetSID();
            
			XSFCore::GetLocalTM( XSFCore::Time(), m_TM );
			struct timeval tv;
			gettimeofday(&tv, 0);

			sprintf( m_sInnerBuffer, "%u-%02u-%02u %02u:%02u:%02u.%06lu %s %u-%u-%u:%u %s %s\n",
				m_TM.year, m_TM.mon, m_TM.mday, m_TM.hour, m_TM.min, m_TM.sec, tv.tv_usec,
				EP2CNName(pSID->S.type), pSID->S.server, pSID->S.type, pSID->S.index, pSID->ID,
				colorFormat[(int)pData->m_nType], pData->m_sLog );

			fputs(m_sInnerBuffer, m_pFileFD);

			delete pData;
			pData = nullptr;
		}

		fflush(m_pFileFD);
	}


	bool XSFLogger::OpenLogFile(void)
	{
		time_t nTime = XSFCore::Time();
		char sTime[BUFFER_SIZE_64] = {0};

		XSF_TM sttime;
		XSFCore::GetUTCTM(nTime, sttime);
		sprintf(sTime, "%d%d%d-%d%d%d", sttime.year, sttime.mon, sttime.mday, sttime.hour, sttime.min, sttime.sec);

		char sLogFilename[BUFFER_SIZE_512] = {0};

		const SID * pSID = XSFCore::GetServer()->GetSID();

		do
		{
			sprintf(sLogFilename, "%s/log/%s-%s.%u-%s.log", 
				XSFCore::GetServer()->GetInitData()->WorkDir, 
				XSFCore::GetServer()->GetInitData()->sTag, 
				EP2Name(pSID->S.type), m_nLogIndex, sTime);

			if(!XSFCore::FileExists(sLogFilename))
			{
				break;
			}

			m_nLogIndex ++;
		} while (true);
		
		fprintf(stdout, "XSFLogger::OpenLogFile filename=%s\n", sLogFilename);

		m_pFileFD = fopen(sLogFilename, "wt");
		if (!m_pFileFD)
		{
			fprintf(stdout, "XSFLogger::OpenLogFile can not open file...\n");
			return false;
		}

		return true;
	}

	void XSFLogger::CloseLogFile(void)
	{
        if( m_pFileFD == nullptr )
            return;

		fclose(m_pFileFD);
	}

	void XSFLogFile( unsigned char nLogType, const char * sFormat, ... )
	{
		va_list ap;
		va_start(ap, sFormat);
		XSFServer::Instance()->GetLogger()->Push(nLogType, sFormat, ap);
		va_end(ap);
	}
}

