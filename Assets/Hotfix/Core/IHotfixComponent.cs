namespace Hotfix
{
    /// <summary>
    /// 热更组件接口
    /// </summary>
    public interface IHotfixComponent
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Init();

        /// <summary>
        /// 组件轮询
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        void OnUpdate(float deltaTime, float unscaledDeltaTime);

        /// <summary>
        /// 关闭并清理游戏框架模块。
        /// </summary>
        void Shutdown();
    }
}
