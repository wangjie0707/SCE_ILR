using System.Collections.Generic;
using System;


namespace Myth
{
    public class SocketManager : ManagerBase, IDisposable
    {
        /// <summary>
        /// SocketTcp访问器链表
        /// </summary>
        private LinkedList<SocketTcpRoutine> m_SocketTcpRoutineList;

        public SocketManager()
        {
            m_SocketTcpRoutineList = new LinkedList<SocketTcpRoutine>();
        }

        /// <summary>
        /// 注册SocketTcp访问器
        /// </summary>
        /// <param name="routine"></param>
        internal void RegisterSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketTcpRoutineList.AddFirst(routine);
        }

        /// <summary>
        /// 移除SocketTcp访问器
        /// </summary>
        /// <param name="routine"></param>
        internal void RemoveSocketTcpRoutine(SocketTcpRoutine routine)
        {
            m_SocketTcpRoutineList.Remove(routine);
        }

        internal void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            for (LinkedListNode<SocketTcpRoutine> curr = m_SocketTcpRoutineList.First; curr != null; curr = curr.Next)
            {
                curr.Value.OnUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        public override void Dispose()
        {
            m_SocketTcpRoutineList.Clear();
        }
    }
}
