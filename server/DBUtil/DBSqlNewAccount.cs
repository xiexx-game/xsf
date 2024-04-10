//////////////////////////////////////////////////////////////////////////
//
// 文件：server/DBUtil/DBSqlNewAccount.cs
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
    public class DBSqlNewAccount : DBSqlBase
    {

        public override DBSqlID ID { get { return DBSqlID.NewAccount; } }

        private string m_Email;
        private string m_Pwd;
        private uint m_nActorID;

        public void SetData(string email, string pwd, uint actorID)
        {
            m_Email = email;
            m_Pwd = pwd;
            m_nActorID = actorID;
        }

        public override void Export()
        {
            Writer.Clear();
            Writer.WriteU16String(m_Email);
            Writer.WriteU16String(m_Pwd);
            Writer.WriteUInt(m_nActorID);
        }

        public override void AddWithValue(MySqlCommand command, byte [] datas) 
        {
            XSFReader reader = new XSFReader(datas);
            reader.ReadU16String(out m_Email);
            command.Parameters.AddWithValue(Scp.inParams[0], m_Email);

            reader.ReadU16String(out m_Pwd);
            command.Parameters.AddWithValue(Scp.inParams[1], m_Pwd);

            reader.ReadUInt(out m_nActorID);
            command.Parameters.AddWithValue(Scp.inParams[2], m_nActorID);
        }
    }
}