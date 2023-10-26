//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Game/Actor/Actor.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：角色管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8600, CS8618
using XSF;

public class Actor
{
    public uint ID { get;  private set; }
    public uint ClientID { get; private set; }

    public Actor(uint id, uint nClientID)
    {
        ID = id;
        ClientID = nClientID;
    }

    public void OnLoginOk()
    {
        var message = XSFUtil.GetMessage((ushort)XsfPb.CMSGID.GCltLoginResult) as XsfMsg.MSG_G_Clt_LoginResult;
        message.mPB.Result = (uint)XsfPb.LoginResult.Success;

        GateA.IGateAcceptor.Instance.SendMessage2Client(ClientID, message);
    }
}