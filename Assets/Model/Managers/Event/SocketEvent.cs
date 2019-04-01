using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Myth
{
    /// <summary>
    /// Socket事件
    /// </summary>
    public class SocketEvent : IDisposable
    {
        public delegate void OnActionHandler(byte[] buffer);
        private Dictionary<ushort, List<OnActionHandler>> m_EventHandlers;

        public SocketEvent()
        {
            m_EventHandlers = new Dictionary<ushort, List<OnActionHandler>>();
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


        #region AddEventListener 添加监听
        /// <summary>
        /// 添加监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public void AddEventListener(ushort key, OnActionHandler handler)
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
        public void RemoveEventListener(ushort key, OnActionHandler handler)
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
        /// 派发
        /// </summary>
        /// <param name="key"></param>
        /// <param name="p"></param>
        public void Dispatch(ushort key, byte[] buffer)
        {
            List<OnActionHandler> lstHandler = null;
            m_EventHandlers.TryGetValue(key, out lstHandler);

            if (lstHandler != null)
            {
                int lstCount = lstHandler.Count; //获取集合数量 只调用一次
                for (int i = 0; i < lstCount; i++)
                {
                    //获取索引数据 只调用一次
                    OnActionHandler handele = lstHandler[i];

                    if (handele != null)
                    {
                        handele(buffer);
                    }
                }
            }
        }

        public void Dispatch(ushort key)
        {
            Dispatch(key, null);
        }
        
        #endregion



        /// <summary>
        /// 事件轮询
        /// </summary>
        internal void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {

        }


        public void Dispose()
        {
            m_EventHandlers.Clear();
        }
    }
}