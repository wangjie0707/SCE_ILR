using UnityEngine;
using Myth;

namespace Myth
{
    /// <summary>
    /// 检查更新流程
    /// </summary>
    public class ProcedureCheckVersion : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter ProcedureCheckVersion");
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate( deltaTime,  unscaledDeltaTime);
        }

        public override void OnLeave()
        {
            base.OnLeave();
            Debug.Log("OnLeave ProcedureCheckVersion");
        }

        public override void OnDestory()
        {
            base.OnDestory();
            
        }
    }
}
