using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 附加子物体信息
    /// </summary>
    internal sealed class AttachEntityInfo
    {
        private readonly Transform m_ParentTransform;
        private readonly object m_UserData;

        public AttachEntityInfo(Transform parentTransform, object userData)
        {
            m_ParentTransform = parentTransform;
            m_UserData = userData;
        }

        /// <summary>
        /// 父物体
        /// </summary>
        public Transform ParentTransform
        {
            get
            {
                return m_ParentTransform;
            }
        }

        /// <summary>
        /// 用户数据
        /// </summary>
        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }
    }
}
