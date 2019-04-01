namespace Myth
{
    public partial class CheckInfo
    {
        /// <summary>
        /// 本地资源状态信息
        /// </summary>
        private struct LocalVersionInfo
        {
            private readonly bool m_Exist;
            private readonly int m_Length;
            private readonly int m_HashCode;

            public LocalVersionInfo( int length, int hashCode)
            {
                m_Exist = true;
                m_Length = length;
                m_HashCode = hashCode;
            }

            public bool Exist
            {
                get
                {
                    return m_Exist;
                }
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
}