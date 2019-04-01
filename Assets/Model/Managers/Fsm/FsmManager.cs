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
    /// 状态机管理器
    /// </summary>
    public class FsmManager : ManagerBase,System.IDisposable
    {
        /// <summary>
        /// 状态机字典
        /// </summary>
        private Dictionary<int, FsmBase> m_FsmDic;

        public FsmManager()
        {
            m_FsmDic = new Dictionary<int, FsmBase>();
        }

        /// <summary>
        /// 创建状态机
        /// </summary>
        /// <typeparam name="T">拥有者的类型</typeparam>
        /// <param name="fsmId">状态机编号</param>
        /// <param name="owner">拥有者</param>
        /// <param name="states">状态数组</param>
        /// <returns></returns>
        public Fsm<T> Create<T> (int fsmId,T owner, FsmState<T>[] states) where T : class
        {
            Fsm<T> fsm = new Fsm<T>(fsmId, owner, states);
            m_FsmDic[fsmId] = fsm;
            return fsm;
        }

        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <typeparam name="T">状态机持有者类型</typeparam>
        /// <param name="fsm">要销毁的状态机</param>
        public void DestoryFsm<T>(Fsm<T> fsm) where T :  class
        {
            int? fsmId =null;
            var enumerator = m_FsmDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Value == fsm)
                {
                    fsmId = enumerator.Current.Key;
                    break;
                }
            }
            if (fsmId.HasValue)
            {
                DestoryFsm(fsmId.Value);
            }
        }

        /// <summary>
        /// 销毁状态机
        /// </summary>
        /// <param name="fsmId"></param>
        public void DestoryFsm(int fsmId)
        {
            FsmBase fsm = null;
            if(m_FsmDic.TryGetValue(fsmId, out fsm))
            {
                fsm.ShutDown();
                m_FsmDic.Remove(fsmId);
            }

        }

        public override void Dispose()
        {
            var enumerator = m_FsmDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.ShutDown();
            }
            m_FsmDic.Clear();
        }
    }
}
