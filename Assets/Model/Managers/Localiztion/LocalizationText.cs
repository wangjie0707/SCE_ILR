using System;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    /// <summary>
    /// 多语言文本
    /// </summary>
    public class LocalizationText : Text
    {
        [Header("本地化语言的key")]
        [SerializeField]
        private string m_Localzation = null;

        protected override void Start()
        {
            base.Start();
            if (GameEntry.Localization != null)
            {
                if (string.IsNullOrEmpty(m_Localzation))
                {
                    throw new Exception("Localzation key is invalid"); ;
                }
                text = GameEntry.Localization.GetString(m_Localzation);
            }
        }
    }
}