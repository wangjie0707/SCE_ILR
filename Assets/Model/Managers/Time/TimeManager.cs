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
    public class TimeManager : ManagerBase, IDisposable
    {
        /// <summary>
        /// 定时器链表
        /// </summary>
        private LinkedList<TimeAction> m_TimeActionList;

        /// <summary>
        /// 定时器数量
        /// </summary>
        public int TimeActionCount
        {
            get
            {
                return m_TimeActionList.Count;
            }
        }



        public TimeManager()
        {
            m_TimeActionList = new LinkedList<TimeAction>();
        }

        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RegisterTimeAction(TimeAction action)
        {
            m_TimeActionList.AddLast(action);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RemoveTimeAction(TimeAction action)
        {
            m_TimeActionList.Remove(action);
        }

        internal void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            for (LinkedListNode<TimeAction> curr = m_TimeActionList.First; curr != null; curr = curr.Next)
            {
                curr.Value.OnUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        /// <summary>
        /// 得到所有定时器
        /// </summary>
        /// <returns>得到的定时器集合</returns>
        internal TimeAction[] GetAllTimeAction()
        {
            int index = 0;

            TimeAction[] results = new TimeAction[m_TimeActionList.Count];
            foreach (var timeAction in m_TimeActionList)
            {
                results[index++] = timeAction;
            }
            return results;
        }

        /// <summary>
        /// 得到所有定时器
        /// </summary>
        internal void GetAllTimeAction(List<TimeAction> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (var timeAction in m_TimeActionList)
            {
                results.Add(timeAction);
            }
        }

        public override void Dispose()
        {
            m_TimeActionList.Clear();
        }
    }
}
