namespace Myth
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public class ResourceInfo
    {
        private readonly string m_ResourceName;
        private readonly int m_Length;
        private readonly int m_HashCode;
        private readonly bool m_StorageInReadOnly;

        /// <summary>
        /// 初始化资源信息的新实例
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="length">资源大小</param>
        /// <param name="hashCode">资源哈希值</param>
        /// <param name="storageInReadOnly">资源是否在只读区</param>
        public ResourceInfo(string resourceName, int length, int hashCode, bool storageInReadOnly)
        {
            m_ResourceName = resourceName;
            m_Length = length;
            m_HashCode = hashCode;
            m_StorageInReadOnly = storageInReadOnly;
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
                return m_Length;
            }
        }

        /// <summary>
        /// 获取资源哈希值
        /// </summary>
        public int HashCode
        {
            get
            {
                return m_HashCode;
            }
        }

        /// <summary>
        /// 获取资源是否在只读区
        /// </summary>
        public bool StorageInReadOnly
        {
            get
            {
                return m_StorageInReadOnly;
            }
        }
    }
}
