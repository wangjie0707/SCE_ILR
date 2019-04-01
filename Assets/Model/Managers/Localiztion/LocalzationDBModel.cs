using System.Collections.Generic;

namespace Myth
{
    /// <summary>
    /// Localzation数据管理
    /// </summary>
    public partial class LocalzationDBModel : DataTableDBModelBase<LocalzationDBModel, DataTableEntityBase>
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public override string DataTableName
        {
            get
            {
                return "Localization/" + GameEntry.Localization.CurrLanguage.ToString();
            }
        }

        /// <summary>
        /// 当前语言字典
        /// </summary>
        public Dictionary<string, string> LocalzationDic = new Dictionary<string, string>();

        /// <summary>
        /// 加载列表
        /// </summary>
        protected override void LoadList(MMO_MemoryStream ms)
        {
            int rows = ms.ReadInt();
            ms.ReadInt();

            for (int i = 0; i < rows; i++)
            {
                LocalzationDic[ms.ReadUTF8String()] = ms.ReadUTF8String();
            }
        }
    }
}