using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 时间组件
    /// </summary>
    public class TimeComponent : GameBaseComponent
    {
        /// <summary>
        /// 游戏运行时间
        /// </summary>
        public float GameTime
        {
            get
            {
                return Time.timeSinceLevelLoad;
            }
        }

        /// <summary>
        /// 真正的游戏运行(没有缩放)
        /// </summary>
        public float RealGameTime
        {
            get
            {
                return Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// 时间管理器
        /// </summary>
        private TimeManager m_TimeManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            float a = Time.realtimeSinceStartup;
            m_TimeManager = new TimeManager();
        }

        #region 定时器
        /// <summary>
        /// 得到定时器数量
        /// </summary>
        public int TimeActionCount
        {
            get
            {
                return m_TimeManager.TimeActionCount;
            }
        }

        /// <summary>
        /// 得到所有定时器
        /// </summary>
        /// <returns>得到的定时器集合</returns>
        public TimeAction[] GetAllTimeAction()
        {
            return m_TimeManager.GetAllTimeAction();
        }

        /// <summary>
        /// 得到所有定时器
        /// </summary>
        public void GetAllTimeAction(List<TimeAction> results)
        {
            m_TimeManager.GetAllTimeAction(results);
        }



        /// <summary>
        /// 注册定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RegisterTimeAction(TimeAction action)
        {
            m_TimeManager.RegisterTimeAction(action);
        }

        /// <summary>
        /// 移除定时器
        /// </summary>
        /// <param name="action"></param>
        internal void RemoveTimeAction(TimeAction action)
        {
            m_TimeManager.RemoveTimeAction(action);
            GameEntry.Pool.UnSpawnClassObject(action);
        }

        /// <summary>
        /// 创建定时器
        /// </summary>
        /// <returns></returns>
        public TimeAction CreateTimeAction()
        {
            return GameEntry.Pool.SpawnClassObject<TimeAction>();
        }
        #endregion

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            //Debug.Log("时间组件更新");
            m_TimeManager.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_TimeManager.Dispose();
        }
    }
}
