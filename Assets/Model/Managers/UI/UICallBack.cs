namespace Myth
{
    /// <summary>
    /// 打开界面成功事件
    /// </summary>
    /// <param name="uiForm">加载成功的界面</param>
    /// <param name="duration">加载持续时间</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void OpenUIFormSuccessEvent(UIFormBase uiForm, float duration, object userData);

    /// <summary>
    /// 打开界面失败事件
    /// </summary>
    /// <param name="serialId">界面序列编号</param>
    /// <param name="uiFormAssetName">界面资源名称</param>
    /// <param name="uiGroupName">界面组名称</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void OpenUIFormFailureEvent(int serialId, string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, string errorMessage, object userData);

    /// <summary>
    /// 打开界面更新事件
    /// </summary>
    /// <param name="serialId">界面序列编号</param>
    /// <param name="uiFormAssetName">界面资源名称</param>
    /// <param name="uiGroupName">界面组名称</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
    /// <param name="progress">打开界面进度</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void OpenUIFormUpdateEvent(int serialId, string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, float progress, object userData);

    /// <summary>
    /// 打开界面时加载依赖资源事件
    /// </summary>
    /// <param name="serialId">界面序列编号</param>
    /// <param name="uiFormAssetName">界面资源名称</param>
    /// <param name="uiGroupName">界面组名称</param>
    /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
    /// <param name="dependencyAssetName">被加载的依赖资源名称</param>
    /// <param name="loadedCount">当前已加载依赖资源数量</param>
    /// <param name="totalCount">总共加载依赖资源数量</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void OpenUIFormDependencyAssetEvent(int serialId, string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, string dependencyAssetName, int loadedCount, int totalCount, object userData);

    /// <summary>
    /// 关闭界面完成事件
    /// </summary>
    /// <param name="serialId">界面序列编号</param>
    /// <param name="uiFormAssetName">界面资源名称</param>
    /// <param name="uiGroup">界面所属的界面组</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void CloseUIFormCompleteEvent(int serialId, string uiFormAssetName, UIGroup uiGroup, object userData);
}
