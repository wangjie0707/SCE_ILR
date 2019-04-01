using UnityEngine;
using System.Collections;

namespace Myth
{
    public static class StringUtil
    {
        /// <summary>
        /// 把string类型转换为int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            int temp = 0;
            int.TryParse(str, out temp);
            return temp;
        }


        /// <summary>
        /// 把string类型转换为long
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ToLong(this string str)
        {
            long temp = 0;
            long.TryParse(str, out temp);
            return temp;
        }


        /// <summary>
        /// 把string类型转换为float
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float ToFloat(this string str)
        {
            float temp = 0;
            float.TryParse(str, out temp);
            return temp;
        }
    }
}