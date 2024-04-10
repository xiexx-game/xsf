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
            return (int)XSFCore.Server.Ports[(int)EP.DB];
        }
    }

    public override void DoRegist()
    {
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.DbcDbHandshake, new Executor_Dbc_Db_Handshake());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.DbcDbHeartbeat, new Executor_Dbc_Db_Heartbeat());
        XSFCore.SetMessageExecutor((ushort)XsfPbid.SMSGID.DbcDbRequest, new Executor_Dbc_Db_Request());
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