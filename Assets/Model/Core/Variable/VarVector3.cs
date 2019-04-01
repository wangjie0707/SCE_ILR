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
    /// Vector3变量
    /// </summary>
    public class VarVector3 : Variable<Vector3>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarVector3 Alloc()
        {
            VarVector3 var = GameEntry.Pool.DeQueueVarObject<VarVector3>();//要从对象池获取
            var.Value = Vector3.zero;
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        /// <returns></returns>
        public static VarVector3 Alloc(Vector3 value)
        {
            VarVector3 var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// VarVector3->Vector3
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Vector3(VarVector3 value)
        {
            return value.Value;
        }
    }
}
