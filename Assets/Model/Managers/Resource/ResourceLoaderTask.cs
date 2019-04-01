using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    public class ResourceLoaderTask : ITask
    {


        private ResourceLoaderTask m_MainTask;
        private readonly LoadAssetCallbacks m_LoadAssetCallbacks;

        private static int s_Serial = 0;
        private readonly int m_SerialId;
        private readonly int m_Priority;
        private bool m_Done;
        private readonly string m_AssetName;
        private readonly string m_AssetFullPath;
        private readonly string m_AssetBundleName;
        private readonly Type m_AssetType;
        private DateTime m_StartTime;
        private string[] m_DependencyAssetNames;
        private readonly List<UnityEngine.AssetBundle> m_DependencyAssets;
        private int m_TotalDependencyAssetCount;
        private readonly object m_UserData;
        private bool m_IsScene;

        public ResourceLoaderTask(string assetName, string assetBundleName, Type assetType, int priority, string[] dependencyAssetNames, bool isScene, ResourceLoaderTask mainTask, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            m_SerialId = s_Serial++;
            m_Priority = priority;
            m_Done = false;
            m_AssetBundleName = assetBundleName;
            m_AssetType = assetType;
            m_DependencyAssetNames = dependencyAssetNames;
            m_UserData = userData;
            m_StartTime = default(DateTime);
            m_LoadAssetCallbacks = loadAssetCallbacks;
            m_TotalDependencyAssetCount = 0;
            m_MainTask = mainTask;
            m_DependencyAssets = new List<AssetBundle>();
            m_IsScene = isScene;
            m_AssetName = assetName;
            if (m_MainTask != null)
            {
                m_MainTask.TotalDependencyAssetCount++;
            }
            if (assetName != null)
            {
                m_AssetFullPath = TextUtil.Format("{0}.{1}", assetBundleName, assetName);
            }
        }

        public int SerialId
        {
            get
            {
                return m_SerialId;
            }
        }

        /// <summary>
        /// 加载优先级
        /// </summary>
        public int Priority
        {
            get
            {
                return m_Priority;
            }
        }

        /// <summary>
        /// 是否加载完毕
        /// </summary>
        public bool Done
        {
            get
            {
                return m_Done;
            }
            set
            {
                m_Done = value;
            }
        }

        /// <summary>
        /// 获取资源名称
        /// </summary>
        public string AssetName
        {
            get
            {
                return m_AssetName;
            }
        }

        /// <summary>
        /// 获取完整资源路径名称
        /// </summary>
        public string AssetFullPath
        {
            get
            {
                return m_AssetFullPath;
            }
        }

        /// <summary>
        /// 获取AssetBundle资源名称
        /// </summary>
        public string AssetBundleName
        {
            get
            {
                return m_AssetBundleName;
            }
        }


        /// <summary>
        /// 获取资源类型
        /// </summary>
        public Type AssetType
        {
            get
            {
                return m_AssetType;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
            }
        }

        public bool IsScene
        {
            get
            {
                return m_IsScene;
            }
        }

        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }

        public int LoadedDependencyAssetCount
        {
            get
            {
                return m_DependencyAssets.Count;
            }
        }

        public int TotalDependencyAssetCount
        {
            get
            {
                return m_TotalDependencyAssetCount;
            }
            set
            {
                m_TotalDependencyAssetCount = value;
            }
        }

        public string[] GetDependencyAssetNames()
        {
            return m_DependencyAssetNames;
        }

        public void OnLoadAssetUpdate(  float progress)
        {
            if (m_MainTask != null)
            {
                return;
            }
            if (m_LoadAssetCallbacks.LoadAssetUpdateCallback != null)
            {
                m_LoadAssetCallbacks.LoadAssetUpdateCallback(AssetBundleName, progress, UserData);
            }
        }

        public void OnLoadAssetFailure( LoadResourceStatus status, string errorMessage)
        {
            if (m_MainTask != null)
            {
                m_MainTask.OnLoadAssetFailure( LoadResourceStatus.DependencyError,
                    string.Format("Can not load dependency asset '{0}', internal status '{1}', internal error message '{2}'.", AssetBundleName, status.ToString(), errorMessage));
                return;
            }
            if (m_LoadAssetCallbacks.LoadAssetFailureCallback != null)
            {
                m_LoadAssetCallbacks.LoadAssetFailureCallback(AssetBundleName, errorMessage, UserData);
            }
        }

        public void OnLoadAssetSuccess( UnityEngine.Object asset, float duration)
        {
            if (m_LoadAssetCallbacks.LoadAssetSuccessCallback != null)
            {
                m_LoadAssetCallbacks.LoadAssetSuccessCallback(AssetName, asset, duration, UserData);
            }
        }

        public void OnLoadDependencyAsset(string dependencyAssetName, UnityEngine.AssetBundle assetBundle)
        {
            if (m_MainTask != null)
            {
                m_MainTask.OnLoadDependencyAsset( dependencyAssetName, assetBundle);
                return;
            }
            m_DependencyAssets.Add(assetBundle);
            if (m_LoadAssetCallbacks.LoadAssetDependencyAssetCallback != null)
            {
                m_LoadAssetCallbacks.LoadAssetDependencyAssetCallback(AssetName, dependencyAssetName, LoadedDependencyAssetCount, TotalDependencyAssetCount, UserData);
            }
        }
    }
}
