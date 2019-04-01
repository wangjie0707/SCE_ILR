using System;
using System.Collections.Generic;

namespace Myth
{
    /// <summary>
    /// 实体信息
    /// </summary>
    public sealed class EntityInfo
    {
        private static readonly EntityBase[] EmptyArray = new EntityBase[] { };

        private readonly EntityBase m_Entity;
        private EntityStatus m_Status;
        private EntityBase m_ParentEntity;
        private List<EntityBase> m_ChildEntities;

        public EntityInfo(EntityBase entity)
        {
            if (entity == null)
            {
                throw new Exception("Entity is invalid.");
            }

            m_Entity = entity;
            m_Status = EntityStatus.WillInit;
            m_ParentEntity = null;
            m_ChildEntities = null;
        }

        public EntityBase Entity
        {
            get
            {
                return m_Entity;
            }
        }

        public EntityStatus Status
        {
            get
            {
                return m_Status;
            }
            set
            {
                m_Status = value;
            }
        }

        public EntityBase ParentEntity
        {
            get
            {
                return m_ParentEntity;
            }
            set
            {
                m_ParentEntity = value;
            }
        }

        public EntityBase[] GetChildEntities()
        {
            if (m_ChildEntities == null)
            {
                return EmptyArray;
            }

            return m_ChildEntities.ToArray();
        }

        public void GetChildEntities(List<EntityBase> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            if (m_ChildEntities == null)
            {
                return;
            }

            foreach (EntityBase childEntity in m_ChildEntities)
            {
                results.Add(childEntity);
            }
        }

        public void AddChildEntity(EntityBase childEntity)
        {
            if (m_ChildEntities == null)
            {
                m_ChildEntities = new List<EntityBase>();
            }

            if (m_ChildEntities.Contains(childEntity))
            {
                throw new Exception("Can not add child entity which is already exist.");
            }

            m_ChildEntities.Add(childEntity);
        }

        public void RemoveChildEntity(EntityBase childEntity)
        {
            if (m_ChildEntities == null || !m_ChildEntities.Remove(childEntity))
            {
                throw new Exception("Can not remove child entity which is not exist.");
            }
        }
    }
}
