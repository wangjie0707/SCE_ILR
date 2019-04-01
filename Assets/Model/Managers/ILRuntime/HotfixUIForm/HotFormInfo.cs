using System;
using UnityEngine;

namespace Myth
{
    [Serializable]
    public class HotFormInfo
    {
        [SerializeField]
        private string m_Name = null;

        [SerializeField]
        private string m_TransformName = null;

        [SerializeField]
        private HotAttributeType m_HotAttributeType = HotAttributeType.Unknow;

        [SerializeField]
        private Transform m_Transform = null;

        /// <summary>
        /// 备注
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// 组件名字
        /// </summary>
        public string TransformName
        {
            get
            {
                return m_TransformName;
            }
            set
            {
                m_TransformName = value;
            }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public HotAttributeType HotAttributeType
        {
            get
            {
                return m_HotAttributeType;
            }
            set
            {
                m_HotAttributeType = value;
            }
        }

        /// <summary>
        /// 对应的实例
        /// </summary>
        public Transform Trans
        {
            get
            {
                return m_Transform;
            }
            set
            {
                m_Transform = value;
            }
        }
    }
}
