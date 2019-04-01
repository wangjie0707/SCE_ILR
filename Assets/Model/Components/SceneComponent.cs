using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Myth
{
    /// <summary>
    /// 场景组件
    /// </summary>
    public class SceneComponent : GameBaseComponent
    {
        private const int DefaultPriority = 0;
        private Scene m_FrameworkScene = default(Scene);

        [SerializeField]
        private bool m_EnableLoadSceneSuccessEvent = true;

        [SerializeField]
        private bool m_EnableLoadSceneFailureEvent = true;

        [SerializeField]
        private bool m_EnableLoadSceneUpdateEvent = true;

        [SerializeField]
        private bool m_EnableLoadSceneDependencyAssetEvent = true;

        [SerializeField]
        private bool m_EnableUnloadSceneSuccessEvent = true;

        [SerializeField]
        private bool m_EnableUnloadSceneFailureEvent = true;

        /// <summary>
        /// 场景管理器
        /// </summary>
        private SceneManager m_SceneManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_SceneManager = new SceneManager();

            m_SceneManager.LoadSceneSuccess = OnLoadSceneSuccess;
            m_SceneManager.LoadSceneFailure = OnLoadSceneFailure;
            m_SceneManager.LoadSceneUpdate = OnLoadSceneUpdate;
            m_SceneManager.LoadSceneDependencyAsset = OnLoadSceneDependencyAsset;
            m_SceneManager.UnloadSceneSuccess = OnUnloadSceneSuccess;
            m_SceneManager.UnloadSceneFailure = OnUnloadSceneFailure;

            m_FrameworkScene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(0);
            if (!m_FrameworkScene.IsValid())
            {
                Log.Error("Framework scene is invalid.");
                return;
            }
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_SceneManager.Dispose();
        }

        /// <summary>
        /// 获取场景是否已加载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否已加载</returns>
        public bool SceneIsLoaded(string sceneAssetName)
        {
            return m_SceneManager.SceneIsLoaded(sceneAssetName);
        }

        /// <summary>
        /// 获取已加载场景的资源名称
        /// </summary>
        /// <returns>已加载场景的资源名称</returns>
        public string[] GetLoadedSceneAssetNames()
        {
            return m_SceneManager.GetLoadedSceneAssetNames();
        }

        /// <summary>
        /// 获取已加载场景的资源名称
        /// </summary>
        /// <param name="results">已加载场景的资源名称</param>
        public void GetLoadedSceneAssetNames(List<string> results)
        {
            m_SceneManager.GetLoadedSceneAssetNames(results);
        }

        /// <summary>
        /// 获取场景是否正在加载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否正在加载</returns>
        public bool SceneIsLoading(string sceneAssetName)
        {
            return m_SceneManager.SceneIsLoading(sceneAssetName);
        }

        /// <summary>
        /// 获取正在加载场景的资源名称
        /// </summary>
        /// <returns>正在加载场景的资源名称</returns>
        public string[] GetLoadingSceneAssetNames()
        {
            return m_SceneManager.GetLoadingSceneAssetNames();
        }

        /// <summary>
        /// 获取正在加载场景的资源名称
        /// </summary>
        /// <param name="results">正在加载场景的资源名称</param>
        public void GetLoadingSceneAssetNames(List<string> results)
        {
            m_SceneManager.GetLoadingSceneAssetNames(results);
        }

        /// <summary>
        /// 获取场景是否正在卸载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否正在卸载</returns>
        public bool SceneIsUnloading(string sceneAssetName)
        {
            return m_SceneManager.SceneIsUnloading(sceneAssetName);
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称
        /// </summary>
        /// <returns>正在卸载场景的资源名称</returns>
        public string[] GetUnloadingSceneAssetNames()
        {
            return m_SceneManager.GetUnloadingSceneAssetNames();
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称
        /// </summary>
        /// <param name="results">正在卸载场景的资源名称</param>
        public void GetUnloadingSceneAssetNames(List<string> results)
        {
            m_SceneManager.GetUnloadingSceneAssetNames(results);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        public void LoadScene(string sceneAssetName)
        {
            LoadScene(sceneAssetName, DefaultPriority, null);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <param name="priority">加载场景资源的优先级</param>
        public void LoadScene(string sceneAssetName, int priority)
        {
            LoadScene(sceneAssetName, priority, null);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadScene(string sceneAssetName, object userData)
        {
            LoadScene(sceneAssetName, DefaultPriority, userData);
        }

        /// <summary>
        /// 加载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <param name="priority">加载场景资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadScene(string sceneAssetName, int priority, object userData)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return;
            }

            if (!sceneAssetName.StartsWith("Assets/") || !sceneAssetName.EndsWith(".unity"))
            {
                Log.Error("Scene asset name '{0}' is invalid.", sceneAssetName);
                return;
            }

            m_SceneManager.LoadScene(sceneAssetName, priority, userData);
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        public void UnloadScene(string sceneAssetName)
        {
            UnloadScene(sceneAssetName, null);
        }

        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void UnloadScene(string sceneAssetName, object userData)
        {
            m_SceneManager.UnloadScene(sceneAssetName, userData);
        }

        private void OnLoadSceneSuccess(string sceneAssetName, UnityEngine.Object asset, float duration, object userData)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() == m_FrameworkScene)
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(ResourceHelper.GetSceneName(sceneAssetName));
                if (!scene.IsValid())
                {
                    Log.Error("Loaded scene '{0}' is invalid.", sceneAssetName);
                    return;
                }
                Debug.Log("设置激活场景" + sceneAssetName);
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
            }

            if (m_EnableLoadSceneSuccessEvent)
            {
                Debug.Log("加载场景成功" + sceneAssetName);
                //todo
            }
        }

        private void OnLoadSceneFailure(string sceneAssetName, string errorMessage, object userData)
        {
            Log.Warning("Load scene failure, scene asset name '{0}', error message '{1}'.", sceneAssetName, errorMessage);
            if (m_EnableLoadSceneFailureEvent)
            {
                //todo
                Debug.Log("加载场景失败" + sceneAssetName + errorMessage);
            }
        }

        private void OnLoadSceneUpdate(string sceneAssetName, float progress, object userData)
        {
            if (m_EnableLoadSceneUpdateEvent)
            {
                //todo
                Debug.Log("加载场景更新 进度=" + sceneAssetName + progress);
            }
        }

        private void OnLoadSceneDependencyAsset(string sceneAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            if (m_EnableLoadSceneDependencyAssetEvent)
            {
                //todo
                Debug.Log("加载场景依赖资源 " + sceneAssetName + "已经加载：" + loadedCount + "  总共：" + totalCount);
            }
        }

        private void OnUnloadSceneSuccess(string sceneAssetName, object userData)
        {
            if (m_EnableUnloadSceneSuccessEvent)
            {
                //todo
                Debug.Log("卸载场景成功 " + sceneAssetName);
            }
        }

        private void OnUnloadSceneFailure(string sceneAssetName, object userData)
        {
            Log.Warning("Unload scene failure, scene asset name '{0}'.", sceneAssetName);
            if (m_EnableUnloadSceneFailureEvent)
            {
                //todo
                Debug.Log("卸载场景失败 " + sceneAssetName);
            }
        }
    }
}
