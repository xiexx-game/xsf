//////////////////////////////////////////////////////////////////////////
// 
// 文件：server/XSF/Net/BufferManager.cs
// 作者：Xoen Xie
// 时间：2023/06/19
// 描述：缓冲管理
// 说明：
//
//////////////////////////////////////////////////////////////////////////
#pragma warning disable CS8600, CS8603

namespace XSF
{
    class BufferData
    {
        private const ushort MAX_BUFFER_SIZE = 1024 * 32;

        private byte [] rawBuffer;

        public byte[] TempBuffer {
            get { return segmentTemp.Array; }
        }
        
        public byte[] LengthData {
            get { return segmentLength.Array; }
        }

        public byte[] RecvBuffer {
            get { return segmentRecv.Array; }
        }

        ArraySegment<byte> segmentRecv;
        ArraySegment<byte> segmentTemp;
        ArraySegment<byte> segmentLength;

        public int BufferLength;

        public BufferData()
        {
            int totalSize = MAX_BUFFER_SIZE * 2 + 4; // 总大小为32KB + 32KB + 4字节

            // 分配一个大块的内存
            rawBuffer = new byte[totalSize];

            // 分割成三个不同的数组
            segmentRecv = new ArraySegment<byte>(rawBuffer, 0, MAX_BUFFER_SIZE);
            segmentTemp = new ArraySegment<byte>(rawBuffer, MAX_BUFFER_SIZE, MAX_BUFFER_SIZE);
            segmentLength = new ArraySegment<byte>(rawBuffer, MAX_BUFFER_SIZE * 2, 4);
            BufferLength = 0;
        }
    }

    class BufferManager : Singleton<BufferManager>
    {
        
        private Queue<BufferData> m_Buffers;

        private object m_Lock;

        public BufferManager()
        {
            m_Buffers = new Queue<BufferData>();
            m_Lock = new object();
        }

        public BufferData GetBuffer()
        {
            lock (m_Lock)
            {
                if(m_Buffers.Count > 0)
                {
                    BufferData bdOld = m_Buffers.Dequeue();
                    bdOld.BufferLength = 0;
                    return bdOld;
                }
            }

            return new BufferData();
        }

        public void SaveBuffer(BufferData bd)
        {
            lock (m_Lock)
            {
                m_Buffers.Enqueue(bd);
            }
        }
    }
}