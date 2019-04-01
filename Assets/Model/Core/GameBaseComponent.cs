//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    public abstract class GameBaseComponent : GameComponent, IUpdateComponent
    {
        protected override void OnAwake()
        {
            base.OnAwake();

            //把自己加入基础组件列表
            GameEntry.RegisterBaseComponent(this);
            GameEntry.RegisterUpdateComponent(this);
        }

        /// <summary>
        /// 关闭方法
        /// </summary>
        public virtual void Shutdown()
        {
            GameEntry.RemoveUpdateComponent(this);
        }

        public virtual void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {

        }
    }
}
