//===================================================
//
//===================================================
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    /// <summary>
    /// 实体管理器
    /// </summary>
    public class EntityManager : ManagerBase
    {
        private readonly Dictionary<int, EntityInfo> m_EntityInfos;
        private readonly Dictionary<string, EntityGroup> m_EntityGroups;
        private readonly Dictionary<int, int> m_EntitiesBeingLoaded;
        private readonly HashSet<int> m_EntitiesToReleaseOnLoad;
        private readonly LinkedList<EntityInfo> m_RecycleQueue;
        private readonly LoadAssetCallbacks m_LoadAssetCallbacks;
        private int m_Serial;
        public ShowEntitySuccessCallBack ShowEntitySuccessCallBack;
        public ShowEntityFailureCallBack ShowEntityFailureCallBack;
        public ShowEntityUpdateCallBack ShowEntityUpdateCallBack;
        public ShowEntityDependencyAssetCallBack ShowEntityDependencyAssetCallBack;
        public HideEntityCompleteCallBack HideEntityCompleteCallBack;

        public EntityManager()
        {
            m_EntityInfos = new Dictionary<int, EntityInfo>();
            m_EntityGroups = new Dictionary<string, EntityGroup>();
            m_EntitiesBeingLoaded = new Dictionary<int, int>();
            m_EntitiesToReleaseOnLoad = new HashSet<int>();
            m_RecycleQueue = new LinkedList<EntityInfo>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadEntitySuccessCallback, LoadEntityFailureCallback, LoadEntityUpdateCallback, LoadEntityDependencyAssetCallback);
            m_Serial = 0;
            ShowEntitySuccessCallBack = null;
            ShowEntityFailureCallBack = null;
            ShowEntityUpdateCallBack = null;
            ShowEntityDependencyAssetCallBack = null;
            HideEntityCompleteCallBack = null;
        }

        #region 加载资源回调

        /// <summary>
        /// 加载实体成功回调
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityAsset">实体资源</param>
        /// <param name="duration">加载时间</param>
        /// <param name="userData">用户数据</param>
        private void LoadEntitySuccessCallback(string entityAssetName, UnityEngine.Object entityAsset, float duration, object userData)
        {
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            if (showEntityInfo == null)
            {
                throw new Exception("Show entity info is invalid.");
            }

            m_EntitiesBeingLoaded.Remove(showEntityInfo.EntityId);
            if (m_EntitiesToReleaseOnLoad.Contains(showEntityInfo.SerialId))
            {
                Log.Info("Release entity '{0}' (serial id '{1}') on loading success.", showEntityInfo.EntityId.ToString(), showEntityInfo.SerialId.ToString());
                m_EntitiesToReleaseOnLoad.Remove(showEntityInfo.SerialId);
                GameEntry.Resource.UnloadAsset(entityAsset);
                return;
            }

            EntityInstanceObject entityInstanceObject = new EntityInstanceObject(entityAssetName, entityAsset, UnityEngine.Object.Instantiate(entityAsset));
            showEntityInfo.EntityGroup.RegisterEntityInstanceObject(entityInstanceObject, true);

            InternalShowEntity(showEntityInfo.EntityId, entityAssetName, showEntityInfo.EntityGroup, entityInstanceObject.Target, true, duration, showEntityInfo.EntityLogicType, showEntityInfo.UserData);
        }

        /// <summary>
        /// 加载实体失败回调
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="errorMessage">错误信息</param>
        /// <param name="userData">用户数据</param>
        private void LoadEntityFailureCallback(string entityAssetName, string errorMessage, object userData)
        {
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            if (showEntityInfo == null)
            {
                throw new Exception("Show entity info is invalid.");
            }

            m_EntitiesBeingLoaded.Remove(showEntityInfo.EntityId);
            m_EntitiesToReleaseOnLoad.Remove(showEntityInfo.SerialId);
            string appendErrorMessage = TextUtil.Format("Load entity failure, asset name '{0}',  error message '{1}'.", entityAssetName, errorMessage);
            if (ShowEntityFailureCallBack != null)
            {
                ShowEntityFailureCallBack(showEntityInfo.EntityId, entityAssetName, showEntityInfo.EntityGroup.Name, appendErrorMessage, showEntityInfo.UserData);
                return;
            }

            throw new Exception(appendErrorMessage);
        }

        /// <summary>
        /// 加载实体更新回调
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="progress">加载进度</param>
        /// <param name="userData">用户数据</param>
        private void LoadEntityUpdateCallback(string entityAssetName, float progress, object userData)
        {
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            if (showEntityInfo == null)
            {
                throw new Exception("Show entity info is invalid.");
            }

            if (ShowEntityUpdateCallBack != null)
            {
                ShowEntityUpdateCallBack(showEntityInfo.EntityId, entityAssetName, showEntityInfo.EntityGroup.Name, progress, showEntityInfo.UserData);
            }
        }

        /// <summary>
        /// 加载实体时加载依赖资源回调
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="dependencyAssetName">依赖资源名称</param>
        /// <param name="loadedCount">加载的数量</param>
        /// <param name="totalCount">总共需要加载的数量</param>
        /// <param name="userData">用户数据</param>
        private void LoadEntityDependencyAssetCallback(string entityAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            ShowEntityInfo showEntityInfo = (ShowEntityInfo)userData;
            if (showEntityInfo == null)
            {
                throw new Exception("Show entity info is invalid.");
            }

            if (ShowEntityDependencyAssetCallBack != null)
            {
                ShowEntityDependencyAssetCallBack(showEntityInfo.EntityId, entityAssetName, showEntityInfo.EntityGroup.Name, dependencyAssetName, loadedCount, totalCount, showEntityInfo.UserData);
            }
        }
        #endregion

        /// <summary>
        /// 获取实体数量
        /// </summary>
        public int EntityCount
        {
            get
            {
                return m_EntityInfos.Count;
            }
        }

        /// <summary>
        /// 获取实体组数量
        /// </summary>
        public int EntityGroupCount
        {
            get
            {
                return m_EntityGroups.Count;
            }
        }

        /// <summary>
        /// 实体管理器轮询
        /// </summary>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            while (m_RecycleQueue.Count > 0)
            {
                EntityInfo entityInfo = m_RecycleQueue.First.Value;
                m_RecycleQueue.RemoveFirst();
                EntityBase entity = entityInfo.Entity;
                EntityGroup entityGroup = entity.EntityGroup;
                if (entityGroup == null)
                {
                    throw new Exception("Entity group is invalid.");
                }

                entityInfo.Status = EntityStatus.WillRecycle;
                entity.OnRecycle();
                entityInfo.Status = EntityStatus.Recycled;
                entityGroup.UnspawnEntity(entity);
            }

            foreach (KeyValuePair<string, EntityGroup> entityGroup in m_EntityGroups)
            {
                entityGroup.Value.OnUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        public override void Dispose()
        {
            HideAllLoadedEntities();
            m_EntityGroups.Clear();
            m_EntitiesBeingLoaded.Clear();
            m_EntitiesToReleaseOnLoad.Clear();
            m_RecycleQueue.Clear();
        }

        /// <summary>
        /// 是否存在实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>是否存在实体组</returns>
        public bool HasEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new Exception("Entity group name is invalid.");
            }

            return m_EntityGroups.ContainsKey(entityGroupName);
        }

        /// <summary>
        /// 获取实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <returns>要获取的实体组</returns>
        public EntityGroup GetEntityGroup(string entityGroupName)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new Exception("Entity group name is invalid.");
            }

            EntityGroup entityGroup = null;
            if (m_EntityGroups.TryGetValue(entityGroupName, out entityGroup))
            {
                return entityGroup;
            }

            return null;
        }

        /// <summary>
        /// 获取所有实体组
        /// </summary>
        /// <returns>所有实体组</returns>
        public EntityGroup[] GetAllEntityGroups()
        {
            int index = 0;
            EntityGroup[] results = new EntityGroup[m_EntityGroups.Count];
            foreach (KeyValuePair<string, EntityGroup> entityGroup in m_EntityGroups)
            {
                results[index++] = entityGroup.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取所有实体组
        /// </summary>
        /// <param name="results">所有实体组</param>
        public void GetAllEntityGroups(List<EntityGroup> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<string, EntityGroup> entityGroup in m_EntityGroups)
            {
                results.Add(entityGroup.Value);
            }
        }

        /// <summary>
        /// 增加实体组
        /// </summary>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="instanceCapacity">实体实例对象池容量</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数</param>
        /// <param name="instancePriority">实体实例对象池的优先级</param>
        /// <param name="groupTransform">实体组Transform</param>
        /// <returns>是否增加实体组成功</returns>
        public bool AddEntityGroup(string entityGroupName, float instanceAutoReleaseInterval, int instanceCapacity, float instanceExpireTime, int instancePriority, Transform groupTransform)
        {
            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new Exception("Entity group name is invalid.");
            }

            if (HasEntityGroup(entityGroupName))
            {
                return false;
            }

            m_EntityGroups.Add(entityGroupName, new EntityGroup(entityGroupName, instanceAutoReleaseInterval, instanceCapacity, instanceExpireTime, instancePriority, groupTransform));

            return true;
        }

        /// <summary>
        /// 是否存在实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>是否存在实体</returns>
        public bool HasEntity(int entityId)
        {
            return m_EntityInfos.ContainsKey(entityId);
        }

        /// <summary>
        /// 是否存在实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>是否存在实体</returns>
        public bool HasEntity(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                throw new Exception("Entity asset name is invalid.");
            }

            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                if (entityInfo.Value.Entity.EntityAssetName == entityAssetName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>要获取的实体</returns>
        public EntityBase GetEntity(int entityId)
        {
            EntityInfo entityInfo = GetEntityInfo(entityId);
            if (entityInfo == null)
            {
                return null;
            }

            return entityInfo.Entity;
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <returns>要获取的实体</returns>
        public EntityBase GetEntity(string entityAssetName)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                throw new Exception("Entity asset name is invalid.");
            }

            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                if (entityInfo.Value.Entity.EntityAssetName == entityAssetName)
                {
                    return entityInfo.Value.Entity;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取实体
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
            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                if (entityInfo.Value.Entity.EntityAssetName == entityAssetName)
                {
                    results.Add(entityInfo.Value.Entity);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取实体
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
            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                if (entityInfo.Value.Entity.EntityAssetName == entityAssetName)
                {
                    results.Add(entityInfo.Value.Entity);
                }
            }
        }

        /// <summary>
        /// 获取所有已加载的实体
        /// </summary>
        /// <returns>所有已加载的实体</returns>
        public EntityBase[] GetAllLoadedEntities()
        {
            int index = 0;
            EntityBase[] results = new EntityBase[m_EntityInfos.Count];
            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                results[index++] = entityInfo.Value.Entity;
            }

            return results;
        }

        /// <summary>
        /// 获取所有已加载的实体
        /// </summary>
        /// <param name="results">所有已加载的实体</param>
        public void GetAllLoadedEntities(List<EntityBase> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
            {
                results.Add(entityInfo.Value.Entity);
            }
        }

        /// <summary>
        /// 获取所有正在加载实体的编号
        /// </summary>
        /// <returns>所有正在加载实体的编号</returns>
        public int[] GetAllLoadingEntityIds()
        {
            int index = 0;
            int[] results = new int[m_EntitiesBeingLoaded.Count];
            foreach (KeyValuePair<int, int> entityBeingLoaded in m_EntitiesBeingLoaded)
            {
                results[index++] = entityBeingLoaded.Key;
            }

            return results;
        }

        /// <summary>
        /// 获取所有正在加载实体的编号
        /// </summary>
        /// <param name="results">所有正在加载实体的编号</param>
        public void GetAllLoadingEntityIds(List<int> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<int, int> entityBeingLoaded in m_EntitiesBeingLoaded)
            {
                results.Add(entityBeingLoaded.Key);
            }
        }

        /// <summary>
        /// 是否正在加载实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>是否正在加载实体</returns>
        public bool IsLoadingEntity(int entityId)
        {
            return m_EntitiesBeingLoaded.ContainsKey(entityId);
        }

        /// <summary>
        /// 是否是合法的实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>实体是否合法</returns>
        public bool IsValidEntity(EntityBase entity)
        {
            if (entity == null)
            {
                return false;
            }

            return HasEntity(entity.Id);
        }

        /// <summary>
        /// 显示实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroupName">实体组名称</param>
        /// <param name="priority">加载实体资源的优先级</param>
        /// <param name="entityLogicType">实体逻辑</param>
        /// <param name="userData">用户自定义数据</param>
        public void ShowEntity(int entityId, string entityAssetName, string entityGroupName, int priority, Type entityLogicType, object userData)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                throw new Exception("Entity asset name is invalid.");
            }

            if (string.IsNullOrEmpty(entityGroupName))
            {
                throw new Exception("Entity group name is invalid.");
            }

            if (m_EntityInfos.ContainsKey(entityId))
            {
                throw new Exception(TextUtil.Format("Entity id '{0}' is already exist.", entityId.ToString()));
            }

            if (IsLoadingEntity(entityId))
            {
                throw new Exception(TextUtil.Format("Entity '{0}' is already being loaded.", entityId.ToString()));
            }

            EntityGroup entityGroup = GetEntityGroup(entityGroupName);
            if (entityGroup == null)
            {
                throw new Exception(TextUtil.Format("Entity group '{0}' is not exist.", entityGroupName));
            }

            EntityInstanceObject entityInstanceObject = entityGroup.SpawnEntityInstanceObject(entityAssetName);
            if (entityInstanceObject == null)
            {
                int serialId = m_Serial++;
                m_EntitiesBeingLoaded.Add(entityId, serialId);
                GameEntry.Resource.LoadAsset(entityAssetName,  typeof(GameObject), priority, m_LoadAssetCallbacks, new ShowEntityInfo(serialId, entityId, entityGroup, entityLogicType, userData));
                return;
            }

            InternalShowEntity(entityId, entityAssetName, entityGroup, entityInstanceObject.Target, false, 0f, entityLogicType, userData);
        }


        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        public void HideEntity(int entityId)
        {
            HideEntity(entityId, null);
        }

        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void HideEntity(int entityId, object userData)
        {
            if (IsLoadingEntity(entityId))
            {
                int serialId = 0;
                if (!m_EntitiesBeingLoaded.TryGetValue(entityId, out serialId))
                {
                    throw new Exception(TextUtil.Format("Can not find entity '{0}'.", entityId.ToString()));
                }

                m_EntitiesToReleaseOnLoad.Add(serialId);
                m_EntitiesBeingLoaded.Remove(entityId);
                return;
            }

            EntityInfo entityInfo = GetEntityInfo(entityId);
            if (entityInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find entity '{0}'.", entityId.ToString()));
            }

            InternalHideEntity(entityInfo, userData);
        }

        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entity">实体</param>
        public void HideEntity(EntityBase entity)
        {
            HideEntity(entity, null);
        }

        /// <summary>
        /// 隐藏实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void HideEntity(EntityBase entity, object userData)
        {
            if (entity == null)
            {
                throw new Exception("Entity is invalid.");
            }

            HideEntity(entity.Id, userData);
        }

        /// <summary>
        /// 隐藏所有已加载的实体
        /// </summary>
        public void HideAllLoadedEntities()
        {
            HideAllLoadedEntities(null);
        }

        /// <summary>
        /// 隐藏所有已加载的实体
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void HideAllLoadedEntities(object userData)
        {
            while (m_EntityInfos.Count > 0)
            {
                foreach (KeyValuePair<int, EntityInfo> entityInfo in m_EntityInfos)
                {
                    InternalHideEntity(entityInfo.Value, userData);
                    break;
                }
            }
        }

        /// <summary>
        /// 隐藏所有正在加载的实体
        /// </summary>
        public void HideAllLoadingEntities()
        {
            foreach (KeyValuePair<int, int> entityBeingLoaded in m_EntitiesBeingLoaded)
            {
                m_EntitiesToReleaseOnLoad.Add(entityBeingLoaded.Value);
            }

            m_EntitiesBeingLoaded.Clear();
        }

        /// <summary>
        /// 获取父实体
        /// </summary>
        /// <param name="childEntityId">要获取父实体的子实体的实体编号</param>
        /// <returns>子实体的父实体</returns>
        public EntityBase GetParentEntity(int childEntityId)
        {
            EntityInfo childEntityInfo = GetEntityInfo(childEntityId);
            if (childEntityInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find child entity '{0}'.", childEntityId.ToString()));
            }

            return childEntityInfo.ParentEntity;
        }

        /// <summary>
        /// 获取父实体
        /// </summary>
        /// <param name="childEntity">要获取父实体的子实体</param>
        /// <returns>子实体的父实体</returns>
        public EntityBase GetParentEntity(EntityBase childEntity)
        {
            if (childEntity == null)
            {
                throw new Exception("Child entity is invalid.");
            }

            return GetParentEntity(childEntity.Id);
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntityId">要获取子实体的父实体的实体编号</param>
        /// <returns>子实体数组</returns>
        public EntityBase[] GetChildEntities(int parentEntityId)
        {
            EntityInfo parentEntityInfo = GetEntityInfo(parentEntityId);
            if (parentEntityInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find parent entity '{0}'.", parentEntityId.ToString()));
            }

            return parentEntityInfo.GetChildEntities();
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntityId">要获取子实体的父实体的实体编号</param>
        /// <param name="results">子实体数组</param>
        public void GetChildEntities(int parentEntityId, List<EntityBase> results)
        {
            EntityInfo parentEntityInfo = GetEntityInfo(parentEntityId);
            if (parentEntityInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find parent entity '{0}'.", parentEntityId.ToString()));
            }

            parentEntityInfo.GetChildEntities(results);
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntity">要获取子实体的父实体</param>
        /// <returns>子实体数组</returns>
        public EntityBase[] GetChildEntities(EntityBase parentEntity)
        {
            if (parentEntity == null)
            {
                throw new Exception("Parent entity is invalid.");
            }

            return GetChildEntities(parentEntity.Id);
        }

        /// <summary>
        /// 获取子实体
        /// </summary>
        /// <param name="parentEntity">要获取子实体的父实体</param>
        /// <param name="results">子实体数组</param>
        public void GetChildEntities(EntityBase parentEntity, List<EntityBase> results)
        {
            if (parentEntity == null)
            {
                throw new Exception("Parent entity is invalid.");
            }

            GetChildEntities(parentEntity.Id, results);
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
            if (childEntityId == parentEntityId)
            {
                throw new Exception(TextUtil.Format("Can not attach entity when child entity id equals to parent entity id '{0}'.", parentEntityId.ToString()));
            }

            EntityInfo childEntityInfo = GetEntityInfo(childEntityId);
            if (childEntityInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find child entity '{0}'.", childEntityId.ToString()));
            }

            if (childEntityInfo.Status >= EntityStatus.WillHide)
            {
                throw new Exception(TextUtil.Format("Can not attach entity when child entity status is '{0}'.", childEntityInfo.Status.ToString()));
            }

            EntityInfo parentEntityInfo = GetEntityInfo(parentEntityId);
            if (parentEntityInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find parent entity '{0}'.", parentEntityId.ToString()));
            }

            if (parentEntityInfo.Status >= EntityStatus.WillHide)
            {
                throw new Exception(TextUtil.Format("Can not attach entity when parent entity status is '{0}'.", parentEntityInfo.Status.ToString()));
            }

            EntityBase childEntity = childEntityInfo.Entity;
            EntityBase parentEntity = parentEntityInfo.Entity;
            DetachEntity(childEntity.Id, userData);
            childEntityInfo.ParentEntity = parentEntity;
            parentEntityInfo.AddChildEntity(childEntity);
            parentEntity.OnAttached(childEntity, parentTransform, userData);
            childEntity.OnAttachTo(parentEntity, parentTransform, userData);
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
                throw new Exception("Child entity is invalid.");
            }

            if (parentEntity == null)
            {
                throw new Exception("Parent entity is invalid.");
            }

            AttachEntity(childEntity.Id, parentEntity.Id, parentTransform, userData);
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntityId">要解除的子实体的实体编号</param>
        public void DetachEntity(int childEntityId)
        {
            DetachEntity(childEntityId, null);
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntityId">要解除的子实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachEntity(int childEntityId, object userData)
        {
            EntityInfo childEntityInfo = GetEntityInfo(childEntityId);
            if (childEntityInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find child entity '{0}'.", childEntityId.ToString()));
            }

            EntityBase parentEntity = childEntityInfo.ParentEntity;
            if (parentEntity == null)
            {
                return;
            }

            EntityInfo parentEntityInfo = GetEntityInfo(parentEntity.Id);
            if (parentEntityInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find parent entity '{0}'.", parentEntity.Id.ToString()));
            }

            EntityBase childEntity = childEntityInfo.Entity;
            childEntityInfo.ParentEntity = null;
            parentEntityInfo.RemoveChildEntity(childEntity);
            parentEntity.OnDetached(childEntity, userData);
            childEntity.OnDetachFrom(parentEntity, userData);
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntity">要解除的子实体</param>
        public void DetachEntity(EntityBase childEntity)
        {
            DetachEntity(childEntity, null);
        }

        /// <summary>
        /// 解除子实体
        /// </summary>
        /// <param name="childEntity">要解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachEntity(EntityBase childEntity, object userData)
        {
            if (childEntity == null)
            {
                throw new Exception("Child entity is invalid.");
            }

            DetachEntity(childEntity.Id, userData);
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntityId">被解除的父实体的实体编号</param>
        public void DetachChildEntities(int parentEntityId)
        {
            DetachChildEntities(parentEntityId, null);
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntityId">被解除的父实体的实体编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachChildEntities(int parentEntityId, object userData)
        {
            EntityInfo parentEntityInfo = GetEntityInfo(parentEntityId);
            if (parentEntityInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find parent entity '{0}'.", parentEntityId.ToString()));
            }

            EntityBase[] childEntities = parentEntityInfo.GetChildEntities();
            foreach (EntityBase childEntity in childEntities)
            {
                DetachEntity(childEntity.Id, userData);
            }
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        public void DetachChildEntities(EntityBase parentEntity)
        {
            DetachChildEntities(parentEntity, null);
        }

        /// <summary>
        /// 解除所有子实体
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        public void DetachChildEntities(EntityBase parentEntity, object userData)
        {
            if (parentEntity == null)
            {
                throw new Exception("Parent entity is invalid.");
            }

            DetachChildEntities(parentEntity.Id, userData);
        }

        /// <summary>
        /// 获取实体信息
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <returns>实体信息</returns>
        private EntityInfo GetEntityInfo(int entityId)
        {
            EntityInfo entityInfo = null;
            if (m_EntityInfos.TryGetValue(entityId, out entityInfo))
            {
                return entityInfo;
            }

            return null;
        }


        private void InternalShowEntity(int entityId, string entityAssetName, EntityGroup entityGroup, object entityInstance, bool isNewInstance, float duration, Type entityLogicType, object userData)
        {
            try
            {
                GameObject gameObject = entityInstance as GameObject;
                if (gameObject == null)
                {
                    Log.Error("Entity instance is invalid.");
                    return;
                }
                Transform transform = gameObject.transform;
                transform.SetParent(entityGroup.GroupTransform);

                EntityBase oldentity = gameObject.GetComponent<EntityBase>();
                EntityBase entity = null;
                bool isNewLogic = false;
                if (oldentity != null)
                {
                    if (oldentity.GetType() == entityLogicType)
                    {
                        oldentity.enabled = true;
                        entity = oldentity;
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(oldentity);
                        entity = gameObject.AddComponent(entityLogicType) as EntityBase;
                        isNewLogic = true;
                    }
                }
                else
                {
                    entity = gameObject.AddComponent(entityLogicType) as EntityBase;
                    isNewLogic = true;
                }

                if (entity == null)
                {
                    throw new Exception(string.Format("Entity '{0}' can not add entity logic.", entityAssetName));
                }

                EntityInfo entityInfo = new EntityInfo(entity);
                m_EntityInfos.Add(entityId, entityInfo);
                entityInfo.Status = EntityStatus.WillInit;
                entity.OnInit(entityId, entityAssetName, entityGroup, isNewInstance, isNewLogic, userData);
                entityInfo.Status = EntityStatus.Inited;
                entityGroup.AddEntity(entity);
                entityInfo.Status = EntityStatus.WillShow;
                entity.OnShow(userData);
                entityInfo.Status = EntityStatus.Showed;

                if (ShowEntitySuccessCallBack != null)
                {
                    ShowEntitySuccessCallBack(entity, duration, userData);
                }
            }
            catch (Exception exception)
            {
                if (ShowEntityFailureCallBack != null)
                {
                    ShowEntityFailureCallBack(entityId, entityAssetName, entityGroup.Name, exception.ToString(), userData);
                    return;
                }

                throw;
            }
        }

        private void InternalHideEntity(EntityInfo entityInfo, object userData)
        {
            EntityBase entity = entityInfo.Entity;
            EntityBase[] childEntities = entityInfo.GetChildEntities();
            foreach (EntityBase childEntity in childEntities)
            {
                HideEntity(childEntity.Id, userData);
            }

            if (entityInfo.Status == EntityStatus.Hidden)
            {
                return;
            }

            DetachEntity(entity.Id, userData);
            entityInfo.Status = EntityStatus.WillHide;
            entity.OnHide(userData);
            entityInfo.Status = EntityStatus.Hidden;

            EntityGroup entityGroup = entity.EntityGroup;
            if (entityGroup == null)
            {
                throw new Exception("Entity group is invalid.");
            }

            entityGroup.RemoveEntity(entity);
            if (!m_EntityInfos.Remove(entity.Id))
            {
                throw new Exception("Entity info is unmanaged.");
            }

            if (HideEntityCompleteCallBack != null)
            {
                HideEntityCompleteCallBack(entity.Id, entity.EntityAssetName, entityGroup, userData);
            }

            m_RecycleQueue.AddLast(entityInfo);
        }



    }
}
