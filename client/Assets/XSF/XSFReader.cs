//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\XSF\XSFReader.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：二进制流式读，小端
// 说明：
//
//////////////////////////////////////////////////////////////////////////


using System;
using System.Text;

namespace XSF
{
    public sealed class XSFReader
    {
        private byte[] m_Buffer;
        private int m_nCurPos;

        public byte[] Buffer { get { return m_Buffer; } }
        public int CurPos { get { return m_nCurPos; } }

        public void Init(byte[] buffer, int pos = 0)
        {
            m_Buffer = buffer;
            m_nCurPos = pos;
        }

        public void ReadByte(out byte value)
        {
            value = m_Buffer[m_nCurPos++];
        }

        public void ReasSbyte(out sbyte value)
        {
            value = (sbyte)m_Buffer[m_nCurPos++];
        }

        public void ReadShort(out short value)
        {
            value = BitConverter.ToInt16(m_Buffer, m_nCurPos);
            m_nCurPos += sizeof(short);
        }

        public void ReadUShort(out ushort value)
        {
            value = BitConverter.ToUInt16(m_Buffer, m_nCurPos);
            m_nCurPos += sizeof(ushort);
        }

        public void ReadInt(out int value)
        {
            value = BitConverter.ToInt32(m_Buffer, m_nCurPos);
            m_nCurPos += sizeof(int);
        }

        public void ReadUInt(out uint value)
        {
            value = BitConverter.ToUInt32(m_Buffer, m_nCurPos);
            m_nCurPos += sizeof(uint);
        }

        public void ReadLong(out long value)
        {
            value = BitConverter.ToInt64(m_Buffer, m_nCurPos);
            m_nCurPos += sizeof(long);
        }

        public void ReadULong(out ulong value)
        {
            value = BitConverter.ToUInt64(m_Buffer, m_nCurPos);
            m_nCurPos += sizeof(ulong);
        }

        public void ReadBool(out bool value)
        {
            value = BitConverter.ToBoolean(m_Buffer, m_nCurPos);
            m_nCurPos += sizeof(bool);
        }

        public void ReadFloat(out float value)
        {
            value = BitConverter.ToSingle(m_Buffer, m_nCurPos);
            m_nCurPos += sizeof(float);
        }

        public void ReadDouble(out double value)
        {
            value = BitConverter.ToDouble(m_Buffer, m_nCurPos);
            m_nCurPos += sizeof(double);
        }

        public void ReadU8String(out string value)
        {
            byte length = 0;
            ReadByte(out length);

            if (length == 0)
            {
                value = null;
                return;
            }

            value = Encoding.UTF8.GetString(m_Buffer, m_nCurPos, length);
            m_nCurPos += length;
        }

        public void ReadU16String(out string value)
        {
            ushort length = 0;
            ReadUShort(out length);

            if (length == 0)
            {
                value = null;
                return;
            }

            value = Encoding.UTF8.GetString(m_Buffer, m_nCurPos, length);
            m_nCurPos += length;
        }
    }
}
