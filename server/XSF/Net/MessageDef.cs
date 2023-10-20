//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Net/MessageDef.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：Server相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8603, CS8618

namespace XSF
{
    public abstract class IMessage
    {
        protected IMessageExecutor m_Executor;

        public virtual ushort ID { get { return 0; } }

        public virtual byte EP { get { return 0; } }

        public abstract byte[] Export();

        public abstract IMessage Import(byte[] data, int offset, int length);

        public void SetExecutor(IMessageExecutor executor)
        {
            if(m_Executor != null)
            {
                Serilog.Log.Warning("IMessage.SetExecutor m_Executor is not null, message id={0}", ID);
            }

            m_Executor = executor;
        }

        public void Execute(object NetObj, ushort nMessageID, uint nRawID, byte[] rawData)
        {
            if(m_Executor == null)
            {
                Serilog.Log.Warning("IMessage.Execute m_Executor is null, message id={0}", ID);
                return;
            }

            m_Executor.OnExecute(NetObj, this, nMessageID, nRawID, rawData);
        }
    }

    public interface IMessageExecutor
    {
        void OnExecute(object NetObj, IMessage message, ushort nMessageID, uint nRawID, byte[] rawData);
    }

    public interface IMessageHelper
    {
        IMessage GetMessage(ushort nID);
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
        Max,
    }
}