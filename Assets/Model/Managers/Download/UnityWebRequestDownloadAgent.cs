using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


namespace Myth
{
    /// <summary>
    /// 使用 UnityWebRequest 实现的下载代理
    /// </summary>
    public class UnityWebRequestDownloadAgent : DownloadAgentBase, IDisposable
    {
        private UnityWebRequest m_UnityWebRequest = null;
        private int m_LastDownloadedSize = 0;
        private bool m_Disposed = false;

        /// <summary>
        /// 通过下载代理下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Download(string downloadUri, object userData)
        {
            if (DownloadAgentUpdate == null || DownloadAgentSuccess == null || DownloadAgentError == null)
            {
                Debug.LogError("Download agent callback is invalid.");
                return;
            }

            m_UnityWebRequest = UnityWebRequest.Get(downloadUri);
#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
        }

        /// <summary>
        /// 通过下载代理下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Download(string downloadUri, int fromPosition, object userData)
        {
            if (DownloadAgentUpdate == null || DownloadAgentSuccess == null || DownloadAgentError == null)
            {
                Debug.LogError("Download agent callback is invalid.");
                return;
            }

            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "Range", TextUtil.Format("bytes={0}-", fromPosition.ToString()) }
            };

            m_UnityWebRequest = UnityWebRequest.Post(downloadUri, header);
#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
        }

        /// <summary>
        /// 通过下载代理下载指定地址的数据
        /// </summary>
        /// <param name="downloadUri">下载地址</param>
        /// <param name="fromPosition">下载数据起始位置</param>
        /// <param name="toPosition">下载数据结束位置</param>
        /// <param name="userData">用户自定义数据</param>
        public override void Download(string downloadUri, int fromPosition, int toPosition, object userData)
        {
            if (DownloadAgentUpdate == null || DownloadAgentSuccess == null || DownloadAgentError == null)
            {
                Debug.LogError("Download agent callback is invalid.");
                return;
            }

            Dictionary<string, string> header = new Dictionary<string, string>
            {
                { "Range", TextUtil.Format("bytes={0}-{1}", fromPosition.ToString(), toPosition.ToString()) }
            };

            m_UnityWebRequest = UnityWebRequest.Post(downloadUri, header);
#if UNITY_2017_2_OR_NEWER
            m_UnityWebRequest.SendWebRequest();
#else
            m_UnityWebRequest.Send();
#endif
        }

        /// <summary>
        /// 重置下载代理
        /// </summary>
        public override void Reset()
        {
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
            }

            m_LastDownloadedSize = 0;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
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
                if (m_UnityWebRequest != null)
                {
                    m_UnityWebRequest.Dispose();
                    m_UnityWebRequest = null;
                }
            }

            m_Disposed = true;
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            if (m_UnityWebRequest == null)
            {
                return;
            }

            if (!m_UnityWebRequest.isDone)
            {
                if (m_LastDownloadedSize < (int)m_UnityWebRequest.downloadedBytes)
                {
                    m_LastDownloadedSize = (int)m_UnityWebRequest.downloadedBytes;
                    DownloadUpdate((int)m_UnityWebRequest.downloadedBytes, null);

                }

                return;
            }

            bool isError = false;
#if UNITY_2017_1_OR_NEWER
            isError = m_UnityWebRequest.isNetworkError;
#else
            isError = m_UnityWebRequest.isError;
#endif
            if (isError)
            {
                DownloadError(m_UnityWebRequest.error);
            }
            else if (m_UnityWebRequest.downloadHandler.isDone)
            {
                DownloadComplete((int)m_UnityWebRequest.downloadedBytes, m_UnityWebRequest.downloadHandler.data);
            }
        }


    }
}
