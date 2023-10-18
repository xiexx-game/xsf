//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Server/ServerDef.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：Server相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
namespace XSF
{
    public interface IUpdateModule
    {
        void OnUpdate(uint nTimeDelta);
    }

    public struct ServerInit
    {
        public string ServerTag;
        public bool OutputConsole;

        public string WorkDir;
    }

    public interface IServer
    {
        public ServerID SID { get; }

        public ServerInit InitData { get; }

        public uint ID { get; }

        void Init(EP ep, string[] args);

        void Run();

        void AddModule(IModule module, ModuleInit init);
    }

    public struct ServerID
    {
        public ushort ID;
        public byte Type;
        public byte Index;

        public static ServerID GetSID(uint id)
        {
            byte[] data = BitConverter.GetBytes(id);

            ServerID sid;
            int pos = 0;
            sid.ID = BitConverter.ToUInt16(data, pos);
            pos += sizeof(ushort);
            sid.Type = data[pos++];
            sid.Index = data[pos++];

            return sid;
        }

        public static uint GetID(ServerID sid)
        {
            byte[] all = new byte[4];
            byte[] data = BitConverter.GetBytes(sid.ID);
            all[0] = data[0];
            all[1] = data[1];
            all[2] = sid.Type;
            all[3] = sid.Index;

            return BitConverter.ToUInt32(all, 0);
        }
    }

    public struct ClientID
    {
        public ushort ID;
        public byte Gate;
        public byte Key;

        public static ClientID GetCID(uint id)
        {
            byte[] data = BitConverter.GetBytes(id);

            ClientID cid;
            int pos = 0;
            cid.ID = BitConverter.ToUInt16(data, pos);
            pos += sizeof(ushort);
            cid.Gate = data[pos++];
            cid.Key = data[pos++];

            return cid;
        }

        public static uint GetID(ClientID cid)
        {
            byte[] all = new byte[4];
            byte[] data = BitConverter.GetBytes(cid.ID);
            all[0] = data[0];
            all[1] = data[1];
            all[2] = cid.Gate;
            all[3] = cid.Key;

            return BitConverter.ToUInt32(all, 0);
        }
    }

    internal class ServerArgException : Exception
    {
        public ServerArgException(string msg)
            : base(msg)
        {
            
        }
    }

    public class ServerNode 
    {
        public string Name = "";
        public EP ep;
        public uint MongoID;
        public uint RedisID;
    }

    public class XSFConfig
    {
        public string Name = "";
        public string Desc = "";
        public bool AutoStart;
        public uint HeartbeatCheck;
        public uint HeartbeatTimeout;
        public uint HeartbeatInterval;
        public uint ReconnectInterval;
        public uint ClientHeartbeatCheck;
        public uint ClientHeartbeatTimeout;
        public string MainCenterIP = "";
        public uint CenterPort;
        public uint InnerPortStart;
        public uint OutPortStart;
        public uint GateMaxCount;
        public string NatsAddress = "";
        public uint AccountLifetime;
        public uint ActorSaveTime;
        public uint ClientMsgMaxLength;
        public uint MaxGate;
        public uint ActorReleaseTime;

        public ServerNode Me;

        public ServerNode[] NodeList;
    }
}
