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
    /// 状态机
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型</typeparam>
    public class Fsm<T> : FsmBase where T : class
    {
        /// <summary>
        /// 状态机是否被销毁
        /// </summary>
        private bool m_IsDestroyed;

        /// <summary>
        /// 状态机拥有者
        /// </summary>
        private readonly T m_Owner;

        /// <summary>
        /// 当前状态
        /// </summary>
        private FsmState<T> m_CurrState;

        /// <summary>
        /// 状态字典
        /// </summary>
        private Dictionary<byte, FsmState<T>> m_StateDic;

        /// <summary>
        /// 参数字典
        /// </summary>
        private Dictionary<string, VariableBase> m_ParamDic;

        /// <summary>
        /// 获取有限状态机是否正在运行
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return m_CurrState != null;
            }
        }

        /// <summary>
        /// 获取有限状态机是否被销毁
        /// </summary>
        public bool IsDestroyed
        {
            get
            {
                return m_IsDestroyed;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态数组</param>
        public Fsm(int fsmId, T owner, FsmState<T>[] states) : base(fsmId)
        {
            if (owner == null)
            {
                throw new Exception("FSM owner is invalid.");
            }

            if (states == null || states.Length < 1)
            {
                throw new Exception("FSM states is invalid.");
            }

            m_Owner = owner;
            m_StateDic = new Dictionary<byte, FsmState<T>>();
            m_ParamDic = new Dictionary<string, VariableBase>();

            //把状态加入字典
            int len = states.Length;
            for (int i = 0; i < len; i++)
            {
                FsmState<T> state = states[i];
                state.CurrFsm = this;
                m_StateDic[(byte)i] = state;
                m_StateDic[(byte)i].OnInit();
            }

            m_CurrState = null;
            m_IsDestroyed = false;
        }


        /// <summary>
        /// 获取有限状态机持有者
        /// </summary>
        public T Owner
        {
            get
            {
                return m_Owner;
            }
        }

        /// <summary>
        /// 获取有限状态机持有者类型
        /// </summary>
        public override Type OwnerType
        {
            get
            {
                return typeof(T);
            }
        }


        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="stateType"></param>
        /// <returns></returns>
        public FsmState<T> GetState(byte stateType)
        {
            FsmState<T> state = null;
            m_StateDic.TryGetValue(stateType, out state);
            return state;
        }

        /// <summary>
        /// 开始有限状态机
        /// </summary>
        /// <param name="stateType">要开始的有限状态机状态类型</param>
        public void Start(byte stateType)
        {
            if (IsRunning)
            {
                throw new Exception("FSM is running, can not start again.");
            }

            if (stateType >= m_StateDic.Count)
            {
                throw new Exception("State type is invalid.");
            }

            FsmState<T> state = GetState(stateType);
            if (state == null)
            {
                throw new Exception("FSM  can not start state .");
            }

            m_CurrState = state;
            m_CurrState.OnEnter();
        }


        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (m_CurrState != null)
            {
                m_CurrState.OnUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="newState"></param>
        public void ChangeState(byte newState)
        {
            //两个状态一样 不重复进入
            if (CurrStateType == newState)
            {
                return;
            }

            if (m_CurrState != null)
            {
                m_CurrState.OnLeave();
            }

            CurrStateType = newState;
            m_CurrState = m_StateDic[CurrStateType];

            //进入新状态
            m_CurrState.OnEnter();
        }

        /// <summary>
        /// 设置参数值
        /// </summary>
        /// <typeparam name="TData">泛型类型</typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetData<TData>(string key, TData value)
        {
            VariableBase itemBase = null;
            if (m_ParamDic.TryGetValue(key, out itemBase))
            {
                Debug.Log("修改已有值");
                Variable<TData> item = itemBase as Variable<TData>;
                item.Value = value;
                m_ParamDic[key] = item;
            }
            else
            {
                Debug.Log("参数原来不存在");
                //参数原来不存在
                Variable<TData> item = new Variable<TData>();
                item.Value = value;
                m_ParamDic[key] = item;
            }
        }

        /// <summary>
        /// 获取参数值
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public TData GetData<TData>(string key)
        {
            VariableBase itemBase = null;
            if (m_ParamDic.TryGetValue(key, out itemBase))
            {
                Variable<TData> item = itemBase as Variable<TData>;
                return item.Value;
            }
            return default(TData);
        }


        /// <summary>
        /// 关闭状态机
        /// </summary>
        public override void ShutDown()
        {
            if (m_CurrState != null)
            {
                m_CurrState.OnLeave();
                m_CurrState = null;
            }
            var enumerator = m_StateDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.OnDestory();
            }

            m_StateDic.Clear();
            m_ParamDic.Clear();

            m_IsDestroyed = true;
        }
    }
}
