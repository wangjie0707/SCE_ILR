namespace Myth
{
    /// <summary>
    /// 事件组件
    /// </summary>
    public class EventComponent : GameBaseComponent
    {
        /// <summary>
        /// 事件管理器
        /// </summary>
        private EventManager m_EventManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_EventManager = new EventManager();

            SocketEvent = m_EventManager.SocketEvent;
            CommonEvent = m_EventManager.CommonEvent;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_EventManager.Dispose();
        }

        /// <summary>
        /// 事件轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_EventManager.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        /// <summary>
        /// Socket事件
        /// </summary>
        public SocketEvent SocketEvent;

        /// <summary>
        /// 通用事件
        /// </summary>
        public CommonEvent CommonEvent;
    }
}