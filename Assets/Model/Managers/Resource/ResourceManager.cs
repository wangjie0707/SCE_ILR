//===================================================
//
//===================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResourceManager : ManagerBase, IDisposable
    {
        /// <summary>
        /// 依赖配置
        /// </summary>
        private AssetBundleManifest m_AssetBundleManifest;

        /// <summary>
        /// Asset资源名称
        /// </summary>
        private HashSet<string> s_AssetNames;

        /// <summary>
        /// AssetBundle资源名称
        /// </summary>
        private HashSet<string> s_ResourceNames;

        private Dictionary<string, string> m_AssetInfos;
        private Dictionary<string, ResourceInfo> m_ResourceInfos;
        private SortedDictionary<string, ReadWriteResourceInfo> m_ReadWriteResourceInfos;
        private string m_UpdatePrefixUri;
        private ResourceMode m_ResourceMode;

        /// <summary>
        /// 任务池
        /// </summary>
        private readonly TaskPool<ResourceLoaderTask> m_TaskPool;

        /// <summary>
        /// 资源池
        /// </summary>
        private readonly AssetPool m_AssetPool;

        /// <summary>
        /// 资源辅助器
        /// </summary>
        private ResourceHelper m_ResourceHelper;

        /// <summary>
        /// 本地文件路径
        /// </summary>
        private string m_ReadWritePath;

        /// <summary>
        /// 本地只读路径
        /// </summary>
        private string m_ReadOnlyPath;

        private ResourceIniter m_ResourceIniter;
        private VersionListProcessor m_VersionListProcessor;
        private ResourceChecker m_ResourceChecker;
        private ResourceUpdater m_ResourceUpdater;
        private PreLoadShaderCallBack m_PreLoadShaderCallBack;

        public InitResourcesCompleteCallback InitResourcesCompleteCallback;
        public UpdateVersionListCallbacks m_UpdateVersionListCallbacks;
        public CheckResourcesCompleteCallback CheckResourcesCompleteCallback;
        public UpdateResourcesCompleteCallback UpdateResourcesCompleteCallback;
        public LoadManifestCompleteCallBack LoadManifestCompleteCallBack;

        public ResourceUpdateStartEvent ResourceUpdateStartEvent;
        public ResourceUpdateChangedEvent ResourceUpdateChangedEvent;
        public ResourceUpdateSuccessEvent ResourceUpdateSuccessEvent;
        public ResourceUpdateFailureEvent ResourceUpdateFailureEvent;

        public ResourceManager()
        {
            m_ResourceMode = ResourceMode.Unspecified;
            m_TaskPool = new TaskPool<ResourceLoaderTask>();
            s_ResourceNames = new HashSet<string>();
            s_AssetNames = new HashSet<string>();
            m_ReadWriteResourceInfos = new SortedDictionary<string, ReadWriteResourceInfo>();
            m_AssetPool = new AssetPool();

            ResourceUpdateStartEvent = null;
            ResourceUpdateChangedEvent = null;
            ResourceUpdateSuccessEvent = null;
            ResourceUpdateFailureEvent = null;
        }


        /// <summary>
        /// 获取或者设置本地读写路径
        /// </summary>
        public string ReadWritePath
        {
            get
            {
                return m_ReadWritePath;
            }
            set
            {
                m_ReadWritePath = value;
            }
        }

        /// <summary>
        /// 获取或者设置本地只读路径
        /// </summary>
        public string ReadOnlyPath
        {
            get
            {
                return m_ReadOnlyPath;
            }
            set
            {
                m_ReadOnlyPath = value;
            }
        }

        /// <summary>
        /// 获取或设置资源更新下载地址前缀
        /// </summary>
        public string UpdatePrefixUri
        {
            get
            {
                return m_UpdatePrefixUri;
            }
            set
            {
                m_UpdatePrefixUri = value;
            }
        }

        /// <summary>
        /// 获取资源辅助器
        /// </summary>
        public ResourceHelper ResourceHelper
        {
            get
            {
                return m_ResourceHelper;
            }
        }

        /// <summary>
        /// 获取或者设置所有Asset资源信息
        /// </summary>
        public Dictionary<string, string> AssetsInfos
        {
            get
            {
                return m_AssetInfos;
            }
            set
            {
                m_AssetInfos = value;
            }
        }

        /// <summary>
        /// 获取或者设置所有AssetBundle资源信息
        /// </summary>
        public Dictionary<string, ResourceInfo> ResourceInfos
        {
            get
            {
                return m_ResourceInfos;
            }
            set
            {
                m_ResourceInfos = value;
            }
        }

        /// <summary>
        /// 获取所有Asset资源信息
        /// </summary>
        public HashSet<string> AssetsNames
        {
            get
            {
                return s_AssetNames;
            }
        }

        /// <summary>
        /// 获取所有AssetBundle资源信息
        /// </summary>
        public HashSet<string> ResourceNames
        {
            get
            {
                return s_ResourceNames;
            }
        }

        /// <summary>
        /// 获取所有资源信息
        /// </summary>
        public SortedDictionary<string, ReadWriteResourceInfo> ReadWriteResourceInfos
        {
            get
            {
                return m_ReadWriteResourceInfos;
            }
        }

        /// <summary>
        /// 获取加载资源加载器总数量
        /// </summary>
        public int TotalLoaderCount
        {
            get
            {
                return m_TaskPool.TotalTaskCount;
            }
        }

        /// <summary>
        /// 获取或设置每下载多少字节的资源，刷新一次资源列表
        /// </summary>
        public int GenerateReadWriteListLength
        {
            get
            {
                return m_ResourceUpdater != null ? m_ResourceUpdater.GenerateReadWriteListLength : 0;
            }
            set
            {
                if (m_ResourceUpdater == null)
                {
                    throw new Exception("You can not use GenerateReadWriteListLength at this time.");
                }

                m_ResourceUpdater.GenerateReadWriteListLength = value;
            }
        }

        /// <summary>
        /// 获取或设置资源更新重试次数
        /// </summary>
        public int UpdateRetryCount
        {
            get
            {
                return m_ResourceUpdater != null ? m_ResourceUpdater.RetryCount : 0;
            }
            set
            {
                if (m_ResourceUpdater == null)
                {
                    throw new Exception("You can not use UpdateRetryCount at this time.");
                }

                m_ResourceUpdater.RetryCount = value;
            }
        }

        /// <summary>
        /// 获取等待更新资源数量
        /// </summary>
        public int UpdateWaitingCount
        {
            get
            {
                return m_ResourceUpdater != null ? m_ResourceUpdater.UpdateWaitingCount : 0;
            }
        }

        /// <summary>
        /// 获取正在更新资源数量
        /// </summary>
        public int UpdatingCount
        {
            get
            {
                return m_ResourceUpdater != null ? m_ResourceUpdater.UpdatingCount : 0;
            }
        }

        /// <summary>
        /// 获取加载资源代理总数量
        /// </summary>
        public int LoadTotalAgentCount
        {
            get
            {
                return m_TaskPool.TotalTaskCount;
            }
        }

        /// <summary>
        /// 获取可用加载资源代理数量
        /// </summary>
        public int FreeLoaderCount
        {
            get
            {
                return m_TaskPool.FreeAgentCount;
            }
        }

        /// <summary>
        /// 获取工作中加载资源代理数量
        /// </summary>
        public int WorkingLoaderCount
        {
            get
            {
                return m_TaskPool.WorkingAgentCount;
            }
        }

        /// <summary>
        /// 获取等待加载资源任务数量
        /// </summary>
        public int LoadWaitingTaskCount
        {
            get
            {
                return m_TaskPool.WaitingTaskCount;
            }
        }

        /// <summary>
        /// 获取已准备完毕Asset资源数量
        /// </summary>
        public int AssetCount
        {
            get
            {
                return m_AssetInfos != null ? m_AssetInfos.Count : 0;
            }
        }

        /// <summary>
        /// 获取已准备完毕AssetBundle资源数量
        /// </summary>
        public int ResourceCount
        {
            get
            {
                return m_ResourceInfos != null ? m_ResourceInfos.Count : 0;
            }
        }


        /// <summary>
        /// 获取资源池
        /// </summary>
        public AssetPool AssetPool
        {
            get
            {
                return m_AssetPool;
            }
        }

        /// <summary>
        /// 获取资源模式
        /// </summary>
        public ResourceMode ResourceMode
        {
            get
            {
                return m_ResourceMode;
            }
        }



        /// <summary>
        /// 设置资源辅助器
        /// </summary>
        /// <param name="resourceHelper"></param>
        public void SetResourceHelper(ResourceHelper resourceHelper)
        {
            if (resourceHelper == null)
            {
                throw new Exception("ResourceHelper is invalid");
            }
            m_ResourceHelper = resourceHelper;
        }

        /// <summary>
        /// 设置资源模式
        /// </summary>
        /// <param name="resourceMode">资源模式</param>
        public void SetResourceMode(ResourceMode resourceMode)
        {
            if (resourceMode == ResourceMode.Unspecified)
            {
                throw new Exception("Resource mode is invalid.");
            }

            if (m_ResourceMode == ResourceMode.Unspecified)
            {
                m_ResourceMode = resourceMode;

                if (m_ResourceMode == ResourceMode.Package)
                {
                    m_ResourceIniter = new ResourceIniter(this);
                    m_ResourceIniter.ResourceInitComplete += OnIniterResourceInitComplete;
                }
                else if (m_ResourceMode == ResourceMode.Updatable)
                {
                    m_VersionListProcessor = new VersionListProcessor(this);
                    m_VersionListProcessor.VersionListUpdateSuccess += OnVersionListProcessorUpdateSuccess;
                    m_VersionListProcessor.VersionListUpdateFailure += OnVersionListProcessorUpdateFailure;

                    m_ResourceChecker = new ResourceChecker(this);
                    m_ResourceChecker.ResourceNeedUpdate += OnCheckerResourceNeedUpdate;
                    m_ResourceChecker.ResourceCheckComplete += OnCheckerResourceCheckComplete;

                    m_ResourceUpdater = new ResourceUpdater(this);
                    m_ResourceUpdater.ResourceUpdateStart += OnUpdaterResourceUpdateStart;
                    m_ResourceUpdater.ResourceUpdateChanged += OnUpdaterResourceUpdateChanged;
                    m_ResourceUpdater.ResourceUpdateSuccess += OnUpdaterResourceUpdateSuccess;
                    m_ResourceUpdater.ResourceUpdateFailure += OnUpdaterResourceUpdateFailure;
                    m_ResourceUpdater.ResourceUpdateAllComplete += OnUpdaterResourceUpdateAllComplete;
                }
            }
            else if (m_ResourceMode != resourceMode)
            {
                throw new Exception("You can not change resource mode at this time.");
            }
        }

        /// <summary>
        /// 设置下载管理器
        /// </summary>
        /// <param name="downloadManager">下载管理器</param>
        public void SetDownloadManager()
        {
            if (m_VersionListProcessor != null)
            {
                m_VersionListProcessor.SetDownloadManager();
            }

            if (m_ResourceUpdater != null)
            {
                m_ResourceUpdater.SetDownloadManager();
            }
        }

        /// <summary>
        /// 异步加载主依赖项
        /// </summary>
        internal void LoadManifestAsync(LoadManifestCompleteCallBack loadManifestCompleteCallBack)
        {
            LoadManifestCompleteCallBack = loadManifestCompleteCallBack;
            string assetName = string.Empty;
#if UNITY_STANDALONE_WIN
            assetName = "windows";
#elif UNITY_ANDROID
            assetName = "android";
#elif UNITY_IPHONE
            assetName = "ios";
#endif
            ResourceInfo resourceInfo = GetResourceInfo(assetName);
            m_ResourceHelper.LoadBytes(PathUtil.GetRemotePath(resourceInfo.StorageInReadOnly ? GameEntry.Resource.ReadOnlyPath : GameEntry.Resource.ReadWritePath, PathUtil.GetResourceNameWithSuffix(assetName)), OnLoadManifestCallBack);
        }

        private void OnLoadManifestCallBack(string fileUri, byte[] bytes, string errorMessage)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                throw new Exception(TextUtil.Format("Manifest '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage));
            }
            bytes = SecurityUtil.Xor(bytes);
            m_ResourceHelper.Parse(bytes, OnLoadManifestCallBack);
        }

        private void OnLoadManifestCallBack(AssetBundleManifest assetBundleManifest)
        {
            m_AssetBundleManifest = assetBundleManifest;
            if (LoadManifestCompleteCallBack != null)
            {
                LoadManifestCompleteCallBack();
            }
        }

        /// <summary>
        /// 读取本地文件到byte数组
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        internal byte[] GetFileBuffer(string path)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
            }

            return buffer;
        }

        internal void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            m_TaskPool.OnUpdate(deltaTime, unscaledDeltaTime);
            m_AssetPool.OnUpdate(deltaTime, unscaledDeltaTime);

            if (m_ResourceUpdater != null)
            {
                m_ResourceUpdater.OnUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        /// <summary>
        /// 得到资源信息
        /// </summary>
        /// <param name="assetPath">路径名称</param>
        /// <returns></returns>
        public ResourceInfo GetResourceInfo(string assetPath)
        {
            ResourceInfo resourceInfo = null;
            m_ResourceInfos.TryGetValue(assetPath, out resourceInfo);
            return resourceInfo;
        }

        /// <summary>
        /// 使用单机模式并初始化资源
        /// </summary>
        /// <param name="initResourcesCompleteCallback">使用单机模式并初始化资源完成的回调函数</param>
        public void InitResources(InitResourcesCompleteCallback initResourcesCompleteCallback)
        {
            if (initResourcesCompleteCallback == null)
            {
                throw new Exception("Init resources complete callback is invalid.");
            }

            if (m_ResourceMode == ResourceMode.Unspecified)
            {
                throw new Exception("You must set resource mode first.");
            }

            if (m_ResourceMode != ResourceMode.Package)
            {
                throw new Exception("You can not use InitResources without package resource mode.");
            }

            if (m_ResourceIniter == null)
            {
                throw new Exception("You can not use InitResources at this time.");
            }

            InitResourcesCompleteCallback = initResourcesCompleteCallback;
            m_ResourceIniter.InitResources();
        }

        /// <summary>
        /// 使用可更新模式并更新版本资源列表
        /// </summary>
        /// <param name="versionListLength">版本资源列表大小</param>
        /// <param name="versionListHashCode">版本资源列表哈希值</param>
        /// <param name="versionListZipLength">版本资源列表压缩后大小</param>
        /// <param name="versionListZipHashCode">版本资源列表压缩后哈希值</param>
        /// <param name="updateVersionListCallbacks">版本资源列表更新回调函数集</param>
        public void UpdateVersionList(int versionListLength, int versionListHashCode, int versionListZipLength, int versionListZipHashCode, UpdateVersionListCallbacks updateVersionListCallbacks)
        {
            if (updateVersionListCallbacks == null)
            {
                throw new Exception("Update version list callbacks is invalid.");
            }

            if (m_ResourceMode == ResourceMode.Unspecified)
            {
                throw new Exception("You must set resource mode first.");
            }

            if (m_ResourceMode != ResourceMode.Updatable)
            {
                throw new Exception("You can not use UpdateVersionList without updatable resource mode.");
            }

            if (m_VersionListProcessor == null)
            {
                throw new Exception("You can not use UpdateVersionList at this time.");
            }

            m_UpdateVersionListCallbacks = updateVersionListCallbacks;
            m_VersionListProcessor.UpdateVersionList(versionListLength, versionListHashCode, versionListZipLength, versionListZipHashCode);
        }

        /// <summary>
        /// 使用可更新模式并检查资源
        /// </summary>
        /// <param name="checkResourcesCompleteCallback">使用可更新模式并检查资源完成的回调函数</param>
        public void CheckResources(CheckResourcesCompleteCallback checkResourcesCompleteCallback)
        {
            if (checkResourcesCompleteCallback == null)
            {
                throw new Exception("Check resources complete callback is invalid.");
            }

            if (m_ResourceMode == ResourceMode.Unspecified)
            {
                throw new Exception("You must set resource mode first.");
            }

            if (m_ResourceMode != ResourceMode.Updatable)
            {
                throw new Exception("You can not use CheckResources without updatable resource mode.");
            }

            if (m_ResourceChecker == null)
            {
                throw new Exception("You can not use CheckResources at this time.");
            }

            CheckResourcesCompleteCallback = checkResourcesCompleteCallback;
            m_ResourceChecker.CheckResources();
        }

        /// <summary>
        /// 使用可更新模式并更新资源
        /// </summary>
        /// <param name="updateResourcesCompleteCallback">使用可更新模式并更新资源全部完成的回调函数</param>
        public void UpdateResources(UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
        {
            if (updateResourcesCompleteCallback == null)
            {
                throw new Exception("Update resources complete callback is invalid.");
            }

            if (m_ResourceMode == ResourceMode.Unspecified)
            {
                throw new Exception("You must set resource mode first.");
            }

            if (m_ResourceMode != ResourceMode.Updatable)
            {
                throw new Exception("You can not use UpdateResources without updatable resource mode.");
            }

            if (m_ResourceUpdater == null)
            {
                throw new Exception("You can not use UpdateResources at this time.");
            }

            UpdateResourcesCompleteCallback = updateResourcesCompleteCallback;
            m_ResourceUpdater.UpdateResources();
        }

        /// <summary>
        /// 检查Asset资源是否存在
        /// </summary>
        /// <param name="assetName">要检查资源的名称</param>
        /// <returns>资源是否存在</returns>
        public bool HasAsset(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new Exception("Asset name is invalid.");
            }

            return s_AssetNames.Contains(assetName);
        }

        /// <summary>
        /// 检查AssetBundle资源是否存在
        /// </summary>
        /// <param name="assetBundleName">要检查AssetBundle资源的名称</param>
        /// <returns>资源是否存在</returns>
        public bool HasAssetBundle(string assetBundleName)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                throw new Exception("Asset name is invalid.");
            }

            return s_ResourceNames.Contains(assetBundleName);
        }

        /// <summary>
        /// 预加载Shader
        /// </summary>
        /// <param name="preLoadShaderCallBack">预加载shader回调函数集</param>
        public void PreLoadShader(PreLoadShaderCallBack preLoadShaderCallBack)
        {
            if (m_ResourceHelper == null)
            {
                throw new Exception("Please set resourceHelper first!");
            }

            m_PreLoadShaderCallBack = preLoadShaderCallBack;
            ResourceInfo resourceInfo = GameEntry.Resource.GetResourceInfo("shader");
            m_ResourceHelper.LoadBytes(PathUtil.GetRemotePath(resourceInfo.StorageInReadOnly ? GameEntry.Resource.ReadOnlyPath : GameEntry.Resource.ReadWritePath, PathUtil.GetResourceNameWithSuffix(resourceInfo.ResourceName)), OnLoadShdaerBytesComplete);
        }

        private void OnLoadShdaerBytesComplete(string fileUri, byte[] bytes, string errorMessage)
        {
            if (bytes == null || bytes.Length <= 0)
            {
                string errormessage = TextUtil.Format("shdaer '{0}' is invalid, error message is '{1}'.", fileUri, string.IsNullOrEmpty(errorMessage) ? "<Empty>" : errorMessage);
                if (m_PreLoadShaderCallBack.PreLoadShaderFailure != null)
                {
                    m_PreLoadShaderCallBack.PreLoadShaderFailure(errormessage);
                }

                throw new Exception(errormessage);
            }
            bytes = SecurityUtil.Xor(bytes);
            m_ResourceHelper.PreLoadShader(bytes, m_PreLoadShaderCallBack);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">要加载资源的名称</param>
        /// <param name="assetType">要加载资源的类型</param>
        /// <param name="priority">加载资源的优先级</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            //依赖项名称
            if (m_AssetBundleManifest == null)
            {
                throw new Exception("Please set manifest first!");
            }

            if (!HasAsset(assetName))
            {
                string errorMessage = string.Format("Can not load asset '{0}'.", assetName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, errorMessage, userData);
                    return;
                }

                throw new Exception(errorMessage);
            }

            string assetBundleName = null;
            if (!m_AssetInfos.TryGetValue(assetName, out assetBundleName))
            {
                string errorMessage = TextUtil.Format("Can not load asset '{0}'.", assetName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetName, errorMessage, userData);
                    return;
                }

                throw new Exception(errorMessage);
            }
           

            if (!CheckAsset(assetBundleName))
            {
                string errorMessage = string.Format("Can not load assetbundle '{0}'.", assetBundleName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(assetBundleName, errorMessage, userData);
                    return;
                }

                throw new Exception(errorMessage);
            }

            string[] dependencyAssetNames = m_AssetBundleManifest.GetAllDependencies(assetBundleName);

            ResourceLoaderTask mainTask = new ResourceLoaderTask(assetName, assetBundleName, assetType, priority, dependencyAssetNames, false, null, loadAssetCallbacks, userData);

            foreach (string dependencyAssetName in dependencyAssetNames)
            {
                if (!LoadDependencyAsset(assetBundleName, dependencyAssetName, priority, mainTask, loadAssetCallbacks, userData))
                {
                    string errorMessage = string.Format("Can not load dependency asset '{0}' when load asset '{1}'.", dependencyAssetName, assetBundleName);
                    if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                    {
                        loadAssetCallbacks.LoadAssetFailureCallback(assetBundleName, errorMessage, userData);
                        return;
                    }

                    throw new Exception(errorMessage);
                }
            }

            m_TaskPool.AddTask(mainTask);
        }


        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称</param>
        /// <param name="priority">加载场景资源的优先级</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        internal void LoadScene(string sceneAssetName, int priority, LoadAssetCallbacks loadSceneCallbacks, object userData)
        {
            string assetBundleName = null;
            if (!m_AssetInfos.TryGetValue(sceneAssetName, out assetBundleName))
            {
                string errorMessage = TextUtil.Format("Can not load scene '{0}'.", sceneAssetName);
                if (loadSceneCallbacks.LoadAssetFailureCallback != null)
                {
                    loadSceneCallbacks.LoadAssetFailureCallback(sceneAssetName, errorMessage, userData);
                    return;
                }

                throw new Exception(errorMessage);
            }
            
            //依赖项名称
            string[] dependencyAssetNames = m_AssetBundleManifest.GetAllDependencies(assetBundleName);

            if (!CheckAsset(assetBundleName))
            {
                string errorMessage = string.Format("Can not load scene '{0}'.", sceneAssetName);
                if (loadSceneCallbacks.LoadAssetFailureCallback != null)
                {
                    loadSceneCallbacks.LoadAssetFailureCallback(sceneAssetName, errorMessage, userData);
                    return;
                }

                throw new Exception(errorMessage);
            }

            ResourceLoaderTask mainTask = new ResourceLoaderTask(sceneAssetName, assetBundleName, null, priority, dependencyAssetNames, true, null, loadSceneCallbacks, userData);

            foreach (string dependencyAssetName in dependencyAssetNames)
            {
                if (!LoadDependencyAsset(assetBundleName, dependencyAssetName, priority, mainTask, loadSceneCallbacks, userData))
                {
                    string errorMessage = string.Format("Can not load dependency asset '{0}' when load scene '{1}'.", dependencyAssetName, sceneAssetName);
                    if (loadSceneCallbacks.LoadAssetFailureCallback != null)
                    {
                        loadSceneCallbacks.LoadAssetFailureCallback(sceneAssetName, errorMessage, userData);
                        return;
                    }

                    throw new Exception(errorMessage);
                }
            }

            m_TaskPool.AddTask(mainTask);
        }

        /// <summary>
        /// 检查资源是否存在
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        private bool CheckAsset(string assetName)
        {
            if (s_ResourceNames.Contains(assetName))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 加载依赖资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <param name="priority">加载优先级</param>
        /// <param name="mainTask">主资源任务</param>
        /// <param name="userData">用户数据</param>
        /// <returns></returns>
        private bool LoadDependencyAsset(string assetName, string dependencyassetname, int priority, ResourceLoaderTask mainTask, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            if (!CheckAsset(dependencyassetname))
            {
                string errorMessage = string.Format("Can not load DependencyAsset '{0}' when load '{1}'.", dependencyassetname, assetName);
                if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    loadAssetCallbacks.LoadAssetFailureCallback(dependencyassetname, errorMessage, userData);
                    return false;
                }
                Debug.LogError(errorMessage);
                return false;
            }

            ResourceLoaderTask dependencyTask = new ResourceLoaderTask(null, dependencyassetname, null, priority, null, false, mainTask, loadAssetCallbacks, userData);

            m_TaskPool.AddTask(dependencyTask);
            return true;
        }

        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            if (m_ResourceHelper == null)
            {
                throw new Exception("You must set resource helper first.");
            }

            m_ResourceHelper.UnloadScene(sceneAssetName, unloadSceneCallbacks, userData);
        }


        /// <summary>
        /// 增加资源加载器
        /// </summary>
        /// <param name="resourceLoader">资源加载器</param>
        internal void AddResourceLoader(ResourceLoader resourceLoader)
        {
            m_TaskPool.AddAgent(resourceLoader);
        }



        public override void Dispose()
        {
            if (m_ResourceIniter != null)
            {
                m_ResourceIniter.Shutdown();
                m_ResourceIniter = null;
            }

            if (m_VersionListProcessor != null)
            {
                m_VersionListProcessor.VersionListUpdateSuccess -= OnVersionListProcessorUpdateSuccess;
                m_VersionListProcessor.VersionListUpdateFailure -= OnVersionListProcessorUpdateFailure;
                m_VersionListProcessor.Shutdown();
                m_VersionListProcessor = null;
            }

            if (m_ResourceChecker != null)
            {
                m_ResourceChecker.ResourceNeedUpdate -= OnCheckerResourceNeedUpdate;
                m_ResourceChecker.ResourceCheckComplete -= OnCheckerResourceCheckComplete;
                m_ResourceChecker.Shutdown();
                m_ResourceChecker = null;
            }

            if (m_ResourceUpdater != null)
            {
                m_ResourceUpdater.ResourceUpdateStart -= OnUpdaterResourceUpdateStart;
                m_ResourceUpdater.ResourceUpdateChanged -= OnUpdaterResourceUpdateChanged;
                m_ResourceUpdater.ResourceUpdateSuccess -= OnUpdaterResourceUpdateSuccess;
                m_ResourceUpdater.ResourceUpdateFailure -= OnUpdaterResourceUpdateFailure;
                m_ResourceUpdater.ResourceUpdateAllComplete -= OnUpdaterResourceUpdateAllComplete;
                m_ResourceUpdater.Shutdown();
                m_ResourceUpdater = null;
            }

            m_AssetBundleManifest = null;
            if (s_ResourceNames != null)
            {
                s_ResourceNames.Clear();
                s_ResourceNames = null;
            }

            if (m_AssetInfos != null)
            {
                m_AssetInfos.Clear();
                m_AssetInfos = null;
            }

            if (m_ResourceInfos != null)
            {
                m_ResourceInfos.Clear();
                m_ResourceInfos = null;
            }

            m_ReadWriteResourceInfos.Clear();

            m_TaskPool.Shutdown();
            m_AssetPool.Shutdown();
        }

        private void OnIniterResourceInitComplete()
        {
            m_ResourceIniter.ResourceInitComplete -= OnIniterResourceInitComplete;
            m_ResourceIniter.Shutdown();
            m_ResourceIniter = null;

            InitResourcesCompleteCallback();
            InitResourcesCompleteCallback = null;
        }

        private void OnVersionListProcessorUpdateSuccess(string downloadPath, string downloadUri)
        {
            m_UpdateVersionListCallbacks.UpdateVersionListSuccessCallback(downloadPath, downloadUri);
        }

        private void OnVersionListProcessorUpdateFailure(string downloadUri, string errorMessage)
        {
            if (m_UpdateVersionListCallbacks.UpdateVersionListFailureCallback != null)
            {
                m_UpdateVersionListCallbacks.UpdateVersionListFailureCallback(downloadUri, errorMessage);
            }
        }

        private void OnCheckerResourceNeedUpdate(string resourceName, int length, int hashCode, int zipLength, int zipHashCode)
        {
            m_ResourceUpdater.AddResourceUpdate(resourceName, length, hashCode, zipLength, zipHashCode, PathUtil.GetCombinePath(m_ReadWritePath, PathUtil.GetResourceNameWithSuffix(resourceName)), PathUtil.GetRemotePath(m_UpdatePrefixUri, PathUtil.GetResourceNameWithCrc32AndSuffix(resourceName, hashCode)), 0);
        }

        private void OnCheckerResourceCheckComplete(int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
        {
            m_VersionListProcessor.VersionListUpdateSuccess -= OnVersionListProcessorUpdateSuccess;
            m_VersionListProcessor.VersionListUpdateFailure -= OnVersionListProcessorUpdateFailure;
            m_VersionListProcessor.Shutdown();
            m_VersionListProcessor = null;
            m_UpdateVersionListCallbacks = null;

            m_ResourceChecker.ResourceNeedUpdate -= OnCheckerResourceNeedUpdate;
            m_ResourceChecker.ResourceCheckComplete -= OnCheckerResourceCheckComplete;
            m_ResourceChecker.Shutdown();
            m_ResourceChecker = null;

            m_ResourceUpdater.CheckResourceComplete(removedCount > 0);

            if (updateCount <= 0)
            {
                m_ResourceUpdater.ResourceUpdateStart -= OnUpdaterResourceUpdateStart;
                m_ResourceUpdater.ResourceUpdateChanged -= OnUpdaterResourceUpdateChanged;
                m_ResourceUpdater.ResourceUpdateSuccess -= OnUpdaterResourceUpdateSuccess;
                m_ResourceUpdater.ResourceUpdateFailure -= OnUpdaterResourceUpdateFailure;
                m_ResourceUpdater.ResourceUpdateAllComplete -= OnUpdaterResourceUpdateAllComplete;
                m_ResourceUpdater.Shutdown();
                m_ResourceUpdater = null;
            }

            CheckResourcesCompleteCallback(updateCount > 0, removedCount, updateCount, updateTotalLength, updateTotalZipLength);
            CheckResourcesCompleteCallback = null;
        }

        private void OnUpdaterResourceUpdateStart(string resourceName, string downloadPath, string downloadUri, int currentLength, int zipLength, int retryCount)
        {
            if (ResourceUpdateStartEvent != null)
            {
                ResourceUpdateStartEvent(resourceName, downloadPath, downloadUri, currentLength, zipLength, retryCount);
            }
        }

        private void OnUpdaterResourceUpdateChanged(string resourceName, string downloadPath, string downloadUri, int currentLength, int zipLength)
        {
            if (ResourceUpdateChangedEvent != null)
            {
                ResourceUpdateChangedEvent(resourceName, downloadPath, downloadUri, currentLength, zipLength);
            }
        }

        private void OnUpdaterResourceUpdateSuccess(string resourceName, string downloadPath, string downloadUri, int length, int zipLength)
        {
            if (ResourceUpdateSuccessEvent != null)
            {
                ResourceUpdateSuccessEvent(resourceName, downloadPath, downloadUri, length, zipLength);
            }
        }

        private void OnUpdaterResourceUpdateFailure(string resourceName, string downloadUri, int retryCount, int totalRetryCount, string errorMessage)
        {
            if (ResourceUpdateFailureEvent != null)
            {
                ResourceUpdateFailureEvent(resourceName, downloadUri, retryCount, totalRetryCount, errorMessage);
            }
        }

        private void OnUpdaterResourceUpdateAllComplete()
        {
            m_ResourceUpdater.ResourceUpdateStart -= OnUpdaterResourceUpdateStart;
            m_ResourceUpdater.ResourceUpdateChanged -= OnUpdaterResourceUpdateChanged;
            m_ResourceUpdater.ResourceUpdateSuccess -= OnUpdaterResourceUpdateSuccess;
            m_ResourceUpdater.ResourceUpdateFailure -= OnUpdaterResourceUpdateFailure;
            m_ResourceUpdater.ResourceUpdateAllComplete -= OnUpdaterResourceUpdateAllComplete;
            m_ResourceUpdater.Shutdown();
            m_ResourceUpdater = null;

            UpdateResourcesCompleteCallback();
            UpdateResourcesCompleteCallback = null;
        }
    }
}
