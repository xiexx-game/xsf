//////////////////////////////////////////////////////////////////////////
//
// 文件：server/CenterConnector/CenterConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：中心服连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using XSF;

namespace CC
{
    public class CenterConnector : ICenterConnector
    {

        public override void SendHandshake()
        {
            Serilog.Log.Information("CenterConnector SendHandshake");
        }

        public override void SendHeartbeat()
        {
            Serilog.Log.Information("CenterConnector SendHeartbeat");
        }

        
    }
}