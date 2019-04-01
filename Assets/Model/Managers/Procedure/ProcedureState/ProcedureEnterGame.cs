using UnityEngine;
using Myth;

namespace Myth
{
    /// <summary>
    /// 进入游戏流程
    /// </summary>
    public class ProcedureEnterGame : ProcedureBase
    {
        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log("OnEnter ProcedureEnterGame");

            string name = GameEntry.Procedure.GetData<string>("name");
            int code = GameEntry.Procedure.GetData<int>("code");

            Debug.Log("name=" + name);
            Debug.Log("code=" + code);
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate( deltaTime,  unscaledDeltaTime);
        }

        public override void OnLeave()
        {
            base.OnLeave();
            Debug.Log("OnLeave ProcedureEnterGame");
        }

        public override void OnDestory()
        {
            base.OnDestory();

        }
    }
}
