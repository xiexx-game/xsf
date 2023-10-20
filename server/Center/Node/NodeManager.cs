//////////////////////////////////////////////////////////////////////////
// 
// 文件：Center/Node/NodeManager.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：节点管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603, CS8600
using XSF;

public class NodeManager : DicNPManager
{
    private List<ServerNode>[] m_LostList;
    private Dictionary<uint, ServerNode> m_Nodes;
    private byte[] m_NodeIndex;
    public NodeManager()
    {   
        m_Nodes = new Dictionary<uint, ServerNode>();
        m_LostList = new List<ServerNode>[(int)EP.Max];
        m_NodeIndex = new byte[(int)EP.Max];

        for(int i = 0; i < (int)EP.Max; i ++)
        {
            m_LostList[i] = new List<ServerNode>();
            m_NodeIndex[i] = 1;
        }
    }

    public override void DoRegist()
    {
        XSFUtil.SetMessageExecutor((ushort)XsfPb.SMSGID.CcCHandshake, new Executor_Cc_C_Handshake());
        XSFUtil.SetMessageExecutor((ushort)XsfPb.SMSGID.CcCHeartbeat, new Executor_Cc_C_Heartbeat());
        XSFUtil.SetMessageExecutor((ushort)XsfPb.SMSGID.CcCServerInfo, new Executor_Cc_C_ServerInfo());
        XSFUtil.SetMessageExecutor((ushort)XsfPb.SMSGID.CcCServerLost, new Executor_Cc_C_ServerLost());
        XSFUtil.SetMessageExecutor((ushort)XsfPb.SMSGID.CcCServerOk, new Executor_Cc_C_ServerOk());
    }

    public ServerNode AddNode(uint nID, string ip, uint[] ports)
    {
        ServerID sid = ServerID.GetSID(nID);
        ServerNode newNode = null;
        if(sid.Index == 0 )
        {
            var nodeLost = GetLostNode(sid.Type, nID, false);
            if(nodeLost == null)
            {
                newNode = new ServerNode();
                GetPort(ref newNode.Ports);

                sid.Index = GetIndex(sid.Type);
                newNode.ID = ServerID.GetID(sid);
                newNode.IP = ip;
                newNode.Status = NodeStatus.New;

                Serilog.Log.Information($"NodeManager AddNode new, id={newNode.ID}, ip={ip}");
            }
            else
            {
                newNode = nodeLost;
                newNode.Status = NodeStatus.New;

                Serilog.Log.Information($"NodeManager AddNode new, find lost server node id={newNode.ID}, ip1={newNode.IP}, ip2={ip}");
            }

            m_Nodes[nID] = newNode;
        }
        else
        {
            if(m_Nodes.TryGetValue(nID, out newNode))
            {
                Serilog.Log.Information($"NodeManager AddNode find exist 1, id={nID}, ip={ip}");
            }
            else
            {
                var nodeLost = GetLostNode(sid.Type, nID, true);
                if(nodeLost == null)
                {
                    Serilog.Log.Error($"NodeManager AddNode find exist, id={nID}, ip={ip}");
                    return null;
                }
                else
                {
                    if(!nodeLost.IP.Equals(ip))
                    {
                        Serilog.Log.Error($"NodeManager AddNode find exist, ip error, id={nID}, ip1={nodeLost.IP}, ip2={ip}");
                        return null;
                    }

                    for(int i = 0; i < (int)EP.Max; i ++)
                    {
                        if(nodeLost.Ports[i] != ports[i])
                        {
                            Serilog.Log.Error($"NodeManager AddNode find exist, port error, id={nID}, i={i} port1={nodeLost.Ports[i]}, port2={ports[i]}");
                            return null;
                        }
                    }
                }

                newNode = nodeLost;
                m_Nodes.Add(nID, newNode);
                Serilog.Log.Information($"NodeManager AddNode find exist 2, id={nID}, ip={ip}");
            }
        }

        return newNode;
    }

    public byte GetIndex(byte nEP)
    {
        var index = m_NodeIndex[nEP];
        m_NodeIndex[nEP] ++;

        return index;
    }

    public void GetPort(ref uint[] ports)
    {

    }

    public ServerNode GetLostNode(byte nEP, uint nID, bool CheckEqual)
    {
        var list = m_LostList[nEP];
        for(int i = 0; i < list.Count; i ++)
        {
            var node = list[i];
            if(CheckEqual)
            {
                if(node.ID == nID)
                {
                    list.RemoveAt(i);
                    return node;
                }
            }
            else
            {
                list.RemoveAt(i);
                return node;
            }
        }

        return null;
    }




    public static void CreateModule()
    {
        NodeManager module = new NodeManager();

        NPManagerInit init = new NPManagerInit();
        init.ID = (int)ModuleID.Node;
        init.Name = "NodeManager";
        init.NoWaitStart = true;
        init.Port = (int)XSFUtil.Config.CenterPort;

        XSFUtil.Server.AddModule(module, init);
    }

    public static NodeManager Instance
    {
        get
        {
            return XSFUtil.Server.GetModule((int)ModuleID.Node) as NodeManager;
        }
    }
}