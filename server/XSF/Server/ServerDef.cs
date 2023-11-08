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
    public struct ServerInit
    {
        public string ServerTag;
        public bool OutputConsole;

        public string WorkDir;
    }

    public interface IServer
    {
        ServerID SID { get; }

        ServerInit InitData { get; }

        uint ID { get; }

        uint[] Ports { get; }

        bool IsRunning { get; }

        void SetPort(byte ep, uint port);

        void Init(EP ep, string[] args);

        void SetID(uint nID);

        void Run();

        void AddModule(IModule module, ModuleInit init);

        IModule GetModule(int nID);

        void DoStart();

        void Stop();
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
            int pos = 2;
            sid.ID = BitConverter.ToUInt16(data, pos);
            sid.Type = data[1];
            sid.Index = data[0];

            return sid;
        }

        public static uint GetID(ServerID sid)
        {
            byte[] all = new byte[4];
            byte[] data = BitConverter.GetBytes(sid.ID);
            all[0] = sid.Index;
            all[1] = sid.Type;
            all[2] = data[0];
            all[3] = data[1];

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
            cid.Key = data[pos++];
            cid.Gate = data[pos++];

            return cid;
        }

        public static uint GetID(ClientID cid)
        {
            byte[] all = new byte[4];
            byte[] data = BitConverter.GetBytes(cid.ID);
            all[0] = data[0];
            all[1] = data[1];
            all[2] = cid.Key;
            all[3] = cid.Gate;

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

    public enum NodeStatus
    {
        None = 0,
        New,
        Ok,
    }

    public class ServerInfo
    {
        public uint ID;

        public string IP = "";
        public NodeStatus Status;
        public uint[] Ports;

        public ServerInfo()
        {
            Ports = new uint[(int)EP.Max];
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
