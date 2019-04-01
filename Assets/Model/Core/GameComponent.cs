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
    /// 组件基类
    /// </summary>
    public abstract class GameComponent : MonoBehaviour
    {
        /// <summary>
        /// 组件实例编号
        /// </summary>
        private int m_InstanceId;

        private void Awake()
        {
            m_InstanceId = GetInstanceID();
            OnAwake();
        }

        private void Start()
        {
            OnStart();
        }

        public int InsatnceId
        {
            get { return m_InstanceId; }
        }

        protected virtual void OnAwake() { }

        protected virtual void OnStart() { }
    }
}