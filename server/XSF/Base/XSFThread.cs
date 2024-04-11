//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/XSF/Base/XSFThread.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：线程
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8618
namespace XSF
{
    public abstract class XSFThread
    {
        Thread m_thread; 
        public void StartThread()
        {
            m_thread = new Thread(ThreadMethod);
            m_thread.Start();
        }

        public void ThreadEnd()
        {
            try
            {
                m_thread.Join();
            }
            catch
            {

            }
        }

        void ThreadMethod()
        {
            Serilog.Log.Information("XSFThread ThreadMethod start");
            OnThreadCall();
            Serilog.Log.Information("XSFThread ThreadMethod end");
        }

        public abstract void OnThreadCall();
    }
}