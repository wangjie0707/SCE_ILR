using System;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 流程组件
    /// </summary>
    public class ProcedureComponent : GameBaseComponent
    {
        private byte? m_EntranceProcedure = null;

        [SerializeField]
        private string[] m_AvailableProcedureTypeNames = null;

        [SerializeField]
        private string m_EntranceProcedureTypeName = null;

        /// <summary>
        /// 流程管理器
        /// </summary>
        private ProcedureManager m_ProcedureManager;

        /// <summary>
        /// 当前流程状态
        /// </summary>
        public ProcedureState CurrProcedureState
        {
            get
            {
                return m_ProcedureManager.CurrProcedureState;
            }
        }

        /// <summary>
        /// 当前的流程
        /// </summary>
        public FsmState<ProcedureManager> CurrProcedure
        {
            get
            {
                return m_ProcedureManager.CurrProcedure;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            m_ProcedureManager = new ProcedureManager();
        }

        protected override void OnStart()
        {
            base.OnStart();

            ProcedureBase[] states = new ProcedureBase[m_AvailableProcedureTypeNames.Length];
            for (int i = 0; i < m_AvailableProcedureTypeNames.Length; i++)
            {
                Type procedureType = AssemblyUtil.GetType(m_AvailableProcedureTypeNames[i]);
                if (procedureType == null)
                {
                    Log.Error("Can not find procedure type '{0}'.", m_AvailableProcedureTypeNames[i]);
                    return;
                }

                states[i] = (ProcedureBase)Activator.CreateInstance(procedureType);
                if (states[i] == null)
                {
                    Log.Error("Can not create procedure instance '{0}'.", m_AvailableProcedureTypeNames[i]);
                    return;
                }

                if (m_EntranceProcedureTypeName == m_AvailableProcedureTypeNames[i])
                {
                    m_EntranceProcedure = (byte)i;
                }
            }

            if (!m_EntranceProcedure.HasValue)
            {
                Log.Error("Entrance procedure is invalid.");
                return;
            }

            //要在Start时候初始化
            m_ProcedureManager.Init(states);
            m_ProcedureManager.StartProcedure(m_EntranceProcedure.Value);
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <typeparam name="TData">泛型类型</typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetData<TData>(string key, TData value)
        {
            m_ProcedureManager.CurrFsm.SetData<TData>(key, value);
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TData GetData<TData>(string key)
        {
            return m_ProcedureManager.CurrFsm.GetData<TData>(key);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(ProcedureState state)
        {
            m_ProcedureManager.ChangeState(state);
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_ProcedureManager.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_ProcedureManager.Dispose();
        }


    }
}
