using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// Http组件
    /// </summary>
    public class HttpComponent : GameBaseComponent
    {
        [SerializeField]
        [Header("Regular Url")]
        private string m_WebAccountUrl;

        [SerializeField]
        [Header("Test Url")]
        private string m_TestWebAccountUrl;

        [SerializeField]
        private bool m_IsTest;

        /// <summary>
        /// 真实的帐号服务器Url
        /// </summary>
        public string RealWebAccountUrl
        {
            get
            {
                return m_IsTest ? m_TestWebAccountUrl : m_WebAccountUrl;
            }
        }

        /// <summary>
        /// Http管理器
        /// </summary>
        private HttpManager m_HttpManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_HttpManager = new HttpManager();
        }

        /// <summary>
        /// 发送Http数据 Get请求
        /// </summary>
        /// <param name="url">Http请求地址</param>
        /// <param name="callBack">Http请求回调</param>
        public void SendData(string url, HttpSendDataCallBack callBack)
        {
            SendData(url, callBack, false, null);
        }

        /// <summary>
        /// 发送Http数据
        /// </summary>
        /// <param name="url">Http请求地址</param>
        /// <param name="callBack">Http请求回调</param>
        /// <param name="isPost">是否post请求</param>
        /// <param name="dic">请求数据</param>
        public void SendData(string url, HttpSendDataCallBack callBack, bool isPost, Dictionary<string, object> dic)
        {
            m_HttpManager.SendData(url, callBack, isPost, dic);
        }

        public override void Shutdown()
        {
            base.Shutdown();
        }
    }
}
