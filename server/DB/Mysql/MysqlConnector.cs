//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/DB/Mysql/MysqlConnector.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：db连接器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600,CS8618,CS8602
using MySqlConnector;

using XSF;
using DBUtil;

public class MysqlConnector : XSFThread
{
    struct RequestData
    {
        public uint nServerID;
        public ulong nSerialID;
        public uint nDBRequestID;
        public byte[] datas;
    }

    struct ResponseData 
    {
        public ulong nSerialID;
        public uint nServerID;
        public uint nCode;
        public uint nTotal;
        public byte[] datas;
    }


    enum WorkingStatus
    {
        None = 0,
        Openning,
        Running,
        Error,
        WaitEnd,
        End,
    }

    MySqlConnection m_Connection;

    MysqlConfig m_Config;
    ManualResetEvent m_MainEvent;

    int m_Status;

    XSFQueue<RequestData> m_Requests;
    XSFQueue<ResponseData> m_Responses;

    XSFWriter m_Writer;

    public void Create(MysqlConfig config)
    {
        m_Writer = new XSFWriter();

        m_Requests = new XSFQueue<RequestData>();
        m_Responses = new XSFQueue<ResponseData>();

        m_Config = config;
        m_MainEvent = new ManualResetEvent(false);
        m_Status = (int)WorkingStatus.None;
        StartThread();
    }

    public void DoClose()
    {
        Serilog.Log.Information("MysqlConnector DoClose");

        Volatile.Write(ref m_Status, (int)WorkingStatus.WaitEnd);

        m_MainEvent.Set();
    }

    public void Release()
    {
        ThreadEnd();
        Serilog.Log.Information("MysqlConnector release done");
    }

    public override void OnThreadCall()
    {
        try
        {
            var connString = $"Server={m_Config.ip};User ID={m_Config.user};Password={m_Config.pwd};Database={m_Config.db}";
            m_Connection = new MySqlConnection(connString);
            m_Connection.Open();
        }
        catch (MySqlException ex)
        {
            Volatile.Write(ref m_Status, (int)WorkingStatus.Error);
            Serilog.Log.Information("MysqlConnector MySQL connection: " + ex.Message);
            return;
        }

        Volatile.Write(ref m_Status, (int)WorkingStatus.Running);

        while(true)
        {
            bool res = m_MainEvent.WaitOne();
            if(res)
            {
                m_MainEvent.Reset();
            }

            var status = Volatile.Read(ref m_Status);
            switch((WorkingStatus)status)
            {
            case WorkingStatus.Running:
                DoRequest();
                break;

            case WorkingStatus.WaitEnd:
                DoRequest();
                Serilog.Log.Information("MysqlConnector OnThreadCall WorkingStatus.WaitEnd");
                Volatile.Write(ref m_Status, (int)WorkingStatus.End);
                m_MainEvent.Set();
                break;

            case WorkingStatus.End:
                Serilog.Log.Information("MysqlConnector OnThreadCall WorkingStatus.End");
                goto END;
            }
        }
END:

        m_Connection.Close();
        m_Connection.Dispose();
    }

    private void DoRequest()
    {
        while(m_Requests.Pop(out RequestData rd))
        {
            ResponseData resd = new ResponseData();
            resd.nCode =  (uint)XsfPb.OpResult.Ok;
            resd.nSerialID = rd.nSerialID;
            resd.nServerID = rd.nServerID;

            DBSqlBase sql = DBSqlBase.GetSql(rd.nDBRequestID);
            if(sql == null)
            {
                resd.nCode = (uint)XsfPb.OpResult.MysqlSqlBaseNotExist;
            }
            else
            {
                try
                {
                    using(var command = new MySqlCommand(sql.Scp.sContent, m_Connection))
                    {
                        sql.AddWithValue(command, rd.datas);

                        if(sql.ExecuteNonQuery)
                        {
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            m_Writer.Clear();

                            using var reader = command.ExecuteReader();
                            while(reader.Read())
                            {
                                resd.nTotal ++;
                                sql.PackRead(m_Writer, reader);
                            }

                            resd.datas = m_Writer.ByteArray;
                        }
                    } 
                }
                catch(Exception ex)
                {
                    Serilog.Log.Error("MysqlConnector DoRequest catch error, message=" + ex.Message + ",trace=" + ex.StackTrace);
                    resd.nCode = (uint)XsfPb.OpResult.MysqlExecuteError;
                    resd.nTotal = 0;
                }
            }

            m_Responses.Push(resd);
            XSFCore.Server.SpeedUp();
        }
    }

    public ModuleRunCode OnCloseCheck()
    {
        if(m_Requests.Count > 0)
            return ModuleRunCode.Wait;
            
        return ModuleRunCode.OK;
    }

    public ModuleRunCode OnStartCheck()
    {
        var status = Volatile.Read(ref m_Status);
        if(status == (int)WorkingStatus.Running) 
            return ModuleRunCode.OK;
        else if(status == (int)WorkingStatus.Error)
            return ModuleRunCode.Error;
        else
            return ModuleRunCode.Wait;
    }

    public void Request(uint nServerID, ulong nSerialID, uint nDBRequestID, byte[] datas)
    {   
        RequestData rd = new RequestData();
        rd.nServerID = nServerID;
        rd.nSerialID = nSerialID;
        rd.nDBRequestID = nDBRequestID;
        rd.datas = datas;

        m_Requests.Push(rd);

        m_MainEvent.Set();
    }

    public void Dispatch()
    {
        while(m_Responses.Pop(out ResponseData rd))
        {
            var node = NodeManager.Instance.Get(rd.nServerID);
            if(node != null)
            {
                var respMsg = XSFCore.GetMessage((ushort)XsfPbid.SMSGID.DbDbcResponse) as XsfMsg.MSG_Db_Dbc_Response;
                respMsg.mPB.SerialId = rd.nSerialID;
                respMsg.mPB.Code = rd.nCode;
                respMsg.mPB.Total = rd.nTotal;
                if(rd.nCode == (uint)XsfPb.OpResult.Ok && rd.datas != null)
                {
                    respMsg.mPB.Datas = Google.Protobuf.ByteString.CopyFrom(rd.datas);
                }
                else
                {
                    respMsg.mPB.Datas = Google.Protobuf.ByteString.Empty;
                }
                
                node.SendMessage(respMsg);
            }
        }
    }
}
