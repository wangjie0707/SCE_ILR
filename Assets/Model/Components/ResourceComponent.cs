using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Myth
{
    /// <summary>
    /// 资源组件
    /// </summary>
    public class ResourceComponent : GameBaseComponent
    {

#if UNITY_EDITOR
        /// <summary>
        /// 编辑器加载场景
        /// </summary>
        private LinkedList<LoadSceneInfo> m_LoadSceneInfos = null;
#endif

        #region 卸载资源
        /// <summary>
        /// 卸载资源
        /// </summary>
        private AsyncOperation m_AsyncOperation = null;

        /// <summary>
        /// 预定执行卸载资源
        /// </summary>
        private bool m_PreorderUnloadUnusedAssets = false;

        /// <summary>
        /// 是否强制执行卸载资源
        /// </summary>
        private bool m_ForceUnloadUnusedAssets = false;

        /// <summary>
        /// 是否强制执行GC
        /// </summary>
        private bool m_PerformGCCollect = false;

        /// <summary>
        /// 最后释放时间
        /// </summary>
        private float m_LastOperationElapse = 0f;

        [SerializeField]
        private float m_UnloadUnusedAssetsInterval = 60f;

        [SerializeField]
        private float m_ResourceAutoReleaseInterval = 60f;

        [SerializeField]
        private float m_ResourceExpireTime = 60f;

        #endregion

        [SerializeField]
        private bool m_EditorResourceMode = true;

        /// <summary>
        /// 资源方式
        /// </summary>
        [SerializeField]
        private ResourceMode m_ResourceMode = ResourceMode.Package;

        /// <summary>
        /// 下载地址
        /// </summary>
        [SerializeField]
        private string m_UpdatePrefixUri = null;

        /// <summary>
        /// 资源加载器数量
        /// </summary>
        [SerializeField]
        private int m_ResourceLoaderCount = 3;

        /// <summary>
        /// 下载多少文件生成一次资源列表
        /// </summary>
        [SerializeField]
        private int m_GenerateReadWriteListLength = 1024 * 1024;

        /// <summary>
        /// 资源重试次数
        /// </summary>
        [SerializeField]
        private int m_UpdateRetryCount = 3;

        /// <summary>
        /// 获取或设置是否使用编辑器资源模式（仅编辑器内有效）
        /// </summary>
        public bool EditorResourceMode
        {
            get
            {
                return m_EditorResourceMode;
            }
            set
            {
                m_EditorResourceMode = value;
            }
        }

        /// <summary>
        /// 获取本地文件路径
        /// </summary>
        public string ReadWritePath
        {
            get
            {
                return m_ResourceManager.ReadWritePath;
            }
        }

        /// <summary>
        /// 获取本地只读路径
        /// </summary>
        public string ReadOnlyPath
        {
            get
            {
                return m_ResourceManager.ReadOnlyPath;
            }
        }

        /// <summary>
        /// 获取或设置资源更新下载地址
        /// </summary>
        public string UpdatePrefixUri
        {
            get
            {
                return m_ResourceManager.UpdatePrefixUri;
            }
            set
            {
                m_ResourceManager.UpdatePrefixUri = m_UpdatePrefixUri = value;
            }
        }

        /// <summary>
        /// 资源管理器
        /// </summary>
        private ResourceManager m_ResourceManager;

        /// <summary>
        /// 获取资源辅助器
        /// </summary>
        public ResourceHelper ResourceHelper
        {
            get
            {
                return m_ResourceManager.ResourceHelper;
            }
        }

        /// <summary>
        /// 获取或设置asset资源释放间隔
        /// </summary>
        public float UnloadUnusedAssetsInterval
        {
            get
            {
                return m_UnloadUnusedAssetsInterval;
            }
            set
            {
                m_UnloadUnusedAssetsInterval = value;
            }
        }

        /// <summary>
        /// 获取或设置assetbundle资源释放间隔
        /// </summary>
        public float ResourceAutoReleaseInterval
        {
            get
            {
                return AssetPool.AutoReleaseInterval;
            }
            set
            {
                AssetPool.AutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 获取或设置资源过期时间
        /// </summary>
        public float ResourceExpireTime
        {
            get
            {
                return AssetPool.ExpireTime;
            }
            set
            {
                AssetPool.ExpireTime = value;
            }
        }

        /// <summary>
        /// 获取已准备完毕Asset资源数量
        /// </summary>
        public int AssetCount
        {
            get
            {
                return m_ResourceManager.AssetCount;
            }
        }

        /// <summary>
        /// 获取已准备完毕AssetBundle资源数量
        /// </summary>
        public int ResourceCount
        {
            get
            {
                return m_ResourceManager.ResourceCount;
            }
        }

        /// <summary>
        /// 获取等待更新资源数量
        /// </summary>
        public int UpdateWaitingCount
        {
            get
            {
                return m_ResourceManager.UpdateWaitingCount;
            }
        }

        /// <summary>
        /// 获取正在更新资源数量
        /// </summary>
        public int UpdatingCount
        {
            get
            {
                return m_ResourceManager.UpdatingCount;
            }
        }

        /// <summary>
        /// 获取加载资源器总数量
        /// </summary>
        public int ResourceLoaderTotalCount
        {
            get
            {
                return m_ResourceManager.TotalLoaderCount;
            }
        }

        /// <summary>
        /// 获取可用加载资源器数量
        /// </summary>
        public int FreeLoaderCount
        {
            get
            {
                return m_ResourceManager.FreeLoaderCount;
            }
        }

        /// <summary>
        /// 获取等待加载资源任务数量
        /// </summary>
        public int LoadWaitingTaskCount
        {
            get
            {
                return m_ResourceManager.LoadWaitingTaskCount;
            }
        }

        /// <summary>
        /// 获取工作中加载资源器数量
        /// </summary>
        public int WorkingLoaderCount
        {
            get
            {
                return m_ResourceManager.WorkingLoaderCount;
            }
        }

        /// <summary>
        /// 资源池
        /// </summary>
        public AssetPool AssetPool
        {
            get
            {
                return m_ResourceManager.AssetPool;
            }
        }

        /// <summary>
        /// 获取资源模式
        /// </summary>
        public ResourceMode ResourceMode
        {
            get
            {
                return m_ResourceManager.ResourceMode;
            }
        }

        /// <summary>
        /// 获取或设置每下载多少字节的资源，刷新一次资源列表
        /// </summary>
        public int GenerateReadWriteListLength
        {
            get
            {
                return m_ResourceManager.GenerateReadWriteListLength;
            }
            set
            {
                m_ResourceManager.GenerateReadWriteListLength = m_GenerateReadWriteListLength = value;
            }
        }

        /// <summary>
        /// 获取或设置资源更新重试次数
        /// </summary>
        public int UpdateRetryCount
        {
            get
            {
                return m_ResourceManager.UpdateRetryCount;
            }
            set
            {
                m_ResourceManager.UpdateRetryCount = m_UpdateRetryCount = value;
            }
        }

        /// <summary>
        /// 获取加载资源代理总数量
        /// </summary>
        public int LoadTotalAgentCount
        {
            get
            {
                return m_ResourceManager.LoadTotalAgentCount;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();

            m_EditorResourceMode &= Application.isEditor;


            if (m_EditorResourceMode)
            {
#if UNITY_EDITOR
                m_LoadSceneInfos = new LinkedList<LoadSceneInfo>();
                Log.Info("During this run, Game Framework will use editor resource files, which you should validate first.");
#endif
            }


            m_ResourceManager = new ResourceManager();

            m_ResourceManager.ReadWritePath = Application.persistentDataPath;
            m_ResourceManager.ReadOnlyPath = Application.streamingAssetsPath;

            m_ResourceManager.AssetPool.AutoReleaseInterval = m_ResourceAutoReleaseInterval;
            m_ResourceManager.AssetPool.ExpireTime = m_ResourceExpireTime;
        }

        protected override void OnStart()
        {
            base.OnStart();

            m_ResourceManager.ResourceUpdateStartEvent += OnResourceUpdateStart;
            m_ResourceManager.ResourceUpdateChangedEvent += OnResourceUpdateChanged;
            m_ResourceManager.ResourceUpdateSuccessEvent += OnResourceUpdateSuccess;
            m_ResourceManager.ResourceUpdateFailureEvent += OnResourceUpdateFailure;

            AddResourceHelper();

            if (m_EditorResourceMode)
            {
                return;
            }

            SetResourceMode(m_ResourceMode);
            m_ResourceManager.SetDownloadManager();

            if (m_ResourceMode == ResourceMode.Updatable)
            {
                m_ResourceManager.UpdatePrefixUri = m_UpdatePrefixUri;
                m_ResourceManager.GenerateReadWriteListLength = m_GenerateReadWriteListLength;
                m_ResourceManager.UpdateRetryCount = m_UpdateRetryCount;
            }

            for (int i = 0; i < m_ResourceLoaderCount; i++)
            {
                AddLoadResourceLoaderHelper(i);
            }

            
        }


        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_ResourceManager.OnUpdate(deltaTime, unscaledDeltaTime);

            m_LastOperationElapse += unscaledDeltaTime;
            if (m_AsyncOperation == null && (m_ForceUnloadUnusedAssets || m_PreorderUnloadUnusedAssets && m_LastOperationElapse >= m_UnloadUnusedAssetsInterval))
            {
                Log.Info("Unload unused assets...");
                m_ForceUnloadUnusedAssets = false;
                m_PreorderUnloadUnusedAssets = false;
                m_LastOperationElapse = 0f;
                AssetPool.UnloadUnusedAssets();
                m_AsyncOperation = Resources.UnloadUnusedAssets();
            }

            if (m_AsyncOperation != null && m_AsyncOperation.isDone)
            {
                m_AsyncOperation = null;
                if (m_PerformGCCollect)
                {
                    Log.Info("GC.Collect...");
                    m_PerformGCCollect = false;
                    GC.Collect();
                }
            }

#if UNITY_EDITOR
            if (m_EditorResourceMode)
            {
                if (m_LoadSceneInfos.Count > 0)
                {
                    LinkedListNode<LoadSceneInfo> current = m_LoadSceneInfos.First;
                    while (current != null)
                    {
                        LoadSceneInfo loadSceneInfo = current.Value;
                        if (loadSceneInfo.AsyncOperation.isDone)
                        {
                            if (loadSceneInfo.AsyncOperation.allowSceneActivation)
                            {
                                if (loadSceneInfo.LoadSceneCallbacks.LoadAssetSuccessCallback != null)
                                {
                                    loadSceneInfo.LoadSceneCallbacks.LoadAssetSuccessCallback(loadSceneInfo.SceneAssetName, null, (float)(DateTime.Now - loadSceneInfo.StartTime).TotalSeconds, loadSceneInfo.UserData);
                                }
                            }
                            else
                            {
                                if (loadSceneInfo.LoadSceneCallbacks.LoadAssetFailureCallback != null)
                                {
                                    loadSceneInfo.LoadSceneCallbacks.LoadAssetFailureCallback(loadSceneInfo.SceneAssetName, "Can not load this scene from asset database.", loadSceneInfo.UserData);
                                }
                            }

                            LinkedListNode<LoadSceneInfo> next = current.Next;
                            m_LoadSceneInfos.Remove(loadSceneInfo);
                            current = next;
                        }
                        else
                        {
                            if (loadSceneInfo.LoadSceneCallbacks.LoadAssetUpdateCallback != null)
                            {
                                loadSceneInfo.LoadSceneCallbacks.LoadAssetUpdateCallback(loadSceneInfo.SceneAssetName, loadSceneInfo.AsyncOperation.progress, loadSceneInfo.UserData);
                            }

                            current = current.Next;
                        }
                    }
                }
            }
#endif
        }

        /// <summary>
        /// 增加资源辅助器
        /// </summary>
        private void AddResourceHelper()
        {
            ResourceHelper resourceHelper = (new GameObject("ResourceHelper - {0}")).AddComponent<ResourceHelper>();
            resourceHelper.transform.SetParent(this.transform);
            resourceHelper.transform.localScale = Vector3.one;
            m_ResourceManager.SetResourceHelper(resourceHelper);
        }


        /// <summary>
        /// 增加加载资源器
        /// </summary>
        /// <param name="index">加载资源代理辅助器索引</param>
        private void AddLoadResourceLoaderHelper(int index)
        {
            ResourceLoader resourceLoader = (new GameObject(string.Format("ResourceLoader - {0}", index))).AddComponent<ResourceLoader>();
            resourceLoader.transform.SetParent(this.transform);
            resourceLoader.transform.localScale = Vector3.one;
            m_ResourceManager.AddResourceLoader(resourceLoader);
        }

        /// <summary>
        /// 设置资源模式
        /// </summary>
        /// <param name="resourceMode">资源模式</param>
        public void SetResourceMode(ResourceMode resourceMode)
        {
            m_ResourceManager.SetResourceMode(resourceMode);
        }


        /// <summary>
        /// 读取本地文件到byte数组
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public byte[] GetFileBuffer(string path)
        {
            return m_ResourceManager.GetFileBuffer(path);
        }


        /// <summary>
        /// 加载主依赖项
        /// </summary>
        public void LoadManifest(LoadManifestCompleteCallBack loadManifestCompleteCallBack)
        {
            m_ResourceManager.LoadManifestAsync(loadManifestCompleteCallBack);
        }

        /// <summary>
        /// 使用单机模式并初始化资源
        /// </summary>
        /// <param name="initResourcesCompleteCallback">使用单机模式并初始化资源完成的回调函数</param>
        public void InitResources(InitResourcesCompleteCallback initResourcesCompleteCallback)
        {
            m_ResourceManager.InitResources(initResourcesCompleteCallback);
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
            m_ResourceManager.UpdateVersionList(versionListLength, versionListHashCode, versionListZipLength, versionListZipHashCode, updateVersionListCallbacks);
        }

        /// <summary>
        /// 使用可更新模式并检查资源
        /// </summary>
        /// <param name="checkResourcesCompleteCallback">使用可更新模式并检查资源完成的回调函数</param>
        public void CheckResources(CheckResourcesCompleteCallback checkResourcesCompleteCallback)
        {
            m_ResourceManager.CheckResources(checkResourcesCompleteCallback);
        }

        /// <summary>
        /// 使用可更新模式并更新资源
        /// </summary>
        /// <param name="updateResourcesCompleteCallback">使用可更新模式并更新资源全部完成的回调函数</param>
        public void UpdateResources(UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
        {
            m_ResourceManager.UpdateResources(updateResourcesCompleteCallback);
        }

        /// <summary>
        /// 检查Asset资源是否存在
        /// </summary>
        /// <param name="assetName">要检查Asset资源的名称</param>
        /// <returns>资源是否存在</returns>
        public bool HasAsset(string assetName)
        {
            return m_ResourceManager.HasAsset(assetName);
        }

        /// <summary>
        /// 检查AssetBundle资源是否存在
        /// </summary>
        /// <param name="assetBundleName">要检查AssetBundle资源的名称</param>
        /// <returns>资源是否存在</returns>
        public bool HasAssetBundle(string assetBundleName)
        {
            return m_ResourceManager.HasAssetBundle(assetBundleName);
        }

        /// <summary>
        /// 预加载Shader
        /// </summary>
        /// <param name="preLoadShaderCallBack">预加载shader回调函数集</param>
        public void PreLoadShader(PreLoadShaderCallBack preLoadShaderCallBack)
        {
            m_ResourceManager.PreLoadShader(preLoadShaderCallBack);
        }

        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称</param>
        /// <param name="priority">加载场景资源的优先级</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadScene(string sceneAssetName, int priority, LoadAssetCallbacks loadSceneCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new Exception("Scene asset name is invalid.");
            }

            if (loadSceneCallbacks == null)
            {
                throw new Exception("Load scene callbacks is invalid.");
            }
#if UNITY_EDITOR
            if (m_EditorResourceMode)
            {
                AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneAssetName, LoadSceneMode.Additive);

                if (asyncOperation == null)
                {
                    return;
                }

                m_LoadSceneInfos.AddLast(new LoadSceneInfo(asyncOperation, sceneAssetName, priority, DateTime.Now, loadSceneCallbacks, userData));

                return;
            }
#endif
            m_ResourceManager.LoadScene(sceneAssetName, priority, loadSceneCallbacks, userData);
        }



        /// <summary>
        /// 异步卸载场景
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new Exception("Scene asset name is invalid.");
            }

            if (unloadSceneCallbacks == null)
            {
                throw new Exception("Unload scene callbacks is invalid.");
            }

            m_ResourceManager.UnloadScene(sceneAssetName, unloadSceneCallbacks, userData);
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="assetName">要加载asset资源的名称</param>
        /// <param name="assetType">要加载资源的类型</param>
        /// <param name="priority">加载资源的优先级</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Log.Error("Asset name is invalid.");
                return;
            }

            if (!assetName.StartsWith("Assets/"))
            {
                Log.Error("Asset name '{0}' is invalid.", assetName);
                return;
            }

#if UNITY_EDITOR
            if (m_EditorResourceMode)
            {
                UnityEngine.Object asset = null;

                DateTime starttime = DateTime.Now;
                if (assetType != null)
                {
                    asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName, assetType);
                }
                else
                {
                    asset = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetName);
                }

                if (asset != null)
                {
                    if (loadAssetCallbacks.LoadAssetSuccessCallback != null)
                    {
                        float elapseSeconds = (float)(DateTime.Now - starttime).TotalSeconds;
                        loadAssetCallbacks.LoadAssetSuccessCallback(assetName, asset, elapseSeconds, userData);
                    }
                }
                else
                {
                    if (loadAssetCallbacks.LoadAssetFailureCallback != null)
                    {
                        loadAssetCallbacks.LoadAssetFailureCallback(assetName, "Can not load this asset from asset database.", userData);
                    }
                }

                return;
            }
#endif
            m_ResourceManager.LoadAsset(assetName, assetType, priority, loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="asset">要卸载的资源</param>
        public void UnloadAsset(UnityEngine.Object asset)
        {
            if (asset == null)
            {
                throw new Exception("Asset is invalid.");
            }

            if (AssetPool == null)
            {
                return;
            }

            AssetPool.UnspawnAsset(asset);
        }

        /// <summary>
        /// 预订执行释放未被使用的资源
        /// </summary>
        /// <param name="performGCCollect">是否使用垃圾回收</param>
        public void UnloadUnusedAssets(bool performGCCollect)
        {
            m_PreorderUnloadUnusedAssets = true;
            if (performGCCollect)
            {
                m_PerformGCCollect = performGCCollect;
            }
        }

        /// <summary>
        /// 强制执行释放未被使用的资源
        /// </summary>
        /// <param name="performGCCollect">是否使用垃圾回收</param>
        public void ForceUnloadUnusedAssets(bool performGCCollect)
        {
            m_ForceUnloadUnusedAssets = true;
            if (performGCCollect)
            {
                m_PerformGCCollect = performGCCollect;
            }
        }

        /// <summary>
        /// 得到资源信息
        /// </summary>
        /// <param name="assetPath">资源路径</param>
        /// <returns></returns>
        public ResourceInfo GetResourceInfo(string assetPath)
        {
            return m_ResourceManager.GetResourceInfo(assetPath);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_ResourceManager.Dispose();
        }

        private void OnResourceUpdateStart(string name, string downloadPath, string downloadUri, int currentLength, int zipLength, int retryCount)
        {
            //Debug.Log(name); todo
        }

        private void OnResourceUpdateChanged(string name, string downloadPath, string downloadUri, int currentLength, int zipLength)
        {
            //Debug.Log(name); todo
        }

        private void OnResourceUpdateSuccess(string name, string downloadPath, string downloadUri, int length, int zipLength)
        {
            //Debug.Log(name); todo
        }

        private void OnResourceUpdateFailure(string name, string downloadUri, int retryCount, int totalRetryCount, string errorMessage)
        {
            //Debug.Log(errorMessage); todo
        }
    }
}
