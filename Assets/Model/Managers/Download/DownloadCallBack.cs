namespace Myth
{
    /// <summary>
    /// 下载代理开始回调
    /// </summary>
    /// <param name="serialId">下载任务的序列编号</param>
    /// <param name="downloadPath">下载后存放路径</param>
    /// <param name="downloadUri">下载地址</param>
    /// <param name="currentLength">当前大小</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void DownloadAgentStart(int serialId, string downloadPath, string downloadUri, int currentLength, object userData);


    /// <summary>
    /// 下载代理更新回调
    /// </summary>
    /// <param name="serialId">下载任务的序列编号</param>
    /// <param name="downloadPath">下载后存放路径</param>
    /// <param name="downloadUri">下载地址</param>
    /// <param name="currentLength">当前大小</param>
    /// <param name="bytes">下载的数据流</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void DownloadAgentUpdate(int serialId, string downloadPath, string downloadUri, int currentLength, object userData);

    /// <summary>
    /// 下载代理成功回调
    /// </summary>
    /// <param name="serialId">下载任务的序列编号</param>
    /// <param name="downloadPath">下载后存放路径</param>
    /// <param name="downloadUri">下载地址</param>
    /// <param name="currentLength">当前大小</param>
    /// <param name="bytes">下载的数据流</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void DownloadAgentSuccess(int serialId, string downloadPath, string downloadUri, int currentLength, object userData);

    /// <summary>
    /// 下载代理失败回调
    /// </summary>
    /// <param name="serialId">下载任务的序列编号</param>
    /// <param name="downloadPath">下载后存放路径</param>
    /// <param name="downloadUri">下载地址</param>
    /// <param name="errorMessage">错误信息</param>
    /// <param name="userData">用户自定义数据</param>
    public delegate void DownloadAgentError(int serialId, string downloadPath, string downloadUri, string errorMessage, object userData);
}
