//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Net\MessageDef.cs
// 作者：Xoen Xie
// 时间：2023/03/23
// 描述：网络相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XsfPb;

namespace XsfNet
{
    public enum EP
    {
        None = 0,
        Client,
        Center,
        Login,
        Gate,
        Game,
        Db,
    }

    public abstract class IMessage
    {
        public virtual CMSGID ID { get { return CMSGID.None; } }

        public abstract byte[] Export();

        public virtual void Import(byte[] data, int offset, int length) { }

        public virtual void Execute(NetClient client) { }
    }
}

