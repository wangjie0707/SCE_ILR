using UnityEngine;
using Myth;

namespace Myth
{
    /// <summary>
    /// 预加载流程
    /// </summary>
    public class ProcedurePreload : ProcedureBase
    {
        public static readonly string[] DataTableNames = new string[]
        {
            "ChapterDBModel",
            "EquipDBModel",
            "GameLevelDBModel",
            "GameLevelGradeDBModel",
         };


        public override void OnEnter()
        {
            base.OnEnter();
            GameEntry.Event.CommonEvent.AddEventListener(LoadAllDataTableCompleteEvent.EventId, OnLoadDataTableComplete);
            GameEntry.Event.CommonEvent.AddEventListener(LoadOneDataTableCompleteEvent.EventId, OnLoadOneDataTableComplete);

            PreloadResources();

        }



        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate( deltaTime,  unscaledDeltaTime);
        }

        public override void OnLeave()
        {
            base.OnLeave();
            GameEntry.Event.CommonEvent.RemoveEventListener(LoadAllDataTableCompleteEvent.EventId, OnLoadDataTableComplete);
            GameEntry.Event.CommonEvent.RemoveEventListener(LoadOneDataTableCompleteEvent.EventId, OnLoadOneDataTableComplete);
        }

        private void PreloadResources()
        {
            GameEntry.Resource.LoadManifest(OnloadManifestComplete);
        }

        private void OnloadManifestComplete()
        {
            Debug.Log("加载依赖配置完毕");
            LoadDataTable();
            LoadHotfixDataTable();
        }

        /// <summary>
        /// 加载主工程里的表格名称
        /// </summary>
        private void LoadDataTable()
        {
            foreach (string dataTableName in DataTableNames)
            {
                
            }
        }

        /// <summary>
        /// 加载热更里面的表格名称
        /// </summary>
        private void LoadHotfixDataTable()
        {
            Debug.Log("todo 加载热更里的表");
            //GameEntry.HotFix.
        }

        /// <summary>
        /// 加载所有表完毕(c#)
        /// </summary>
        /// <param name="userData"></param>
        private void OnLoadDataTableComplete(GameEventBase gameEventBase)
        {
            LoadAllDataTableCompleteEvent loadAllDataTableCompleteEvent = gameEventBase as LoadAllDataTableCompleteEvent;
            if (loadAllDataTableCompleteEvent == null)
            {
                return;
            }
            Debug.Log("加载所有表完毕 表格数量 =" + loadAllDataTableCompleteEvent.TableNumber);

        }

        /// <summary>
        /// 加载单一表完毕
        /// </summary>
        /// <param name="userData"></param>
        private void OnLoadOneDataTableComplete(GameEventBase gameEventBase)
        {
            LoadOneDataTableCompleteEvent loadOneDataTableCompleteEvent = gameEventBase as LoadOneDataTableCompleteEvent;
            if (loadOneDataTableCompleteEvent == null)
            {
                return;
            }
            Debug.Log("tableName=" + loadOneDataTableCompleteEvent.TableName);
        }

        

        public override void OnDestory()
        {
            base.OnDestory();
        }
    }
}
