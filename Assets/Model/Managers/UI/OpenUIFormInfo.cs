namespace Myth
{
    /// <summary>
    /// 打开UI窗口信息
    /// </summary>
    public class OpenUIFormInfo
    {
        private readonly int m_SerialId;
        private readonly UIGroup m_UIGroup;
        private readonly bool m_PauseCoveredUIForm;
        private readonly object m_UserData;

        public OpenUIFormInfo(int serialId, UIGroup uiGroup, bool pauseCoveredUIForm, object userData)
        {
            m_SerialId = serialId;
            m_UIGroup = uiGroup;
            m_PauseCoveredUIForm = pauseCoveredUIForm;
            m_UserData = userData;
        }

        /// <summary>
        /// 获取UI序列号
        /// </summary>
        public int SerialId
        {
            get
            {
                return m_SerialId;
            }
        }

        /// <summary>
        /// 获取UI组
        /// </summary>
        public UIGroup UIGroup
        {
            get
            {
                return m_UIGroup;
            }
        }

        /// <summary>
        /// 获取是否暂停被覆盖的界面
        /// </summary>
        public bool PauseCoveredUIForm
        {
            get
            {
                return m_PauseCoveredUIForm;
            }
        }

        /// <summary>
        /// 获取用户数据
        /// </summary>
        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }
    }
}
