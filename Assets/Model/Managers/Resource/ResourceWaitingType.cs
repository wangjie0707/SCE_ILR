
namespace Myth
{
    /// <summary>
    /// 等待资源类型
    /// </summary>
    public enum WaitingType
    {
        None = 0,
        /// <summary>
        /// 等待Asset
        /// </summary>
        WaitForAsset,
        /// <summary>
        /// 等待依赖资源
        /// </summary>
        WaitForDependencyAsset,
        /// <summary>
        /// 等待资源
        /// </summary>
        WaitForResource,
    }
}
