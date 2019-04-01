namespace Myth
{
    #region delegate
    /// <summary>
    /// 显示实体成功回调
    /// </summary>
    /// <param name="entity">显示的实体</param>
    /// <param name="duration">加载时长</param>
    /// <param name="userData">用户数据</param>
    /// <returns></returns>
    public delegate void ShowEntitySuccessCallBack(EntityBase entity, float duration, object userData);

    /// <summary>
    /// 显示实体失败回调
    /// </summary>
    /// <param name="entityId">实体编号</param>
    /// <param name="entityAssetName">实体资源名称</param>
    /// <param name="entityGroupName">实体组名称</param>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void ShowEntityFailureCallBack(int entityId, string entityAssetName, string entityGroupName, string errorMessage, object userData);

    /// <summary>
    /// 显示实体更新回调
    /// </summary>
    /// <param name="entityId">实体编号</param>
    /// <param name="entityAssetName">实体资源名称</param>
    /// <param name="entityGroupName">实体组名称</param>
    /// <param name="progress">显示实体进度</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void ShowEntityUpdateCallBack(int entityId, string entityAssetName, string entityGroupName, float progress, object userData);

    /// <summary>
    /// 显示实体时加载依赖资源回调
    /// </summary>
    /// <param name="entityId">实体编号</param>
    /// <param name="entityAssetName">实体资源名称</param>
    /// <param name="entityGroupName">实体组名称</param>
    /// <param name="dependencyAssetName">被加载的依赖资源名称</param>
    /// <param name="loadedCount">当前已加载依赖资源数量</param>
    /// <param name="totalCount">总共加载依赖资源数量</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void ShowEntityDependencyAssetCallBack(int entityId, string entityAssetName, string entityGroupName, string dependencyAssetName, int loadedCount, int totalCount, object userData);

    /// <summary>
    /// 隐藏实体完成回调
    /// </summary>
    /// <param name="entityId">实体编号</param>
    /// <param name="entityAssetName">实体资源名称</param>
    /// <param name="entityGroup">实体所属的实体组</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void HideEntityCompleteCallBack(int entityId, string entityAssetName, EntityGroup entityGroup, object userData);
    #endregion
}
