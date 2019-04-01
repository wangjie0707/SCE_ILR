using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 数据表组件
    /// </summary>
    public class DataTableComponent : GameBaseComponent
    {

        [SerializeField]
        private bool m_EnableLoadDataTableSuccessEvent = true;

        [SerializeField]
        private bool m_EnableLoadDataTableFailureEvent = true;

        [SerializeField]
        private bool m_EnableLoadDataTableUpdateEvent = false;

        [SerializeField]
        private bool m_EnableLoadDataTableDependencyAssetEvent = false;

        /// <summary>
        /// 表格管理器
        /// </summary>
        public DataTableManager m_DataTableManager;

        protected override void OnAwake()
        {
            base.OnAwake();
            m_DataTableManager = new DataTableManager();
            m_DataTableManager.LoadDataTableSuccess += OnLoadDataTableSuccess;
            m_DataTableManager.LoadDataTableFailure += OnLoadDataTableFailure;
            m_DataTableManager.LoadDataTableUpdate += OnLoadDataTableUpdate;
            m_DataTableManager.LoadDataTableDependencyAsset += OnLoadDataTableDependencyAsset;
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_DataTableManager.Dispose();
        }



        /// <summary>
        /// 已经加载的表格数量
        /// </summary>
        public int DataTablesCount
        {
            get
            {
                return m_DataTableManager.DataTablesCount;
            }
        }
        

        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <typeparam name="T">数据表的类型</typeparam>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable<T>() where T : IDataTable
        {
            return m_DataTableManager.HasDataTable<T>();
        }

        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <param name="dataTableType">数据表的类型</param>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable(Type dataTableType)
        {
            return m_DataTableManager.HasDataTable(dataTableType);
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="T">要加载的表格类型</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="priority">加载数据表资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable<T>(string dataTableAssetName,  int priority, object userData) where T : IDataTable
        {
            m_DataTableManager.LoadDataTable<T>(dataTableAssetName,  priority, userData);
        }


        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="datatableType">要加载的表格类型</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="priority">加载数据表资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable(Type datatableType, string dataTableAssetName,   int priority, object userData)
        {
            m_DataTableManager.LoadDataTable(datatableType, dataTableAssetName,  priority, userData);
        }

        /// <summary>
        /// 获取表格
        /// </summary>
        /// <param name="datatableType">要获取的表格类型</param>
        /// <returns>获取到的表格</returns>
        public IDataTable GetDataTable(Type datatableType)
        {
            return m_DataTableManager.GetDataTable(datatableType);
        }

        /// <summary>
        /// 获取表格
        /// </summary>
        /// <typeparam name="T">要获取的表格类型</typeparam>
        /// <returns>获取到的表格</returns>
        public T GetDataTable<T>() where T : IDataTable
        {
            return m_DataTableManager.GetDataTable<T>();
        }

        /// <summary>
        /// 获取所有数据表
        /// </summary>
        /// <returns>所有数据表</returns>
        public IDataTable[] GetAllDataTables()
        {
            return m_DataTableManager.GetAllDataTables();
        }

        /// <summary>
        /// 获取所有数据表
        /// </summary>
        /// <param name="results">所有数据表</param>
        public void GetAllDataTables(List<IDataTable> results)
        {
            m_DataTableManager.GetAllDataTables(results);
        }

        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <typeparam name="T">要移除的表格类型</typeparam>
        /// <returns>是否销毁数据表成功</returns>
        public bool DestroyDataTable<T>() where T : IDataTable
        {
            return m_DataTableManager.DestroyDataTable<T>();
        }

        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <param name="dataRowType">数据表的类型</param>
        /// <returns>是否销毁数据表成功</returns>
        public bool DestroyDataTable(Type dataTableType)
        {
            return m_DataTableManager.DestroyDataTable(dataTableType);
        }




        private void OnLoadDataTableSuccess(string dataTableAssetName, float duration, object userData)
        {
            if (m_EnableLoadDataTableSuccessEvent)
            {
                //todo
            }
        }

        private void OnLoadDataTableFailure(string dataTableAssetName, string errorMessage, object userData)
        {
            Log.Warning("Load data table failure, asset name '{0}', error message '{1}'.", dataTableAssetName, errorMessage);
            if (m_EnableLoadDataTableFailureEvent)
            {
                //todo
            }
        }

        private void OnLoadDataTableUpdate(string dataTableAssetName, float progress, object userData)
        {
            if (m_EnableLoadDataTableUpdateEvent)
            {
                //todo
            }
        }

        private void OnLoadDataTableDependencyAsset(string dataTableAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            if (m_EnableLoadDataTableDependencyAssetEvent)
            {
                //todo
            }
        }
    }
}
