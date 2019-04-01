//===================================================
//
//===================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    /// <summary>
    /// 场景管理器
    /// </summary>
    public class SceneManager : ManagerBase, IDisposable
    {
        private readonly List<string> m_LoadedSceneAssetNames;
        private readonly List<string> m_LoadingSceneAssetNames;
        private readonly List<string> m_UnloadingSceneAssetNames;
        private readonly LoadAssetCallbacks m_LoadSceneCallbacks;
        private readonly UnloadSceneCallbacks m_UnloadSceneCallbacks;

        private LoadAssetSuccessCallback m_LoadSceneSuccessEventHandler;
        private LoadAssetFailureCallback m_LoadSceneFailureEventHandler;
        private LoadAssetUpdateCallback m_LoadSceneUpdateEventHandler;
        private LoadAssetDependencyAssetCallback m_LoadSceneDependencyAssetEventHandler;
        private UnloadSceneSuccessCallback m_UnloadSceneSuccessEventHandler;
        private UnloadSceneFailureCallback m_UnloadSceneFailureEventHandler;
        public SceneManager()
        {
            m_LoadedSceneAssetNames = new List<string>();
            m_LoadingSceneAssetNames = new List<string>();
            m_UnloadingSceneAssetNames = new List<string>();
            m_LoadSceneCallbacks = new LoadAssetCallbacks(LoadSceneSuccessCallback, LoadSceneFailureCallback, LoadSceneUpdateCallback, LoadSceneDependencyAssetCallback);
            m_UnloadSceneCallbacks = new UnloadSceneCallbacks(UnloadSceneSuccessCallback, UnloadSceneFailureCallback);
        }

        /// <summary>
        /// 加载场景成功事件
        /// </summary>
        public LoadAssetSuccessCallback LoadSceneSuccess
        {
            get
            {
                return m_LoadSceneSuccessEventHandler;
            }
            set
            {
                m_LoadSceneSuccessEventHandler = value;
            }
        }

        /// <summary>
        /// 加载场景失败事件
        /// </summary>
        public LoadAssetFailureCallback LoadSceneFailure
        {
            get
            {
                return m_LoadSceneFailureEventHandler;
            }
            set
            {
                m_LoadSceneFailureEventHandler = value;
            }
        }

        /// <summary>
        /// 加载场景更新事件
        /// </summary>
        public LoadAssetUpdateCallback LoadSceneUpdate
        {
            get
            {
                return m_LoadSceneUpdateEventHandler;
            }
            set
            {
                m_LoadSceneUpdateEventHandler = value;
            }
        }

        /// <summary>
        /// 加载场景时加载依赖资源事件
        /// </summary>
        public LoadAssetDependencyAssetCallback LoadSceneDependencyAsset
        {
            get
            {
                return m_LoadSceneDependencyAssetEventHandler;
            }
            set
            {
                m_LoadSceneDependencyAssetEventHandler = value;
            }
        }

        /// <summary>
        /// 卸载场景成功事件
        /// </summary>
        public UnloadSceneSuccessCallback UnloadSceneSuccess
        {
            get
            {
                return m_UnloadSceneSuccessEventHandler;
            }
            set
            {
                m_UnloadSceneSuccessEventHandler = value;
            }
        }

        /// <summary>
        /// 卸载场景失败事件
        /// </summary>
        public UnloadSceneFailureCallback UnloadSceneFailure
        {
            get
            {
                return m_UnloadSceneFailureEventHandler;
            }
            set
            {
                m_UnloadSceneFailureEventHandler = value;
            }
        }


        #region  回调函数

        private void LoadSceneSuccessCallback(string sceneAssetName, UnityEngine.Object asset, float duration, object userData)
        {
            m_LoadingSceneAssetNames.Remove(sceneAssetName);
            m_LoadedSceneAssetNames.Add(sceneAssetName);
            if (m_LoadSceneSuccessEventHandler != null)
            {
                m_LoadSceneSuccessEventHandler(sceneAssetName, asset, duration, userData);
            }
            Debug.Log("加载场景成功");
        }

        private void LoadSceneFailureCallback(string sceneAssetName, string errorMessage, object userData)
        {
            m_LoadingSceneAssetNames.Remove(sceneAssetName);
            string appendErrorMessage = string.Format("Load scene failure, scene asset name '{0}', error message '{1}'.", sceneAssetName, errorMessage);
            if (m_LoadSceneFailureEventHandler != null)
            {
                m_LoadSceneFailureEventHandler(sceneAssetName, appendErrorMessage, userData);
                return;
            }

            throw new Exception(appendErrorMessage);
        }

        private void LoadSceneUpdateCallback(string sceneAssetName, float progress, object userData)
        {
            if (m_LoadSceneUpdateEventHandler != null)
            {
                m_LoadSceneUpdateEventHandler(sceneAssetName, progress, userData);
            }
        }

        private void LoadSceneDependencyAssetCallback(string sceneAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            if (m_LoadSceneDependencyAssetEventHandler != null)
            {
                m_LoadSceneDependencyAssetEventHandler(sceneAssetName, dependencyAssetName, loadedCount, totalCount, userData);
            }
        }

        private void UnloadSceneSuccessCallback(string sceneAssetName, object userData)
        {
            m_UnloadingSceneAssetNames.Remove(sceneAssetName);
            m_LoadedSceneAssetNames.Remove(sceneAssetName);
            if (m_UnloadSceneSuccessEventHandler != null)
            {
                m_UnloadSceneSuccessEventHandler(sceneAssetName, userData);
            }
        }

        private void UnloadSceneFailureCallback(string sceneAssetName, object userData)
        {
            m_UnloadingSceneAssetNames.Remove(sceneAssetName);
            if (m_UnloadSceneFailureEventHandler != null)
            {
                m_UnloadSceneFailureEventHandler(sceneAssetName, userData);
                return;
            }

            throw new Exception(string.Format("Unload scene failure, scene asset name '{0}'.", sceneAssetName));
        }
        #endregion

        /// <summary>
        /// 获取场景是否已加载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否已加载</returns>
        public bool SceneIsLoaded(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new Exception("Scene asset name is invalid.");
            }

            return m_LoadedSceneAssetNames.Contains(sceneAssetName);
        }

        /// <summary>
        /// 获取已加载场景的资源名称
        /// </summary>
        /// <returns>已加载场景的资源名称</returns>
        public string[] GetLoadedSceneAssetNames()
        {
            return m_LoadedSceneAssetNames.ToArray();
        }

        /// <summary>
        /// 获取已加载场景的资源名称
        /// </summary>
        /// <param name="results">已加载场景的资源名称</param>
        public void GetLoadedSceneAssetNames(List<string> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            results.AddRange(m_LoadedSceneAssetNames);
        }

        /// <summary>
        /// 获取场景是否正在加载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否正在加载</returns>
        public bool SceneIsLoading(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new Exception("Scene asset name is invalid.");
            }

            return m_LoadingSceneAssetNames.Contains(sceneAssetName);
        }

        /// <summary>
        /// 获取正在加载场景的资源名称
        /// </summary>
        /// <returns>正在加载场景的资源名称</returns>
        public string[] GetLoadingSceneAssetNames()
        {
            return m_LoadingSceneAssetNames.ToArray();
        }

        /// <summary>
        /// 获取正在加载场景的资源名称
        /// </summary>
        /// <param name="results">正在加载场景的资源名称</param>
        public void GetLoadingSceneAssetNames(List<string> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            results.AddRange(m_LoadingSceneAssetNames);
        }

        /// <summary>
        /// 获取场景是否正在卸载
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <returns>场景是否正在卸载</returns>
        public bool SceneIsUnloading(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new Exception("Scene asset name is invalid.");
            }

            return m_UnloadingSceneAssetNames.Contains(sceneAssetName);
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称
        /// </summary>
        /// <returns>正在卸载场景的资源名称</returns>
        public string[] GetUnloadingSceneAssetNames()
        {
            return m_UnloadingSceneAssetNames.ToArray();
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称
        /// </summary>
        /// <param name="results">正在卸载场景的资源名称</param>
        public void GetUnloadingSceneAssetNames(List<string> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            results.AddRange(m_UnloadingSceneAssetNames);
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
                throw new Exception("Scene asset name is invalid.");
            }

            if (SceneIsUnloading(sceneAssetName))
            {
                throw new Exception(string.Format("Scene asset '{0}' is being unloaded.", sceneAssetName));
            }

            if (SceneIsLoading(sceneAssetName))
            {
                throw new Exception(string.Format("Scene asset '{0}' is being loaded.", sceneAssetName));
            }

            if (SceneIsLoaded(sceneAssetName))
            {
                throw new Exception(string.Format("Scene asset '{0}' is already loaded.", sceneAssetName));
            }

            m_LoadingSceneAssetNames.Add(sceneAssetName);
            GameEntry.Resource.LoadScene(sceneAssetName, priority, m_LoadSceneCallbacks, userData);
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
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new Exception("Scene asset name is invalid.");
            }

            if (SceneIsUnloading(sceneAssetName))
            {
                throw new Exception(string.Format("Scene asset '{0}' is being unloaded.", sceneAssetName));
            }

            if (SceneIsLoading(sceneAssetName))
            {
                throw new Exception(string.Format("Scene asset '{0}' is being loaded.", sceneAssetName));
            }

            if (!SceneIsLoaded(sceneAssetName))
            {
                throw new Exception(string.Format("Scene asset '{0}' is not loaded yet.", sceneAssetName));
            }

            m_UnloadingSceneAssetNames.Add(sceneAssetName);
            GameEntry.Resource.UnloadScene(sceneAssetName, m_UnloadSceneCallbacks, userData);
        }

        public override void Dispose()
        {
            string[] loadedSceneAssetNames = m_LoadedSceneAssetNames.ToArray();
            foreach (string loadedSceneAssetName in loadedSceneAssetNames)
            {
                if (SceneIsUnloading(loadedSceneAssetName))
                {
                    continue;
                }

                UnloadScene(loadedSceneAssetName);
            }

            m_LoadedSceneAssetNames.Clear();
            m_LoadingSceneAssetNames.Clear();
            m_UnloadingSceneAssetNames.Clear();
        }
    }
}
