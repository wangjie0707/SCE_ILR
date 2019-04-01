namespace Myth
{
    /// <summary>
    /// 实体状态
    /// </summary>
    public enum EntityStatus
    {
        /// <summary>
        /// 将要初始化
        /// </summary>
        WillInit,
        /// <summary>
        /// 已经初始化
        /// </summary>
        Inited,
        /// <summary>
        /// 将要显示
        /// </summary>
        WillShow,
        /// <summary>
        /// 已经显示
        /// </summary>
        Showed,
        /// <summary>
        /// 将要影藏
        /// </summary>
        WillHide,
        /// <summary>
        /// 已经影藏
        /// </summary>
        Hidden,
        /// <summary>
        /// 将要回收
        /// </summary>
        WillRecycle,
        /// <summary>
        /// 已经回收
        /// </summary>
        Recycled,
    }
}