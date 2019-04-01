using System;

namespace Myth
{
    /// <summary>
    /// 资源检查信息
    /// </summary>
    public partial class CheckInfo
    {
        private readonly string m_ResourceName;
        private CheckStatus m_Status;
        private bool m_NeedRemove;
        private RemoteVersionInfo m_VersionInfo;
        private LocalVersionInfo m_ReadOnlyInfo;
        private LocalVersionInfo m_ReadWriteInfo;

        /// <summary>
        /// 初始化资源检查信息的新实例
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        public CheckInfo(string resourceName)
        {
            m_ResourceName = resourceName;
            m_Status = CheckStatus.Unknown;
            m_NeedRemove = false;
            m_VersionInfo = default(RemoteVersionInfo);
            m_ReadOnlyInfo = default(LocalVersionInfo);
            m_ReadWriteInfo = default(LocalVersionInfo);
        }

        /// <summary>
        /// 获取资源名称
        /// </summary>
        public string ResourceName
        {
            get
            {
                return m_ResourceName;
            }
        }

        /// <summary>
        /// 获取资源大小
        /// </summary>
        public int Length
        {
            get
            {
                return m_VersionInfo.Length;
            }
        }

        /// <summary>
        /// 获取资源哈希值
        /// </summary>
        public int HashCode
        {
            get
            {
                return m_VersionInfo.HashCode;
            }
        }

        /// <summary>
        /// 获取压缩包大小
        /// </summary>
        public int ZipLength
        {
            get
            {
                return m_VersionInfo.ZipLength;
            }
        }

        /// <summary>
        /// 获取压缩包哈希值
        /// </summary>
        public int ZipHashCode
        {
            get
            {
                return m_VersionInfo.ZipHashCode;
            }
        }

        /// <summary>
        /// 获取资源检查状态
        /// </summary>
        public CheckStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        /// <summary>
        /// 获取资源是否可以从读写区移除
        /// </summary>
        public bool NeedRemove
        {
            get
            {
                return m_NeedRemove;
            }
        }

        /// <summary>
        /// 设置资源在版本中的信息
        /// </summary>
        /// <param name="length">资源大小</param>
        /// <param name="hashCode">资源哈希值</param>
        /// <param name="zipLength">压缩包大小</param>
        /// <param name="zipHashCode">压缩包哈希值</param>
        public void SetVersionInfo(int length, int hashCode, int zipLength, int zipHashCode)
        {
            if (m_VersionInfo.Exist)
            {
                throw new Exception(TextUtil.Format("You must set version info of '{0}' only once.", m_ResourceName));
            }

            m_VersionInfo = new RemoteVersionInfo(length, hashCode, zipLength, zipHashCode);
        }

        /// <summary>
        /// 设置资源在只读区中的信息
        /// </summary>
        /// <param name="length">资源大小</param>
        /// <param name="hashCode">资源哈希值</param>
        public void SetReadOnlyInfo(int length, int hashCode)
        {
            if (m_ReadOnlyInfo.Exist)
            {
                throw new Exception(TextUtil.Format("You must set readonly info of '{0}' only once.", m_ResourceName));
            }

            m_ReadOnlyInfo = new LocalVersionInfo( length, hashCode);
        }

        /// <summary>
        /// 设置资源在读写区中的信息
        /// </summary>
        /// <param name="length">资源大小</param>
        /// <param name="hashCode">资源哈希值</param>
        public void SetReadWriteInfo(int length, int hashCode)
        {
            if (m_ReadWriteInfo.Exist)
            {
                throw new Exception(TextUtil.Format("You must set read-write info of '{0}' only once.", m_ResourceName));
            }

            m_ReadWriteInfo = new LocalVersionInfo(length, hashCode);
        }

        /// <summary>
        /// 刷新资源信息状态
        /// </summary>
        /// <param name="currentVariant">当前变体</param>
        public void RefreshStatus()
        {
            if (!m_VersionInfo.Exist)
            {
                m_Status = CheckStatus.Disuse;
                m_NeedRemove = m_ReadWriteInfo.Exist;
                return;
            }
            
            if (m_ReadOnlyInfo.Exist  && m_ReadOnlyInfo.Length == m_VersionInfo.Length && m_ReadOnlyInfo.HashCode == m_VersionInfo.HashCode)
            {
                m_Status = CheckStatus.StorageInReadOnly;
                m_NeedRemove = m_ReadWriteInfo.Exist;
            }
            else if (m_ReadWriteInfo.Exist &&  m_ReadWriteInfo.Length == m_VersionInfo.Length && m_ReadWriteInfo.HashCode == m_VersionInfo.HashCode)
            {
                m_Status = CheckStatus.StorageInReadWrite;
                m_NeedRemove = false;
            }
            else
            {
                m_Status = CheckStatus.NeedUpdate;
                m_NeedRemove = m_ReadWriteInfo.Exist;
            }
        }
    }
}


