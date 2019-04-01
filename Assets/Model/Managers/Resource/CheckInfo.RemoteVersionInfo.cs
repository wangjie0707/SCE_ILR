﻿namespace Myth
{
    public partial class CheckInfo
    {
        /// <summary>
        /// 远程资源状态信息
        /// </summary>
        private struct RemoteVersionInfo
        {
            private readonly bool m_Exist;
            private readonly int m_Length;
            private readonly int m_HashCode;
            private readonly int m_ZipLength;
            private readonly int m_ZipHashCode;

            public RemoteVersionInfo(int length, int hashCode, int zipLength, int zipHashCode)
            {
                m_Exist = true;
                m_Length = length;
                m_HashCode = hashCode;
                m_ZipLength = zipLength;
                m_ZipHashCode = zipHashCode;
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

            public int ZipLength
            {
                get
                {
                    return m_ZipLength;
                }
            }

            public int ZipHashCode
            {
                get
                {
                    return m_ZipHashCode;
                }
            }
        }
    }
} 