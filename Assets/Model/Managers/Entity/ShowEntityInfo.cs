using System;

namespace Myth
{
    public sealed class ShowEntityInfo
    {
        private readonly int m_SerialId;
        private readonly int m_EntityId;
        private readonly EntityGroup m_EntityGroup;
        private readonly Type m_EntityLogicType;
        private readonly object m_UserData;

        public ShowEntityInfo(int serialId, int entityId, EntityGroup entityGroup, Type entityLogicType, object userData)
        {
            m_SerialId = serialId;
            m_EntityId = entityId;
            m_EntityGroup = entityGroup;
            m_EntityLogicType = entityLogicType;
            m_UserData = userData;
        }

        public int SerialId
        {
            get
            {
                return m_SerialId;
            }
        }

        public int EntityId
        {
            get
            {
                return m_EntityId;
            }
        }

        public EntityGroup EntityGroup
        {
            get
            {
                return m_EntityGroup;
            }
        }

        public Type EntityLogicType
        {
            get
            {
                return m_EntityLogicType;
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

