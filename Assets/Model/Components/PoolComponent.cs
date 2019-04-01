using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 对象池组件
    /// </summary>
    public class PoolComponent : GameBaseComponent
    {
        private const int DefaultCapacity = int.MaxValue;
        private const float DefaultExpireTime = float.MaxValue;
        private const int DefaultPriority = 0;


        private PoolManager m_PoolManager;

        /// <summary>
        /// 得到对象池管理器
        /// </summary>
        public PoolManager PoolManager
        {
            get
            {
                return m_PoolManager;
            }
        }


        protected override void OnAwake()
        {
            base.OnAwake();
            m_PoolManager = new PoolManager();

            m_PoolManager.ClearClassObjectInterval = m_ClearCalssObjectInterval;
        }

        protected override void OnStart()
        {
            base.OnStart();
            InitClassReside();
        }

        /// <summary>
        /// 初始化常用类常驻数量
        /// </summary>
        private void InitClassReside()
        {
            GameEntry.Pool.SetClassObjectResideCount<HttpRoutine>(3);
            GameEntry.Pool.SetClassObjectResideCount<Dictionary<string, object>>(3);
        }

        #region SetResideCount 设置类常驻数量
        /// <summary>
        /// 设置类常驻数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        public void SetClassObjectResideCount<T>(byte count) where T : class
        {
            m_PoolManager.ClassObjectPool.SetResideCount<T>(count);
        }
        #endregion



        #region SpawnClassObject 取出一个对象
        /// <summary>
        /// 取出一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T SpawnClassObject<T>() where T : class, new()
        {
            return m_PoolManager.ClassObjectPool.Dequeue<T>();
        }
        #endregion

        #region UnSpawnClassObject 对象回池
        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="obj"></param>
        public void UnSpawnClassObject(object obj)
        {
            m_PoolManager.ClassObjectPool.Enqueue(obj);
        }
        #endregion


        #region 变量对象池

        /// <summary>
        /// 变量对象池锁
        /// </summary>
        private object m_VarObjectLock = new object();


        /// <summary>
        /// 在监视面板显示的信息
        /// </summary>
        public Dictionary<Type, int> VarObjectDic = new Dictionary<Type, int>();


        /// <summary>
        /// 取出一个变量对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T DeQueueVarObject<T>() where T : VariableBase, new()
        {
            lock (m_VarObjectLock)
            {
                T item = SpawnClassObject<T>();

                Type t = item.GetType();
                if (VarObjectDic.ContainsKey(t))
                {
                    VarObjectDic[t]++;
                }
                else
                {
                    VarObjectDic[t] = 1;
                }

                return item;
            }
        }

        /// <summary>
        /// 变量对象回池
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void EnqueueVarObject<T>(T item) where T : VariableBase
        {
            lock (m_VarObjectLock)
            {
                UnSpawnClassObject(item);

                Type t = item.GetType();
                if (VarObjectDic.ContainsKey(t))
                {
                    VarObjectDic[t]--;
                    if (VarObjectDic[t] == 0)
                    {
                        VarObjectDic.Remove(t);
                    }
                }
            }
        }

        #endregion



        public override void Shutdown()
        {
            base.Shutdown();
            m_PoolManager.Dispose();
        }

        /// <summary>
        /// 类对象池释放间隔
        /// </summary>
        [SerializeField]
        public float m_ClearCalssObjectInterval = 60;

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_PoolManager.OnUpdate(deltaTime, unscaledDeltaTime);
        }


        /// <summary>
        /// 在监视面板显示的信息
        /// </summary>
        public Dictionary<Type, int> InspectorDic
        {
            get
            {
                return m_PoolManager.ClassObjectPool.ObjectTypeDic;
            }
        }

        /// <summary>
        /// 获取对象池数量
        /// </summary>
        public int Count
        {
            get
            {
                return m_PoolManager.Count;
            }
        }

        #region HasObjectPool 检查是否存在对象池
        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool<T>() where T : ObjectBase
        {
            return m_PoolManager.HasObjectPool<T>();
        }

        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool(Type objectType)
        {
            return m_PoolManager.HasObjectPool(objectType);
        }

        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool<T>(string name) where T : ObjectBase
        {
            return m_PoolManager.HasObjectPool<T>(name);
        }

        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool(Type objectType, string name)
        {
            return m_PoolManager.HasObjectPool(objectType, name);
        }

        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <param name="fullName">对象池完整名称</param>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool(string fullName)
        {
            return m_PoolManager.HasObjectPool(fullName);
        }

        /// <summary>
        /// 检查是否存在对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>是否存在对象池</returns>
        public bool HasObjectPool(Predicate<ObjectPoolBase> condition)
        {
            return m_PoolManager.HasObjectPool(condition);
        }
        #endregion

        #region GetObjectPool 获取对象池
        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>要获取的对象池</returns>
        public IObjectPool<T> GetObjectPool<T>() where T : ObjectBase
        {
            return m_PoolManager.GetObjectPool<T>();
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>要获取的对象池</returns>
        public ObjectPoolBase GetObjectPool(Type objectType)
        {
            return m_PoolManager.GetObjectPool(objectType);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>要获取的对象池</returns>
        public IObjectPool<T> GetObjectPool<T>(string name) where T : ObjectBase
        {
            return m_PoolManager.GetObjectPool<T>(name);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>要获取的对象池</returns>
        public ObjectPoolBase GetObjectPool(Type objectType, string name)
        {
            return m_PoolManager.GetObjectPool(objectType, name);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="fullName">对象池完整名称</param>
        /// <returns>要获取的对象池</returns>
        public ObjectPoolBase GetObjectPool(string fullName)
        {
            return m_PoolManager.GetObjectPool(fullName);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>要获取的对象池</returns>
        public ObjectPoolBase GetObjectPool(Predicate<ObjectPoolBase> condition)
        {
            return m_PoolManager.GetObjectPool(condition);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <returns>要获取的对象池</returns>
        public ObjectPoolBase[] GetObjectPools(Predicate<ObjectPoolBase> condition)
        {
            return m_PoolManager.GetObjectPools(condition);
        }

        /// <summary>
        /// 获取对象池
        /// </summary>
        /// <param name="condition">要检查的条件</param>
        /// <param name="results">要获取的对象池</param>
        public void GetObjectPools(Predicate<ObjectPoolBase> condition, List<ObjectPoolBase> results)
        {
            m_PoolManager.GetObjectPools(condition, results);
        }
        #endregion

        #region GetAllObjectPools 获取所有对象池
        /// <summary>
        /// 获取所有对象池
        /// </summary>
        public ObjectPoolBase[] GetAllObjectPools()
        {
            return m_PoolManager.GetAllObjectPools();
        }

        /// <summary>
        /// 获取所有对象池
        /// </summary>
        /// <param name="results">所有对象池</param>
        public void GetAllObjectPools(List<ObjectPoolBase> results)
        {
            m_PoolManager.GetAllObjectPools(results);
        }

        /// <summary>
        /// 获取所有对象池
        /// </summary>
        /// <param name="sort">是否根据对象池的优先级排序</param>
        /// <returns>所有对象池</returns>
        public ObjectPoolBase[] GetAllObjectPools(bool sort)
        {
            return m_PoolManager.GetAllObjectPools(sort);
        }

        /// <summary>
        /// 获取所有对象池
        /// </summary>
        /// <param name="sort">是否根据对象池的优先级排序</param>
        /// <param name="results">所有对象池</param>
        public void GetAllObjectPools(bool sort, List<ObjectPoolBase> results)
        {
            m_PoolManager.GetAllObjectPools(sort, results);
        }
        #endregion

        #region CreateSingleSpawnObjectPool 创建允许单次获取的对象池
        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>() where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>();
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(name);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, name);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(capacity);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, capacity);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(float expireTime) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, float expireTime)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(name, capacity);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, name, capacity);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float expireTime) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(name, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, float expireTime)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, name, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, float expireTime) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(capacity, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity, float expireTime)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, capacity, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(capacity, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity, int priority)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, capacity, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, float expireTime, int priority)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, float expireTime) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(name, capacity, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity, float expireTime)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, name, capacity, expireTime);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(name, capacity, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity, int priority)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, name, capacity, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(name, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, float expireTime, int priority)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, name, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, int capacity, float expireTime, int priority)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(name, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, int capacity, float expireTime, int priority)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, name, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public IObjectPool<T> CreateSingleSpawnObjectPool<T>(string name, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateSingleSpawnObjectPool<T>(name, autoReleaseInterval, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许单次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许单次获取的对象池</returns>
        public ObjectPoolBase CreateSingleSpawnObjectPool(Type objectType, string name, float autoReleaseInterval, int capacity, float expireTime, int priority)
        {
            return m_PoolManager.CreateSingleSpawnObjectPool(objectType, name, autoReleaseInterval, capacity, expireTime, priority);
        }
        #endregion

        #region CreateMultiSpawnObjectPool 创建允许多次获取的对象池
        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>() where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>();
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(name);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, name);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(capacity);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, capacity);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(float expireTime) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(expireTime);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, float expireTime)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, expireTime);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(name, capacity);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, name, capacity);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float expireTime) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(name, expireTime);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, float expireTime)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, name, expireTime);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, float expireTime) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(capacity, expireTime);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity, float expireTime)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, capacity, expireTime);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(capacity, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity, int priority)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, capacity, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, float expireTime, int priority)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, float expireTime) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(name, capacity, expireTime);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity, float expireTime)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, name, capacity, expireTime);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(name, capacity, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity, int priority)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, name, capacity, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(name, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, float expireTime, int priority)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, name, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, int capacity, float expireTime, int priority)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(name, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, int capacity, float expireTime, int priority)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, name, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public IObjectPool<T> CreateMultiSpawnObjectPool<T>(string name, float autoReleaseInterval, int capacity, float expireTime, int priority) where T : ObjectBase
        {
            return m_PoolManager.CreateMultiSpawnObjectPool<T>(name, autoReleaseInterval, capacity, expireTime, priority);
        }

        /// <summary>
        /// 创建允许多次获取的对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">对象池名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放可释放对象的间隔秒数</param>
        /// <param name="capacity">对象池的容量</param>
        /// <param name="expireTime">对象池对象过期秒数</param>
        /// <param name="priority">对象池的优先级</param>
        /// <returns>要创建的允许多次获取的对象池</returns>
        public ObjectPoolBase CreateMultiSpawnObjectPool(Type objectType, string name, float autoReleaseInterval, int capacity, float expireTime, int priority)
        {
            return m_PoolManager.CreateMultiSpawnObjectPool(objectType, name, autoReleaseInterval, capacity, expireTime, priority);
        }
        #endregion

        #region DestroyObjectPool 销毁对象池
        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool<T>() where T : ObjectBase
        {
            return m_PoolManager.DestroyObjectPool<T>();
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool(Type objectType)
        {
            return m_PoolManager.DestroyObjectPool(objectType);
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">要销毁的对象池名称</param>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool<T>(string name) where T : ObjectBase
        {
            return m_PoolManager.DestroyObjectPool<T>(name);
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="name">要销毁的对象池名称</param>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool(Type objectType, string name)
        {
            return m_PoolManager.DestroyObjectPool(objectType, name);
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="objectPool">要销毁的对象池</param>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool<T>(IObjectPool<T> objectPool) where T : ObjectBase
        {
            return m_PoolManager.DestroyObjectPool(objectPool);
        }

        /// <summary>
        /// 销毁对象池
        /// </summary>
        /// <param name="objectPool">要销毁的对象池</param>
        /// <returns>是否销毁对象池成功</returns>
        public bool DestroyObjectPool(ObjectPoolBase objectPool)
        {
            return m_PoolManager.DestroyObjectPool(objectPool);
        }
        #endregion

        /// <summary>
        /// 释放对象池中的可释放对象
        /// </summary>
        public void Release()
        {
            Log.Info("Object pool release...");
            m_PoolManager.Release();
        }

        /// <summary>
        /// 释放对象池中的所有未使用对象
        /// </summary>
        public void ReleaseAllUnused()
        {
            Log.Info("Object pool release all unused...");
            m_PoolManager.ReleaseAllUnused();
        }

    }
}
