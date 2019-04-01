namespace Myth
{
    /// <summary>
    /// 状态机组件
    /// </summary>
    public class FsmComponent : GameBaseComponent
    {
        /// <summary>
        /// 状态机管理器
        /// </summary>
        private FsmManager m_FsmManager;

        /// <summary>
        /// 状态机临时编号
        /// </summary>
        private int m_TemFsmId = 0;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_FsmManager = new FsmManager();
        }

        #region Create 创建状态机
        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <typeparam name="T">拥有者的类型</typeparam>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态数组</param>
        /// <returns></returns>
        public Fsm<T> Create<T>(T owner, FsmState<T>[] states) where T : class
        {
            return m_FsmManager.Create<T>(m_TemFsmId++, owner, states);
        }

        #endregion

        #region DestoryFsm 销毁状态机
        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <param name="fsm">要销毁的状态机</param>
        public void DestoryFsm<T>(Fsm<T> fsm) where T :  class
        {
            m_FsmManager.DestoryFsm(fsm);
        }

        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <param name="fsmId">状态机编号</param>
        public void DestoryFsm(int fsmId)
        {
            m_FsmManager.DestoryFsm(fsmId);
        }
        #endregion


        public override void Shutdown()
        {
            base.Shutdown();
            m_FsmManager.Dispose();
        }
    }
}