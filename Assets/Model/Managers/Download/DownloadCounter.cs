using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 下载计数器
    /// </summary>
    public sealed partial class DownloadCounter
    {
        private readonly Queue<DownloadCounterNode> m_DownloadCounterNodes;
        private float m_UpdateInterval;
        private float m_RecordInterval;
        private float m_CurrentSpeed;
        private float m_Accumulator;
        private float m_TimeLeft;

        public DownloadCounter(float updateInterval, float recordInterval)
        {
            if (updateInterval <= 0f)
            {
                throw new Exception("Update interval is invalid.");
            }

            if (recordInterval <= 0f)
            {
                throw new Exception("Record interval is invalid.");
            }

            m_DownloadCounterNodes = new Queue<DownloadCounterNode>();
            m_UpdateInterval = updateInterval;
            m_RecordInterval = recordInterval;
            Reset();
        }

        public float UpdateInterval
        {
            get
            {
                return m_UpdateInterval;
            }
            set
            {
                if (value <= 0f)
                {
                    throw new Exception("Update interval is invalid.");
                }

                m_UpdateInterval = value;
                Reset();
            }
        }

        public float RecordInterval
        {
            get
            {
                return m_RecordInterval;
            }
            set
            {
                if (value <= 0f)
                {
                    throw new Exception("Record interval is invalid.");
                }

                m_RecordInterval = value;
                Reset();
            }
        }

        public float CurrentSpeed
        {
            get
            {
                return m_CurrentSpeed;
            }
        }

        public void Shutdown()
        {
            Reset();
        }

        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (m_DownloadCounterNodes.Count <= 0)
            {
                return;
            }

            m_Accumulator += unscaledDeltaTime;
            if (m_Accumulator > m_RecordInterval)
            {
                m_Accumulator = m_RecordInterval;
            }

            m_TimeLeft -= unscaledDeltaTime;
            foreach (DownloadCounterNode downloadCounterNode in m_DownloadCounterNodes)
            {
                downloadCounterNode.OnUpdate(deltaTime, unscaledDeltaTime);
            }

            while (m_DownloadCounterNodes.Count > 0 && m_DownloadCounterNodes.Peek().ElapseSeconds >= m_RecordInterval)
            {
                m_DownloadCounterNodes.Dequeue();
            }

            if (m_DownloadCounterNodes.Count <= 0)
            {
                Reset();
                return;
            }

            if (m_TimeLeft <= 0f)
            {
                int totalDownloadLength = 0;
                foreach (DownloadCounterNode downloadCounterNode in m_DownloadCounterNodes)
                {
                    totalDownloadLength += downloadCounterNode.DownloadedLength;
                }


                m_CurrentSpeed = m_Accumulator > 0f ? totalDownloadLength / m_Accumulator : 0f;
                m_TimeLeft += m_UpdateInterval;
            }
        }

        public void RecordDownloadedLength(int downloadedLength)
        {
            if (downloadedLength <= 0)
            {
                return;
            }

            m_DownloadCounterNodes.Enqueue(new DownloadCounterNode(downloadedLength));
        }

        private void Reset()
        {
            m_DownloadCounterNodes.Clear();
            m_CurrentSpeed = 0f;
            m_Accumulator = 0f;
            m_TimeLeft = 0f;
        }
    }
}

