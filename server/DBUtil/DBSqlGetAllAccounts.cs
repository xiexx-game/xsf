//////////////////////////////////////////////////////////////////////////
//
// 文件：server/DBUtil/DBSqlGetAllAccounts.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：sql 对象
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8602, CS8600, CS8618, CS8601

using XSF;
using XsfScp;
using MySqlConnector;

namespace DBUtil
{
    public class AccountData
    {
        public string email;
        public string pwd;
        public uint actorID;
    }

    public class DBSqlGetAllAccounts : DBSqlBase
    {

        public override DBSqlID ID { get { return DBSqlID.GetAllAccounts; } }

        public override bool ExecuteNonQuery { get { return false; } }

        private uint m_nStart;
        private uint m_nCount;

        public void SetData(uint start, uint count)
        {
            m_nStart = start;
            m_nCount = count;
        }

        public override void Export()
        {
            Writer.Clear();
            Writer.WriteUInt(m_nStart);
            Writer.WriteUInt(m_nCount);
        }

        public override void PackRead(XSFWriter writer, MySqlDataReader reader) 
        {
            writer.WriteU16String(reader.GetString(Scp.outParams[0]));
            writer.WriteU16String(reader.GetString(Scp.outParams[1]));
            writer.WriteUInt(reader.GetUInt32(Scp.outParams[2]));
        }

        public override void AddWithValue(MySqlCommand command, byte [] datas) 
        {
            XSFReader reader = new XSFReader(datas);

            reader.ReadUInt(out m_nStart);
            command.Parameters.AddWithValue(Scp.inParams[0], m_nStart);

            reader.ReadUInt(out m_nCount);
            command.Parameters.AddWithValue(Scp.inParams[1], m_nCount);
        }

        public AccountData Read(XSFReader reader)
        {
            AccountData ad = new AccountData();
            reader.ReadU16String(out ad.email);
            reader.ReadU16String(out ad.pwd);
            reader.ReadUInt(out ad.actorID);

            return ad;
        }
    }
}