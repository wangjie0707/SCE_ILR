namespace Myth
{
    /// <summary>
    /// 流程状态
    /// </summary>
    public enum ProcedureState
    {
        /// <summary>
        /// 启动
        /// </summary>
        Launch = 0,
        /// <summary>
        /// 检查更新
        /// </summary>
        CheckVersion = 1,
        /// <summary>
        /// 预加载
        /// </summary>
        Preload = 2,
        /// <summary>
        /// 切换场景
        /// </summary>
        ChangeScene = 3,
        /// <summary>
        /// 登录
        /// </summary>
        LogOn = 4,
        /// <summary>
        /// 选人
        /// </summary>
        SelectRole = 5,
        /// <summary>
        /// 进入游戏
        /// </summary>
        EnterGame = 6,
        /// <summary>
        /// 世界地图
        /// </summary>
        WorldMap = 7,
        /// <summary>
        /// 游戏关卡
        /// </summary>
        GameLevel = 8
    }
}
