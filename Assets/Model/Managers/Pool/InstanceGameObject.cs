using System;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 游戏物体对象实例
    /// </summary>
    public class InstanceGameObject : ObjectBase
    {
        public InstanceGameObject(string name, object gameobjectInstance)
           : base(name, gameobjectInstance)
        {
            if (gameobjectInstance == null)
            {
                throw new Exception("GameobjectInstance is invalid.");
            }
        }

        /// <summary>
        /// 获取对象事件
        /// </summary>
        protected internal override void OnSpawn()
        {
            base.OnSpawn();
            GameObject gameObject = (GameObject)Target;
            if (gameObject!=null)
            {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 回收对象事件
        /// </summary>
        protected internal override void OnUnspawn()
        {
            base.OnUnspawn();
            GameObject gameObject = (GameObject)Target;
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }

        protected internal override void Release(bool isShutdown)
        {
            UnityEngine.Object gameobject = (UnityEngine.Object)Target;
            if (gameobject == null)
            {
                return;
            }
            Debug.Log("销毁");
            UnityEngine.Object.Destroy(gameobject);
        }
    }
}
