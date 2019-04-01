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
    /// 通用事件
    /// </summary>
    public class CommonEvent : IDisposable
    {
        /// <summary>
        /// 事件队列
        /// </summary>
        private readonly Queue<GameEventBase> m_Events;

        public delegate void OnActionHandler(GameEventBase gameEventBase);
        private Dictionary<int, List<OnActionHandler>> m_EventHandlers;

        public CommonEvent()
        {
            m_Events = new Queue<GameEventBase>();
            m_EventHandlers = new Dictionary<int, List<OnActionHandler>>();
        }

       

        /// <summary>
        /// 获取事件处理函数的数量
        /// </summary>
        public int EventHandlerCount
        {
            get
            {
                return m_EventHandlers.Count;
            }
        }

        /// <summary>
        /// 获取事件数量
        /// </summary>
        public int EventCount
        {
            get
            {
                return m_Events.Count;
            }
        }


        #region AddEventListener 添加监听
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void AddEventListener(int key, OnActionHandler handler)
        {
            List<OnActionHandler> lstHandler = null;
            m_EventHandlers.TryGetValue(key, out lstHandler);
            if (lstHandler == null)
            {
                lstHandler = new List<OnActionHandler>();
                m_EventHandlers[key] = lstHandler;
            }
            lstHandler.Add(handler);
        }
        #endregion

        #region RemoveEventListener 移除监听
        /// <summary>
        /// 移除监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void RemoveEventListener(int key, OnActionHandler handler)
        {
            List<OnActionHandler> lstHandler = null;
            m_EventHandlers.TryGetValue(key, out lstHandler);

            if (lstHandler != null)
            {
                lstHandler.Remove(handler);
                if (lstHandler.Count == 0)
                {
                    m_EventHandlers.Remove(key);
                }
            }
        }
        #endregion



        #region Dispatch 派发
        /// <summary>
        /// 立刻派发事件，这不是线程安全的
        /// </summary>
        /// <param name="sender">事件发送者，一般填this即可</param>
        /// <param name="gameEventBase">事件内容</param>
        public void DispatchNow(object sender, GameEventBase gameEventBase)
        {
            List<OnActionHandler> lstHandler = null;
            m_EventHandlers.TryGetValue(gameEventBase.Id, out lstHandler);
            gameEventBase.Sender = sender;

            if (lstHandler != null)
            {
                int lstCount = lstHandler.Count; //获取集合数量 只调用一次
                for (int i = 0; i < lstCount; i++)
                {
                    //获取索引数据 只调用一次
                    OnActionHandler handele = lstHandler[i];

                    if (handele != null)
                    {
                        handele(gameEventBase);
                    }
                }
            }
        }


        /// <summary>
        /// 派发事件，这是线程安全的
        /// </summary>
        /// <param name="sender">事件发送者，一般填this即可</param>
        /// <param name="gameEventBase"></param>
        public void Dispatch(object sender, GameEventBase gameEventBase)
        {
            lock (m_Events)
            {
                gameEventBase.Sender = sender;
                m_Events.Enqueue(gameEventBase);
            }
        }

        #endregion

        /// <summary>
        /// 事件轮询
        /// </summary>
        internal void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            lock (m_Events)
            {
                while (m_Events.Count > 0)
                {
                    GameEventBase gameEventBase = m_Events.Dequeue();
                    DispatchNow(gameEventBase.Sender, gameEventBase);
                }

            }
        }

        public void Dispose()
        {
            m_EventHandlers.Clear();
            m_Events.Clear();
        }
    }
}
