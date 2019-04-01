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
    /// 定时器
    /// </summary>
    public class TimeAction
    {


        /// <summary>
        /// 是否运行中
        /// </summary>
        public bool IsRuning
        {
            get;
            private set;
        }

        /// <summary>
        /// 定时器是否暂停
        /// </summary>
        private bool m_IsPause;
        
        /// <summary>
        /// 定时器名称
        /// </summary>
        private string m_TimeActionName;

        /// <summary>
        /// 暂停时长
        /// </summary>
        private float m_PauseTime;

        /// <summary>
        /// 单次运行时间
        /// </summary>
        private float m_OnceTime;

        /// <summary>
        /// 开始运行时间
        /// </summary>
        private float m_StartTime;

        /// <summary>
        /// 当前运行的时间
        /// </summary>
        private float m_CurrRunTime;

        /// <summary>
        /// 当前循环次数
        /// </summary>
        private int m_CurrLoop;

        /// <summary>
        /// 延迟时间
        /// </summary>
        private float m_DelayTime;

        /// <summary>
        /// 间隔 (秒)
        /// </summary>
        private float m_Interval;

        /// <summary>
        /// 循环次数(-1表示无限循环 0也会循环一次)
        /// </summary>
        private int m_Loop;

        /// <summary>
        /// 开始运行
        /// </summary>
        private Action m_OnStart;

        /// <summary>
        /// 运行中 参数表示剩余次数
        /// </summary>
        private Action<int> m_OnUpdate;

        /// <summary>
        /// 运行完毕
        /// </summary>
        private Action m_OnComplete;


        /// <summary>
        /// 获取定时器是否暂停
        /// </summary>
        public bool IsPause
        {
            get
            {
                return m_IsPause;
            }
        }

        /// <summary>
        /// 获取大世界单次运行了多少时间
        /// </summary>
        public float OnceTime
        {
            get
            {
                return m_OnceTime;
            }
        }

        /// <summary>
        /// 获取定时器开始运行时间
        /// </summary>
        public float StartTime
        {
            get
            {
                return m_StartTime;
            }
        }

        /// <summary>
        /// 获取定时器暂停了多久
        /// </summary>
        public float PauseTime
        {
            get
            {
                return m_PauseTime;
            }
        }

        /// <summary>
        /// 得到定时器延迟时间
        /// </summary>
        public float Delaytime
        {
            get
            {
                return m_DelayTime;
            }
        }

        /// <summary>
        /// 得到定时器每次间隔时间
        /// </summary>
        public float Interval
        {
            get
            {
                return m_Interval;
            }
        }

        /// <summary>
        /// 得到定时器总循环次数
        /// </summary>
        public int Loop
        {
            get
            {
                return m_Loop;
            }
        }

        /// <summary>
        /// 得到定时器当前循环次数
        /// </summary>
        public int CurrLoop
        {
            get
            {
                return m_CurrLoop;
            }
        }

        /// <summary>
        /// 得到定时器当前运行时间
        /// </summary>
        public float CurrRunTime
        {
            get
            {
                return m_CurrRunTime;
            }
        }

        /// <summary>
        /// 定时器名称
        /// </summary>
        public string TimeActionName
        {
            get
            {
                return m_TimeActionName;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeActionName">定时器名称</param>
        /// <param name="delaytime">延迟时间</param>
        /// <param name="interval">间隔</param>
        /// <param name="loop">循环次数</param>
        /// <param name="onStart">开始回调</param>
        /// <param name="onUpdate">运行中回调</param>
        /// <param name="onComplete">运行完毕回调</param>
        /// <returns></returns>
        public TimeAction Init(string timeActionName, float delaytime, float interval, int loop, Action onStart, Action<int> onUpdate, Action onComplete)
        {
            m_TimeActionName = timeActionName;
            m_DelayTime = delaytime;
            m_Interval = interval;
            m_Loop = loop;
            m_OnStart = onStart;
            m_OnUpdate = onUpdate;
            m_OnComplete = onComplete;

            m_PauseTime = 0;
            m_StartTime = -1;
            m_OnceTime = -1;
            return this;
        }

        /// <summary>
        /// 运行
        /// </summary>
        public void Run()
        {
            //1.需要先把自己加入时间管理器的链表中
            GameEntry.Time.RegisterTimeAction(this);

            //2.设置当前运行的时间
            m_CurrRunTime = Time.time;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            m_IsPause = true;
        }

        /// <summary>
        /// 恢复暂停
        /// </summary>
        public void RePause()
        {
            m_IsPause = false;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (m_OnComplete != null)
            {
                m_OnComplete();
            }

            IsRuning = false;

            //把自己从定时器链表移除
            GameEntry.Time.RemoveTimeAction(this);
        }

        /// <summary>
        /// 每帧执行
        /// </summary>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (m_IsPause)
            {
                m_PauseTime += Time.deltaTime;
                return;
            }

            if (!IsRuning && Time.time > m_CurrRunTime + m_DelayTime + m_PauseTime)
            {
                //当程序执行到这里 表示已经第一次过了延迟时间
                IsRuning = true;
                m_CurrRunTime = Time.time;
                m_StartTime = Time.time;

                if (m_OnStart != null)
                {
                    m_OnStart();
                }
            }

            if (!IsRuning) return;

            m_OnceTime += Time.deltaTime;
            if (Time.time > m_CurrRunTime + m_PauseTime)
            {
                m_OnceTime = 0;
                m_CurrRunTime = Time.time + m_Interval - m_PauseTime;

                //以下代码 间隔m_Interval 时间 执行一次
                if (m_OnUpdate != null)
                {
                    m_OnUpdate(m_Loop - m_CurrLoop);
                }

                if (m_Loop > -1)
                {
                    m_CurrLoop++;
                    if (m_CurrLoop >= m_Loop)
                    {
                        Stop();
                    }
                }
            }
        }
    }
}