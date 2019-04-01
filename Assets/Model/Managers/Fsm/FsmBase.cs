//===================================================
//
//===================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;



namespace Myth
{
    /// <summary>
    /// 状态机基类
    /// </summary>
    public abstract class FsmBase
    {
        /// <summary>
        /// 状态机编号
        /// </summary>
        public int FsmId { get; private set; }

        /// <summary>
        /// 获取有限状态机持有者类型
        /// </summary>
        public abstract Type OwnerType
        {
            get;
        }

        /// <summary>
        /// 当前状态的类型 
        /// </summary>
        public byte CurrStateType;

        public FsmBase(int fsmId)
        {
            FsmId = fsmId;
        }

        /// <summary>
        /// 关闭状态机
        /// </summary>
        public abstract void ShutDown();
    }


}
