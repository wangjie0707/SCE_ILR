using UnityEngine;

namespace Myth
{
    internal sealed class PlaySoundInfo
    {
        private readonly EntityBase m_BindingEntity;
        private readonly Vector3 m_WorldPosition;
        private readonly object m_UserData;

        public PlaySoundInfo(EntityBase bindingEntity, Vector3 worldPosition, object userData)
        {
            m_BindingEntity = bindingEntity;
            m_WorldPosition = worldPosition;
            m_UserData = userData;
        }

        public EntityBase BindingEntity
        {
            get
            {
                return m_BindingEntity;
            }
        }

        public Vector3 WorldPosition
        {
            get
            {
                return m_WorldPosition;
            }
        }

        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }
    }
}
