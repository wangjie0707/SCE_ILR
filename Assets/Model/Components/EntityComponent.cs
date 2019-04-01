using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 游戏对象组件
    /// </summary>
    public class EntityComponent : GameBaseComponent, IUpdateComponent
    {
        private const int DefaultPriority = 0;

        private readonly List<EntityBase> m_InternalEntityResultsCache = new List<EntityBase>();

        [SerializeField]
        private bool m_EnableShowEntitySuccessEvent = true;

        [SerializeField]
        private bool m_EnableShowEntityFailureEvent = true;

        [SerializeField]
        private bool m_EnableShowEntityUpdateEvent = false;

        [SerializeField]
        private bool m_EnableShowEntityDependencyAssetEvent = false;

        [SerializeField]
        private bool m_EnableHideEntityCompleteEvent = true;

        [SerializeField]
        private EntityGroupInfo[] m_EntityGroupInfos = null;

        /// <summary>
        /// 实体管理器
        /// </summary>
        private EntityManager m_EntityManager;

        /// <summary>
        /// 获取实体数量
        /// </summary>
        public int EntityCount
        {
            get
            {
                return m_EntityManager.EntityCount;
            }
        }

        /// <summary>
        /// 获取实体组数量
        /// </summary>
        public int EntityGroupCount
        {
            get
            {
                return m_EntityManager.EntityGroupCount;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            m_EntityManager = new EntityManager();

            m_EntityManager.ShowEntitySuccessCallBack += OnShowEntitySuccess;
            m_EntityManager.ShowEntityFailureCallBack += OnShowEntityFailure;
            m_EntityManager.ShowEntityUpdateCallBack += OnShowEntityUpdate;
            m_EntityManager.ShowEntityDependencyAssetCallBack += OnShowEntityDependencyAsset;
            m_EntityManager.HideEntityCompleteCallBack += OnHideEntityComplete;
        }

        protected override void OnStart()
        {
            base.OnStart();

            for (int i = 0; i < m_EntityGroupInfos.Length; i++)
            {
                if (!AddEntityGroup(m_EntityGroupInfos[i].Name, m_EntityGroupInfos[i].InstanceAutoReleaseInterval, m_EntityGroupInfos[i].InstanceCapacity, m_EntityGroupInfos[i].InstanceExpireTime, m_EntityGroupInfos[i].InstancePriority))
                {
                    Debug.LogWarning(string.Format("Add entity group '{0}' failure.", m_EntityGroupInfos[i].Name));
                    continue;
                }
            }
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_EntityManager.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_EntityManager.Dispose();
        }

        /// <summary>
        /// 是否存在实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>是否存在实体组</returns>
        public bool HasEntityGroup(string entityGroupName)
        {
            return m_EntityManager.HasEntityGroup(entityGroupName);
        }

        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>要获取的实体组</returns>
        public EntityGroup GetEntityGroup(string entityGroupName)
        {
            return m_EntityManager.GetEntityGroup(entityGroupName);
        }

        /// <summary>
        /// 获取所有实体组
        /// </summary>
        /// <returns>所有实体组</returns>
        public EntityGroup[] GetAllEntityGroups()
        {
            return m_EntityManager.GetAllEntityGroups();
        }

        /// <summary>
        /// 获取所有实体组
        /// </summary>
        /// <param name="results">所有实体组</param>
        public void GetAllEntityGroups(List<EntityGroup> results)
        {
            m_EntityManager.GetAllEntityGroups(results);
        }

        /// <summary>
        /// 增加实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="instanceCapacity">实体实例对象池容量</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数</param>
        /// <param name="instancePriority">实体实例对象池的优先级</param>
        /// <returns>是否增加实体组成功</returns>
        public bool AddEntityGroup(string entityGroupName, float instanceAutoReleaseInterval, int instanceCapacity, float instanceExpireTime, int instancePriority)
        {
            if (m_EntityManager.HasEntityGroup(entityGroupName))
            {
                return false;
            }

            GameObject entityGroup = new GameObject();
            entityGroup.name = TextUtil.Format("Entity Group - {0}", entityGroupName);
            Transform transform = entityGroup.transform;
            transform.SetParent(this.transform);
            transform.localScale = Vector3.one;

            return m_EntityManager.AddEntityGroup(entityGroupName, instanceAutoReleaseInterval, instanceCapacity, instanceExpireTime, instancePriority, transform);
        }

        /// <summary>
        /// 是否存在实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>是否存在实体</returns>
        public bool HasEntity(int entityId)
        {
            return m_EntityManager.HasEntity(entityId);
        }

        /// <summary>
        /// 是否存在实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>是否存在实体</returns>
        public bool HasEntity(string entityAssetName)
        {
            return m_EntityManager.HasEntity(entityAssetName);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>实体</returns>
        public EntityBase GetEntity(int entityId)
        {
            return m_EntityManager.GetEntity(entityId);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public EntityBase GetEntity(string entityAssetName)
        {
            return m_EntityManager.GetEntity(entityAssetName);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public EntityBase[] GetEntities(string entityAssetName)
        {
            EntityBase[] entities = m_EntityManager.GetEntities(entityAssetName);
            EntityBase[] entityImpls = new EntityBase[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                entityImpls[i] = entities[i];
            }

            return entityImpls;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="results">要获取的实体</param>
        public void GetEntities(string entityAssetName, List<EntityBase> results)
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            m_EntityManager.GetEntities(entityAssetName, m_InternalEntityResultsCache);
            foreach (EntityBase entity in m_InternalEntityResultsCache)
            {
                results.Add(entity);
            }
        }

        /// <summary>
        /// 获取所有已加载的实体
        /// </summary>
        /// <returns>所有已加载的实体</returns>
        public EntityBase[] GetAllLoadedEntities()
        {
            EntityBase[] entities = m_EntityManager.GetAllLoadedEntities();
            EntityBase[] entityImpls = new EntityBase[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                entityImpls[i] = entities[i];
            }

            return entityImpls;
        }

        /// <summary>
        /// 获取所有已加载的实体
        /// </summary>
        /// <param name="results">所有已加载的实体</param>
        public void GetAllLoadedEntities(List<EntityBase> results)
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            m_EntityManager.GetAllLoadedEntities(m_InternalEntityResultsCache);
            foreach (EntityBase entity in m_InternalEntityResultsCache)
            {
                results.Add(entity);
            }
        }

        /// <summary>
        /// 获取所有正在加载实体的编号
        /// </summary>
        /// <returns>所有正在加载实体的编号</returns>
        public int[] GetAllLoadingEntityIds()
        {
            return m_EntityManager.GetAllLoadingEntityIds();
        }

        /// <summary>
        /// 获取所有正在加载实体的编号
        /// </summary>
        /// <param name="results">所有正在加载实体的编号</param>
        public void GetAllLoadingEntityIds(List<int> results)
        {
            m_EntityManager.GetAllLoadingEntityIds(results);
        }

        /// <summary>
        /// 是否正在加载实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>是否正在加载实体</returns>
        public bool IsLoadingEntity(int entityId)
        {
            return m_EntityManager.IsLoadingEntity(entityId);
        }

        /// <summary>
        /// 是否是合法的实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>实体是否合法</returns>
        public bool IsValidEntity(EntityBase entity)
        {
            return m_EntityManager.IsValidEntity(entity);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类型</typeparam>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        public void ShowEntity<T>(int entityId, string entityAssetName, string entityGroupName) where T : EntityBase
        {
            ShowEntity(entityId, typeof(T), entityAssetName, entityGroupName, DefaultPriority, null);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityBase">实体逻辑类型</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        public void ShowEntity(int entityId, Type entityBase, string entityAssetName, string entityGroupName)
        {
            ShowEntity(entityId, entityBase, entityAssetName, entityGroupName, DefaultPriority, null);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类型</typeparam>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="priority">加载实体资源的优先级</param>
        public void ShowEntity<T>(int entityId, string entityAssetName, string entityGroupName, int priority) where T : EntityBase
        {
            ShowEntity(entityId, typeof(T), entityAssetName, entityGroupName, priority, null);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityBase">实体逻辑类型</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="priority">加载实体资源的优先级</param>
        public void ShowEntity(int entityId, Type entityBase, string entityAssetName, string entityGroupName, int priority)
        {
            ShowEntity(entityId, entityBase, entityAssetName, entityGroupName, priority, null);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类型</typeparam>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity<T>(int entityId, string entityAssetName, string entityGroupName, object userData) where T : EntityBase
        {
            ShowEntity(entityId, typeof(T), entityAssetName, entityGroupName, DefaultPriority, userData);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityLogicType">实体逻辑类型</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity(int entityId, Type entityBase, string entityAssetName, string entityGroupName, object userData)
        {
            ShowEntity(entityId, entityBase, entityAssetName, entityGroupName, DefaultPriority, userData);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <typeparam name="T">实体逻辑类型</typeparam>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="priority">加载实体资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity<T>(int entityId, string entityAssetName, string entityGroupName, int priority, object userData) where T : EntityBase
        {
            ShowEntity(entityId, typeof(T), entityAssetName, entityGroupName, priority, userData);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityBase">实体逻辑类型</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="priority">加载实体资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity(int entityId, Type entityBase, string entityAssetName, string entityGroupName, int priority, object userData)
        {
            if (entityBase == null)
            {
                Debug.LogError("Entity type is invalid.");
                return;
            }

            m_EntityManager.ShowEntity(entityId, entityAssetName, entityGroupName, priority, entityBase, userData);
        }


        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        public void HideEntity(int entityId)
        {
            m_EntityManager.HideEntity(entityId);
        }

        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void HideEntity(int entityId, object userData)
        {
            m_EntityManager.HideEntity(entityId, userData);
        }

        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void HideEntity(EntityBase entity)
        {
            m_EntityManager.HideEntity(entity);
        }

        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void HideEntity(EntityBase entity, object userData)
        {
            m_EntityManager.HideEntity(entity, userData);
        }

        /// <summary>
        /// 隐藏所有已加载的实体
        /// </summary>
        public void HideAllLoadedEntities()
        {
            m_EntityManager.HideAllLoadedEntities();
        }

        /// <summary>
        /// 隐藏所有已加载的实体
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void HideAllLoadedEntities(object userData)
        {
            m_EntityManager.HideAllLoadedEntities(userData);
        }

        /// <summary>
        /// 隐藏所有正在加载的实体
        /// </summary>
        public void HideAllLoadingEntities()
        {
            m_EntityManager.HideAllLoadingEntities();
        }

        /// <summary>
        /// 获取父实体
        /// </summary>
        /// <param name="childEntityId">要获取父实体的子实体的实体编号</param>
        /// <returns>子实体的父实体</returns>
        public EntityBase GetParentEntity(int childEntityId)
        {
            return m_EntityManager.GetParentEntity(childEntityId);
        }

        /// <summary>
        /// 获取父实体
        /// </summary>
        /// <param name="childEntity">要获取父实体的子实体</param>
        /// <returns>子实体的父实体</returns>
        public EntityBase GetParentEntity(EntityBase childEntity)
        {
            return m_EntityManager.GetParentEntity(childEntity);
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntityId">要获取子实体的父实体的实体编号</param>
        /// <returns>子实体数组</returns>
        public EntityBase[] GetChildEntities(int parentEntityId)
        {
            EntityBase[] entities = m_EntityManager.GetChildEntities(parentEntityId);
            EntityBase[] entityImpls = new EntityBase[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                entityImpls[i] = entities[i];
            }

            return entityImpls;
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntityId">要获取子实体的父实体的实体编号</param>
        /// <param name="results">子实体数组</param>
        public void GetChildEntities(int parentEntityId, List<EntityBase> results)
        {
            if (results == null)
            {
                Debug.LogError("Results is invalid.");
                return;
            }

            results.Clear();
            m_EntityManager.GetChildEntities(parentEntityId, m_InternalEntityResultsCache);
            foreach (EntityBase entity in m_InternalEntityResultsCache)
            {
                results.Add(entity);
            }
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntity">要获取子实体的父实体</param>
        /// <returns>子实体数组</returns>
        public EntityBase[] GetChildEntities(EntityBase parentEntity)
        {
            EntityBase[] entities = m_EntityManager.GetChildEntities(parentEntity);
            EntityBase[] entityImpls = new EntityBase[entities.Length];
            for (int i = 0; i < entities.Length; i++)
            {
                entityImpls[i] = entities[i];
            }

            return entityImpls;
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntity">要获取子实体的父实体</param>
        /// <param name="results">子实体数组</param>
        public void GetChildEntities(EntityBase parentEntity, List<EntityBase> results)
        {
            if (results == null)
            {
                Debug.LogError("Results is invalid.");
                return;
            }

            results.Clear();
            m_EntityManager.GetChildEntities(parentEntity, m_InternalEntityResultsCache);
            foreach (EntityBase entity in m_InternalEntityResultsCache)
            {
                results.Add(entity);
            }
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        public void AttachEntity(int childEntityId, int parentEntityId)
        {
            AttachEntity(GetEntity(childEntityId), GetEntity(parentEntityId), string.Empty, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntity">被附加的父实体</param>
        public void AttachEntity(int childEntityId, EntityBase parentEntity)
        {
            AttachEntity(GetEntity(childEntityId), parentEntity, string.Empty, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        public void AttachEntity(EntityBase childEntity, int parentEntityId)
        {
            AttachEntity(childEntity, GetEntity(parentEntityId), string.Empty, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntity">被附加的父实体</param>
        public void AttachEntity(EntityBase childEntity, EntityBase parentEntity)
        {
            AttachEntity(childEntity, parentEntity, string.Empty, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        public void AttachEntity(int childEntityId, int parentEntityId, string parentTransformPath)
        {
            AttachEntity(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransformPath, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        public void AttachEntity(int childEntityId, EntityBase parentEntity, string parentTransformPath)
        {
            AttachEntity(GetEntity(childEntityId), parentEntity, parentTransformPath, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        public void AttachEntity(EntityBase childEntity, int parentEntityId, string parentTransformPath)
        {
            AttachEntity(childEntity, GetEntity(parentEntityId), parentTransformPath, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        public void AttachEntity(EntityBase childEntity, EntityBase parentEntity, string parentTransformPath)
        {
            AttachEntity(childEntity, parentEntity, parentTransformPath, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        public void AttachEntity(int childEntityId, int parentEntityId, Transform parentTransform)
        {
            AttachEntity(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransform, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        public void AttachEntity(int childEntityId, EntityBase parentEntity, Transform parentTransform)
        {
            AttachEntity(GetEntity(childEntityId), parentEntity, parentTransform, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        public void AttachEntity(EntityBase childEntity, int parentEntityId, Transform parentTransform)
        {
            AttachEntity(childEntity, GetEntity(parentEntityId), parentTransform, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        public void AttachEntity(EntityBase childEntity, EntityBase parentEntity, Transform parentTransform)
        {
            AttachEntity(childEntity, parentEntity, parentTransform, null);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, int parentEntityId, object userData)
        {
            AttachEntity(GetEntity(childEntityId), GetEntity(parentEntityId), string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, EntityBase parentEntity, object userData)
        {
            AttachEntity(GetEntity(childEntityId), parentEntity, string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(EntityBase childEntity, int parentEntityId, object userData)
        {
            AttachEntity(childEntity, GetEntity(parentEntityId), string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(EntityBase childEntity, EntityBase parentEntity, object userData)
        {
            AttachEntity(childEntity, parentEntity, string.Empty, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, int parentEntityId, string parentTransformPath, object userData)
        {
            AttachEntity(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransformPath, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, EntityBase parentEntity, string parentTransformPath, object userData)
        {
            AttachEntity(GetEntity(childEntityId), parentEntity, parentTransformPath, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(EntityBase childEntity, int parentEntityId, string parentTransformPath, object userData)
        {
            AttachEntity(childEntity, GetEntity(parentEntityId), parentTransformPath, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransformPath">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(EntityBase childEntity, EntityBase parentEntity, string parentTransformPath, object userData)
        {
            if (childEntity == null)
            {
                Debug.LogError("Child entity is invalid.");
                return;
            }

            if (parentEntity == null)
            {
                Debug.LogWarning("Parent entity is invalid.");
                return;
            }

            Transform parentTransform = null;
            if (string.IsNullOrEmpty(parentTransformPath))
            {
                parentTransform = parentEntity.SelfTransform;
            }
            else
            {
                parentTransform = parentEntity.SelfTransform.Find(parentTransformPath);
                if (parentTransform == null)
                {
                    Debug.LogWarning(string.Format("Can not find transform path '{0}' from parent entity '{1}'.", parentTransformPath, parentEntity.Name));
                    parentTransform = parentEntity.SelfTransform;
                }
            }

            AttachEntity(childEntity, parentEntity, parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, int parentEntityId, Transform parentTransform, object userData)
        {
            AttachEntity(GetEntity(childEntityId), GetEntity(parentEntityId), parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntityId">要附加的子实体的实体编号</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(int childEntityId, EntityBase parentEntity, Transform parentTransform, object userData)
        {
            AttachEntity(GetEntity(childEntityId), parentEntity, parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntityId">被附加的父实体的实体编号</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(EntityBase childEntity, int parentEntityId, Transform parentTransform, object userData)
        {
            AttachEntity(childEntity, GetEntity(parentEntityId), parentTransform, userData);
        }

        /// <summary>
        /// 附加子实体
        /// </summary>
        /// <param name="childEntity">要附加的子实体</param>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        public void AttachEntity(EntityBase childEntity, EntityBase parentEntity, Transform parentTransform, object userData)
        {
            if (childEntity == null)
            {
                Debug.LogWarning("Child entity is invalid.");
                return;
            }

            if (parentEntity == null)
            {
                Debug.LogWarning("Parent entity is invalid.");
                return;
            }

            if (parentTransform == null)
            {
                parentTransform = parentEntity.SelfTransform;
            }

            m_EntityManager.AttachEntity(childEntity, parentEntity, parentTransform, userData);
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntityId">要解除的子实体的实体编号</param>
        public void DetachEntity(int childEntityId)
        {
            m_EntityManager.DetachEntity(childEntityId);
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntityId">要解除的子实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachEntity(int childEntityId, object userData)
        {
            m_EntityManager.DetachEntity(childEntityId, userData);
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntity">要解除的子实体</param>
        public void DetachEntity(EntityBase childEntity)
        {
            m_EntityManager.DetachEntity(childEntity);
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntity">要解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachEntity(EntityBase childEntity, object userData)
        {
            m_EntityManager.DetachEntity(childEntity, userData);
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntityId">被解除的父实体的实体编号</param>
        public void DetachChildEntities(int parentEntityId)
        {
            m_EntityManager.DetachChildEntities(parentEntityId);
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntityId">被解除的父实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachChildEntities(int parentEntityId, object userData)
        {
            m_EntityManager.DetachChildEntities(parentEntityId, userData);
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        public void DetachChildEntities(EntityBase parentEntity)
        {
            m_EntityManager.DetachChildEntities(parentEntity);
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachChildEntities(EntityBase parentEntity, object userData)
        {
            m_EntityManager.DetachChildEntities(parentEntity, userData);
        }

        /// <summary>
        /// 设置实体是否被加锁
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="locked">实体是否被加锁</param>
        public void SetEntityInstanceLocked(EntityBase entity, bool locked)
        {
            if (entity == null)
            {
                Debug.LogWarning("Entity is invalid.");
                return;
            }

            EntityGroup entityGroup = entity.EntityGroup;
            if (entityGroup == null)
            {
                Debug.LogWarning("Entity group is invalid.");
                return;
            }

            entityGroup.SetEntityInstanceLocked(entity.gameObject, locked);
        }

        /// <summary>
        /// 设置实体的优先级
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="priority">实体优先级</param>
        public void SetInstancePriority(EntityBase entity, int priority)
        {
            if (entity == null)
            {
                Debug.LogWarning("Entity is invalid.");
                return;
            }

            EntityGroup entityGroup = entity.EntityGroup;
            if (entityGroup == null)
            {
                Debug.LogWarning("Entity group is invalid.");
                return;
            }

            entityGroup.SetEntityInstancePriority(entity.gameObject, priority);
        }

        private void OnShowEntitySuccess(EntityBase entity, float duration, object userData)
        {
            if (m_EnableShowEntitySuccessEvent)
            {
                Debug.Log("显示实体成功");
            }
        }


        private void OnShowEntityFailure(int entityId, string entityAssetName, string entityGroupName, string errorMessage, object userData)
        {
            Debug.LogError(string.Format("Show entity failure, entity id '{0}', asset name '{1}', entity group name '{2}', error message '{3}'.", entityId.ToString(), entityAssetName, entityGroupName, errorMessage));
            if (m_EnableShowEntityFailureEvent)
            {
                Debug.Log("显示实体失败");
            }
        }

        private void OnShowEntityUpdate(int entityId, string entityAssetName, string entityGroupName, float progress, object userData)
        {
            if (m_EnableShowEntityUpdateEvent)
            {
                Debug.Log("显示实体更新");
            }
        }

        private void OnShowEntityDependencyAsset(int entityId, string entityAssetName, string entityGroupName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            if (m_EnableShowEntityDependencyAssetEvent)
            {
                Debug.Log("显示实体加载依赖资源");
            }
        }

        private void OnHideEntityComplete(int entityId, string entityAssetName, EntityGroup entityGroup, object userData)
        {
            if (m_EnableHideEntityCompleteEvent)
            {
                Debug.Log("影藏实体成功");
            }
        }


    }
}