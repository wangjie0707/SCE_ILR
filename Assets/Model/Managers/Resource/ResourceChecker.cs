using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 资源检查器
    /// </summary>
    public class ResourceChecker
    {
        private const string VersionListFileName = "version";
        private const string ResourceListFileName = "list";
        private const string BackupFileSuffixName = ".bak";

        private ResourceManager m_ResourceManager;
        private readonly Dictionary<string, CheckInfo> m_CheckInfos;
        private bool m_VersionListReady;
        private bool m_ReadOnlyListReady;
        private bool m_ReadWriteListReady;

        public Action<string, int, int, int, int> ResourceNeedUpdate;
        public Action<int, int, long, long> ResourceCheckComplete;

        /// <summary>
        /// 初始化资源检查器的新实例
        /// </summary>
        /// <param name="resourceManager">资源管理器</param>
        public ResourceChecker(ResourceManager resourceManager)
        {
            m_ResourceManager = resourceManager;
            m_CheckInfos = new Dictionary<string, CheckInfo>();
            m_VersionListReady = false;
            m_ReadOnlyListReady = false;
            m_ReadWriteListReady = false;

            ResourceNeedUpdate = null;
            ResourceCheckComplete = null;
        }

        /// <summary>
        /// 关闭并清理资源检查器
        /// </summary>
        public void Shutdown()
        {
            m_CheckInfos.Clear();
        }

        public void CheckResources()
        {
            TryRecoverReadWriteList();

            if (GameEntry.Resource.ResourceHelper == null)
            {
                throw new Exception("Resource helper is invalid.");
            }

            GameEntry.Resource.ResourceHelper.LoadBytes(PathUtil.GetRemotePath(GameEntry.Resource.ReadWritePath, PathUtil.GetResourceNameWithSuffix(VersionListFileName)), ParseVersionList);
            GameEntry.Resource.ResourceHelper.LoadBytes(PathUtil.GetRemotePath(GameEntry.Resource.ReadOnlyPath, PathUtil.GetResourceNameWithSuffix(ResourceListFileName)), ParseReadOnlyList);
            GameEntry.Resource.ResourceHelper.LoadBytes(PathUtil.GetRemotePath(GameEntry.Resource.ReadWritePath, PathUtil.GetResourceNameWithSuffix(ResourceListFileName)), ParseReadWriteList);
        }

        private void SetVersionInfo(string resourceName, int length, int hashCode, int zipLength, int zipHashCode)
        {
            GetOrAddCheckInfo(resourceName).SetVersionInfo(length, hashCode, zipLength, zipHashCode);
        }

        private void SetReadOnlyInfo(string resourceName, int length, int hashCode)
        {
            GetOrAddCheckInfo(resourceName).SetReadOnlyInfo(length, hashCode);
        }

        private void SetReadWriteInfo(string resourceName, int length, int hashCode)
        {
            GetOrAddCheckInfo(resourceName).SetReadWriteInfo(length, hashCode);
        }

        private CheckInfo GetOrAddCheckInfo(string resourceName)
        {
            CheckInfo checkInfo = null;
            if (m_CheckInfos.TryGetValue(resourceName, out checkInfo))
            {
                return checkInfo;
            }

            checkInfo = new CheckInfo(resourceName);
            m_CheckInfos.Add(checkInfo.ResourceName, checkInfo);

            return checkInfo;
        }

        private void RefreshCheckInfoStatus()
        {
            if (!m_VersionListReady || !m_ReadOnlyListReady || !m_ReadWriteListReady)
            {
                return;
            }

            int removedCount = 0;
            int updateCount = 0;
            long updateTotalLength = 0L;
            long updateTotalZipLength = 0L;
            foreach (KeyValuePair<string, CheckInfo> checkInfo in m_CheckInfos)
            {
                CheckInfo ci = checkInfo.Value;
                ci.RefreshStatus();
                if (ci.Status == CheckInfo.CheckStatus.StorageInReadOnly)
                {
                    ProcessResourceInfo(ci.ResourceName, ci.Length, ci.HashCode, true);
                }
                else if (ci.Status == CheckInfo.CheckStatus.StorageInReadWrite)
                {
                    ProcessResourceInfo(ci.ResourceName, ci.Length, ci.HashCode, false);
                }
                else if (ci.Status == CheckInfo.CheckStatus.NeedUpdate)
                {
                    updateCount++;
                    updateTotalLength += ci.Length;
                    updateTotalZipLength += ci.ZipLength;

                    ResourceNeedUpdate(ci.ResourceName, ci.Length, ci.HashCode, ci.ZipLength, ci.ZipHashCode);
                }
                else if (ci.Status == CheckInfo.CheckStatus.Disuse)
                {
                    // Do nothing.
                }
                else
                {
                    throw new Exception(TextUtil.Format("Check resources '{0}' error with unknown status.", ci.ResourceName));
                }

                if (ci.NeedRemove)
                {
                    removedCount++;
                    string path = PathUtil.GetCombinePath(m_ResourceManager.ReadWritePath, PathUtil.GetResourceNameWithSuffix(ci.ResourceName));
                    File.Delete(path);

                    if (!m_ResourceManager.ReadWriteResourceInfos.ContainsKey(ci.ResourceName))
                    {
                        throw new Exception(TextUtil.Format("Resource '{0}' is not exist in read-write list.", ci.ResourceName));
                    }

                    m_ResourceManager.ReadWriteResourceInfos.Remove(ci.ResourceName);
                }
            }

            ResourceCheckComplete(removedCount, updateCount, updateTotalLength, updateTotalZipLength);
        }

        /// <summary>
        /// 尝试恢复读写区资源列表
        /// </summary>
        /// <returns>是否恢复成功</returns>
        private bool TryRecoverReadWriteList()
        {
            string file = PathUtil.GetCombinePath(m_ResourceManager.ReadWritePath, PathUtil.GetResourceNameWithSuffix(ResourceListFileName));
            string backupFile = file + BackupFileSuffixName;

            try
            {
                if (!File.Exists(backupFile))
                {
                    return false;
                }

                if (File.Exists(file))
                {
                    File.Delete(file);
                }

                File.Move(backupFile, file);
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 解析版本资源列表
        /// </summary>
        /// <param name="fileUri">版本资源列表文件路径</param>
        /// <param name="bytes">要解析的数据</param>
        /// <param name="errorMessage">错误信息</param>
        private void ParseVersionList(string fileUri, byte[] bytes, string errorMessage)
        {
            if (m_VersionListReady)
            {
                throw new Exception("Version list has been parsed.");
            }

            if (bytes == null || bytes.Length <= 0)
            {
                throw new Exception(TextUtil.Format("Version list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
            }

            try
            {
                using (MMO_MemoryStream ms = new MMO_MemoryStream(bytes))
                {
                    int assetCount = ms.ReadInt();
                    m_ResourceManager.AssetsInfos = new Dictionary<string, string>(assetCount);
                    int resourceCount = ms.ReadInt();
                    m_ResourceManager.ResourceInfos = new Dictionary<string, ResourceInfo>(resourceCount);

                    for (int i = 0; i < resourceCount; i++)
                    {
                        string resourceName = ms.ReadUTF8String();
                        
                        int length = ms.ReadInt();
                        int hashCode = ms.ReadInt();
                        int zipLength = ms.ReadInt();
                        int zipHashCode = ms.ReadInt();

                        int assetNamesCount = ms.ReadInt();

                        for (int j = 0; j < assetNamesCount; j++)
                        {
                            string assetName = ms.ReadUTF8String();

                            m_ResourceManager.AssetsInfos.Add(assetName, resourceName);
                            m_ResourceManager.AssetsNames.Add(assetName);
                        }

                        SetVersionInfo(resourceName, length, hashCode, zipLength, zipHashCode);
                    }
                }

                m_VersionListReady = true;
                RefreshCheckInfoStatus();
            }
            catch (Exception exception)
            {
                throw new Exception(TextUtil.Format("Parse version list exception '{0}'.", exception.Message), exception);
            }
        }

        /// <summary>
        /// 解析只读区资源列表
        /// </summary>
        /// <param name="fileUri">只读区资源列表文件路径</param>
        /// <param name="bytes">要解析的数据</param>
        /// <param name="errorMessage">错误信息</param>
        private void ParseReadOnlyList(string fileUri, byte[] bytes, string errorMessage)
        {
            if (m_ReadOnlyListReady)
            {
                throw new Exception("Readonly list has been parsed.");
            }

            if (bytes == null || bytes.Length <= 0)
            {
                m_ReadOnlyListReady = true;
                RefreshCheckInfoStatus();
                return;
            }

            try
            {
                using (MMO_MemoryStream ms = new MMO_MemoryStream(bytes))
                {
                    int resourceCount = ms.ReadInt();

                    for (int i = 0; i < resourceCount; i++)
                    {
                        string name = ms.ReadUTF8String();

                        int length = ms.ReadInt();
                        int hashCode = ms.ReadInt();

                        SetReadOnlyInfo(name, length, hashCode);
                    }
                }

                m_ReadOnlyListReady = true;
                RefreshCheckInfoStatus();
            }
            catch (Exception exception)
            {
                throw new Exception(TextUtil.Format("Parse readonly list exception '{0}'.", exception.Message), exception);
            }
        }

        /// <summary>
        /// 解析读写区资源列表
        /// </summary>
        /// <param name="fileUri">读写区资源列表文件路径</param>
        /// <param name="bytes">要解析的数据</param>
        /// <param name="errorMessage">错误信息</param>
        private void ParseReadWriteList(string fileUri, byte[] bytes, string errorMessage)
        {
            if (m_ReadWriteListReady)
            {
                throw new Exception("Read-write list has been parsed.");
            }

            if (bytes == null || bytes.Length <= 0)
            {
                m_ReadWriteListReady = true;
                RefreshCheckInfoStatus();
                return;
            }

            try
            {
                using (MMO_MemoryStream ms = new MMO_MemoryStream(bytes))
                {
                    int resourceCount = ms.ReadInt();
                    for (int i = 0; i < resourceCount; i++)
                    {
                        string resourceName = ms.ReadUTF8String();

                        int length = ms.ReadInt();
                        int hashCode = ms.ReadInt();
                        SetReadWriteInfo(resourceName, length, hashCode);
                        
                        if (m_ResourceManager.ReadWriteResourceInfos.ContainsKey(resourceName))
                        {
                            throw new Exception(TextUtil.Format("Read-write resource info '{0}' is already exist.", resourceName));
                        }
                        m_ResourceManager.ReadWriteResourceInfos.Add(resourceName, new ReadWriteResourceInfo(length, hashCode));
                    }
                }

                m_ReadWriteListReady = true;
                RefreshCheckInfoStatus();
            }
            catch (Exception exception)
            {
                throw new Exception(TextUtil.Format("Parse read-write list exception '{0}'.", exception.Message), exception);
            }
        }

        private void ProcessResourceInfo(string resourceName, int length, int hashCode, bool storageInReadOnly)
        {
            if (m_ResourceManager.ResourceInfos.ContainsKey(resourceName))
            {
                throw new Exception(TextUtil.Format("Resource info '{0}' is already exist.", resourceName));
            }

            m_ResourceManager.ResourceInfos.Add(resourceName, new ResourceInfo(resourceName, length, hashCode, storageInReadOnly));
            m_ResourceManager.ResourceNames.Add(resourceName);
        }
    }
}
