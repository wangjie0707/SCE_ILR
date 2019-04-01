using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 下载计数器
    /// </summary>
    public partial class DownloadCounter
    {
        /// <summary>
        /// 下载计数器节点
        /// </summary>
        private sealed class DownloadCounterNode
        {
            private readonly int m_DownloadedLength;
            private float m_ElapseSeconds;

            public DownloadCounterNode(int downloadedLength)
            {
                m_DownloadedLength = downloadedLength;
                m_ElapseSeconds = 0f;
            }

            public int DownloadedLength
            {
                get
                {
                    return m_DownloadedLength;
                }
            }

            public float ElapseSeconds
            {
                get
                {
                    return m_ElapseSeconds;
                }
            }

            public void OnUpdate(float deltaTime, float unscaledDeltaTime)
            {
                m_ElapseSeconds += unscaledDeltaTime;
            }
        }
    }
}