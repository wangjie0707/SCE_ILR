﻿//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    /// <summary>
    /// 状态机的状态
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FsmState<T> where T : class
    {
        /// <summary>
        /// 状态对应的状态机
        /// </summary>
        public Fsm<T> CurrFsm;

        /// <summary>
        /// 初始化状态
        /// </summary>
        public virtual void OnInit() { }

        /// <summary>
        /// 进入状态
        /// </summary>
        public virtual void OnEnter() { }
        
        /// <summary>
        /// 执行状态
        /// </summary>
        public virtual void OnUpdate(float deltaTime, float unscaledDeltaTime) { }

        /// <summary>
        /// 离开状态
        /// </summary>
        public virtual void OnLeave() { }

        /// <summary>
        /// 状态机销毁时候调用
        /// </summary>
        public virtual void OnDestory() { }
    }
}
