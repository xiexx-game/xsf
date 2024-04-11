//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Base/XSFWriter.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：二进制流式写，小端
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Text;

namespace XSF
{
    public sealed class XSFWriter
    {
        private MemoryStream m_Stream;

        public XSFWriter()
        {
            m_Stream = new MemoryStream();
        }

        public byte[] Buffer
        {
            get
            {
                return m_Stream.GetBuffer();
            }
        }

        public long Size
        {
            get
            {
                return m_Stream.Position;
            }
        }

        public byte[] ByteArray
        {
            get {
                byte[] data = new byte[Size];
                Array.Copy(Buffer, data, Size);
                return data;
            }
        }

        public void Clear()
        {
            m_Stream.Seek(0, SeekOrigin.Begin);
        }

        public void WriteByte(byte value)
        {
            m_Stream.WriteByte(value);
        }

        public void WriteSbyte(sbyte value)
        {
            m_Stream.WriteByte((byte)value);
        }

        public void WriteShort(short value)
        {
            byte[] data = BitConverter.GetBytes(value);
            m_Stream.Write(data, 0, data.Length);
        }

        public void WriteUShort(ushort value)
        {
            byte[] data = BitConverter.GetBytes(value);
            m_Stream.Write(data, 0, data.Length);
        }

        public void WriteInt(int value)
        {
            byte[] data = BitConverter.GetBytes(value);
            m_Stream.Write(data, 0, data.Length);
        }

        public void WriteUInt(uint value)
        {
            byte[] data = BitConverter.GetBytes(value);
            m_Stream.Write(data, 0, data.Length);
        }

        public void WriteLong(long value)
        {
            byte[] data = BitConverter.GetBytes(value);
            m_Stream.Write(data, 0, data.Length);
        }

        public void WriteULong(ulong value)
        {
            byte[] data = BitConverter.GetBytes(value);
            m_Stream.Write(data, 0, data.Length);
        }

        public void WriteBool(bool value)
        {
            byte[] data = BitConverter.GetBytes(value);
            m_Stream.Write(data, 0, data.Length);
        }

        public void WriteFloat(float value)
        {
            byte[] data = BitConverter.GetBytes(value);
            m_Stream.Write(data, 0, data.Length);
        }

        public void WriteDouble(double value)
        {
            byte[] data = BitConverter.GetBytes(value);
            m_Stream.Write(data, 0, data.Length);
        }

        public void WriteU8String(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);

            byte length = 0xFF;
            if (data.Length < length)
                length = (byte)data.Length;

            m_Stream.WriteByte(length);
            m_Stream.Write(data, 0, length);
        }

        public void WriteU16String(string value)
        {
            byte[] data = Encoding.UTF8.GetBytes(value);

            ushort length = 0xFFFF;
            if (data.Length < length)
                length = (ushort)data.Length;

            WriteUShort(length);
            m_Stream.Write(data, 0, length);
        }

        public void WriteBuffer(byte[] value)
        {
            m_Stream.Write(value, 0, value.Length);
        }

        public void WriteBuffer(byte[] value, int nLength)
        {
            m_Stream.Write(value, 0, nLength);
        }
    }
}



