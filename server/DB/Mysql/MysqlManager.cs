//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/DB/Mysql/MysqlManager.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：db管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600,CS8618,CS8603,CS8602
using XSF;
using XsfScp;
using XsfPb;

public class MysqlManager : IModule
{
    Dictionary<string, MysqlConnectorPool> m_Pools;

    public override bool Init(ModuleInit init)
    {
        base.Init(init);

        m_Pools = new Dictionary<string, MysqlConnectorPool>();

        return true;
    }

    public override bool Start()
    {
        string ext = XSFCore.Config.Me.Ext;
        string [] items = ext.Split(",");

        for(int i = 0; i < items.Length; i ++)
        {
            int nIndex = items[i].IndexOf("mysql");
            if(nIndex >= 0)
            {
                var idStr = items[i].Substring(nIndex + 5);
                try
                {
                    uint id = Convert.ToUInt32(idStr);
                    MysqlConfig current = null;

                    for(int J = 0; J < XSFCore.Config.mysql.Length; J ++)
                    {
                        var m = XSFCore.Config.mysql[J];
                        if(id == m.id)
                        {
                            current = m;
                            break;
                        }
                    }

                    if(current == null)
                    {
                        Serilog.Log.Error("Mysql config not exit, id=" + id);
                        return false;
                    }

                    MysqlConnectorPool pool = new MysqlConnectorPool();
                    if(!pool.Create(current))
                    {
                        return false;
                    }

                    m_Pools.Add(current.db, pool);
                }
                catch(Exception e)
                {
                    Serilog.Log.Error("Mysql pool create error, message=" + e.Message + ", stack="+ e.StackTrace);
                    return false;
                }
            }
        }

        return true;
    }

    public override void DoClose()
    {
        foreach(var kv in m_Pools)
        {
            kv.Value.DoClose();
        }
    }

    public override void Release()
    {
        foreach(var kv in m_Pools)
        {
            kv.Value.Release();
        }
    }

    public override void OnUpdate(uint nDeltaTime)
    {
        foreach(var kv in m_Pools)
        {
            kv.Value.Dispatch();
        }
    }

    public override ModuleRunCode OnCloseCheck()
    {
        ModuleRunCode res = ModuleRunCode.OK;
        foreach(var kv in m_Pools)
        {
            if(kv.Value.OnCloseCheck() != ModuleRunCode.OK)
            {
                res = ModuleRunCode.Wait;
            }
        }

        return res;
    }

    public override ModuleRunCode OnStartCheck()
    {
        ModuleRunCode res = ModuleRunCode.OK;
        foreach(var kv in m_Pools)
        {
            var checkRes = kv.Value.OnStartCheck();
            if(checkRes == ModuleRunCode.Error)
            {
                return ModuleRunCode.Error;
            }
            else if(checkRes == ModuleRunCode.Wait)
            {
                res = ModuleRunCode.Wait;
            }
        }

        return res;
    }

    public uint MysqlRequest(uint nServerID, ulong nSerialID, uint nDBRequestID, uint nQueueID, byte[] datas)
    {
        var SqlSchema = XSFCore.GetSchema((int)SchemaID.DBSql) as SchemaDBSql;
        var Scp = SqlSchema.Get(nDBRequestID);
        if(Scp == null)
        {
            Serilog.Log.Error("MysqlManager MysqlRequest, Scp == null id=" + ID);
            return (uint)OpResult.MysqlSchemaError; 
        }

        if(m_Pools.TryGetValue(Scp.sDBName, out MysqlConnectorPool pool))
        {
            pool.Request(nServerID, nSerialID, nDBRequestID, nQueueID, datas);
        }
        else
        {
            Serilog.Log.Error("MysqlManager MysqlRequest, pool not exist, pool=" + Scp.sDBName);
            return (uint)OpResult.MysqlPoolNotExist; 
        }

        return (uint)OpResult.Ok;
    }



    public static MysqlManager Instance
    {
        get
        {
            return XSFCore.Server.GetModule((int)ModuleID.Mysql) as MysqlManager;
        }
        
    }

    public static void CreateModule()
    {
        MysqlManager module = new MysqlManager();

        ModuleInit init = new ModuleInit();
        init.ID = (int)ModuleID.Mysql;
        init.Name = "MysqlManager";

        XSFCore.Server.AddModule(module, init);
    }
}

