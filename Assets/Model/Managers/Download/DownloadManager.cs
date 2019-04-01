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
    public class DownloadManager : ManagerBase
    {
        private readonly TaskPool<DownloadTask> m_TaskPool;
        private readonly DownloadCounter m_DownloadCounter;
        private int m_FlushSize;
        private float m_Timeout;

        public DownloadAgentStart DownloadStartEvent;
        public DownloadAgentUpdate DownloadUpdateEvent;
        public DownloadAgentSuccess DownloadSuccessEvent;
        public DownloadAgentError DownloadFailureEvent;

        public DownloadManager()
        {
            m_TaskPool = new TaskPool<DownloadTask>();
            m_DownloadCounter = new DownloadCounter(1f, 30f);
            m_FlushSize = 1024 * 1024;
            m_Timeout = 30f;

            DownloadStartEvent = null;
            DownloadUpdateEvent = null;
            DownloadSuccessEvent = null;
            DownloadFailureEvent = null;
        }


        /// <summary>
        /// 获取下载代理总数量
        /// </summary>
        public int TotalAgentCount
        {
            get
            {
                return m_TaskPool.TotalTaskCount;
            }
        }

        /// <summary>
        /// 获取可用下载代理数量
        /// </summary>
        public int FreeAgentCount
        {
            get
            {
                return m_TaskPool.FreeAgentCount;
            }
        }

        /// <summary>
        /// 获取工作中下载代理数量
        /// </summary>
        public int WorkingAgentCount
        {
            get
            {
                return m_TaskPool.WorkingAgentCount;
            }
        }

        /// <summary>
        /// 获取等待下载任务数量
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return m_TaskPool.WaitingTaskCount;
            }
        }

        /// <summary>
        /// 获取或设置将缓冲区写入磁盘的临界大小
        /// </summary>
        public int FlushSize
        {
            get
            {
                return m_FlushSize;
            }
            set
            {
                m_FlushSize = value;
            }
        }

        /// <summary>
        /// 获取或设置下载超时时长，以秒为单位
        /// </summary>
        public float Timeout
        {
            get
            {
                return m_Timeout;
            }
            set
            {
                m_Timeout = value;
            }
        }

        /// <summary>
        /// 获取当前下载速度
        /// </summary>
        public float CurrentSpeed
        {
            get
            {
                return m_DownloadCounter.CurrentSpeed;
            }
        }

        /// <summary>
        /// 下载管理器轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            m_TaskPool.OnUpdate( deltaTime,  unscaledDeltaTime);
            m_DownloadCounter.OnUpdate( deltaTime,  unscaledDeltaTime);
        }

        /// <summary>
        /// 关闭并清理下载管理器
        /// </summary>
        public override void Dispose()
        {
            m_TaskPool.Shutdown();
            m_DownloadCounter.Shutdown();
        }

        /// <summary>
        /// 增加下载代理辅助器
        /// </summary>
        /// <param name="downloadAgentHelper">要增加的下载代理辅助器</param>
        public void AddDownloadAgent(DownloadAgentBase downloadAgent)
        {
            downloadAgent.DownloadAgentStart += OnDownloadAgentStart;
            downloadAgent.DownloadAgentUpdate += OnDownloadAgentUpdate;
            downloadAgent.DownloadAgentSuccess += OnDownloadAgentSuccess;
            downloadAgent.DownloadAgentError += OnDownloadAgentError;

            m_TaskPool.AddAgent(downloadAgent);
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
            if (string.IsNullOrEmpty(downloadPath))
            {
                throw new Exception("Download path is invalid.");
            }

            if (string.IsNullOrEmpty(downloadUri))
            {
                throw new Exception("Download uri is invalid.");
            }

            if (TotalAgentCount <= 0)
            {
                throw new Exception("You must add download agent first.");
            }

            DownloadTask downloadTask = new DownloadTask(downloadPath, downloadUri, priority, m_FlushSize, m_Timeout, userData, success);
            m_TaskPool.AddTask(downloadTask);

            return downloadTask.SerialId;
        }

        /// <summary>
        /// 移除下载任务
        /// </summary>
        /// <param name="serialId">要移除下载任务的序列编号</param>
        /// <returns>是否移除下载任务成功</returns>
        public bool RemoveDownload(int serialId)
        {
            return m_TaskPool.RemoveTask(serialId) != null;
        }

        /// <summary>
        /// 移除所有下载任务
        /// </summary>
        public void RemoveAllDownload()
        {
            m_TaskPool.RemoveAllTasks();
        }




        #region  下载回调
        private void OnDownloadAgentStart(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            if (DownloadStartEvent != null)
            {
                DownloadStartEvent(serialId, downloadPath, downloadUri, currentLength, userData);
            }
        }

        private void OnDownloadAgentUpdate(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            m_DownloadCounter.RecordDownloadedLength(currentLength);
            if (DownloadUpdateEvent != null)
            {
                DownloadUpdateEvent(serialId, downloadPath, downloadUri, currentLength, userData);
            }
        }

        private void OnDownloadAgentSuccess(int serialId, string downloadPath, string downloadUri, int currentLength, object userData)
        {
            m_DownloadCounter.RecordDownloadedLength(currentLength);
            if (DownloadSuccessEvent != null)
            {
                DownloadSuccessEvent(serialId, downloadPath, downloadUri, currentLength, userData);
            }
        }

        private void OnDownloadAgentError(int serialId, string downloadPath, string downloadUri, string errorMessage, object userData)
        {
            if (DownloadFailureEvent != null)
            {
                DownloadFailureEvent(serialId, downloadPath, downloadUri, errorMessage, userData);
            }
        }
        #endregion
    }
}
