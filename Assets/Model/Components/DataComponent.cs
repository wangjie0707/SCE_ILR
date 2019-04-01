namespace Myth
{
    /// <summary>
    /// 数据组件
    /// </summary>
    public class DataComponent : GameBaseComponent
    {
        /// <summary>
        /// 临时缓存数据
        /// </summary>
        public CacheData CacheData
        {
            get;
            private set;
        }

        /// <summary>
        /// 系统数据
        /// </summary>
        public SysData SysData
        {
            get;
            private set;
        }

        /// <summary>
        /// 用户数据
        /// </summary>
        public UserData UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 关卡地图数据
        /// </summary>
        public PVEMapData PVEMapData
        {
            get;
            private set;
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            CacheData = new CacheData();
            SysData = new SysData();
            UserData = new UserData();
            PVEMapData = new PVEMapData();
        }

        public override void Shutdown()
        {
            base.Shutdown();
            CacheData.Dispose();
            SysData.Dispose();
            UserData.Dispose();
            PVEMapData.Dispose();
        }
    }
}