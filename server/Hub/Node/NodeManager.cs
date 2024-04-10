//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/DB/Node/NodeManager.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：节点管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603, CS8600, CS8602, CS8604, CS8618
using XSF;

public class NodeManager : DicNPManager
{
    public override int Port
    {
        get
        {
            return (int)XSFCore.Server.Ports[(int)EP.Hub];
        }
    }

    protected override NetPoint NewNP()
    {
        return new Node();
    }

    public override void DoRegist()
    {
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.HcHHandshake, new Executor_Hc_H_Handshake());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.HcHHeartbeat, new Executor_Hc_H_Heartbeat());
    }

    public override ModuleRunCode OnCloseCheck()
    {
        if(Total > 0)
            return ModuleRunCode.Wait;

        return ModuleRunCode.OK;
    }

    public static void CreateModule()
    {
        NodeManager module = new NodeManager();

        NPManagerInit init = new NPManagerInit();
        init.ID = (int)ModuleID.Node;
        init.Name = "NodeManager";
        init.packer = new LocalPacker();

        XSFCore.Server.AddModule(module, init);
    }

    public static NodeManager Instance
    {
        get
        {
            return XSFCore.Server.GetModule((int)ModuleID.Node) as NodeManager;
        }
    }
}