using Myth;

namespace Hotfix
{
    /// <summary>
    /// 热更新层UI界面
    /// </summary>
    public abstract class HotUIForm
    {
        private HotfixForm m_UIForm;

        /// <summary>
        /// 获取主工程的界面脚本
        /// </summary>
        public HotfixForm UIForm
        {
            get
            {
                return m_UIForm;
            }
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="uiForm">真正的UI窗口</param>
        /// <param name="userData">用户数据</param>
        public virtual void OnInit(HotfixForm uiForm, object userData)
        {
            m_UIForm = uiForm;
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnOpen(object userData)
        {

        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnClose(object userData)
        {

        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        public virtual void OnPause()
        {

        }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        /// <param name="userData">用户数据</param>
        public virtual void OnResume(object userData)
        {

        }

        /// <summary>
        /// 界面遮挡
        /// </summary>
        /// <param name="userData">用户数据</param>
        public virtual void OnCover(object userData)
        {

        }

        /// <summary>
        /// 界面遮挡恢复
        /// </summary>
        public virtual void OnReveal()
        {

        }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public virtual void OnRefocus(object userData)
        {

        }

        /// <summary>
        /// 界面轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        public virtual void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {

        }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        public virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {

        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        public void Close()
        {
            m_UIForm.Close();
        }
    }
}

