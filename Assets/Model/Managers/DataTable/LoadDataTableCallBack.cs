namespace Myth
{
    /// <summary>
    /// 加载表格成功事件
    /// </summary>
    /// <param name="dataTableAssetName">数据表资源名称</param>
    /// <param name="duration">加载持续时间</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadDataTableSuccessEvent(string dataTableAssetName, float duration, object userData);

    /// <summary>
    /// 加载表格失败事件
    /// </summary>
    /// <param name="dataTableAssetName">数据表资源名称</param>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadDataTableFailureEvent(string dataTableAssetName, string errorMessage, object userData);

    /// <summary>
    /// 加载表格更新事件
    /// </summary>
    /// <param name="dataTableAssetName">数据表资源名称</param>
    /// <param name="progress">加载数据表进度</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadDataTableUpdateEvent(string dataTableAssetName, float progress, object userData);

    /// <summary>
    /// 初始化加载数据表时加载依赖资源事件的新实例
    /// </summary>
    /// <param name="dataTableAssetName">数据表资源名称</param>
    /// <param name="dependencyAssetName">被加载的依赖资源名称</param>
    /// <param name="loadedCount">当前已加载依赖资源数量</param>
    /// <param name="totalCount">总共加载依赖资源数量</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadDataTableDependencyAssetEvent(string dataTableAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData);
}
