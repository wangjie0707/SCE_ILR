//===================================================
//
//===================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    /// <summary>
    /// 流程基类
    /// </summary>
    public abstract class ProcedureBase : FsmState<ProcedureManager>
    {
        public override void OnInit()
        {
            base.OnInit();
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        public override void OnLeave()
        {
            base.OnLeave();
        }

        public override void OnDestory()
        {
            base.OnDestory();
        }
    }
}
