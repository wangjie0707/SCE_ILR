namespace Myth
{
    /// <summary>
    /// Socket组件
    /// </summary>
    public class SocketComponent : GameBaseComponent
    {
        private SocketManager m_SocketManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_SocketManager = new SocketManager();
        }

        protected override void OnStart()
        {
            base.OnStart();
            m_MainSocket = CreateSocketTcpRoutine();
            SocketProtoListener.AddProtoListener();
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_SocketManager.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        /// <summary>
        /// 注册SocketTcp访问器
        /// </summary>
        /// <param name="routine"></param>
        internal void RegisterSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketManager.RegisterSocketTcpRoutine(routine);
        }

        /// <summary>
        /// 移除SocketTcp访问器
        /// </summary>
        /// <param name="routine"></param>
        internal void RemoveSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketManager.RemoveSocketTcpRoutine(routine);
        }


        /// <summary>
        /// 创建SocketTcp访问器
        /// </summary>
        /// <returns></returns>
        public SocketTcpRoutine CreateSocketTcpRoutine()
        {
            //从池中获取
            return GameEntry.Pool.SpawnClassObject<SocketTcpRoutine>();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_SocketManager.Dispose();
            GameEntry.Pool.UnSpawnClassObject(m_MainSocket);
            SocketProtoListener.RemoveProtoListener();
        }



        //=============================================

        /// <summary>
        /// 主Socket
        /// </summary>
        private SocketTcpRoutine m_MainSocket;

        /// <summary>
        /// 连接主Socket
        /// </summary>
        /// <param name="ip">Ip地址</param>
        /// <param name="port">端口号</param>
        public void ConnectToMainSocket(string ip, int port)
        {
            m_MainSocket.Connect(ip, port);
        }

        /// <summary>
        /// 连断开主Socket
        /// </summary>
        public void DisConnect()
        {
            m_MainSocket.DisConnect();
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="buffer"></param>
        public void SendMeg(byte[] buffer)
        {
            m_MainSocket.SendMsg(buffer);
        }
    }
}
