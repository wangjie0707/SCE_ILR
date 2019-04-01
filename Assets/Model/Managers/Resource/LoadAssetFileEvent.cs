
namespace Myth
{
    /// <summary>
    /// 加载资源文件事件
    /// </summary>
    public class LoadAssetFileEvent : GameEventBase
    {
        /// <summary>
        /// 加载资源文件事件Id
        /// </summary>
        public static readonly int EventId = typeof(LoadAssetFileEvent).GetHashCode();

        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        /// <summary>
        /// 加载的资源名称
        /// </summary>
        public string AssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 加载的buffer
        /// </summary>
        public byte[] Buffer
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success
        {
            get;
            private set;
        }

        /// <summary>
        /// 填充加载依赖项事件
        /// </summary>
        /// <param name="buffer">加载的buffer</param>
        /// <param name="success">是否成功</param>
        /// <returns></returns>
        public LoadAssetFileEvent Fill(string assetname,byte[] buffer, bool success)
        {
            AssetName = assetname;
            Buffer = buffer;
            Success = success;

            return this;
        }
    }
}
