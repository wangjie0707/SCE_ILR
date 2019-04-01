using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class EntityBase : MonoBehaviour
    {
        private bool m_Available = false;
        private bool m_Visible = false;
        private int m_Id;
        private string m_EntityAssetName;
        private EntityGroup m_EntityGroup;

        /// <summary>
        /// 获取或设置实体名称
        /// </summary>
        public string Name
        {
            get
            {
                return gameObject.name;
            }
            set
            {
                gameObject.name = value;
            }
        }

        /// <summary>
        /// 获取实体是否可用
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return m_Available;
            }
        }

        /// <summary>
        /// 获取或设置实体是否可见
        /// </summary>
        public bool Visible
        {
            get
            {
                return m_Available && m_Visible;
            }
            set
            {
                if (!m_Available)
                {
                    Log.Warning("Entity '{0}' is not available.", Name);
                    return;
                }

                if (m_Visible == value)
                {
                    return;
                }

                m_Visible = value;
                InternalSetVisible(value);
            }
        }

        /// <summary>
        /// 获取实体编号
        /// </summary>
        public int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取实体资源名称
        /// </summary>
        public string EntityAssetName
        {
            get
            {
                return m_EntityAssetName;
            }
        }

        /// <summary>
        /// 获取实体实例
        /// </summary>
        public object InstanceGameObject
        {
            get
            {
                return this.gameObject;
            }
        }

        /// <summary>
        /// 获取自身的 Transform
        /// </summary>
        public Transform SelfTransform
        {
            get
            {
                return transform;
            }
        }

        /// <summary>
        /// 获取实体所属的实体组
        /// </summary>
        public EntityGroup EntityGroup
        {
            get
            {
                return m_EntityGroup;
            }
        }

        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="entityId">实体编号</param>
        /// <param name="entityAssetName">实体资源名称</param>
        /// <param name="entityGroup">实体所属的实体组</param>
        /// <param name="isNewInstance">是否是新实例</param>
        /// <param name="isNewLogic">是否是新逻辑</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnInit(int entityId, string entityAssetName, EntityGroup entityGroup, bool isNewInstance, bool isNewLogic, object userData)
        {
            m_Id = entityId;
            m_EntityAssetName = entityAssetName;
            
            if (isNewInstance|| isNewLogic)
            {
                m_EntityGroup = entityGroup;
            }
            else if (m_EntityGroup != entityGroup)
            {
                Log.Error("Entity group is inconsistent for non-new-instance entity.");
                return;
            }

            if (isNewLogic)
            {
                OnInit(userData);
            }
        }

        /// <summary>
        /// 实体回收
        /// </summary>
        public void OnRecycle()
        {
            m_Id = 0;
            this.enabled = false;
        }

        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected abstract void OnInit(object userData);

        /// <summary>
        /// 实体显示
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnShow(object userData)
        {
            m_Available = true;
            Visible = true;
        }

        /// <summary>
        /// 实体隐藏
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnHide(object userData)
        {
            Visible = false;
            m_Available = false;
        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="childEntity">附加的子实体</param>
        /// <param name="parentTransform">相对于被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnAttached(EntityBase childEntity, Transform parentTransform, object userData)
        {

        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="childEntity">解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnDetached(EntityBase childEntity, object userData)
        {

        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnAttachTo(EntityBase parentEntity, Transform parentTransform, object userData)
        {

        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnDetachFrom(EntityBase parentEntity, object userData)
        {

        }

        /// <summary>
        /// 实体轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        protected internal virtual void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {

        }
        

        /// <summary>
        /// 设置实体的可见性
        /// </summary>
        /// <param name="visible">实体的可见性</param>
        protected virtual void InternalSetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }
    }
}
