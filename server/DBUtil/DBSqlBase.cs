//////////////////////////////////////////////////////////////////////////
//
// 文件：server/DBUtil/DBSqlBase.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：sql 对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8600, CS8618, CS8603

using XSF;
using XsfScp;
using MySqlConnector;

namespace DBUtil
{
    public enum DBSqlID
    {
        NewAccount = 10000,
        GetAllAccounts = 10001,
        SetAccountActorID = 10002,
    }

    public abstract class DBSqlBase
    {

        public virtual DBSqlID ID { get { return 0; } }

        public ScpDBSql Scp { get; private set; }

        public virtual bool ExecuteNonQuery { get { return true; } }

        public virtual void PackRead(XSFWriter writer, MySqlDataReader reader) {}

        public virtual void AddWithValue(MySqlCommand command, byte [] datas) {}

        public virtual void Export() { }

        private static XSFWriter m_Writer;

        protected XSFWriter Writer {
            get {
                if(m_Writer == null)
                    m_Writer = new XSFWriter();

                return m_Writer;
            }
        }

        public byte[] Buffer { get { return Writer.Buffer; } }
        public uint Size { get { return (uint)Writer.Size; } }

        internal void Init()
        {
            var schema = SchemaModule.DBSql;
            Scp = schema.Get((uint)ID);
            if(Scp == null)
            {
                
            }
        }

        public static DBSqlBase GetSql(uint nID)
        {
            switch((DBSqlID)nID)
            {
            case DBSqlID.NewAccount:    return NewAccount;
            case DBSqlID.GetAllAccounts: return GetAllAccounts;
            case DBSqlID.SetAccountActorID: return SetAccountActorID;
            default:    return null;
            }
        }

        public static DBSqlNewAccount NewAccount 
        {
            get {
                var sql = new DBSqlNewAccount();
                sql.Init();

                return sql;
            }
        }

        public static DBSqlGetAllAccounts GetAllAccounts 
        {
            get {
                var sql = new DBSqlGetAllAccounts();
                sql.Init();

                return sql;
            }
        }

        public static DBSqlSetAccountActorID SetAccountActorID 
        {
            get {
                var sql = new DBSqlSetAccountActorID();
                sql.Init();

                return sql;
            }
        }
    }
}