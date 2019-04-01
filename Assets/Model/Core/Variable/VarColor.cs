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
    /// Color变量
    /// </summary>
    public class VarColor : Variable<Color>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarColor Alloc()
        {
            VarColor var = GameEntry.Pool.DeQueueVarObject<VarColor>();//要从对象池获取
            var.Value = Color.white;
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        /// <returns></returns>
        public static VarColor Alloc(Color value)
        {
            VarColor var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// VarColor->Color
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Color(VarColor value)
        {
            return value.Value;
        }
    }
}
