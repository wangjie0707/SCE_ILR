using System;

namespace Myth
{
    /// <summary>
    /// 加载数据表信息
    /// </summary>
    public class LoadDataTableInfo
    {
        private readonly Type m_DataTableType;
        private readonly object m_UserData;

        public LoadDataTableInfo(Type dataTableType,object userData)
        {
            m_DataTableType = dataTableType;
            m_UserData = userData;
        }

        public Type DataTableType
        {
            get
            {
                return m_DataTableType;
            }
        }

        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }
    }


}
