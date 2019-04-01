using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Myth
{
    /// <summary>
    /// 多语言图片
    /// </summary>
    public class LocalizationImage : Image
    {
        [Header("本地化语言的key")]
        [SerializeField]
        private string m_Localzation;

        protected override void Start()
        {
            base.Start();
            if (GameEntry.Localization != null)
            {
                string path = string.Format("Assets/Download/UI/UIResources/{0}.png", GameEntry.Localization.GetString(m_Localzation));
#if UNITY_EDITOR
                Texture2D texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(path) as Texture2D;

                Sprite obj = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                sprite = obj;
                SetNativeSize();
#endif
            }
        }
    }
}