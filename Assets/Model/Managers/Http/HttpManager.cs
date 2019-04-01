//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    public class HttpManager : ManagerBase
    {
        public HttpManager()
        {

        }

        public override void Dispose()
        {
            
        }

        /// <summary>
        /// 发送Http数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack"></param>
        /// <param name="isPost"></param>
        /// <param name="json"></param>
        public void SendData(string url, HttpSendDataCallBack callBack, bool isPost = false, Dictionary<string, object> dic = null)
        {
            Debug.Log("从池中获取Http访问器");

            HttpRoutine http = GameEntry.Pool.SpawnClassObject<HttpRoutine>();
            http.SendData(url, callBack, isPost, dic);
        }
    }
}
