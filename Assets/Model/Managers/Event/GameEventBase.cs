namespace Myth
{
    public abstract class GameEventBase
    {
        /// <summary>
        /// 获取类型编号
        /// </summary>
        public abstract int Id
        {
            get;
        }

        /// <summary>
        /// 事件发送者
        /// </summary>
        public object Sender
        {
            get;
            set;
        }
    }
}
