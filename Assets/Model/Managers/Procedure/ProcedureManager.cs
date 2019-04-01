//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    /// <summary>
    /// 流程管理器
    /// </summary>
    public class ProcedureManager : ManagerBase
    {
        /// <summary>
        /// 流程状态机
        /// </summary>
        private Fsm<ProcedureManager> m_CurrFsm;

        /// <summary>
        /// 当前流程状态机
        /// </summary>
        public Fsm<ProcedureManager> CurrFsm
        {
            get
            {
                return m_CurrFsm;
            }
        }


        /// <summary>
        /// 当前流程状态
        /// </summary>
        public ProcedureState CurrProcedureState
        {
            get
            {
                return (ProcedureState)m_CurrFsm.CurrStateType;
            }
        }

        /// <summary>
        /// 当前的流程
        /// </summary>
        public FsmState<ProcedureManager> CurrProcedure
        {
            get
            {
                return m_CurrFsm.GetState(m_CurrFsm.CurrStateType);
            }
        }

        public ProcedureManager()
        {

        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="states">流程管理器包含的流程</param>
        public void Init(params ProcedureBase[] states)
        {
            m_CurrFsm = GameEntry.Fsm.Create(this, states);
        }

        /// <summary>
        /// 开始流程
        /// </summary>
        /// <param name="procedureType">要开始的流程类型</param>
        public void StartProcedure(byte procedureType)
        {
            m_CurrFsm.Start(procedureType);
        }


        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(ProcedureState state)
        {
            m_CurrFsm.ChangeState((byte)state);
        }

        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            m_CurrFsm.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        public override void Dispose()
        {
            if (m_CurrFsm != null)
            {
                GameEntry.Fsm.DestoryFsm(m_CurrFsm);
                m_CurrFsm = null;
            }
            m_CurrFsm = null;
        }
    }

}
