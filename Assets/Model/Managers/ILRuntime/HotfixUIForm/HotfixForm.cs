using ILRuntime.CLR.TypeSystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Myth
{
    /// <summary>
    /// HotfixForm窗口
    /// </summary>
    public class HotfixForm : UIForm
    {
        [SerializeField]
        private HotFormInfo[] m_HotFormInfos;

        //热更新层的方法缓存
        private ILInstanceMethod m_OnOpen;
        private ILInstanceMethod m_OnClose;
        private ILInstanceMethod m_OnPause;
        private ILInstanceMethod m_OnResume;
        private ILInstanceMethod m_OnCover;
        private ILInstanceMethod m_OnReveal;
        private ILInstanceMethod m_OnRefocus;
        private ILInstanceMethod m_OnUpdate;
        private ILInstanceMethod m_OnDepthChanged;


        /// <summary>
        /// 获取界面组件信息
        /// </summary>
        public HotFormInfo[] HotFormInfos
        {
            get
            {
                return m_HotFormInfos;
            }
        }

        /// <summary>
        /// 根据索引返回组件 
        /// </summary>
        /// <param name="index">索引编号</param>
        /// <returns>返回的组件</returns>
        public UnityEngine.Object GetTransType(int index)
        {
            HotFormInfo hotFormInfo = HotFormInfos[index];
            switch (hotFormInfo.HotAttributeType)
            {
                case HotAttributeType.Unknow:
                    return hotFormInfo.Trans;
                case HotAttributeType.Text:
                    return hotFormInfo.Trans.GetComponent<Text>();
                case HotAttributeType.Image:
                    return hotFormInfo.Trans.GetComponent<Image>();
                case HotAttributeType.RawImage:
                    return hotFormInfo.Trans.GetComponent<RawImage>();
                case HotAttributeType.Button:
                    return hotFormInfo.Trans.GetComponent<Button>();
                case HotAttributeType.LocalizationImage:
                    return hotFormInfo.Trans.GetComponent<LocalizationImage>();
                case HotAttributeType.LocalizationText:
                    return hotFormInfo.Trans.GetComponent<LocalizationText>();
                case HotAttributeType.Slider:
                    return hotFormInfo.Trans.GetComponent<Slider>();
                case HotAttributeType.Scrollbar:
                    return hotFormInfo.Trans.GetComponent<Scrollbar>();
                case HotAttributeType.ScrollRect:
                    return hotFormInfo.Trans.GetComponent<ScrollRect>();
                case HotAttributeType.Toggle:
                    return hotFormInfo.Trans.GetComponent<Toggle>();
                case HotAttributeType.Mask:
                    return hotFormInfo.Trans.GetComponent<Mask>();
                case HotAttributeType.InputField:
                    return hotFormInfo.Trans.GetComponent<InputField>();
                case HotAttributeType.Dropdown:
                    return hotFormInfo.Trans.GetComponent<Dropdown>();
            }
            return hotFormInfo.Trans;
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="userData">用户数据</param>
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            string hotfixUGuiFormFullName = this.gameObject.name;
            if (hotfixUGuiFormFullName.Contains("(Clone)"))
            {
                hotfixUGuiFormFullName = hotfixUGuiFormFullName.Split(new string[] { "(Clone)" }, StringSplitOptions.RemoveEmptyEntries)[0];
            }
            hotfixUGuiFormFullName = TextUtil.Format("Hotfix.{0}View", hotfixUGuiFormFullName);

            //获取热更新层的实例
            IType type = GameEntry.ILRuntime.AppDomain.LoadedTypes[hotfixUGuiFormFullName];
            object hotfixInstance = ((ILType)type).Instantiate();

            //获取热更新层的方法并缓存
            m_OnOpen = new ILInstanceMethod(hotfixInstance, hotfixUGuiFormFullName, "OnOpen", 1);
            m_OnClose = new ILInstanceMethod(hotfixInstance, hotfixUGuiFormFullName, "OnClose", 1);
            m_OnPause = new ILInstanceMethod(hotfixInstance, hotfixUGuiFormFullName, "OnPause", 0);
            m_OnResume = new ILInstanceMethod(hotfixInstance, hotfixUGuiFormFullName, "OnResume", 0);
            m_OnCover = new ILInstanceMethod(hotfixInstance, hotfixUGuiFormFullName, "OnCover", 0);
            m_OnReveal = new ILInstanceMethod(hotfixInstance, hotfixUGuiFormFullName, "OnReveal", 0);
            m_OnRefocus = new ILInstanceMethod(hotfixInstance, hotfixUGuiFormFullName, "OnRefocus", 1);
            m_OnUpdate = new ILInstanceMethod(hotfixInstance, hotfixUGuiFormFullName, "OnUpdate", 2);
            m_OnDepthChanged = new ILInstanceMethod(hotfixInstance, hotfixUGuiFormFullName, "OnDepthChanged", 2);

            //调用热更新层的OnInit
            GameEntry.ILRuntime.AppDomain.Invoke(hotfixUGuiFormFullName, "OnInit", hotfixInstance, this, userData);
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData"></param>
        protected internal override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            m_OnOpen?.Invoke(userData);
        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData"></param>
        protected internal override void OnClose(object userData)
        {
            base.OnClose(userData);

            m_OnClose?.Invoke(userData);
        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        protected internal override void OnPause()
        {
            base.OnPause();

            m_OnPause?.Invoke();
        }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        protected internal override void OnResume()
        {
            base.OnResume();

            m_OnResume?.Invoke();
        }

        /// <summary>
        /// 界面遮挡
        /// </summary>
        protected internal override void OnCover()
        {
            base.OnCover();

            m_OnCover?.Invoke();
        }

        /// <summary>
        /// 界面遮挡恢复
        /// </summary>
        protected internal override void OnReveal()
        {
            base.OnReveal();

            m_OnReveal?.Invoke();
        }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        protected internal override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);

            m_OnRefocus?.Invoke(userData);
        }

        /// <summary>
        /// 界面轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        protected internal override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);

            m_OnUpdate?.Invoke(deltaTime, unscaledDeltaTime);
        }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        protected internal override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);

            m_OnDepthChanged?.Invoke(uiGroupDepth, depthInUIGroup);
        }

    }
}
