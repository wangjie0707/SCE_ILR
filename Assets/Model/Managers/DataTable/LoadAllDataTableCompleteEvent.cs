namespace Myth
{
    /// <summary>
    /// 加载所有表格事件
    /// </summary>
    public class LoadAllDataTableCompleteEvent : GameEventBase
    {
        /// <summary>
        /// 加载所有表格事件Id
        /// </summary>
        public static readonly int EventId = typeof(LoadAllDataTableCompleteEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }


        /// <summary>
        /// 表格数量
        /// </summary>
        public int TableNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// 填充加载所有表格事件
        /// </summary>
        /// <param name="buffer">加载的buffer</param>
        /// <param name="success">是否成功</param>
        /// <returns></returns>
        public LoadAllDataTableCompleteEvent Fill(int tableNumber)
        {
            TableNumber = tableNumber;

            return this;
        }
    }
}
