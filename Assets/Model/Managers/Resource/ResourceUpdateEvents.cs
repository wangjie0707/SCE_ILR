using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 资源更新开始事件
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="downloadPath">资源下载后存放路径</param>
    /// <param name="downloadUri">资源下载地址</param>
    /// <param name="currentLength">当前下载大小</param>
    /// <param name="zipLength">压缩包大小</param>
    /// <param name="retryCount">已重试下载次数</param>
    public delegate void ResourceUpdateStartEvent(string name, string downloadPath, string downloadUri, int currentLength, int zipLength, int retryCount);

    /// <summary>
    /// 资源更新改变事件
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="downloadPath">资源下载后存放路径</param>
    /// <param name="downloadUri">资源下载地址</param>
    /// <param name="currentLength">当前下载大小</param>
    /// <param name="zipLength">压缩包大小</param>
    public delegate void ResourceUpdateChangedEvent(string name, string downloadPath, string downloadUri, int currentLength, int zipLength);

    /// <summary>
    /// 资源更新成功事件
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="downloadPath">资源下载后存放路径</param>
    /// <param name="downloadUri">资源下载地址</param>
    /// <param name="length">资源大小</param>
    /// <param name="zipLength">压缩包大小</param>
    public delegate void ResourceUpdateSuccessEvent(string name, string downloadPath, string downloadUri, int length, int zipLength);

    /// <summary>
    /// 资源更新失败事件
    /// </summary>
    /// <param name="name">资源名称</param>
    /// <param name="downloadUri">下载地址</param>
    /// <param name="retryCount">已重试次数</param>
    /// <param name="totalRetryCount">设定的重试次数</param>
    /// <param name="errorMessage">错误信息</param>
    /// <remarks>当已重试次数达到设定的重试次数时，将不再重试</remarks>
    public delegate void ResourceUpdateFailureEvent(string name, string downloadUri, int retryCount, int totalRetryCount, string errorMessage);
    
    /// <summary>
    /// 使用单机模式并初始化资源完成的回调事件
    /// </summary>
    public delegate void InitResourcesCompleteCallback();

    /// <summary>
    /// 版本资源列表更新成功回调事件
    /// </summary>
    /// <param name="downloadPath">版本资源列表更新后存放路径</param>
    /// <param name="downloadUri">版本资源列表更新地址</param>
    public delegate void UpdateVersionListSuccessCallback(string downloadPath, string downloadUri);

    /// <summary>
    /// 版本资源列表更新失败回调事件
    /// </summary>
    /// <param name="downloadUri">版本资源列表更新地址</param>
    /// <param name="errorMessage">错误信息</param>
    public delegate void UpdateVersionListFailureCallback(string downloadUri, string errorMessage);

    /// <summary>
    /// 使用可更新模式并检查资源完成的回调事件
    /// </summary>
    /// <param name="needUpdateResources">是否需要进行资源更新</param>
    /// <param name="removedCount">已移除的资源数量</param>
    /// <param name="updateCount">要更新的资源数量</param>
    /// <param name="updateTotalLength">要更新的资源总大小</param>
    /// <param name="updateTotalZipLength">要更新的压缩包总大小</param>
    public delegate void CheckResourcesCompleteCallback(bool needUpdateResources, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength);

    /// <summary>
    /// 使用可更新模式并更新资源全部完成的回调事件
    /// </summary>
    public delegate void UpdateResourcesCompleteCallback();

    /// <summary>
    /// 加载依赖配置事件
    /// </summary>
    /// <param name="assetBundleManifest">加载的依赖配置文件</param>
    public delegate void LoadManifestCallBack(AssetBundleManifest assetBundleManifest);

    /// <summary>
    /// 加载依赖配置成功事件
    /// </summary>
    public delegate void LoadManifestCompleteCallBack();
}
