//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Net/MessageDef.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：Server相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

namespace XSF
{
    public class IMessage
    {
        public virtual short ID { get { return 0; } }

        public virtual byte EP { get { return 0; } }

        public virtual void Export(XSFWriter writer) { }

        public virtual void Import(byte[] data, int offset, int length) { }
    }

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
}