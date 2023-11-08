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
        var message = XSFCore.GetMessage((ushort)XsfPbid.CMSGID.GCltLoginResult) as XsfMsg.MSG_G_Clt_LoginResult;
        message.mPB.Result = (uint)XsfPb.LoginResult.Success;

        GateA.IGateAcceptor.Instance.SendMessage2Client(ClientID, message);

        var testMsg = XSFCore.GetMessage((ushort)XsfPbid.CMSGID.GCltTestData) as XsfMsg.MSG_G_Clt_TestData;
        testMsg.mPB.Message = "test1";
        GateA.IGateAcceptor.Instance.Broadcast2AllClient(testMsg);


        testMsg.mPB.Message = "test2";
        GateA.IGateAcceptor.Instance.BeginBroadcast();
        GateA.IGateAcceptor.Instance.PushClientID(ClientID);
        GateA.IGateAcceptor.Instance.EndBroadcast(testMsg);
    }
}