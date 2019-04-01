namespace Myth
{
    /// <summary>
    /// 加载单个表格事件
    /// </summary>
    public class LoadOneDataTableCompleteEvent : GameEventBase
    {
        /// <summary>
        /// 加载单个表格事件Id
        /// </summary>
        public static readonly int EventId = typeof(LoadOneDataTableCompleteEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        

        /// <summary>
        /// 表格名称
        /// </summary>
        public string TableName
        {
            get;
            private set;
        }

        /// <summary>
        /// 填充单个表格事件
        /// </summary>
        /// <param name="tableName">加载的表格名称</param>
        /// <returns></returns>
        public LoadOneDataTableCompleteEvent Fill(string tableName)
        {
            TableName = tableName;

            return this;
        }
    }
}
