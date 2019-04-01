using System;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 下载组件
    /// </summary>
    public class DownloadComponent : GameBaseComponent
    {
        private const int DefaultPriority = 0;

        private EventComponent m_EventComponent = null;

        [SerializeField]
        private DownloadType m_DownloadType = DownloadType.WWW;

        [SerializeField]
        private int m_DownloadAgentCount = 3;

        [SerializeField]
        private float m_Timeout = 30f;

        [SerializeField]
        private int m_FlushSize = 1024 * 1024;

        /// <summary>
        /// 获取下载方式
        /// </summary>
        public DownloadType DownloadType
        {
            get
            {
                return m_DownloadType;
            }
        }

        /// <summary>
        /// 获取下载代理总数量
        /// </summary>
        public int TotalAgentCount
        {
            get
            {
                return m_DownloadManager.TotalAgentCount;
            }
        }

        /// <summary>
        /// 获取可用下载代理数量
        /// </summary>
        public int FreeAgentCount
        {
            get
            {
                return m_DownloadManager.FreeAgentCount;
            }
        }

        /// <summary>
        /// 获取工作中下载代理数量
        /// </summary>
        public int WorkingAgentCount
        {
            get
            {
                return m_DownloadManager.WorkingAgentCount;
            }
        }

        /// <summary>
        /// 获取等待下载任务数量
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return m_DownloadManager.WaitingTaskCount;
            }
        }

        /// <summary>
        /// 获取或设置下载超时时长，以秒为单位
        /// </summary>
        public float Timeout
        {
            get
            {
                return m_DownloadManager.Timeout;
            }
            set
            {
                m_DownloadManager.Timeout = m_Timeout = value;
            }
        }

        /// <summary>
        /// 获取或设置将缓冲区写入磁盘的临界大小，仅当开启断点续传时有效
        /// </summary>
        public int FlushSize
        {
            get
            {
                return m_DownloadManager.FlushSize;
            }
            set
            {
                m_DownloadManager.FlushSize = m_FlushSize = value;
            }
        }

        /// <summary>
        /// 获取当前下载速度
        /// </summary>
        public float CurrentSpeed
        {
            get
            {
                return m_DownloadManager.CurrentSpeed;
            }
        }

        public DownloadAgentStart DownloadStart;
        public DownloadAgentUpdate DownloadUpdate;
        public DownloadAgentSuccess DownloadSuccess;
        public DownloadAgentError DownloadFailure;

        /// <summary>
        /// 下载管理器
        /// </summary>
        private DownloadManager m_DownloadManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_DownloadManager = new DownloadManager();

            m_DownloadManager.DownloadStartEvent += OnDownloadStart;
            m_DownloadManager.DownloadUpdateEvent += OnDownloadUpdate;
            m_DownloadManager.DownloadSuccessEvent += OnDownloadSuccess;
            m_DownloadManager.DownloadFailureEvent += OnDownloadFailure;
            m_DownloadManager.FlushSize = m_FlushSize;
            m_DownloadManager.Timeout = m_Timeout;
        }


        protected override void OnStart()
        {
            base.OnStart();
            m_EventComponent = GameEntry.GetBaseComponent<EventComponent>();
            if (m_EventComponent == null)
            {
                Debug.LogError("Event component is invalid.");
                return;
            }

            for (int i = 0; i < m_DownloadAgentCount; i++)
            {
                AddDownloadAgent(i);
            }
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_DownloadManager.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_DownloadManager.Dispose();
        }

        /// <summary>
        /// 增加下载任务
        /// </summary>
        /// <param name="downloadPath">下载后存放路径</param>
        /// <param name="downloadUri">原始下载地址</param>
        /// <returns>新增下载任务的序列编号</returns>
        public int AddDownload(string downloadPath, string downloadUri, Action<bool> success = null)
        {
            return AddDownload(downloadPath, downloadUri, DefaultPriority, null, success);
        }

        /// <summary>
        /// 增加下载任务
        /// </summary>
        /// <param name="downloadPath">下载后存放路径</param>
        /// <param name="downloadUri">原始下载地址</param>
        /// <param name="priority">下载任务的优先级</param>
        /// <returns>新增下载任务的序列编号</returns>
        public int AddDownload(string downloadPath, string downloadUri, int priority, Action<bool> success = null)
        {
            return AddDownload(downloadPath, downloadUri, priority, null, success);
        }

        /// <summary>
        /// 增加下载任务
        /// </summary>
        /// <param name="downloadPath">下载后存放路径</param>
        /// <param name="downloadUri">原始下载地址</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增下载任务的序列编号</returns>
        public int AddDownload(string downloadPath, string downloadUri, object userData, Action<bool> success = null)
        {
            return AddDownload(downloadPath, downloadUri, DefaultPriority, userData, success);
        }

        /// <summary>
        /// 增加下载任务
        /// </summary>
        /// <param name="downloadPath">下载后存放路径</param>
        /// <param name="downloadUri">原始下载地址</param>
        /// <param name="priority">下载任务的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>新增下载任务的序列编号</returns>
        public int AddDownload(string downloadPath, string downloadUri, int priority, object userData, Action<bool> success = null)
        {
            return m_DownloadManager.AddDownload(downloadPath, downloadUri, priority, userData, success);
        }

        /// <summary>
        /// 移除下载任务
        /// </summary>
        /// <param name="serialId">要移除下载任务的序列编号</param>
        public void RemoveDownload(int serialId)
        {
            m_DownloadManager.RemoveDownload(serialId);
        }

        /// <summary>
        /// 移除所有下载任务
        /// </summary>
        public void RemoveAllDownload()
        {
            m_DownloadManager.RemoveAllDownload();
        }

        /// <summary>
        /// 增加下载代理辅助器
        /// </summary>
        /// <param name="index">下载代理辅助器索引</param>
        private void AddDownloadAgent(int index)
        {
            DownloadAgentBase downloadAgent = null;
            switch (DownloadType)
            {
                case DownloadType.WWW:
                    downloadAgent = new GameObject().AddComponent<WWWDownloadAgent>();
                    break;
                case DownloadType.UnityWebRequest:
                    downloadAgent = new GameObject().AddComponent<UnityWebRequestDownloadAgent>();
                    break;
            }

            if (downloadAgent == null)
            {
                Debug.LogError("Can not create download agent.");
                return;
            }

            downloadAgent.name = TextUtil.Format("Download Agent - {0}", index.ToString());
            Transform transform = downloadAgent.transform;
            transform.SetParent(this.transform);
            transform.localScale = Vector3.one;

            m_DownloadManager.AddDownloadAgent(downloadAgent);
        }


        private void OnDownloadStart(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            if (DownloadStart != null)
            {
                DownloadStart(serialId, downloadPath, downloadUri, currentLength, userData);
            }
            Debug.Log("下载开始");
        }

        private void OnDownloadUpdate(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            if (DownloadUpdate != null)
            {
                DownloadUpdate(serialId, downloadPath, downloadUri, currentLength, userData);
            }
            //Debug.Log("下载更新");
        }

        private void OnDownloadSuccess(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            if (DownloadSuccess != null)
            {
                DownloadSuccess(serialId, downloadPath, downloadUri, currentLength, userData);
            }
            Debug.Log("下载成功");
        }

        private void OnDownloadFailure(int serialId, string downloadPath, string downloadUri, string errorMessage, object userData)
        {
            if (DownloadFailure != null)
            {
                DownloadFailure(serialId, downloadPath, downloadUri, errorMessage, userData);
            }
            Debug.Log("下载失败" + errorMessage);
        }
    }
}
