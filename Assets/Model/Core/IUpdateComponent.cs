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
    /// 更新组件接口
    /// </summary>
    public interface IUpdateComponent 
    {
        /// <summary>
        /// 更新方法
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        void OnUpdate(float deltaTime, float unscaledDeltaTime);

        /// <summary>
        /// 实例编号
        /// </summary>
        int InsatnceId { get; }
    }
}
