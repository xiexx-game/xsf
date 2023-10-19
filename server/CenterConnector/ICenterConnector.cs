//////////////////////////////////////////////////////////////////////////
//
// 文件：server/CenterConnector/ICenterConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：中心服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using XSF;

namespace CC
{
    public abstract class ICenterConnector : NetConnector
    {
        public static ICenterConnector CreateModule()
        {
            return new CenterConnector();
        }
    }
}