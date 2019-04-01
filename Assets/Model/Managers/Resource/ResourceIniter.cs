using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 资源初始化器
    /// </summary>
    public class ResourceIniter
    {
        private const string VersionListFileName = "version";

        private readonly ResourceManager m_ResourceManager;

        public Action ResourceInitComplete;

        /// <summary>
        /// 初始化资源初始化器的新实例
        /// </summary>
        /// <param name="resourceManager">资源管理器</param>
        public ResourceIniter(ResourceManager resourceManager)
        {
            m_ResourceManager = resourceManager;

            ResourceInitComplete = null;
        }

        /// <summary>
        /// 关闭并清理资源初始化器
        /// </summary>
        public void Shutdown()
        {

        }

        /// <summary>
        /// 初始化资源
        /// </summary>
        public void InitResources()
        {
            if (GameEntry.Resource.ResourceHelper == null)
            {
                throw new Exception("Resource helper is invalid.");
            }

            GameEntry.Resource.ResourceHelper.LoadBytes(PathUtil.GetRemotePath(GameEntry.Resource.ReadOnlyPath, PathUtil.GetResourceNameWithSuffix(VersionListFileName)), ParsePackageList);
        }

        /// <summary>
        /// 解析资源包资源列表
        /// </summary>
        /// <param name="fileUri">版本资源列表文件路径</param>
        /// <param name="bytes">要解析的数据</param>
        /// <param name="errorMessage">错误信息</param>
        private void ParsePackageList(string fileUri, byte[] bytes, string errorMessage)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                throw new Exception(TextUtil.Format("Package list '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
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

                        int assetNamesCount = ms.ReadInt();
                        for (int j = 0; j < assetNamesCount; j++)
                        {
                            string assetName = ms.ReadUTF8String();
                            m_ResourceManager.AssetsInfos.Add(assetName, resourceName);
                            m_ResourceManager.AssetsNames.Add(assetName);
                        }

                        ProcessResourceInfo(resourceName, length, hashCode);
                    }
                }
                
                ResourceInitComplete();
            }
            catch (Exception exception)
            {

                throw new Exception(TextUtil.Format("Parse package list exception '{0}'.", exception.Message), exception);
            }
        }

        private void ProcessResourceInfo(string resourceName, int length, int hashCode)
        {
            if (m_ResourceManager.ResourceInfos.ContainsKey(resourceName))
            {
                throw new Exception(TextUtil.Format("Resource info '{0}' is already exist.", resourceName));
            }

            m_ResourceManager.ResourceInfos.Add(resourceName, new ResourceInfo(resourceName, length, hashCode, true));
            m_ResourceManager.ResourceNames.Add(resourceName);
        }
    }
}