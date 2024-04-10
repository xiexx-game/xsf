//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Hub/Node/Node.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：节点管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603, CS8600, CS8602, CS8604, CS8618
using XSF;

public class Node : NetPoint
{
    public override void OnRecv(IConnection connection, IMessage message, ushort nMessageID, uint nRawID, byte[]? data)
    {
        if(message != null)
            message.Execute(this, nMessageID, nRawID, data);
        else
        {
            Serilog.Log.Information($"收到消息 message:{nMessageID} 来源:{ID} 目标{nRawID}");

            var serverIDData = BitConverter.GetBytes(ID);
            Array.Copy(serverIDData, 0, data, 6, serverIDData.Length);

            if(nRawID > 0)
            {
                var np = NodeManager.Instance.Get(nRawID);
                if(np != null)
                {
                    np.SendData(nMessageID, data);
                }
            }
            else
            {
                NodeManager.Instance.BroadcastData(nMessageID, data, ID);
            }
                
        }
    }
}