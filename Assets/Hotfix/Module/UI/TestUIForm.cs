using UnityEngine.UI;
using Myth;
using UnityEngine;

namespace Hotfix
{
    public class TestUIForm : HotUIForm
    {
        /// <summary>
        /// 文本
        /// </summary>
        public Text Text
        {
            get;
            private set;
        }

        /// <summary>
        /// 界面初始化
        /// </summary>
        /// <param name="uiForm">真正的UI窗口</param>
        /// <param name="userData">用户数据</param>
        public override void OnInit(HotfixForm uiForm, object userData)
        {
            base.OnInit(uiForm, userData);
            UnityEngine.Debug.Log("初始化UI");
            Text = uiForm.GetTransType(0) as Text;
        }

        /// <summary>
        /// 界面打开
        /// </summary>
        /// <param name="userData">用户数据</param>
        public override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            UnityEngine.Debug.Log("打开UI");
        }

        /// <summary>
        /// 界面关闭
        /// </summary>
        /// <param name="userData">用户数据</param>
        public override void OnClose(object userData)
        {
            base.OnClose(userData);
            UnityEngine.Debug.Log("关闭UI");
        }

        /// <summary>
        /// 界面暂停
        /// </summary>
        public override void OnPause()
        {
            base.OnPause();
        }

        /// <summary>
        /// 界面暂停恢复
        /// </summary>
        /// <param name="userData"></param>
        public override void OnResume(object userData)
        {
            base.OnResume(userData);
        }

        /// <summary>
        /// 界面遮挡
        /// </summary>
        /// <param name="userData">用户数据</param>
        public override void OnCover(object userData)
        {
            base.OnCover(userData);
        }

        /// <summary>
        /// 界面遮挡恢复
        /// </summary>
        public override void OnReveal()
        {
            base.OnReveal();
        }

        /// <summary>
        /// 界面激活
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
        }

        /// <summary>
        /// 界面轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        /// <summary>
        /// 界面深度改变
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        public override void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
            base.OnDepthChanged(uiGroupDepth, depthInUIGroup);
        }
    }
}
