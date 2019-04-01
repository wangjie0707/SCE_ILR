﻿namespace Myth
{
    /// <summary>
    /// 下载任务的状态
    /// </summary>
    public enum DownloadTaskStatus
    {
        /// <summary>
        /// 准备下载
        /// </summary>
        Todo,

        /// <summary>
        /// 下载中
        /// </summary>
        Doing,

        /// <summary>
        /// 下载完成
        /// </summary>
        Done,

        /// <summary>
        /// 下载错误
        /// </summary>
        Error
    }
}
