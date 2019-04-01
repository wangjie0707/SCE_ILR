using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myth
{
    /// <summary>
    /// 任务代理接口
    /// </summary>
    /// <typeparam name="T">任务类型</typeparam>
    internal interface ITaskAgent<T> where T : ITask
    {
        /// <summary>
        /// 获取任务
        /// </summary>
        T Task
        {
            get;
        }

        /// <summary>
        /// 初始化任务代理
        /// </summary>
        void Initialize();

        /// <summary>
        /// 任务代理轮询
        /// </summary>
        void OnUpdate(float deltaTime, float unscaledDeltaTime);

        /// <summary>
        /// 关闭并清理任务代理
        /// </summary>
        void Shutdown();

        /// <summary>
        /// 开始处理任务
        /// </summary>
        /// <param name="task">要处理的任务</param>
        void StartTask(T task);

        /// <summary>
        /// 停止正在处理的任务并重置任务代理
        /// </summary>
        void Reset();
    }
}
