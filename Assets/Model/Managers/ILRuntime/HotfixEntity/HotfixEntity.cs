using ILRuntime.CLR.TypeSystem;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 热更新层实体逻辑
    /// </summary>
    public class HotfixEntity : Entity
    {
        private HotfixEntityData m_Data;
        
        private string m_HotfixEntityFullName;

        //热更新层的方法缓存
        private ILInstanceMethod m_OnShow;
        private ILInstanceMethod m_OnHide;
        private ILInstanceMethod m_OnAttached;
        private ILInstanceMethod m_OnDetached;
        private ILInstanceMethod m_OnAttachTo;
        private ILInstanceMethod m_OnDetachFrom;
        private ILInstanceMethod m_OnUpdate;

        /// <summary>
        /// 热更逻辑的全名称
        /// </summary>
        public string HotfixEntityFullName
        {
            get
            {
                return m_HotfixEntityFullName;
            }
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            m_Data = (HotfixEntityData)userData;
            m_HotfixEntityFullName = m_Data.HotfixEntityName;

            //获取热更新层的实例
            IType type = GameEntry.ILRuntime.AppDomain.LoadedTypes[m_HotfixEntityFullName];
            object hotfixInstance = ((ILType)type).Instantiate();

            //获取热更新层的方法并缓存
            m_OnShow = new ILInstanceMethod(hotfixInstance, m_HotfixEntityFullName, "OnShow", 1);
            m_OnHide = new ILInstanceMethod(hotfixInstance, m_HotfixEntityFullName, "OnHide", 1);
            m_OnAttached = new ILInstanceMethod(hotfixInstance, m_HotfixEntityFullName, "OnAttached", 3);
            m_OnDetached = new ILInstanceMethod(hotfixInstance, m_HotfixEntityFullName, "OnDetached", 2);
            m_OnAttachTo = new ILInstanceMethod(hotfixInstance, m_HotfixEntityFullName, "OnAttachTo", 3);
            m_OnDetachFrom = new ILInstanceMethod(hotfixInstance, m_HotfixEntityFullName, "OnDetachFrom", 2);
            m_OnUpdate = new ILInstanceMethod(hotfixInstance, m_HotfixEntityFullName, "OnUpdate", 2);

            //调用热更新层的OnInit
            GameEntry.ILRuntime.AppDomain.Invoke(m_HotfixEntityFullName, "OnInit", hotfixInstance, this, m_Data.UserData);
        }

        protected internal override void OnShow(object userData)
        {
            base.OnShow(userData);

            m_Data = (HotfixEntityData)userData;
            m_OnShow.Invoke(m_Data.UserData);
        }

        protected internal override void OnHide(object userData)
        {
            base.OnHide(userData);

            m_OnHide.Invoke(userData);
            
        }

        protected internal override void OnAttached(EntityBase childEntity, Transform parentTransform, object userData)
        {
            base.OnAttached(childEntity, parentTransform, userData);

            m_OnAttached.Invoke(childEntity, parentTransform, userData);
        }

        protected internal override void OnDetached(EntityBase childEntity, object userData)
        {
            base.OnDetached(childEntity, userData);

            m_OnDetached.Invoke(childEntity, userData);
        }

        protected internal override void OnAttachTo(EntityBase parentEntity, Transform parentTransform, object userData)
        {
            base.OnAttachTo(parentEntity, parentTransform, userData);

            m_OnAttachTo.Invoke(parentEntity, parentTransform, userData);
        }

        protected internal override void OnDetachFrom(EntityBase parentEntity, object userData)
        {
            base.OnDetachFrom(parentEntity, userData);

            m_OnDetachFrom.Invoke(parentEntity, userData);
        }

        protected internal override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);

            m_OnUpdate.Invoke(deltaTime, unscaledDeltaTime);
        }


    }
}

