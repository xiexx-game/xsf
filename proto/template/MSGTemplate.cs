//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\Messages\MSG__MSG_NAME_.cs
// 作者：Xoen Xie
// 时间：_MSG_DATE_
// 描述：_MSG_DESC_
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using XsfPbid;
using XsfPb;
using XsfNet;

namespace XsfMsg
{
    public sealed class MSG__MSG_NAME_ : IMessage
    {
        public override CMSGID ID { get { return CMSGID._MSG_ID_NAME_; } }

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

        public override void Import(byte[] data, int offset, int length)
        {
            m_PB = _MSG_NAME_.Parser.ParseFrom(data, offset, length);
        }

        // 消息处理接口
        public override void Execute(NetClient client)
        {



        }
    }
}