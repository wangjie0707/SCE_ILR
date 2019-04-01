
using System;
using UnityEngine;

namespace Myth
{
    public class GameObjectPool
    {
        private const int m_DefaultCapacity = int.MaxValue;
        private const float m_DefaultExpireTime = float.MaxValue;
        private const int m_DefaultPriority = 0;

        private IObjectPool<InstanceGameObject> m_InstanceGameObjectPool = null;

        private GameObject m_GameobjectAsset;

        #region 初始化GameObject对象池
        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        public GameObjectPool(GameObject gameobjectAsset)
            : this(gameobjectAsset, string.Empty, m_DefaultExpireTime, m_DefaultCapacity, m_DefaultExpireTime, m_DefaultPriority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="name">对象名称</param>
        public GameObjectPool(GameObject gameobjectAsset, string name)
            : this(gameobjectAsset, name, m_DefaultExpireTime, m_DefaultCapacity, m_DefaultExpireTime, m_DefaultPriority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="capacity">对象池容量</param>
        public GameObjectPool(GameObject gameobjectAsset, int capacity)
            : this(gameobjectAsset, string.Empty, m_DefaultExpireTime, capacity, m_DefaultExpireTime, m_DefaultPriority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="expireTime">对象过期时间</param>
        public GameObjectPool(GameObject gameobjectAsset, float expireTime)
            : this(gameobjectAsset, string.Empty, expireTime, m_DefaultCapacity, expireTime, m_DefaultPriority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="name">对象名称</param>
        /// <param name="capacity">对象池容量</param>
        public GameObjectPool(GameObject gameobjectAsset, string name, int capacity)
            : this(gameobjectAsset, name, m_DefaultExpireTime, capacity, m_DefaultExpireTime, m_DefaultPriority)
        {

        }

        public GameObjectPool(GameObject gameobjectAsset, string name, float expireTime)
            : this(gameobjectAsset, name, expireTime, m_DefaultCapacity, expireTime, m_DefaultPriority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="capacity">对象池容量</param>
        /// <param name="expireTime">对象过期时间</param>
        public GameObjectPool(GameObject gameobjectAsset, int capacity, float expireTime)
            : this(gameobjectAsset, string.Empty, expireTime, capacity, expireTime, m_DefaultPriority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="capacity">对象池容量</param>
        /// <param name="priority">对象池优先级</param>
        public GameObjectPool(GameObject gameobjectAsset, int capacity, int priority)
            : this(gameobjectAsset, string.Empty, m_DefaultExpireTime, capacity, m_DefaultExpireTime, priority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="expireTime">对象过期时间</param>
        /// <param name="priority">对象池优先级</param>
        public GameObjectPool(GameObject gameobjectAsset, float expireTime, int priority)
            : this(gameobjectAsset, string.Empty, expireTime, m_DefaultCapacity, expireTime, priority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="name">对象名称</param>
        /// <param name="capacity">对象池容量</param>
        /// <param name="expireTime">对象过期时间</param>
        public GameObjectPool(GameObject gameobjectAsset, string name, int capacity, float expireTime)
            : this(gameobjectAsset, name, expireTime, capacity, expireTime, m_DefaultPriority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="name">对象名称</param>
        /// <param name="capacity">对象池容量</param>
        /// <param name="priority">对象池优先级</param>
        public GameObjectPool(GameObject gameobjectAsset, string name, int capacity, int priority)
            : this(gameobjectAsset, name, m_DefaultExpireTime, capacity, m_DefaultExpireTime, priority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="name">对象名称</param>
        /// <param name="expireTime">对象过期时间</param>
        /// <param name="priority">对象池优先级</param>
        public GameObjectPool(GameObject gameobjectAsset, string name, float expireTime, int priority)
           : this(gameobjectAsset, name, expireTime, m_DefaultCapacity, expireTime, priority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="capacity">对象池容量</param>
        /// <param name="expireTime">对象过期时间</param>
        /// <param name="priority">对象池优先级</param>
        public GameObjectPool(GameObject gameobjectAsset, int capacity, float expireTime, int priority)
            : this(gameobjectAsset, string.Empty, expireTime, capacity, expireTime, priority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="name">对象名称</param>
        /// <param name="capacity">对象池容量</param>
        /// <param name="expireTime">对象过期时间</param>
        /// <param name="priority">对象池优先级</param>
        public GameObjectPool(GameObject gameobjectAsset, string name, int capacity, float expireTime, int priority)
            : this(gameobjectAsset, name, expireTime, capacity, expireTime, priority)
        {

        }

        /// <summary>
        /// 初始化GameObject对象池
        /// </summary>
        /// <param name="gameobjectAsset">对象资源</param>
        /// <param name="name">对象名称</param>
        /// <param name="autoReleaseInterval">对象池自动释放间隔</param>
        /// <param name="capacity">对象池容量</param>
        /// <param name="expireTime">对象过期时间</param>
        /// <param name="priority">对象池优先级</param>
        public GameObjectPool(GameObject gameobjectAsset, string name, float autoReleaseInterval, int capacity, float expireTime, int priority)
        {
            m_GameobjectAsset = gameobjectAsset;
            m_InstanceGameObjectPool = GameEntry.Pool.CreateSingleSpawnObjectPool<InstanceGameObject>(TextUtil.Format("GameObject Instance Pool ({0})", name), autoReleaseInterval, capacity, expireTime, priority);
        }
        #endregion

        /// <summary>
        /// 获取对象池对象资源
        /// </summary>
        public GameObject GameobjectAsset
        {
            get
            {
                return m_GameobjectAsset;
            }
        }

        /// <summary>
        /// 获取对象池名称
        /// </summary>
        public string Name
        {
            get
            {
                return m_InstanceGameObjectPool.Name;
            }
        }

        /// <summary>
        /// 获取对象池完整名称
        /// </summary>
        public string FullName
        {
            get
            {
                return m_InstanceGameObjectPool.FullName;
            }
        }

        /// <summary>
        /// 获取对象池对象类型
        /// </summary>
        public Type ObjectType
        {
            get
            {
                return m_InstanceGameObjectPool.ObjectType;
            }
        }

        /// <summary>
        /// 获取对象池中对象的数量
        /// </summary>
        public int Count
        {
            get
            {
                return m_InstanceGameObjectPool.Count;
            }
        }

        /// <summary>
        /// 获取对象池中能被释放的对象的数量
        /// </summary>
        public int CanReleaseCount
        {
            get
            {
                return m_InstanceGameObjectPool.CanReleaseCount;
            }
        }

        /// <summary>
        /// 获取或设置对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float AutoReleaseInterval
        {
            get
            {
                return m_InstanceGameObjectPool.AutoReleaseInterval;
            }
            set
            {
                m_InstanceGameObjectPool.AutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 获取或设置对象池的容量
        /// </summary>
        public int Capacity
        {
            get
            {
                return m_InstanceGameObjectPool.Capacity;
            }
            set
            {
                m_InstanceGameObjectPool.Capacity = value;
            }
        }

        /// <summary>
        /// 获取或设置对象池对象过期秒数
        /// </summary>
        public float ExpireTime
        {
            get
            {
                return m_InstanceGameObjectPool.ExpireTime;
            }
            set
            {
                m_InstanceGameObjectPool.ExpireTime = value;
            }
        }

        /// <summary>
        /// 获取或设置对象池的优先级
        /// </summary>
        public int Priority
        {
            get
            {
                return m_InstanceGameObjectPool.Priority;
            }
            set
            {
                m_InstanceGameObjectPool.Priority = value;
            }
        }



        /// <summary>
        /// 根据名称获取实例化的GameObject (注意：GameObject的名称为string.Empty)
        /// </summary>
        /// <returns>实例化的GameObject</returns>
        public GameObject Spawn()
        {
            return Spawn(string.Empty);
        }

        /// <summary>
        /// 根据名称获取实例化的GameObject
        /// </summary>
        /// <param name="name">GameObject的名称</param>
        /// <returns>实例化的GameObject</returns>
        public GameObject Spawn(string name)
        {
            GameObject gameObject = null;

            InstanceGameObject instanceGameObject = m_InstanceGameObjectPool.Spawn(name);

            if (instanceGameObject != null)
            {
                gameObject = (GameObject)instanceGameObject.Target;
            }
            else
            {
                gameObject = UnityEngine.Object.Instantiate(m_GameobjectAsset);
                m_InstanceGameObjectPool.Register(new InstanceGameObject(name, gameObject), true);
            }
            return gameObject;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        /// <param name="InstanceGameObject">要回收的实例化对象</param>
        public void UnSpawn(GameObject instanceGameObject)
        {
            if (instanceGameObject == null)
            {
                throw new Exception("InstanceGameObject is invalid.");
            }
            m_InstanceGameObjectPool.Unspawn(instanceGameObject);
        }

        /// <summary>
        /// 清理对象池
        /// </summary>
        public void Clear()
        {
            GameEntry.Pool.DestroyObjectPool(m_InstanceGameObjectPool);
        }
    }
}
