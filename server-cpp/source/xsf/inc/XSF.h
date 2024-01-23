//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/inc/XSF.h
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：Main XSF
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#ifndef _XSF_H_
#define _XSF_H_

#include "XSFDef.h"

#include "XSFLog.h"
#include "IXSFSchema.h"
#include "IXSFNet.h"
#include "IXSFServer.h"
#include "IXSFMessage.h"
#include "IXSFTimer.h"
#include "XSFMallocHook.h"
#include "XSFServerDef.h"

namespace xsf
{
    class XSFCore
    {
        friend class XSFHelper;
    public:
        // 获取当前启动到现在的时间
        static uint32 ClockMonotonic(void);

        static uint32 TickCount(void);

        // 获取服务器当前时间，基于现在的Unix时间戳，可以继续用32位来存储
        // 游戏中获取当前时间，必须要用这个函数
        static uint32 Time(void);

        // 获取服务器当前时间，基于现在的Unix时间戳，毫秒数
        static uint64 TimeMS(void);

        // 设置服务器穿越时间, 单位秒，正数往未来增加，负数往过去增加
        static void TimeTravel(int nSeconds);

        // 获得穿梭的时间
        static int32 GetTimeTravel();

        // 获取工作目录
        static char * GetWorkPath(char * buf, size_t size);

        // 创建目录，类似mkdir -pint
        static int32 Mkdir(const char * sDir, int32 nMode );

        static bool FileExists(const char * sFilename);

        // 产生一个[0, 32767]之间的随机数
        static int32 Random(void);

        // 指定范围内[nMin, nMax]的随机数
        static int32 RandomRange(int32 nMin, int32 nMax);

        // 按照指定的字符进行字符串分割
        static void Split( const char * sIn, char cSplit, vector<string> & outList );

        static bool GetArgValue( int32 argc, char * argv[], const char * param, char * sOut );

        static void GetUTCTM( uint32 nTime, XSF_TM & tmOut );

        static void GetLocalTM( uint32 nTime, XSF_TM & tmOut );

    public:
        static bool ShellExec( const char * sShell );

    public:
        // 配置加载
        static bool LoadSchema( ISchema * pSchema, uint8 nType, uint32 nID, const char * sFilename, bool IsColTable);

    public:
        // 创建一个网络连接对象
        static IConnection * CreateConnection( INetHandler * pHandler, INetPacker *packer );

    public:
        static IServer * GetServer(void);

        static bool Create(uint8 nLocalEP, int argc, char* argv[]);

        // main函数主逻辑
        static void Run();

        static INetPacker * GetServerPacker(void);

        static INetPacker * GetClientPacker(void);

        static IMessage * GetMessage(uint16 nMessageID);

        static void SetMessageExecutor(uint16 nMessageID, IMessageExecutor * pExecutor);

        static ISchema * GetSchema(uint32 nSchemaID);

    private:
        static uint64 mTickStart;
        static uint32 mRandomSeed;
        static int32 mTimeTravel;

        static INetPacker *m_ServerPacker;
        static INetPacker *m_ClientPacker;

    public:
        static ISchemaHelper * mSchemaHelper;
        static IMessageHelper * mMessageHelper;
    };
}





#endif          // end of _XSF_H_