using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 加载资源成功回调函数
    /// </summary>
    /// <param name="assetName">要加载的资源名称</param>
    /// <param name="asset">已加载的资源</param>
    /// <param name="duration">加载持续时间</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadAssetSuccessCallback(string assetName, Object asset, float duration, object userData);

    /// <summary>
    /// 加载资源时加载依赖资源回调函数
    /// </summary>
    /// <param name="assetName">要加载的资源名称</param>
    /// <param name="dependencyAssetName">被加载的依赖资源名称</param>
    /// <param name="loadedCount">当前已加载依赖资源数量</param>
    /// <param name="totalCount">总共加载依赖资源数量</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadAssetDependencyAssetCallback(string assetName, string dependencyAssetName, int loadedCount, int totalCount, object userData);

    /// <summary>
    /// 加载资源失败回调函数
    /// </summary>
    /// <param name="assetName">要加载的资源名称</param>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadAssetFailureCallback(string assetName, string errorMessage, object userData);


    /// <summary>
    /// 加载资源更新回调函数
    /// </summary>
    /// <param name="assetName">要加载的资源名称</param>
    /// <param name="progress">加载资源进度</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void LoadAssetUpdateCallback(string assetName, float progress, object userData);


    /// <summary>
    /// 读取数据流回调函数
    /// </summary>
    /// <param name="fileUri">文件路径</param>
    /// <param name="bytes">数据流</param>
    /// <param name="errorMessage">错误信息</param>
    public delegate void LoadBytesCallback(string fileUri, byte[] bytes, string errorMessage);

    #region 场景相关

    /// <summary>
    /// 卸载场景成功回调函数
    /// </summary>
    /// <param name="sceneAssetName">要卸载的场景资源名称</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void UnloadSceneSuccessCallback(string sceneAssetName, object userData);

    /// <summary>
    /// 卸载场景失败回调函数
    /// </summary>
    /// <param name="sceneAssetName">要卸载的场景资源名称</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void UnloadSceneFailureCallback(string sceneAssetName, object userData);
    #endregion
}
