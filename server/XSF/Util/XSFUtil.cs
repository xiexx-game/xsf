//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Util/XSFUtil.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：通用函数
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8618, CS8600, CS8604
using System.Diagnostics;

namespace XSF
{
    public static class XSFUtil
    {
        public static ISchemaHelper schemaHelper;
        public static IMessageHelper messageHelper;

        private static INetPacker m_ServerPacker;

        public static IServer Server 
        {
            get {
                return XSFServer.Instance;
            }
        }

        public static XSFConfig Config
        {
            get 
            {
                return XSFServer.Instance.Config;
            }
        }

        public static INetPacker ServerPakcer
        {
            get
            {
                if(m_ServerPacker == null)
                    m_ServerPacker = new ServerPacker();

                return m_ServerPacker;
            }
        }

        public static ISchema GetSchema(int nID)
        {
            return schemaHelper.GetSchema(nID);
        }

        public static IMessage GetMessage(ushort nID)
        {
            return messageHelper.GetMessage(nID);
        }

        public static void SetMessageExecutor(ushort nID, IMessageExecutor executor)
        {
            var message = GetMessage(nID);
            message.SetExecutor(executor);
        }

        public static bool GetArg(string[] args, string tag, out string data)
        {
            data = "";
            for(int i = 0; i < args.Length; i ++)
            {
                if(args[i].Equals(tag))
                {
                    if(i+1>= args.Length)
                    {
                        return true;
                    }
                    else
                    {
                        data = args[i+1];
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 64位混合ID
        /// </summary>
        public static ulong UINT64_ID(uint k1, uint k2)
        {
            return (((ulong)k1) << 32 | ((ulong)k2));
        }

        public static uint FIRST_UINT64_ID(ulong k)
        {
            return (uint)(k >> 32);
        }

        public static uint SECOND_UINT64_ID(ulong k)
        {
            return ((uint)(k & 0xFFFFFFFF));
        }


        /// <summary>
        /// 32位混合KEY
        /// </summary>
        public static uint UINT_ID(ushort k1, ushort k2)
        {
            return (uint)(k1 << 16 | k2);
        }

        public static ushort FIRST_UINT_ID(uint k)
        {
            return (ushort)(k >> 16);
        }

        public static ushort SECOND_UINT_ID(uint k)
        {
            return (ushort)(k & 0x0000FFFF);
        }


        /// <summary>
        /// 16位混合ID
        /// </summary>
        public static ushort USHORT_ID(byte k1, byte k2)
        {
            return (ushort)(k1 << 8 | k2);
        }

        public static byte FIRST_USHORT_ID(ushort k)
        {
            return (byte)(k >> 8);
        }

        public static byte SECOND_USHORT_ID(ushort k)
        {
            return (byte)(k & 0x000000FF);
        }

        /// <summary>
        /// 时间戳计算时间
        /// </summary>
        public static DateTime DateFrom = new(1970, 1, 1, 0, 0, 0);

        // 当前UTC时间，单位毫秒
        public static ulong CurrentMS => (ulong)(DateTime.Now - DateFrom).TotalMilliseconds;

        public static uint CurrentS => (uint)(CurrentMS/1000);

        public static EP Name2EP(string name)
        {
            if(name == "client") { return EP.Client; }
            if(name == "center") { return EP.Center; }
            if(name == "login") { return EP.Login; }
            if(name == "gate") { return EP.Gate; }
            if(name == "game") { return EP.Game; }
            if(name == "db") { return EP.Db; }
            else 
                return EP.None;
        }

        public static string EP2Name(byte nEP)
        {
            switch((EP)nEP)
            {
            case EP.Client:   return "Client";
            case EP.Center: return "Center";
            case EP.Login:  return "Login";
            case EP.Gate:   return "Gate";
            case EP.Game:   return "Game";
            case EP.Db: return "DB";
            default:    return "Unknow";
            }
        }

        public static string EP2CNName(byte nEP)
        {
            switch((EP)nEP)
            {
            case EP.Client:   return "客户端";
            case EP.Center: return "中心服";
            case EP.Login:  return "登录服";
            case EP.Gate:   return "网关服";
            case EP.Game:   return "游戏服";
            case EP.Db: return "数据服";
            default:    return "未知";
            }
        }

        public static bool IsEPInvalid(byte nEP)
        {
            switch((EP)nEP)
            {
            case EP.Client:   return true;
            case EP.Center: return true;
            case EP.Login:  return true;
            case EP.Gate:   return true;
            case EP.Game:   return true;
            case EP.Db: return true;
            default:    return false;
            }
        }

        public static bool StartProcess(string sFilename, string sArgs, string sWorkDir)
        {
            Process ps = null;
            bool fail = false;

            try
            {
                ProcessStartInfo StartInfo = new ProcessStartInfo();
                StartInfo.FileName = sFilename;
                StartInfo.Arguments = sArgs;

                StartInfo.CreateNoWindow = true;
                StartInfo.UseShellExecute = false;
                if (!string.IsNullOrEmpty(sWorkDir))
                    StartInfo.WorkingDirectory = sWorkDir;

                ps = new Process();
                ps.StartInfo = StartInfo;
                
                ps.Start();
                ps.WaitForExit();
            }
            catch (Exception e)
            {
                Serilog.Log.Error(e.Message);
            }
            finally
            {
                ps.Dispose();
            }

            return !fail;
        }
    }
}