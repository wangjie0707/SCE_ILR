using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System.Collections.Generic;
using System.Reflection;
using Myth;

namespace ILHotfix
{
    /// <summary>
    /// 热修复脚本
    /// </summary>
    public class HotScripts 
    {
        public const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        /// <summary>
        /// 替换函数
        /// </summary>
        /// <param name="type">被替换的类型</param>
        /// <param name="name">被替换的方法名称</param>
        /// <param name="info">替换的方法</param>
        /// <returns>是否替换成功</returns>
        public static bool ReplaceFunc(System.Type type, string name, MethodInfo info)
        {
            return ReplaceField(type, "__Hotfix_" + name, info);
        }

        /// <summary>
        /// 替换属性
        /// </summary>
        /// <param name="type">被替换的类型</param>
        /// <param name="fieldName">被替换的属性</param>
        /// <param name="info">替换的方法</param>
        /// <returns>是否替换成功</returns>
        public static bool ReplaceField(System.Type type, string fieldName, MethodInfo info)
        {
            var field = type.GetField(fieldName, bindingFlags);
            if (field == null)
            {
                Log.Info("ReplaceField type:{0} fieldName:{1} not find!", type.Name, fieldName);
                return false;
            }

            if (info == null)
            {
                field.SetValue(null, null);
                Log.Info("ReplaceFunction type:{0} fieldName:{1} Cannel!", type.Name, fieldName);
            }
            else
            {
                if (!IsStatic(info))
                {
                    Log.Error("ReplaceField type:{0} fieldName:{1} methodInfo is not static!", type.Name, fieldName, info.Name);
                    return false;
                }

                field.SetValue(null, new ILHotfix.DelegateBridge(info));
                Log.Info("ReplaceFunction type:{0} fieldName:{1}", type.Name, fieldName);
            }

            return true;
        }

        /// <summary>
        /// 是否静态
        /// </summary>
        /// <param name="info">替换方法</param>
        /// <returns>返回是否静态</returns>
        private static bool IsStatic(MethodInfo info)
        {

            if (info is ILRuntime.Reflection.ILRuntimeMethodInfo)
            {
                var ilMI = info as ILRuntime.Reflection.ILRuntimeMethodInfo;
                return ilMI.ILMethod.IsStatic;
            }

            return info.IsStatic;
        }
    }
}
