//////////////////////////////////////////////////////////////////////////
//
// 文件：server/GateAcceptor/Gate.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：网关接收器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8600, CS8618

using XSF;
using Google.Protobuf.Collections;

namespace GateA
{
    public class Gate : NetPoint
    {
        private List<uint> m_ClientIDs;

        public Gate()
        {
            m_ClientIDs = new List<uint>();
        }

        public void Add(uint nClientID)
        {
            m_ClientIDs.Add(nClientID);
        }

        public void Clear()
        {
            m_ClientIDs.Clear();
        }

        public void Boradcast(Google.Protobuf.ByteString data)
        {
            if(m_ClientIDs.Count > 0)
            {
                var messageWrap = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.GtAGtClientMessage) as XsfMsg.MSG_GtA_Gt_ClientMessage;
                messageWrap.mPB.ClientId.Clear();

                for(int i = 0; i < m_ClientIDs.Count; i ++)
                {
                    messageWrap.mPB.ClientId.Add(m_ClientIDs[i]);
                }
                
                messageWrap.mPB.ClientMessage = data;

                SendMessage(messageWrap);
            }
        }
    }
}