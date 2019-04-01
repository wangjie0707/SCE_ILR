using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 资源更新器
    /// </summary>
    public class ResourceUpdater
    {
        private const string ResourceListFileName = "list";
        private const string BackupFileSuffixName = ".bak";
        private const int OneMegaBytes = 1024 * 1024;


        private Stream m_DecompressCache;
        private byte[] m_UpdateFileCache;
        private readonly ResourceManager m_ResourceManager;
        private readonly List<UpdateInfo> m_UpdateWaitingInfo;
        private bool m_CheckResourcesComplete;
        private bool m_UpdateAllowed;
        private bool m_UpdateComplete;
        private int m_GenerateReadWriteListLength;
        private int m_CurrentGenerateReadWriteListLength;
        private int m_RetryCount;
        private int m_UpdatingCount;

        public Action<string, string, string, int, int, int> ResourceUpdateStart;
        public Action<string, string, string, int, int> ResourceUpdateChanged;
        public Action<string, string, string, int, int> ResourceUpdateSuccess;
        public Action<string, string, int, int, string> ResourceUpdateFailure;
        public Action ResourceUpdateAllComplete;

        /// <summary>
        /// 初始化资源更新器的新实例
        /// </summary>
        /// <param name="resourceManager">资源管理器</param>
        public ResourceUpdater(ResourceManager resourceManager)
        {
            m_ResourceManager = resourceManager;
            m_UpdateWaitingInfo = new List<UpdateInfo>();
            m_CheckResourcesComplete = false;
            m_UpdateAllowed = false;
            m_UpdateComplete = false;
            m_GenerateReadWriteListLength = 0;
            m_CurrentGenerateReadWriteListLength = 0;
            m_RetryCount = 3;
            m_UpdatingCount = 0;

            ResourceUpdateStart = null;
            ResourceUpdateChanged = null;
            ResourceUpdateSuccess = null;
            ResourceUpdateFailure = null;
            ResourceUpdateAllComplete = null;
        }

        /// <summary>
        /// 获取或设置每下载多少字节的资源，刷新一次资源列表
        /// </summary>
        public int GenerateReadWriteListLength
        {
            get
            {
                return m_GenerateReadWriteListLength;
            }
            set
            {
                m_GenerateReadWriteListLength = value;
            }
        }

        /// <summary>
        /// 获取或设置资源更新重试次数
        /// </summary>
        public int RetryCount
        {
            get
            {
                return m_RetryCount;
            }
            set
            {
                m_RetryCount = value;
            }
        }

        /// <summary>
        /// 获取等待更新队列大小
        /// </summary>
        public int UpdateWaitingCount
        {
            get
            {
                return m_UpdateWaitingInfo.Count;
            }
        }

        /// <summary>
        /// 获取正在更新队列大小
        /// </summary>
        public int UpdatingCount
        {
            get
            {
                return m_UpdatingCount;
            }
        }

        /// <summary>
        /// 资源更新器轮询
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位</param>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (m_UpdateAllowed && !m_UpdateComplete)
            {
                if (m_UpdateWaitingInfo.Count > 0)
                {
                    if (GameEntry.Download.FreeAgentCount > 0)
                    {
                        UpdateInfo updateInfo = m_UpdateWaitingInfo[0];
                        m_UpdateWaitingInfo.RemoveAt(0);
                        GameEntry.Download.AddDownload(updateInfo.DownloadPath, updateInfo.DownloadUri, updateInfo);
                        m_UpdatingCount++;
                    }
                }
                else if (m_UpdatingCount <= 0)
                {
                    m_UpdateComplete = true;
                    PathUtil.RemoveEmptyDirectory(m_ResourceManager.ReadWritePath);
                    if (ResourceUpdateAllComplete != null)
                    {
                        ResourceUpdateAllComplete();
                    }
                }
            }
        }

        /// <summary>
        /// 关闭并清理资源更新器
        /// </summary>
        public void Shutdown()
        {
            if (GameEntry.Download != null)
            {
                GameEntry.Download.DownloadStart -= OnDownloadStart;
                GameEntry.Download.DownloadUpdate -= OnDownloadUpdate;
                GameEntry.Download.DownloadSuccess -= OnDownloadSuccess;
                GameEntry.Download.DownloadFailure -= OnDownloadFailure;
            }

            m_UpdateWaitingInfo.Clear();
        }



        /// <summary>
        /// 设置下载管理器
        /// </summary>
        /// <param name="downloadManager">下载管理器</param>
        public void SetDownloadManager()
        {
            GameEntry.Download.DownloadStart += OnDownloadStart;
            GameEntry.Download.DownloadUpdate += OnDownloadUpdate;
            GameEntry.Download.DownloadSuccess += OnDownloadSuccess;
            GameEntry.Download.DownloadFailure += OnDownloadFailure;
        }

        /// <summary>
        /// 增加资源更新
        /// </summary>
        /// <param name="resourceName">资源名称</param>
        /// <param name="length">资源大小</param>
        /// <param name="hashCode">资源哈希值</param>
        /// <param name="zipLength">压缩包大小</param>
        /// <param name="zipHashCode">压缩包哈希值</param>
        /// <param name="downloadPath">下载后存放路径</param>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="retryCount">已重试次数</param>
        public void AddResourceUpdate(string resourceName, int length, int hashCode, int zipLength, int zipHashCode, string downloadPath, string downloadUri, int retryCount)
        {
            m_UpdateWaitingInfo.Add(new UpdateInfo(resourceName, length, hashCode, zipLength, zipHashCode, downloadPath, downloadUri, retryCount));
        }

        /// <summary>
        /// 检查资源完成
        /// </summary>
        /// <param name="needGenerateReadWriteList">是否需要生成读写区资源列表</param>
        public void CheckResourceComplete(bool needGenerateReadWriteList)
        {
            m_CheckResourcesComplete = true;
            if (needGenerateReadWriteList)
            {
                GenerateReadWriteList();
            }
        }

        /// <summary>
        /// 更新资源
        /// </summary>
        public void UpdateResources()
        {
            if (!m_CheckResourcesComplete)
            {
                throw new Exception("You must check resources complete first.");
            }

            m_UpdateAllowed = true;
        }

        private void GenerateReadWriteList()
        {
            string file = PathUtil.GetCombinePath(m_ResourceManager.ReadWritePath, PathUtil.GetResourceNameWithSuffix(ResourceListFileName));
            string backupFile = null;

            if (File.Exists(file))
            {
                backupFile = file + BackupFileSuffixName;
                if (File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }

                File.Move(file, backupFile);
            }


            try
            {
                using (FileStream fileStream = new FileStream(file, FileMode.CreateNew, FileAccess.Write))
                {
                    byte[] buffer = null;
                    using (MMO_MemoryStream ms = new MMO_MemoryStream())
                    {
                        ms.WriteInt(m_ResourceManager.ReadWriteResourceInfos.Count);
                        foreach (var readWriteResourceInfo in m_ResourceManager.ReadWriteResourceInfos)
                        {
                            int length = readWriteResourceInfo.Value.Length;
                            int hashcode = readWriteResourceInfo.Value.HashCode;

                            ms.WriteUTF8String(readWriteResourceInfo.Key);
                            ms.WriteInt(length);
                            ms.WriteInt(hashcode);
                        }
                        buffer = ms.ToArray();
                    }
                    fileStream.Write(buffer, 0, buffer.Length);
                }
                
                if (!string.IsNullOrEmpty(backupFile))
                {
                    File.Delete(backupFile);
                }
            }
            catch (Exception exception)
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                if (!string.IsNullOrEmpty(backupFile))
                {
                    File.Move(backupFile, file);
                }

                throw new Exception(TextUtil.Format("Pack save exception '{0}'.", exception.Message), exception);
            }
        }

        private void OnDownloadStart(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            UpdateInfo updateInfo = userData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (currentLength > updateInfo.ZipLength)
            {
                GameEntry.Download.RemoveDownload(serialId);
                string downloadFile = TextUtil.Format("{0}.download", downloadPath);
                if (File.Exists(downloadFile))
                {
                    File.Delete(downloadFile);
                }

                string errorMessage = TextUtil.Format("When download start, downloaded length is larger than zip length, need '{0}', current '{1}'.", updateInfo.ZipLength.ToString(), currentLength.ToString());
                OnDownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
                return;
            }

            if (ResourceUpdateStart != null)
            {
                ResourceUpdateStart(updateInfo.ResourceName, downloadPath, downloadUri, currentLength, updateInfo.ZipLength, updateInfo.RetryCount);
            }
        }

        private void OnDownloadUpdate(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            UpdateInfo updateInfo = userData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (currentLength > updateInfo.ZipLength)
            {
                GameEntry.Download.RemoveDownload(serialId);
                string downloadFile = TextUtil.Format("{0}.download", downloadPath);
                if (File.Exists(downloadFile))
                {
                    File.Delete(downloadFile);
                }

                string errorMessage = TextUtil.Format("When download update, downloaded length is larger than zip length, need '{0}', current '{1}'.", updateInfo.ZipLength.ToString(), currentLength.ToString());
                OnDownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
                return;
            }

            if (ResourceUpdateChanged != null)
            {
                ResourceUpdateChanged(updateInfo.ResourceName, downloadPath, downloadUri, currentLength, updateInfo.ZipLength);
            }
        }

        private void OnDownloadSuccess(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            UpdateInfo updateInfo = userData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            using (FileStream fileStream = new FileStream(downloadPath, FileMode.Open, FileAccess.ReadWrite))
            {
                bool zip = (updateInfo.Length != updateInfo.ZipLength || updateInfo.HashCode != updateInfo.ZipHashCode);

                int length = (int)fileStream.Length;
                if (length != updateInfo.ZipLength)
                {
                    fileStream.Close();
                    string errorMessage = TextUtil.Format("Zip length error, need '{0}', downloaded '{1}'.", updateInfo.ZipLength.ToString(), length.ToString());
                    OnDownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
                    return;
                }

                if (m_UpdateFileCache == null || m_UpdateFileCache.Length < length)
                {
                    m_UpdateFileCache = new byte[(length / OneMegaBytes + 1) * OneMegaBytes];
                }

                int offset = 0;
                int count = length;
                while (count > 0)
                {
                    int bytesRead = fileStream.Read(m_UpdateFileCache, offset, count);
                    if (bytesRead <= 0)
                    {
                        throw new Exception(TextUtil.Format("Unknown error when load file '{0}'.", downloadPath));
                    }

                    offset += bytesRead;
                    count -= bytesRead;
                }

                int hashCode = ConverterUtil.GetInt32(VerifierUtil.GetCrc32(m_UpdateFileCache, 0, length));
                if (hashCode != updateInfo.ZipHashCode)
                {
                    fileStream.Close();
                    string errorMessage = TextUtil.Format("Zip hash code error, need '{0}', downloaded '{1}'.", updateInfo.ZipHashCode.ToString("X8"), hashCode.ToString("X8"));
                    OnDownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
                    return;
                }

                if (zip)
                {
                    try
                    {
                        if (m_DecompressCache == null)
                        {
                            m_DecompressCache = new MemoryStream();
                        }

                        m_DecompressCache.Position = 0L;
                        m_DecompressCache.SetLength(0L);
                        if (!ZipUtil.Decompress(m_UpdateFileCache, 0, length, m_DecompressCache))
                        {
                            fileStream.Close();
                            string errorMessage = TextUtil.Format("Unable to decompress from file '{0}'.", downloadPath);
                            OnDownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
                            return;
                        }

                        if (m_DecompressCache.Length != updateInfo.Length)
                        {
                            fileStream.Close();
                            string errorMessage = TextUtil.Format("Resource length error, need '{0}', downloaded '{1}'.", updateInfo.Length.ToString(), m_UpdateFileCache.Length.ToString());
                            OnDownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
                            return;
                        }

                        fileStream.Position = 0L;
                        fileStream.SetLength(0L);
                        m_DecompressCache.Position = 0L;
                        int bytesRead = 0;
                        while ((bytesRead = m_DecompressCache.Read(m_UpdateFileCache, 0, m_UpdateFileCache.Length)) > 0)
                        {
                            fileStream.Write(m_UpdateFileCache, 0, bytesRead);
                        }
                    }
                    catch (Exception exception)
                    {
                        fileStream.Close();
                        string errorMessage = TextUtil.Format("Unable to decompress from file '{0}' with error message '{1}'.", downloadPath, exception.Message);
                        OnDownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
                        return;
                    }
                }
            }

            m_UpdatingCount--;

            if (m_ResourceManager.ResourceInfos.ContainsKey(updateInfo.ResourceName))
            {
                throw new Exception(TextUtil.Format("Resource info '{0}' is already exist.", updateInfo.ResourceName));
            }

            m_ResourceManager.ResourceInfos.Add(updateInfo.ResourceName, new ResourceInfo(updateInfo.ResourceName, updateInfo.Length, updateInfo.HashCode, false));
            m_ResourceManager.ResourceNames.Add(updateInfo.ResourceName);

            if (m_ResourceManager.ReadWriteResourceInfos.ContainsKey(updateInfo.ResourceName))
            {
                throw new Exception(TextUtil.Format("Read-write resource info '{0}' is already exist.", updateInfo.ResourceName));
            }

            m_ResourceManager.ReadWriteResourceInfos.Add(updateInfo.ResourceName, new ReadWriteResourceInfo(updateInfo.Length, updateInfo.HashCode));

            m_CurrentGenerateReadWriteListLength += updateInfo.ZipLength;
            if (m_UpdatingCount <= 0 || m_CurrentGenerateReadWriteListLength >= m_GenerateReadWriteListLength)
            {
                m_CurrentGenerateReadWriteListLength = 0;
                GenerateReadWriteList();
            }

            if (ResourceUpdateSuccess != null)
            {
                ResourceUpdateSuccess(updateInfo.ResourceName, downloadPath, downloadUri, updateInfo.Length, updateInfo.ZipLength);
            }
        }

        private void OnDownloadFailure(int serialId, string downloadPath, string downloadUri, string errorMessage, object userData)
        {
            UpdateInfo updateInfo = userData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
            }

            if (ResourceUpdateFailure != null)
            {
                ResourceUpdateFailure(updateInfo.ResourceName, downloadUri, updateInfo.RetryCount, m_RetryCount, errorMessage);
            }

            if (updateInfo.RetryCount < m_RetryCount)
            {
                m_UpdatingCount--;
                UpdateInfo newUpdateInfo = new UpdateInfo(updateInfo.ResourceName, updateInfo.Length, updateInfo.HashCode, updateInfo.ZipLength, updateInfo.ZipHashCode, updateInfo.DownloadPath, updateInfo.DownloadUri, updateInfo.RetryCount + 1);
                if (m_UpdateAllowed)
                {
                    m_UpdateWaitingInfo.Add(newUpdateInfo);
                }
                else
                {
                    throw new Exception("Update state error.");
                }
            }
        }
    }
}
