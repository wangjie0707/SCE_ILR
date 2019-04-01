using System;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 界面实例对象
    /// </summary>
    public sealed class UIFormInstanceObject : ObjectBase
    {
        private readonly object m_UIFormAsset;

        public UIFormInstanceObject(string name, object uiFormAsset, object uiFormInstance)
            : base(name, uiFormInstance)
        {
            if (uiFormAsset == null)
            {
                throw new Exception("UI form asset is invalid.");
            }
            
            m_UIFormAsset = uiFormAsset;
        }

        protected internal override void Release(bool isShutdown)
        {
            GameEntry.Resource.UnloadAsset((UnityEngine.Object)m_UIFormAsset);
            UnityEngine.Object.Destroy((UnityEngine.Object)Target);
        }
    }
}
