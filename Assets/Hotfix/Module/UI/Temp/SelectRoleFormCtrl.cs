using UnityEngine;
using UnityEngine.UI;
using Myth;

namespace Hotfix
{
    public class SelectRoleFormCtrl
    {
        /// <summary>
        /// 控制的窗口
        /// <summary>
        private SelectRoleFormView m_SelectRoleFormView;

        /// <summary>
        /// 界面初始化
        /// <summary>
        /// <param name="view">控制的窗口</param>
        /// <param name="userData">用户数据</param>
        public void OnInit(SelectRoleFormView view, object userData)
        {
            m_SelectRoleFormView = view;
        }

        /// <summary>
        /// 界面打开
        /// <summary>
        /// <param name="userData">用户数据</param>
        public void OnOpen(object userData)
        {
        }

        /// <summary>
        /// 界面关闭
        /// <summary>
        /// <param name="userData">用户数据</param>
        public void OnClose(object userData)
        {
        }

        /// <summary>
        /// 界面暂停
        /// <summary>
        public void OnPause()
        {
        }

        /// <summary>
        /// 界面暂停恢复
        /// <summary>
        /// <param name="userData">用户数据</param>
        public void OnResume(object userData)
        {
        }

        /// <summary>
        /// 界面遮挡
        /// <summary>
        /// <param name="userData">用户数据</param>
        public void OnCover(object userData)
        {
        }

        /// <summary>
        /// 界面遮挡恢复
        /// <summary>
        public void OnReveal()
        {
        }

        /// <summary>
        /// 界面激活
        /// <summary>
        /// <param name="userData">用户数据</param>
        public void OnRefocus(object userData)
        {
        }

        /// <summary>
        /// 界面轮询
        /// <summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
        }

        /// <summary>
        /// 界面深度改变
        /// <summary>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度</param>
        public void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
        }
    }
}
