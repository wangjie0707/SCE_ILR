using System;

namespace Myth
{
    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager : ManagerBase, IDisposable
    {
        /// <summary>
        /// Socket事件
        /// </summary>
        public SocketEvent SocketEvent
        {
            private set;
            get;
        }

        /// <summary>
        /// 通用事件
        /// </summary>
        public CommonEvent CommonEvent
        {
            private set;
            get;
        }

        /// <summary>
        /// 获取通用事件处理函数的数量
        /// </summary>
        public int CommonEventHandlerCount
        {
            get
            {
                return CommonEvent.EventHandlerCount;
            }
        }

        /// <summary>
        /// 获取通用事件数量
        /// </summary>
        public int CommonEventCount
        {
            get
            {
                return CommonEvent.EventCount;
            }
        }


        /// <summary>
        /// 获取Socket事件处理函数的数量
        /// </summary>
        public int SocketEventHandlerCount
        {
            get
            {
                return SocketEvent.EventHandlerCount;
            }
        }


        public EventManager()
        {
            SocketEvent = new SocketEvent();
            CommonEvent = new CommonEvent();
        }



        /// <summary>
        /// 事件轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        internal void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            SocketEvent.OnUpdate(deltaTime, unscaledDeltaTime);
            CommonEvent.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        public override void Dispose()
        {
            SocketEvent.Dispose();
            CommonEvent.Dispose();
        }
    }
}