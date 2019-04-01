using System;
using System.IO;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 下载代理基类
    /// </summary>
    public abstract class DownloadAgentBase : MonoBehaviour, ITaskAgent<DownloadTask>, IDisposable
    {
        private DownloadTask m_Task;
        private FileStream m_FileStream;
        private int m_WaitFlushSize;
        private float m_WaitTime;
        private int m_StartLength;
        private int m_DownloadedLength;
        private int m_SavedLength;
        private bool m_Disposed;

        /// <summary>
        /// 下载代理开始回调
        /// </summary>
        public DownloadAgentStart DownloadAgentStart = null;

        /// <summary>
        /// 下载代理更新回调
        /// </summary>
        public DownloadAgentUpdate DownloadAgentUpdate = null;

        /// <summary>
        /// 下载代理完成回调
        /// </summary>
        public DownloadAgentSuccess DownloadAgentSuccess = null;

        /// <summary>
        /// 下载代理错误回调
        /// </summary>
        public DownloadAgentError DownloadAgentError = null;

        public DownloadAgentBase()
        {
            m_Task = null;
            m_FileStream = null;
            m_WaitFlushSize = 0;
            m_WaitTime = 0f;
            m_StartLength = 0;
            m_DownloadedLength = 0;
            m_SavedLength = 0;
            m_Disposed = false;

            DownloadAgentStart = null;
            DownloadAgentUpdate = null;
            DownloadAgentSuccess = null;
            DownloadAgentError = null;
        }

        /// <summary>
        /// 获取下载任务
        /// </summary>
        public DownloadTask Task
        {
            get
            {
                return m_Task;
            }
        }

        /// <summary>
        /// 获取已经等待时间
        /// </summary>
        public float WaitTime
        {
            get
            {
                return m_WaitTime;
            }
        }

        /// <summary>
        /// 获取开始下载时已经存在的大小
        /// </summary>
        public int StartLength
        {
            get
            {
                return m_StartLength;
            }
        }

        /// <summary>
        /// 获取本次已经下载的大小
        /// </summary>
        public int DownloadedLength
        {
            get
            {
                return m_DownloadedLength;
            }
        }

        /// <summary>
        /// 获取当前的大小
        /// </summary>
        public int CurrentLength
        {
            get
            {
                return m_StartLength + m_DownloadedLength;
            }
        }

        /// <summary>
        /// 获取已经存盘的大小
        /// </summary>
        public long SavedLength
        {
            get
            {
                return m_SavedLength;
            }
        }


        /// <summary>
        /// 初始化下载代理
        /// </summary>
        public virtual void Initialize()
        {

        }

        /// <summary>
        /// 下载代理轮询
        /// </summary>
        public virtual void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (m_Task.Status == DownloadTaskStatus.Doing)
            {
                m_WaitTime += Time.unscaledDeltaTime;
                if (m_WaitTime >= m_Task.Timeout)
                {
                    DownloadError("Timeout");
                }
            }
        }

        /// <summary>
        /// 关闭并清理下载代理
        /// </summary>
        public virtual void Shutdown()
        {
            Dispose();
        }

        /// <summary>
        /// 开始处理下载任务
        /// </summary>
        /// <param name="task"></param>
        public virtual void StartTask(DownloadTask task)
        {
            if (task == null)
            {
                throw new Exception("Task is invalid.");
            }

            m_Task = task;

            m_Task.Status = DownloadTaskStatus.Doing;
            string downloadFile = TextUtil.Format("{0}.download", m_Task.DownloadPath);

            try
            {
                if (File.Exists(downloadFile))
                {
                    m_FileStream = File.OpenWrite(downloadFile);
                    m_FileStream.Seek(0, SeekOrigin.End);
                    m_StartLength = m_SavedLength = (int)m_FileStream.Length;
                    m_DownloadedLength = 0;
                }
                else
                {
                    string directory = Path.GetDirectoryName(m_Task.DownloadPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    m_FileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                    m_StartLength = m_SavedLength = m_DownloadedLength = 0;
                }

                if (DownloadAgentStart != null)
                {
                    DownloadAgentStart(m_Task.SerialId, m_Task.DownloadPath, m_Task.DownloadUri, CurrentLength, m_Task.UserData);
                }

                if (m_StartLength > 0)
                {
                    Download(m_Task.DownloadUri, m_StartLength, m_Task.UserData);
                }
                else
                {
                    Download(m_Task.DownloadUri, m_Task.UserData);
                }
            }
            catch (Exception exception)
            {
                DownloadError(exception.Message);
            }
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            Clear(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">释放资源标记</param>
        private void Clear(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (m_FileStream != null)
                {
                    m_FileStream.Dispose();
                    m_FileStream = null;
                }
            }

            m_Disposed = true;
        }

        protected void DownloadUpdate(int length, byte[] bytes)
        {
            m_WaitTime = 0f;

            SaveBytes(bytes);

            m_DownloadedLength = length;

            if (DownloadAgentUpdate != null)
            {
                DownloadAgentUpdate(m_Task.SerialId, m_Task.DownloadPath, m_Task.DownloadUri, bytes != null ? bytes.Length : 0, m_Task.UserData);
            }
        }

        protected void DownloadError(string errormessage)
        {
            Reset();

            if (m_FileStream != null)
            {
                m_FileStream.Close();
                m_FileStream = null;
            }

            m_Task.Status = DownloadTaskStatus.Error;

            if (DownloadAgentError != null)
            {
                DownloadAgentError(m_Task.SerialId, m_Task.DownloadPath, m_Task.DownloadUri, errormessage, m_Task.UserData);
            }
            if (m_Task.Success != null)
            {
                m_Task.Success(false);
            }

            m_Task.Done = true;
        }

        protected void DownloadComplete(int length, byte[] bytes)
        {
            m_WaitTime = 0f;

            SaveBytes(bytes);

            m_DownloadedLength = length;

            if (m_SavedLength != CurrentLength)
            {
                throw new Exception("Internal download error.");
            }

            Reset();
            m_FileStream.Close();
            m_FileStream = null;

            if (File.Exists(m_Task.DownloadPath))
            {
                File.Delete(m_Task.DownloadPath);
            }

            File.Move(TextUtil.Format("{0}.download", m_Task.DownloadPath), m_Task.DownloadPath);

            m_Task.Status = DownloadTaskStatus.Done;

            if (DownloadAgentSuccess != null)
            {
                DownloadAgentSuccess(m_Task.SerialId, m_Task.DownloadPath, m_Task.DownloadUri, bytes != null ? bytes.Length : 0, m_Task.UserData);
            }
            if (m_Task.Success != null)
            {
                m_Task.Success(true);
            }

            m_Task.Done = true;
        }


        private void SaveBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                return;
            }

            try
            {
                int length = bytes.Length;
                m_FileStream.Write(bytes, 0, length);
                m_WaitFlushSize += length;
                m_SavedLength += length;

                if (m_WaitFlushSize >= m_Task.FlushSize)
                {
                    m_FileStream.Flush();
                    m_WaitFlushSize = 0;
                }
            }
            catch (Exception exception)
            {
                DownloadAgentError(m_Task.SerialId, m_Task.DownloadPath, m_Task.DownloadUri, exception.Message, m_Task.UserData);
            }
        }




        /// <summary>
        /// 通过下载代理下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="userData">用户自定义数据</param>
        public abstract void Download(string downloadUri, object userData);

        /// <summary>
        /// 通过下载代理下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="userData">用户自定义数据</param>
        public abstract void Download(string downloadUri, int fromPosition, object userData);

        /// <summary>
        /// 通过下载代理下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="toPosition">下载数据结束位置</param>
        /// <param name="userData">用户自定义数据</param>
        public abstract void Download(string downloadUri, int fromPosition, int toPosition, object userData);

        /// <summary>
        /// 重置下载代理
        /// </summary>
        public abstract void Reset();


    }
}
