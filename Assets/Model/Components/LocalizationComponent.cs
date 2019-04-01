using System;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 本地化组件
    /// </summary>
    public class LocalizationComponent : GameBaseComponent
    {
        [SerializeField]
        private Language m_CurrLanguage;

        /// <summary>
        /// 当前语言 要和本地化表的语言字段一致
        /// </summary>
        public Language CurrLanguage
        {
            get
            {
                return m_CurrLanguage;
            }
            set
            {
                m_CurrLanguage = value;
            }
        }

        private LocaliztionManager m_LocaliztionManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_LocaliztionManager = new LocaliztionManager();
        }

        public string GetString(string key, params object[] args)
        {
            return m_LocaliztionManager.GetString(key, args);
        }

        
        public override void Shutdown()
        {
            m_LocaliztionManager.Dispose();
        }
    }
}
