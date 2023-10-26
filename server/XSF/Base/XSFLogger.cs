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
            config = config.MinimumLevel.Debug().Enrich.WithProperty("DefaultPrefix", $"[{XSFUtil.EP2CNName(XSFServer.Instance.SID.Type)}]");
            
            var InitData = XSFServer.Instance.InitData;

            string template =  "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u5} {DefaultPrefix} {Message:lj}{NewLine}{Exception}";

            if(InitData.OutputConsole)
                config = config.WriteTo.Console(outputTemplate: template);

            string logName = "";
            int logIndex = 1;

            string dir = $"{InitData.WorkDir}/logs";
            if(Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir);

                bool checkAgain = false;
                do
                {
                    checkAgain = false;
                    logName = $"{InitData.ServerTag}-{XSFUtil.EP2Name(XSFServer.Instance.SID.Type).ToLower()}-{logIndex}";

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
                logName = $"{InitData.ServerTag}-{XSFUtil.EP2Name(XSFServer.Instance.SID.Type).ToLower()}-{logIndex}";
                Directory.CreateDirectory(dir);
            } 
            
            string finalName = $"{dir}/{logName}-.log";
            
            Log.Logger = config.WriteTo.File(finalName, rollingInterval: RollingInterval.Day, outputTemplate: template)
                .CreateLogger();
        }

        public static void End()
        {
            Log.CloseAndFlush();
        }
    }
}