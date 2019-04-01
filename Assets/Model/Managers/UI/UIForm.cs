using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Myth
{
    public abstract class UIForm : UIFormBase
    {
        public const int DepthFactor = 100;
        private const int UGuiGroupDepthFactor = 10000;
        private const float FadeTime = 0.3f;

        private bool m_Available = false;
        private bool m_Visible = false;
        private int m_OriginalLayer = 0;
        private Canvas m_CachedCanvas = null;
        private CanvasGroup m_CanvasGroup = null;

        /// <summary>
        /// 初始层级数
        /// </summary>
        public int OriginalDepth
        {
            get;
            private set;
        }

        /// <summary>
        /// 当前层级数
        /// </summary>
        public int Depth
        {
            get
            {
                return m_CachedCanvas.sortingOrder;
            }
        }

        /// <summary>
        /// 获取或设置界面名称
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
        /// 获取界面是否可用
        /// </summary>
        public bool Available
        {
            get
            {
                return m_Available;
            }
        }

        /// <summary>
        /// 获取或设置界面是否可见
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
                    Log.Warning("UI form '{0}' is not available.", Name);
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
        /// 获取已缓存的 Transform
        /// </summary>
        public Transform CachedTransform
        {
            get;
            private set;
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected override void OnInit(object userData)
        {
            if (CachedTransform == null)
            {
                CachedTransform = transform;
            }

            m_OriginalLayer = gameObject.layer;

            m_CachedCanvas = gameObject.GetOrAddComponent<Canvas>();
            m_CachedCanvas.overrideSorting = true;
            OriginalDepth = m_CachedCanvas.sortingOrder;

            m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            RectTransform RectTransform = GetComponent<RectTransform>();
            RectTransform.anchorMin = Vector2.zero;
            RectTransform.anchorMax = Vector2.one;
            RectTransform.anchoredPosition = Vector2.zero;
            RectTransform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            m_Available = true;
            Visible = true;

            m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnClose(object userData)
        {
            base.OnClose(userData);
            gameObject.SetLayerRecursively(m_OriginalLayer);
            Visible = false;
            m_Available = false;
        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        protected internal override void OnPause()
        {
            base.OnPause();
            Visible = false;
        }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        protected internal override void OnResume()
        {
            base.OnResume();
            Visible = true;

            m_CanvasGroup.alpha = 0f;
            StopAllCoroutines();
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, FadeTime));
            }
        }

        /// <summary>
        /// 界面遮挡
        /// </summary>
        protected internal override void OnCover()
        {
            base.OnCover();
        }

        /// <summary>
        /// 界面遮挡恢复
        /// </summary>
        protected internal override void OnReveal()
        {
            base.OnReveal();
        }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
        }

        /// <summary>
        /// 界面轮询
        /// </summary>
        protected internal override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        protected internal override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int oldDepth = Depth;
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
            int deltaDepth = UGuiGroupDepthFactor * uiGroupDepth + DepthFactor * depthInUIGroup - oldDepth + OriginalDepth;
            Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].sortingOrder += deltaDepth;
            }
        }

        /// <summary>
        /// 设置界面的可见性
        /// </summary>
        /// <param name="visible">界面的可见性</param>
        protected virtual void InternalSetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        /// <summary>
        /// 关闭自身界面
        /// </summary>
        public virtual void Close()
        {
            GameEntry.UI.CloseUIForm(this);
        }
    }
}