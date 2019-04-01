namespace Myth
{
    /// <summary>
    /// 读写资源信息
    /// </summary>
    public struct ReadWriteResourceInfo
    {
        private readonly int m_Length;
        private readonly int m_HashCode;

        public ReadWriteResourceInfo(int length, int hashCode)
        {
            m_Length = length;
            m_HashCode = hashCode;
        }
        
        public int Length
        {
            get
            {
                return m_Length;
            }
        }

        public int HashCode
        {
            get
            {
                return m_HashCode;
            }
        }
    }
}