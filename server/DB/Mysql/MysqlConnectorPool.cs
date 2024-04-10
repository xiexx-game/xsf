//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/DB/Mysql/MysqlConnectorPool.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：db连接器池
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600,CS8618
using XSF;
using MySqlConnector;

public class MysqlConnectorPool
{
    MysqlConnector [] m_Connectors;

    public bool Create(MysqlConfig config)
    {
        if(XSFCore.Server.SID.Index == 1)
        {
            if(!CreateAndUpdate(config))
            {
                return false;
            }
        }

        m_Connectors = new MysqlConnector[config.count];

        for(uint i = 0; i < config.count; i++ )
        {
            var conn = new MysqlConnector();
            conn.Create(config);
            m_Connectors[i] = conn;
        }

        return true;
    }

    private bool CreateAndUpdate(MysqlConfig config)
    {
        if(!CreateDataBase(config))
        {
            return false;
        }

        var connString = $"Server={config.ip};User ID={config.user};Password={config.pwd};Database={config.db}";
        using (MySqlConnection connection = new MySqlConnection(connString))
        {
            connection.Open();

            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = $@"
                    CREATE TABLE IF NOT EXISTS ver (
                        code INT UNSIGNED DEFAULT 0 COMMENT '版本号'
                    )";
                command.ExecuteNonQuery();
            }

            uint code = 0;
            bool newInsert = false;
            using (MySqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT code FROM ver LIMIT 1";
                using var reader = command.ExecuteReader();
                if(reader.Read())
                {
                    code = reader.GetUInt32("code");
                }
                else
                {
                    newInsert = true;
                }
            }

            if(newInsert)
            {
                using (MySqlCommand command2 = connection.CreateCommand())
                {
                    command2.CommandText = "INSERT INTO ver SET code=0";
                    command2.ExecuteNonQuery();
                }
            }

            while(true)
            {
                code ++;
                var initData = XSFCore.Server.InitData;
                string filepath = $"{initData.WorkDir}/mysql/{config.db}.{code}.sql";
                if(File.Exists(filepath))
                {
                    Serilog.Log.Information("MysqlConnectorPool CreateAndUpdate read sql file=" + filepath);
                    var lines = File.ReadAllLines(filepath);
                    int flag = 0;
                    string sql = "";
                    for(int i = 0; i < lines.Length; i ++)
                    {
                        if(flag == 0)
                        {
                            if(lines[i].Contains("--BEGIN"))
                            {
                                flag = 1;
                            }
                        }
                        else if(flag == 1)
                        {
                            if(lines[i].Contains("--END"))
                            {
                                flag = 0;

                                using (MySqlCommand command = connection.CreateCommand())
                                {
                                    command.CommandText = sql;
                                    command.ExecuteNonQuery();
                                }
                            }
                            else
                            {
                                sql += $"{lines[i]}\n";
                            }
                        }
                    }
                } else {
                    using (MySqlCommand command = connection.CreateCommand())
                    {
                        Serilog.Log.Information($"MysqlConnectorPool CreateAndUpdate current code is code={code-1}");
                        command.CommandText = $"UPDATE ver SET code={code-1}";
                        command.ExecuteNonQuery();
                    }
                    break;
                }
            }

            connection.Close();
        }

        Serilog.Log.Information("MysqlConnectorPool CreateAndUpdate done ...");

        return true;
    }

    private bool CreateDataBase(MysqlConfig config)
    {
        var connString = $"Server={config.ip};User ID={config.user};Password={config.pwd}";
        using (MySqlConnection connection = new MySqlConnection(connString))
        {
            try
            {
                connection.Open();

                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE DATABASE IF NOT EXISTS {config.db}";
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
            catch (MySqlException ex)
            {
                Serilog.Log.Error("MysqlConnectorPool CreateAndUpdate error, message: " + ex.Message);
                return false;
            }
        }

        return true;
    }

    public void DoClose()
    {
        Serilog.Log.Information("MysqlConnectorPool DoClose");
        for(int i = 0; i < m_Connectors.Length; i ++)
        {
            m_Connectors[i].DoClose();
        }
    }

    public void Release()
    {
        for(int i = 0; i < m_Connectors.Length; i ++)
        {
            m_Connectors[i].Release();
        }
    }

    public ModuleRunCode OnCloseCheck()
    {
        ModuleRunCode result = ModuleRunCode.OK;
        for(int i = 0; i < m_Connectors.Length; i ++)
        {
            var res = m_Connectors[i].OnCloseCheck();
            if(res != ModuleRunCode.OK)
            {
                result = ModuleRunCode.Wait;
            }
        }

        return result;
    }

    public ModuleRunCode OnStartCheck()
    {
        ModuleRunCode result = ModuleRunCode.OK;
        for(int i = 0; i < m_Connectors.Length; i ++)
        {
            var res = m_Connectors[i].OnCloseCheck();
            if(res == ModuleRunCode.Error)
            {
                return ModuleRunCode.Error;
            }
            else if(res == ModuleRunCode.Wait)
            {
                result = res;
            }
        }

        return result;
    }

    public void Request(uint nServerID, ulong nSerialID, uint nDBRequestID, uint nQueueID, byte[] datas)
    {
        var nIndex = nQueueID%m_Connectors.Length;
        m_Connectors[nIndex].Request(nServerID, nSerialID, nDBRequestID, datas);
    }

    public void Dispatch()
    {
        for(int i = 0; i < m_Connectors.Length; i ++)
        {
            m_Connectors[i].Dispatch();
        }
    }
}
