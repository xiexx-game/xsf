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
        public override ushort ID { get { return (ushort)SMSGID._MSG_ID_NAME_; } }

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

        public override void Export(XSFWriter writer)
        {
            // 写入PB数据
            byte[] bytes = new byte[mPB.CalculateSize()];
            mPB.WriteTo(new Google.Protobuf.CodedOutputStream(bytes));
            writer.WriteBuffer(bytes);
        }

        public override void Import(byte[] data, int offset, int length)
        {
            m_PB = _MSG_NAME_.Parser.ParseFrom(data, offset, length);
        }
    }
}
