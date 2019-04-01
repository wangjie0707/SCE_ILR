using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    public abstract class UIFormBase : MonoBehaviour
    {

        private int m_SerialId;
        private string m_UIFormAssetName;
        private UIGroup m_UIGroup;
        private int m_DepthInUIGroup;
        private bool m_PauseCoveredUIForm;

        /// <summary>
        /// 获取界面序列编号
        /// </summary>
        public int SerialId
        {
            get
            {
                return m_SerialId;
            }
        }

        /// <summary>
        /// 获取界面资源名称
        /// </summary>
        public string UIFormAssetName
        {
            get
            {
                return m_UIFormAssetName;
            }
        }

        /// <summary>
        /// 获取界面实例
        /// </summary>
        public object Handle
        {
            get
            {
                return gameObject;
            }
        }

        /// <summary>
        /// 获取界面所属的界面组
        /// </summary>
        public UIGroup UIGroup
        {
            get
            {
                return m_UIGroup;
            }
        }

        /// <summary>
        /// 获取界面深度
        /// </summary>
        public int DepthInUIGroup
        {
            get
            {
                return m_DepthInUIGroup;
            }
        }

        /// <summary>
        /// 获取是否暂停被覆盖的界面
        /// </summary>
        public bool PauseCoveredUIForm
        {
            get
            {
                return m_PauseCoveredUIForm;
            }
        }

        /// <summary>
        /// 初始化界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroup">界面所处的界面组</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <param name="isNewInstance">是否是新实例</param>
        /// <param name="userData">用户自定义数据</param>
        public void OnInit(int serialId, string uiFormAssetName, UIGroup uiGroup, bool pauseCoveredUIForm, bool isNewInstance, object userData)
        {
            m_SerialId = serialId;
            m_UIFormAssetName = uiFormAssetName;
            if (isNewInstance)
            {
                m_UIGroup = uiGroup;
            }
            else if (m_UIGroup != uiGroup)
            {
                Log.Error("UI group is inconsistent for non-new-instance UI form.");
                return;
            }

            m_DepthInUIGroup = 0;
            m_PauseCoveredUIForm = pauseCoveredUIForm;

            if (!isNewInstance)
            {
                return;
            }

            OnInit(userData);
        }

        /// <summary>
        /// 界面回收
        /// </summary>
        public void OnRecycle()
        {
            m_SerialId = 0;
            m_DepthInUIGroup = 0;
            m_PauseCoveredUIForm = true;
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected abstract void OnInit(object userData);

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnOpen(object userData)
        {
           
        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnClose(object userData)
        {
            
        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        protected internal virtual void OnPause()
        {
            
        }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        protected internal virtual void OnResume()
        {
            
        }

        /// <summary>
        /// 界面遮挡
        /// </summary>
        protected internal virtual void OnCover()
        {
            
        }

        /// <summary>
        /// 界面遮挡恢复
        /// </summary>
        protected internal virtual void OnReveal()
        {
            
        }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal virtual void OnRefocus(object userData)
        {
           
        }

        /// <summary>
        /// 界面轮询
        /// </summary>
        protected internal virtual void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            
        }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        protected internal virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            m_DepthInUIGroup = depthInUIGroup;
        }
    }
}
