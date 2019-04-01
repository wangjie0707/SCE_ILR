using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 常量
    /// </summary>
    public static partial class Constant
    {
        /// <summary>
        /// 层
        /// </summary>
		public static class Leyer
        {
            public const string DefaultLayerName = "Default";
            /// <summary>
            /// Default层
            /// </summary>
            public static readonly int DefaultLayerId = LayerMask.NameToLayer(DefaultLayerName);



            public const string UILayerName = "UI";
            /// <summary>
            /// UI层
            /// </summary>
            public static readonly int UILayerId = LayerMask.NameToLayer(UILayerName);
        }

    }
}
