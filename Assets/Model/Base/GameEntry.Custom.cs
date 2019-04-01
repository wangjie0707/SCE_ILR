using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    public partial class GameEntry
    {
        /// <summary>
        /// 输入组件
        /// </summary>
        public static InputComponent Input
        {
            get;
            private set;
            
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private void InitCustomComponents()
        {
            Input = GetBaseComponent<InputComponent>();
        }
    }
}
