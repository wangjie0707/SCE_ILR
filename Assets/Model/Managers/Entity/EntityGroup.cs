using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 实体组
    /// </summary>
    public sealed class EntityGroup
    {
        private readonly string m_Name;
        private readonly IObjectPool<EntityInstanceObject> m_InstancePool;
        private readonly LinkedList<EntityBase> m_Entities;
        private readonly Transform m_GroupTransform;

        /// <summary>
        /// 初始化实体组的新实例
        /// </summary>
        /// <param name="name">实体组名称</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="instanceCapacity">实体实例对象池容量</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数</param>
        /// <param name="instancePriority">实体实例对象池的优先级</param>
        /// <param name="instancePriority">实体组Transform</param>
        public EntityGroup(string name, float instanceAutoReleaseInterval, int instanceCapacity, float instanceExpireTime, int instancePriority, Transform groupTransform)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("Entity group name is invalid.");
            }

            m_Name = name;
            m_InstancePool = GameEntry.Pool.CreateSingleSpawnObjectPool<EntityInstanceObject>(TextUtil.Format("Entity Instance Pool ({0})", name), instanceCapacity, instanceExpireTime, instancePriority);
            m_InstancePool.AutoReleaseInterval = instanceAutoReleaseInterval;
            m_Entities = new LinkedList<EntityBase>();
            m_GroupTransform = groupTransform;
        }



        /// <summary>
        /// 获取实体组名称
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 获取实体组Transform
        /// </summary>
        public Transform GroupTransform
        {
            get
            {
                return m_GroupTransform;
            }
        }

        /// <summary>
        /// 获取实体组中实体数量
        /// </summary>
        public int EntityCount
        {
            get
            {
                return m_Entities.Count;
            }
        }

        /// <summary>
        /// 获取或设置实体组实例对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get
            {
                return m_InstancePool.AutoReleaseInterval;
            }
            set
            {
                m_InstancePool.AutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 获取或设置实体组实例对象池的容量
        /// </summary>
        public int InstanceCapacity
        {
            get
            {
                return m_InstancePool.Capacity;
            }
            set
            {
                m_InstancePool.Capacity = value;
            }
        }

        /// <summary>
        /// 获取或设置实体组实例对象池对象过期秒数
        /// </summary>
        public float InstanceExpireTime
        {
            get
            {
                return m_InstancePool.ExpireTime;
            }
            set
            {
                m_InstancePool.ExpireTime = value;
            }
        }

        /// <summary>
        /// 获取或设置实体组实例对象池的优先级
        /// </summary>
        public int InstancePriority
        {
            get
            {
                return m_InstancePool.Priority;
            }
            set
            {
                m_InstancePool.Priority = value;
            }
        }

        /// <summary>
        /// 实体组轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            LinkedListNode<EntityBase> current = m_Entities.First;
            while (current != null)
            {
                LinkedListNode<EntityBase> next = current.Next;
                current.Value.OnUpdate(deltaTime, unscaledDeltaTime);
                current = next;
            }
        }

        /// <summary>
        /// 实体组中是否存在实体
        /// </summary>
        /// <param name="entityId">实体序列编号</param>
        /// <returns>实体组中是否存在实体</returns>
        public bool HasEntity(int entityId)
        {
            foreach (EntityBase entity in m_Entities)
            {
                if (entity.Id == entityId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 实体组中是否存在实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>实体组中是否存在实体</returns>
        public bool HasEntity(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                throw new Exception("Entity asset name is invalid.");
            }

            foreach (EntityBase entity in m_Entities)
            {
                if (entity.EntityAssetName == entityAssetName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 从实体组中获取实体
        /// </summary>
        /// <param name="entityId">实体序列编号</param>
        /// <returns>要获取的实体</returns>
        public EntityBase GetEntity(int entityId)
        {
            foreach (EntityBase entity in m_Entities)
            {
                if (entity.Id == entityId)
                {
                    return entity;
                }
            }

            return null;
        }

        /// <summary>
        /// 从实体组中获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public EntityBase GetEntity(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                throw new Exception("Entity asset name is invalid.");
            }

            foreach (EntityBase entity in m_Entities)
            {
                if (entity.EntityAssetName == entityAssetName)
                {
                    return entity;
                }
            }

            return null;
        }

        /// <summary>
        /// 从实体组中获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public EntityBase[] GetEntities(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                throw new Exception("Entity asset name is invalid.");
            }

            List<EntityBase> results = new List<EntityBase>();
            foreach (EntityBase entity in m_Entities)
            {
                if (entity.EntityAssetName == entityAssetName)
                {
                    results.Add(entity);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 从实体组中获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="results">要获取的实体</param>
        public void GetEntities(string entityAssetName, List<EntityBase> results)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                throw new Exception("Entity asset name is invalid.");
            }

            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (EntityBase entity in m_Entities)
            {
                if (entity.EntityAssetName == entityAssetName)
                {
                    results.Add(entity);
                }
            }
        }

        /// <summary>
        /// 从实体组中获取所有实体
        /// </summary>
        /// <returns>实体组中的所有实体</returns>
        public EntityBase[] GetAllEntities()
        {
            List<EntityBase> results = new List<EntityBase>();
            foreach (EntityBase entity in m_Entities)
            {
                results.Add(entity);
            }

            return results.ToArray();
        }

        /// <summary>
        /// 从实体组中获取所有实体
        /// </summary>
        /// <param name="results">实体组中的所有实体</param>
        public void GetAllEntities(List<EntityBase> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (EntityBase entity in m_Entities)
            {
                results.Add(entity);
            }
        }

        /// <summary>
        /// 往实体组增加实体
        /// </summary>
        /// <param name="entity">要增加的实体</param>
        public void AddEntity(EntityBase entity)
        {
            m_Entities.AddLast(entity);
        }

        /// <summary>
        /// 从实体组移除实体
        /// </summary>
        /// <param name="entity">要移除的实体</param>
        public void RemoveEntity(EntityBase entity)
        {
            m_Entities.Remove(entity);
        }

        public void RegisterEntityInstanceObject(EntityInstanceObject obj, bool spawned)
        {
            m_InstancePool.Register(obj, spawned);
        }

        public EntityInstanceObject SpawnEntityInstanceObject(string name)
        {
            return m_InstancePool.Spawn(name);
        }

        public void UnspawnEntity(EntityBase entity)
        {
            m_InstancePool.Unspawn(entity.InstanceGameObject);
        }

        public void SetEntityInstanceLocked(object entityInstance, bool locked)
        {
            if (entityInstance == null)
            {
                throw new Exception("Entity instance is invalid.");
            }

            m_InstancePool.SetLocked(entityInstance, locked);
        }

        public void SetEntityInstancePriority(object entityInstance, int priority)
        {
            if (entityInstance == null)
            {
                throw new Exception("Entity instance is invalid.");
            }

            m_InstancePool.SetPriority(entityInstance, priority);
        }
    }
}
