//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/Message/Messages/MSG__MSG_NAME_.cs
// 作者：Xoen Xie
// 时间：_MSG_DATE_
// 描述：_MSG_DESC_
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618

using XsfPb;
using XSF;

namespace XsfMsg
{
    public sealed class MSG__MSG_NAME_ : IMessage
    {
        public override ushort ID { get { return (ushort)_ID_PREFIX_._MSG_ID_NAME_; } }

        public override byte DestEP { get { return (byte)EP._EP_NAME_; } }

        private _MSG_NAME_ m_PB;
        public _MSG_NAME_ mPB
        {
            get
            {
                if (m_PB == null)
                    m_PB = new _MSG_NAME_();

                return m_PB;
            }
        }

        public override byte[] Export()
        {
            byte[] bytes = new byte[mPB.CalculateSize()];
            mPB.WriteTo(new Google.Protobuf.CodedOutputStream(bytes));

            return bytes;
        }

        public override IMessage Import(byte[] data, int offset, int length)
        {
            var message = new MSG__MSG_NAME_();
            message.m_PB = _MSG_NAME_.Parser.ParseFrom(data, offset, length);
            message.m_Executor = m_Executor;
            return message;
        }
    }
}
