using UnityEngine;
namespace Myth
{
    /// <summary>
    /// 实体抽象类
    /// </summary>
    public abstract class Entity : EntityBase
    {
        private int m_OriginalLayer = 0;
        private Transform m_OriginalTransform = null;

        /// <summary>
        /// 初始化实体
        /// </summary>
        /// <param name="userData"></param>
        protected override void OnInit(object userData)
        {
            m_OriginalLayer = gameObject.layer;
            m_OriginalTransform = SelfTransform.parent;
        }

        /// <summary>
        /// 实体显示
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);
            SelfTransform.localScale = Vector3.one;
        }

        /// <summary>
        /// 实体隐藏
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnHide(object userData)
        {
            base.OnHide(userData);
            gameObject.SetLayerRecursively(m_OriginalLayer);
        }

        /// <summary>
        /// 实体附加子实体(父物体触发)
        /// </summary>
        /// <param name="childEntity">附加的子实体</param>
        /// <param name="parentTransform">被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnAttached(EntityBase childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);
        }

        /// <summary>
        /// 实体解除子实体(父物体触发)
        /// </summary>
        /// <param name="childEntity">解除的子实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnDetached(EntityBase childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);
        }

        /// <summary>
        /// 实体附加子实体(子物体触发)
        /// </summary>
        /// <param name="parentEntity">被附加的父实体</param>
        /// <param name="parentTransform">被附加父实体的位置</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnAttachTo(EntityBase parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);
            SelfTransform.SetParent(parentTransform);
        }

        /// <summary>
        /// 实体解除子实体(子物体触发)
        /// </summary>
        /// <param name="parentEntity">被解除的父实体</param>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnDetachFrom(EntityBase parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);
            SelfTransform.SetParent(m_OriginalTransform);
        }

        /// <summary>
        /// 实体轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        protected internal override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
        }
    }
}