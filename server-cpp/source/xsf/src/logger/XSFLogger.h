//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/logger/XSFLogger.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：日志
// 说明：
//
//////////////////////////////////////////////////////////////////////////

#ifndef _XSF_LOGGER_H_
#define _XSF_LOGGER_H_

#include <stdarg.h>

#include "XSFThread.h"
#include "XSFMallocHook.h"
#include "XSFQueue.h"

namespace xsf
{
    #define MAX_LOG_SIZE 16384     // 16*1024

    struct ServerInit;

    class XSFLogger : public XSFLoopThread
    {
        struct LogData
        {
            uint32 m_nTime = 0;
            uint8 m_nType = 0;

            char * m_sLog = nullptr;

            ~LogData()
            {
                XSF_FREE(m_sLog);
            };

            JEMALLOC_HOOK

            void Set(const char * log)
            {
                int nLength = strlen(log);
                m_sLog = (char*)xsf_malloc(nLength + 1);
                memcpy(m_sLog, log, nLength);

                m_sLog[nLength] = '\0';
            }
        };


    public:
        XSFLogger(void) {}
        ~XSFLogger(void) {}

        bool Create(ServerInit * pInit);

        void Release(void);

        void Push(uint8 nType, const char *format, va_list ap);

        // 线程退出前被调用
        void OnThreadExit(void) override;

        void OnThreadLoop(void) override;

        bool IsAllDone(void);

        
    private:
        void Pop();

        bool OpenLogFile(void);
        void CloseLogFile(void);

    public:
        XSF_SPINLOCK_DEFINE;

        bool m_bOutputConsole = false;

        FILE * m_pFileFD = nullptr;

        XSFQueue<LogData*> m_LogQueue;

        char m_sInnerBuffer[MAX_LOG_SIZE+BUFFER_SIZE_64] = {0};

        XSF_TM m_TM;

        uint8 m_nLogLevel = 0;
        int32 m_nLogIndex = 1;
    };
}


#endif      // end of _XSF_LOGGER_H_