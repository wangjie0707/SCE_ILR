namespace Myth
{
    /// <summary>
    /// 本地化管理器
    /// </summary>
    public class LocaliztionManager : ManagerBase
    {
        public LocaliztionManager()
        {

        }

        public override void Dispose()
        {
            
        }

        public string GetString(string key, params object[] args)
        {
            string value = null;
            if (GameEntry.DataTable.GetDataTable<LocalzationDBModel>().LocalzationDic.TryGetValue(key, out value))
            {
                return string.Format(value, args);
            }
            return value;
        }
    }
}
