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

        public void Boradcast(IMessage message)
        {
            if(m_ClientIDs.Count > 0)
            {
                SendMessage(message);
            }
        }
    }
}