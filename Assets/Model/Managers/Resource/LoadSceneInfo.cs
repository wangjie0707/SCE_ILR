#if UNITY_EDITOR
using System;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 编辑器下加载场景信息
    /// </summary>
    public class LoadSceneInfo
    {
        private readonly AsyncOperation m_AsyncOperation;
        private readonly string m_SceneAssetName;
        private readonly int m_Priority;
        private readonly DateTime m_StartTime;
        private readonly LoadAssetCallbacks m_LoadSceneCallbacks;
        private readonly object m_UserData;

        public LoadSceneInfo(AsyncOperation asyncOperation, string sceneAssetName, int priority, DateTime startTime, LoadAssetCallbacks loadSceneCallbacks, object userData)
        {
            m_AsyncOperation = asyncOperation;
            m_SceneAssetName = sceneAssetName;
            m_Priority = priority;
            m_StartTime = startTime;
            m_LoadSceneCallbacks = loadSceneCallbacks;
            m_UserData = userData;
        }

        public AsyncOperation AsyncOperation
        {
            get
            {
                return m_AsyncOperation;
            }
        }

        public string SceneAssetName
        {
            get
            {
                return m_SceneAssetName;
            }
        }

        public int Priority
        {
            get
            {
                return m_Priority;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }
        }

        public LoadAssetCallbacks LoadSceneCallbacks
        {
            get
            {
                return m_LoadSceneCallbacks;
            }
        }

        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }
    }
}
#endif