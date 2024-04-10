//////////////////////////////////////////////////////////////////////////
//
// 文件：server/DBUtil/DBSqlSetAccountActorID.cs
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
    public class DBSqlSetAccountActorID : DBSqlBase
    {

        public override DBSqlID ID { get { return DBSqlID.SetAccountActorID; } }

        private uint m_nActorID;
        private string m_Email;

        public void SetData(uint actorID, string email)
        {
            m_nActorID = actorID;
            m_Email = email;
        }

        public override void Export()
        {
            Writer.Clear();
            Writer.WriteUInt(m_nActorID);
            Writer.WriteU16String(m_Email);
        }

        public override void AddWithValue(MySqlCommand command, byte [] datas) 
        {
            XSFReader reader = new XSFReader(datas);
            reader.ReadUInt(out m_nActorID);
            command.Parameters.AddWithValue(Scp.inParams[0], m_nActorID);

            reader.ReadU16String(out m_Email);
            command.Parameters.AddWithValue(Scp.inParams[1], m_Email);
        }
    }
}