using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 版本资源列表处理器
    /// </summary>
    public sealed class VersionListProcessor
    {
        private const string VersionListFileName = "version";
        private const int OneMegaBytes = 1024 * 1024;
        private byte[] m_UpdateFileCache;

        private Stream m_DecompressCache;
        private readonly ResourceManager m_ResourceManager;
        private int m_VersionListLength;
        private int m_VersionListHashCode;
        private int m_VersionListZipLength;
        private int m_VersionListZipHashCode;

        public Action<string, string> VersionListUpdateSuccess;
        public Action<string, string> VersionListUpdateFailure;

        /// <summary>
        /// 初始化版本资源列表处理器的新实例
        /// </summary>
        /// <param name="resourceManager">资源管理器</param>
        public VersionListProcessor(ResourceManager resourceManager)
        {
            m_ResourceManager = resourceManager;
            m_VersionListLength = 0;
            m_VersionListHashCode = 0;
            m_VersionListZipLength = 0;
            m_VersionListZipHashCode = 0;

            VersionListUpdateSuccess = null;
            VersionListUpdateFailure = null;
        }

        /// <summary>
        /// 关闭并清理版本资源列表处理器
        /// </summary>
        public void Shutdown()
        {
            if (GameEntry.Download != null)
            {
                GameEntry.Download.DownloadSuccess -= OnDownloadSuccess;
                GameEntry.Download.DownloadFailure -= OnDownloadFailure;
            }
        }

        /// <summary>
        /// 设置下载管理器
        /// </summary>
        /// <param name="downloadManager">下载管理器</param>
        public void SetDownloadManager()
        {
            if (GameEntry.Download == null)
            {
                throw new Exception("Download manager is invalid.");
            }

            GameEntry.Download.DownloadSuccess += OnDownloadSuccess;
            GameEntry.Download.DownloadFailure += OnDownloadFailure;
        }

        /// <summary>
        /// 更新版本资源列表
        /// </summary>
        /// <param name="versionListLength">版本资源列表大小</param>
        /// <param name="versionListHashCode">版本资源列表哈希值</param>
        /// <param name="versionListZipLength">版本资源列表压缩后大小</param>
        /// <param name="versionListZipHashCode">版本资源列表压缩后哈希值</param>
        public void UpdateVersionList(int versionListLength, int versionListHashCode, int versionListZipLength, int versionListZipHashCode)
        {
            if (GameEntry.Download == null)
            {
                throw new Exception("You must set download manager first.");
            }

            m_VersionListLength = versionListLength;
            m_VersionListHashCode = versionListHashCode;
            m_VersionListZipLength = versionListZipLength;
            m_VersionListZipHashCode = versionListZipHashCode;
            string versionListFileName = PathUtil.GetResourceNameWithSuffix(VersionListFileName);
            string latestVersionListFileName = PathUtil.GetResourceNameWithCrc32AndSuffix(VersionListFileName, m_VersionListHashCode);
            string localVersionListFilePath = PathUtil.GetCombinePath(m_ResourceManager.ReadWritePath, versionListFileName);
            string latestVersionListFileUri = PathUtil.GetRemotePath(m_ResourceManager.UpdatePrefixUri, latestVersionListFileName);
            GameEntry.Download.AddDownload(localVersionListFilePath, latestVersionListFileUri, this);
        }

        private void OnDownloadSuccess(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            VersionListProcessor versionListProcessor = userData as VersionListProcessor;
            if (versionListProcessor == null || versionListProcessor != this)
            {
                return;
            }

            using (FileStream fileStream = new FileStream(downloadPath, FileMode.Open, FileAccess.ReadWrite))
            {
                int length = (int)fileStream.Length;
                if (length != m_VersionListZipLength)
                {
                    fileStream.Close();
                    string errorMessage = TextUtil.Format("Latest version list zip length error, need '{0}', downloaded '{1}'.", m_VersionListZipLength.ToString(), length.ToString());
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
                if (hashCode != m_VersionListZipHashCode)
                {
                    fileStream.Close();
                    string errorMessage = TextUtil.Format("Latest version list zip hash code error, need '{0}', downloaded '{1}'.", m_VersionListZipHashCode.ToString("X8"), hashCode.ToString("X8"));
                    OnDownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
                    return;
                }
                
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
                        string errorMessage = TextUtil.Format("Unable to decompress latest version list '{0}'.", downloadPath);
                        OnDownloadFailure(serialId,downloadPath,downloadUri, errorMessage, userData);
                        return;
                    }


                    if (m_DecompressCache.Length != m_VersionListLength)
                    {
                        fileStream.Close();
                        string errorMessage = TextUtil.Format("Latest version list length error, need '{0}', downloaded '{1}'.", m_VersionListLength.ToString(), m_DecompressCache.Length.ToString());
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
                    string errorMessage = TextUtil.Format("Unable to decompress latest version list '{0}' with error message '{1}'.", downloadPath, exception.Message);
                    OnDownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
                    return;
                }
            }

            if (VersionListUpdateSuccess != null)
            {
                VersionListUpdateSuccess(downloadPath, downloadUri);
            }
        }

        private void OnDownloadFailure(int serialId, string downloadPath, string downloadUri, string errorMessage, object userData)
        {
            VersionListProcessor versionListProcessor = userData as VersionListProcessor;
            if (versionListProcessor == null || versionListProcessor != this)
            {
                return;
            }
            
            if (File.Exists(downloadPath))
            {
                File.Delete(downloadPath);
            }

            if (VersionListUpdateFailure != null)
            {
                VersionListUpdateFailure(downloadUri, errorMessage);
            }
        }
    }
}
