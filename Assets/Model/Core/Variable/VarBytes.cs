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
    /// byte[]变量
    /// </summary>
    public class VarBytes : Variable<byte[]>
    {
        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <returns></returns>
        public static VarBytes Alloc()
        {
            VarBytes var = GameEntry.Pool.DeQueueVarObject<VarBytes>();//要从对象池获取
            var.Value = null;
            var.Retain();
            return var;
        }

        /// <summary>
        /// 分配一个对象
        /// </summary>
        /// <param name="value">初始值</param>
        /// <returns></returns>
        public static VarBytes Alloc(byte[] value)
        {
            VarBytes var = Alloc();
            var.Value = value;
            return var;
        }

        /// <summary>
        /// VarBytes-> byte[]
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator byte[] (VarBytes value)
        {
            return value.Value;
        }
    }
}
