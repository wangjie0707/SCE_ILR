using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Myth;

namespace Hotfix
{
    /// <summary>
    /// 热更新层实体
    /// </summary>
    public abstract class HotEntity
    {
        private HotfixEntity m_Entity;

        /// <summary>
        /// 获取真正的实体
        /// </summary>
        public HotfixEntity Entity
        {
            get
            {
                return m_Entity;
            }
        }

        /// <summary>
        /// 实体初始化
        /// </summary>
        public virtual void OnInit(HotfixEntity entity, object userData)
        {
            m_Entity = entity;
        }

        /// <summary>
        /// 实体显示
        /// </summary>
        public virtual void OnShow(object userData)
        {
            if (HotfixEntry.Entity.EntityInfos.ContainsKey(m_Entity.Id))
            {
                throw new Exception(TextUtil.Format("Entity id '{0}' is already exist.", m_Entity.Id.ToString()));
            }
            HotfixEntry.Entity.EntityInfos.Add(m_Entity.Id, this);
            Debug.Log(userData);
        }

        /// <summary>
        /// 实体隐藏
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnHide(object userData)
        {
            if (!HotfixEntry.Entity.EntityInfos.Remove(m_Entity.Id))
            {
                throw new Exception("Entity info is unmanaged.");
            }
            Debug.Log(userData);
        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        public virtual void OnAttached(EntityBase childEntity, Transform parentTransform, object userData)
        {

        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        public virtual void OnDetached(EntityBase childEntity, object userData)
        {

        }

        /// <summary>
        /// 实体附加子实体
        /// </summary>
        public virtual void OnAttachTo(EntityBase parentEntity, Transform parentTransform, object userData)
        {

        }

        /// <summary>
        /// 实体解除子实体
        /// </summary>
        public virtual void OnDetachFrom(EntityBase parentEntity, object userData)
        {

        }

        /// <summary>
        /// 实体轮询
        /// </summary>
        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
    }
}

