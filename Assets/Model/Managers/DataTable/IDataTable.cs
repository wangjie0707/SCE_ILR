using System;

namespace Myth
{
    /// <summary>
    /// 数据表接口
    /// </summary>
    public interface IDataTable
    {
        /// <summary>
        /// 数据实体数量
        /// </summary>
        int EntityCount
        {
            get;
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <param name="buffer">数据内容</param>
        void LoadData(byte[] buffer);

        /// <summary>
        /// 清理数据
        /// </summary>
        void Clear();

        /// <summary>
        /// 获取数据表的类型
        /// </summary>
        Type Type
        {
            get;
        }
    }
}
