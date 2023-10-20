//////////////////////////////////////////////////////////////////////////
// 
// 文件：Center/Node/ServerNode.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：节点对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603
using XSF;

public enum NodeStatus
{
    None = 0,
    New,
    Ok,
}

public class ServerNode
{
    public uint ID;

    public string IP = "";
    public NodeStatus Status;
    public uint[] Ports;

    public ServerNode()
    {
        Ports = new uint[(int)EP.Max];
    }
}