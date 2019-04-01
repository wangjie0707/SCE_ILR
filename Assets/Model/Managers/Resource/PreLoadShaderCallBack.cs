namespace Myth
{
    /// <summary>
    /// 预加载Shader成功
    /// </summary>
    public delegate void PreLoadShaderSuccess();

    /// <summary>
    /// 预加载Shdaer失败
    /// </summary>
    /// <param name="errorMessage">错误信息</param>
    public delegate void PreLoadShaderFailure(string errorMessage);

    public class PreLoadShaderCallBack
    {
        private readonly PreLoadShaderSuccess m_PreLoadShaderSuccess;
        private readonly PreLoadShaderFailure m_PreLoadShaderFailure;

        /// <summary>
        /// 初始化预加载shader回调函数集的新实例
        /// </summary>
        /// <param name="preLoadShaderSuccess"></param>
        /// <param name="preLoadShaderFailure"></param>
        public PreLoadShaderCallBack(PreLoadShaderSuccess preLoadShaderSuccess, PreLoadShaderFailure preLoadShaderFailure)
        {
            m_PreLoadShaderSuccess = preLoadShaderSuccess;
            m_PreLoadShaderFailure = preLoadShaderFailure;
        }

        /// <summary>
        /// 获取加载Shdaer失败函数
        /// </summary>
        public PreLoadShaderFailure PreLoadShaderFailure
        {
            get
            {
                return m_PreLoadShaderFailure;
            }
        }

        /// <summary>
        /// 获取加载Shdaer成功函数
        /// </summary>
        public PreLoadShaderSuccess PreLoadShaderSuccess
        {
            get
            {
                return m_PreLoadShaderSuccess;
            }
        }
    }
}
