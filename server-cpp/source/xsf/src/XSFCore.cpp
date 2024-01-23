//////////////////////////////////////////////////////////////////////////
//
// 文件：source/xsf/src/XSF.cpp
// 作者：Xoen Xie
// 时间：2023/08/14
// 描述：The Xsf
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#include "XSF.h"

#include <sys/stat.h>
#include <sys/ioctl.h>
#include <net/if.h>
#include <regex>

#include "XSFMallocHook.h"

#include "SchemaReader.h"
#include "tinyxml2.h"

#include "XSFNet.h"
#include "ConnectionTCP.h"
#include "XSFServer.h"
#include "NetPakcer.h"

namespace xsf
{
	uint64 XSFCore::mTickStart = 0;
	uint32 XSFCore::mRandomSeed = 0;
	int32 XSFCore::mTimeTravel = 0;

	INetPacker * XSFCore::m_ServerPacker = nullptr;
	INetPacker *XSFCore::m_ClientPacker = nullptr;

	ISchemaHelper * XSFCore::mSchemaHelper = nullptr;
    IMessageHelper * XSFCore::mMessageHelper = nullptr;

	// 获取当前启动到现在的时间
    uint32 XSFCore::ClockMonotonic(void)
	{
		struct timespec spec;
		clock_gettime( CLOCK_MONOTONIC, &spec );

		return spec.tv_sec;
	}

	uint32 XSFCore::TickCount(void)
	{
		struct timespec spec;
		clock_gettime( CLOCK_MONOTONIC, &spec );

		uint64 tend = spec.tv_sec * 1000 + spec.tv_nsec/1000000;

		return (uint)(tend - mTickStart);
	}

	// 获取服务器当前时间，基于现在的Unix时间戳，可以继续用32位来存储
	// 游戏中获取当前时间，必须要用这个函数
	uint32 XSFCore::Time(void)
	{
		return time(0) + mTimeTravel;
	}

	// 获取服务器当前时间，基于现在的Unix时间戳，毫秒数
	uint64 XSFCore::TimeMS(void)
	{
		struct timeval tv;
		gettimeofday(&tv, 0);

		return (uint64)(tv.tv_sec + mTimeTravel) * 1000 + tv.tv_usec / 1000;
	}

    // 设置服务器穿越时间, 单位秒，正数往未来增加，负数往过去增加
	void XSFCore::TimeTravel(int nSeconds)
	{
		mTimeTravel = nSeconds;
	}

	// 获得穿梭的时间
	int32 XSFCore::GetTimeTravel()
	{
		return mTimeTravel;
	}

	// 获取工作目录
	char * XSFCore::GetWorkPath(char * buf, size_t size)
	{
		return getcwd(buf, size);
	}

	// 创建目录，类似mkdir -p
	int XSFCore::Mkdir(const char * sDir, int32 nMode )
	{
		if (sDir == nullptr || strlen(sDir) == 0)
		{
			return MKDIR_ERROR_PARAM;
		}

		char sTmp[BUFFER_SIZE_512] = { 0 };
		char * p = nullptr;
		char * pStart = nullptr;

		if (sDir[0] == '/')			// 表示从根目录下开始创建
		{
			pStart = (char*)(sDir + 1);
		}
		else if (sDir[0] == '.' && sDir[1] == '/')	// 表示从当前目录下开始创建
		{
			pStart = (char*)(sDir + 2);
		}
		else		// 其余的都不执行
		{
			return MKDIR_ERROR_DIR_NAME;
		}

		// 递归创建所有不存在的目录
		bool bNeedCreate = true;
		while (bNeedCreate)
		{
			p = strchr(pStart, '/');
			if (p == NULL)
			{
				strncpy(sTmp, sDir, strlen(sDir));
				bNeedCreate = false;
			}
			else
			{
				strncpy(sTmp, sDir, p - sDir);
				pStart = p + 1;
			}

			int s = access(sTmp, F_OK);
			if (s == 0)
				continue;

			int res = mkdir(sTmp, nMode);
			if (res != 0)
			{
				return res;
			}
		}

		return MKDIR_OK;
	}

	// 文件或者目录是否存在
	bool XSFCore::FileExists(const char * sFilename)
	{
		return (access(sFilename, F_OK) == 0);
	}

	// 产生一个[0, 32767]之间的随机数
	int32 XSFCore::Random(void)
	{
		return ((((mRandomSeed) = (mRandomSeed) * 214013L + 2531011L) >> 16) & 0x7fff);
	}

	// 指定范围内[nMin, nMax]的随机数
	int32 XSFCore::RandomRange(int32 nMin, int32 nMax)
	{
		if (nMax > nMin)
		{
			int32 nNum = Random();
			double r = (double)nNum * 0.000030517578125;
			return (int32)floor(r * (nMax - nMin + 1)) + nMin;
		}

		return nMin;
	}

	void XSFCore::Split( const char * sIn, char cSplit, vector<string> & outList )
	{
		if( sIn == nullptr )
			return;

		int32 nLength = strlen(sIn);
		if( nLength <= 0 )
			return;

		char * sCopy = (char*)xsf_malloc(nLength + 1);
		memcpy( sCopy, sIn, nLength);
		sCopy[nLength] = '\0';

		char * pStart = sCopy;
		for( int32 i = 0; i < nLength; ++ i )
		{
			if( sCopy[i] == cSplit )
			{
				sCopy[i] = '\0';

				string s = pStart;

				outList.push_back(s);

				pStart = sCopy + (i + 1);
			}
		}

		if( pStart < sCopy + nLength )
		{
			string s = pStart;
			outList.push_back(s);
		}
		else
		{
			string s = "";
			outList.push_back(s);
		}
		
		xsf_free(sCopy);
	}

	bool XSFCore::GetArgValue( int32 argc, char * argv[], const char * param, char * sOut )
	{
		sOut[0] = '\0';

		for (int32 i = 0; i < argc; ++i)
		{
			char * pArgv = argv[i];

			if (strncmp(param, pArgv, strlen(param)) == 0)
			{
				if(i + 1 >= argc)
				{
					return true;
				}
				else
				{
					strcpy(sOut, argv[i+1]);

					return true;
				}
			}
		}

		return false;
	}

    void XSFCore::GetUTCTM( uint32 nTime, XSF_TM & tmOut )
    {
        time_t t = nTime;
        tm utcinfo;
        gmtime_r(&t, &utcinfo);

        tmOut.sec = utcinfo.tm_sec;
        tmOut.min = utcinfo.tm_min;
        tmOut.hour = utcinfo.tm_hour;
        tmOut.mday = utcinfo.tm_mday;
        tmOut.mon = utcinfo.tm_mon + 1;
        tmOut.year = utcinfo.tm_year + 1900;
        tmOut.wday = utcinfo.tm_wday;
        tmOut.yday = utcinfo.tm_yday;
    }

    void XSFCore::GetLocalTM( uint32 nTime, XSF_TM & tmOut )
    {
        GetUTCTM(nTime, tmOut);
    }

    bool XSFCore::ShellExec( const char * sShell )
    {
        XSF_WARN("XSFCore::ShellExec shell=%s", sShell);

        int32 ret = -1;
	    ret = system(sShell);

	    return (ret != -1 && WIFEXITED(ret) && WEXITSTATUS(ret) == 0);
    }

	// 配置加载
	bool XSFCore::LoadSchema( ISchema * pSchema, uint8 nType, uint32 nID, const char * sFilename, bool IsColTable)
	{
		if( nType == SchemaType_CSV )
		{
			CSVReader reader = CSVReader(IsColTable);
			if( !reader.LoadFile(sFilename) )
				return false;
			
			return pSchema->OnSchemaLoad( nID, &reader);
		}
		else
		{
			XMLReader reader;
			if( !reader.LoadFile(sFilename) )
				return false;

			return pSchema->OnSchemaLoad( nID, &reader);
		}
	}

	// 创建一个网络连接对象
    IConnection * XSFCore::CreateConnection( INetHandler * pHandler, INetPacker *packer )
	{
		return XSFServer::Instance()->GetNet()->CreateConnection(pHandler, packer);
	}

	IServer * XSFCore::GetServer(void)
	{
		return XSFServer::Instance();
	}

	INetPacker * XSFCore::GetServerPacker(void)
    {
        if(m_ServerPacker == nullptr) 
		{
			m_ServerPacker = new ServerPacker();
		}

		return m_ServerPacker;
    }

	INetPacker * XSFCore::GetClientPacker(void)
	{
		if(m_ClientPacker == nullptr)
		{
			m_ClientPacker = new ClientPacker();
		}

		return m_ClientPacker;
	}

	IMessage * XSFCore::GetMessage(uint16 nMessageID)
	{
		return mMessageHelper->GetMessage(nMessageID);
	}

	void XSFCore::SetMessageExecutor(uint16 nMessageID, IMessageExecutor * pExecutor)
	{
		auto pMessage = GetMessage(nMessageID);
		if(pMessage == nullptr)
		{
			XSF_ERROR("XSFCore::SetMessageExecutor pMessage == nullptr, message id=%u", nMessageID);
		}
		else
		{
			pMessage->SetExecutor(pExecutor);
		}
		
	}

	ISchema * XSFCore::GetSchema(uint32 nSchemaID)
	{
		return mSchemaHelper->GetSchema(nSchemaID);
	}
}



