// ////////////////////////////////////////////////////////////////////////
//
// 文件：XSF/Base/XSFLogger.cs
// 作者：Xiexx
// 时间：2023/01/09
// 描述：日志模块
// 说明：
//
// ////////////////////////////////////////////////////////////////////////
using Serilog;

namespace XSF
{
    internal class XSFLogger
    {
        public static void Init()
        {
            var config = new LoggerConfiguration();
            config.MinimumLevel.Debug();

            var InitData = XSFServer.Instance.InitData;

            if(InitData.OutputConsole)
                config.WriteTo.Console();

            string logName = "";
            int logIndex = 0;

            string dir = $"{InitData.WorkDir}/logs";
            if(Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);

                bool checkAgain = false;
                do
                {
                    checkAgain = false;
                    logName = $"{InitData.ServerTag}-{XSFUtil.EP2Name(XSFServer.Instance.SID.Type)}-{logIndex}";

                    for(int i = 0; i < files.Length; i ++)
                    {
                        if(files[i].Contains(logName))
                        {
                            checkAgain = true;
                            logIndex ++;
                            break;
                        }
                    }
                    
                }
                while(checkAgain);
            } 
            else
            {
                logName = $"{InitData.ServerTag}-{XSFUtil.EP2Name(XSFServer.Instance.SID.Type)}-{logIndex}";
                Directory.CreateDirectory(dir);
            } 
            
            string finalName = $"{dir}/{logName}-.log";
            
            Log.Logger = config.WriteTo.File(finalName, rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static void End()
        {
            Log.CloseAndFlush();
        }
    }
}