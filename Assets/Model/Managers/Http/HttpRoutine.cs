//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using UnityEngine.Networking;


namespace Myth
{
    /// <summary>
    /// Http发送数据的回调委托
    /// </summary>
    /// <param name="args"></param>
    public delegate void HttpSendDataCallBack(HttpCallBackArgs args);

    /// <summary>
    /// Http访问器
    /// </summary>
    public class HttpRoutine
    {

        #region 属性
        /// <summary>
        /// Http请求回调
        /// </summary>
        private HttpSendDataCallBack m_CallBack;

        /// <summary>
        /// Http请求回调数据
        /// </summary>
        private HttpCallBackArgs m_callBackArgs;


        /// <summary>
        /// 是否繁忙
        /// </summary>
        public bool IsBusy
        {
            get;
            private set;
        }
        #endregion

        public HttpRoutine()
        {
            m_callBackArgs = new HttpCallBackArgs();
        }

        #region SendData 发送web数据
        /// <summary>
        /// 发送web数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        /// <param name="isPost"></param>
        /// <param name="json"></param>
        public void SendData(string url, HttpSendDataCallBack callBack, bool isPost = false, Dictionary<string, object> dic = null)
        {
            if (IsBusy) return;

            IsBusy = true;
            m_CallBack = callBack;

            if (!isPost)
            {
                GetUrl(url);
            }
            else
            {
                //web加密
                if (dic != null)
                {
                    //客户端标识符
                    dic["deviceIdentifier"] = DeviceUtil.DeviceIdentifier;

                    //设备型号
                    dic["deviceModel"] = DeviceUtil.DeviceModel;

                    long t = GameEntry.Data.SysData.CurrServerTime;
                    //签名
                    dic["sign"] = EncryptUtil.Md5(string.Format("{0}:{1}", t, DeviceUtil.DeviceIdentifier));

                    //时间戳
                    dic["t"] = t;
                }

                string json = string.Empty;

                
                if (dic != null)
                {
                    //================使用Json发送数据=================================
                    json = JsonMapper.ToJson(dic);
                    //=================================================================


                    //================使用JsonUtility发送数据==========================(解析字典有问题 使用上面的Json解析)
                    //json = JsonUtility.ToJson(dic);
                    //=================================================================
#if DEBUG_LOG_PROTO
                    Debug.Log("<color=#ffa200>发送消息:</color><color=#FFFB80>" + url + "</color>");
                    Debug.Log("<color=#ffdeb3>==>>" + json + "</color>");
#endif
                    GameEntry.Pool.UnSpawnClassObject(dic);
                }
                

                PostUrl(url, json);
            }
        }
        #endregion

        #region GetUrl Get请求 
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        private void GetUrl(string url)
        {
            //======================UnityWebRequest方法请求=======
            UnityWebRequest data = UnityWebRequest.Get(url);
            GameEntry.Http.StartCoroutine(Request(data));
            //======================UnityWebRequest方法请求=======


            //======================WWW方法请求=======
            /*WWW data = new WWW(url);
            GameEntry.Http.StartCoroutine(Request(data));*/
            //======================WWW方法请求=======

        }


        #endregion

        #region PostUrl Post请求
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        private void PostUrl(string url, string json)
        {
            //定义一个表单
            WWWForm form = new WWWForm();

            //给表单添加值
            form.AddField("", json);

            //======================UnityWebRequest方法请求=======
            UnityWebRequest data = UnityWebRequest.Post(url, form);
            GameEntry.Http.StartCoroutine(Request(data));
            //======================UnityWebRequest方法请求=======


            //======================WWW方法请求=======
            /*WWW data = new WWW(url, form);
            GameEntry.Http.StartCoroutine(Request(data));*/
            //======================WWW方法请求=======
        }
        #endregion


        #region Request 请求服务器 UnityWebRequest版
        /// <summary>
        /// 请求服务器
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerator Request(UnityWebRequest data)
        {
            yield return data.SendWebRequest();

            IsBusy = false;
            if (data.isNetworkError || data.isHttpError)
            {
                if (m_CallBack != null)
                {
                    m_callBackArgs.HasError = true;
                    m_callBackArgs.Value = data.error;
                    m_CallBack(m_callBackArgs);
                }
            }
            else
            {
                if (m_CallBack != null)
                {
                    m_callBackArgs.HasError = false;
                    m_callBackArgs.Value = data.downloadHandler.text;
                    m_CallBack(m_callBackArgs);
                }
            }

#if DEBUG_LOG_PROTO
            Debug.Log("<color=#00eaff>接收消息:</color><color=#00ff9c>" + data.url + "</color>");
            Debug.Log("<color=#c5e1dc>==>>" + JsonUtility.ToJson(m_callBackArgs) + "</color>");
#endif

            data.Dispose();
            data = null;

            Debug.Log(("把Http访问器回池"));
            GameEntry.Pool.UnSpawnClassObject(this);
        }
        #endregion

        #region Request 请求服务器 WWW版
        /// <summary>
        /// 请求服务器
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IEnumerator Request(WWW data)
        {
            yield return data;

            IsBusy = false;
            if (string.IsNullOrEmpty(data.error))
            {
                if (data.text == "null")
                {
                    if (m_CallBack != null)
                    {
                        m_callBackArgs.HasError = true;
                        m_CallBack(m_callBackArgs);
                    }
                }
                else
                {
                    if (m_CallBack != null)
                    {
                        m_callBackArgs.HasError = false;
                        m_callBackArgs.Value = data.text;
                        m_CallBack(m_callBackArgs);
                    }
                }
            }
            else
            {
                Debug.Log("data.error=" + data.error);
                if (m_CallBack != null)
                {
                    m_callBackArgs.HasError = true;
                    m_CallBack(m_callBackArgs);
                }
            }
        }
        #endregion
    }
}