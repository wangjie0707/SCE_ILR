//===================================================
//
//===================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    /// <summary>
    /// 表格管理器
    /// </summary>
    public class DataTableManager : ManagerBase
    {
        private Dictionary<string, IDataTable> m_DataTables;

        public LoadDataTableSuccessEvent LoadDataTableSuccess;
        public LoadDataTableFailureEvent LoadDataTableFailure;
        public LoadDataTableUpdateEvent LoadDataTableUpdate;
        public LoadDataTableDependencyAssetEvent LoadDataTableDependencyAsset;

        private readonly LoadAssetCallbacks m_LoadAssetCallbacks;

        public DataTableManager()
        {
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadDataTableSuccessCallback, LoadDataTableFailureCallback, LoadDataTableUpdateCallback, LoadDataTableDependencyAssetCallback);
            m_DataTables = new Dictionary<string, IDataTable>();
            LoadDataTableSuccess = null;
            LoadDataTableFailure = null;
        }


        public override void Dispose()
        {
            lock (m_DataTables)
            {
                foreach (KeyValuePair<string, IDataTable> dataTable in m_DataTables)
                {
                    dataTable.Value.Clear();
                }

                m_DataTables.Clear();
            }
        }

        /// <summary>
        /// 已经加载的表格数量
        /// </summary>
        public int DataTablesCount
        {
            get
            {
                lock (m_DataTables)
                {
                    return m_DataTables.Count;
                }
            }
        }

        
        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <typeparam name="T">数据表的类型</typeparam>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable<T>() where T : IDataTable
        {
            return HasDataTable(typeof(T).FullName);
        }

        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <param name="dataTableType">数据表的类型</param>
        /// <returns>是否存在数据表</returns>
        public bool HasDataTable(Type dataTableType)
        {
            if (dataTableType == null)
            {
                throw new Exception("dataTableType is invalid.");
            }

            return HasDataTable(dataTableType.FullName);
        }

        /// <summary>
        /// 是否存在数据表
        /// </summary>
        /// <param name="dataTableName">数据表名称</param>
        /// <returns>是否存在数据表</returns>
        private bool HasDataTable(string dataTableName)
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                throw new Exception("dataTableName is invalid.");
            }
            lock (m_DataTables)
            {
                return m_DataTables.ContainsKey(dataTableName);
            }
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="T">要加载的表格类型</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="priority">加载数据表资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable<T>(string dataTableAssetName, int priority, object userData) where T : IDataTable
        {
            LoadDataTable(typeof(T), dataTableAssetName,  priority, userData);
        }

        /// <summary>
        /// 加载数据表
        /// </summary>
        /// <param name="datatableType">要加载的表格类型</param>
        /// <param name="dataTableAssetName">数据表资源名称</param>
        /// <param name="priority">加载数据表资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        public void LoadDataTable(Type datatableType, string dataTableAssetName,  int priority, object userData)
        {
            GameEntry.Resource.LoadAsset(dataTableAssetName,  typeof(TextAsset), priority, m_LoadAssetCallbacks, new LoadDataTableInfo(datatableType, userData));
        }

        /// <summary>
        /// 获取表格
        /// </summary>
        /// <param name="datatableName">要获取的表格名称</param>
        /// <returns>获取到的表格</returns>
        private IDataTable GetDataTable(string datatableName)
        {
            IDataTable dataTable = null;
            lock (m_DataTables)
            {
                if (m_DataTables.TryGetValue(datatableName, out dataTable))
                {
                    return dataTable;
                }
            }
            throw new Exception(TextUtil.Format("Could find datatable '{0}'", datatableName));
        }

        /// <summary>
        /// 获取表格
        /// </summary>
        /// <param name="datatableType">要获取的表格类型</param>
        /// <returns>获取到的表格</returns>
        public IDataTable GetDataTable(Type datatableType)
        {
            IDataTable dataTable = null;
            string fullName = datatableType.FullName;
            lock (m_DataTables)
            {
                if (m_DataTables.TryGetValue(fullName, out dataTable))
                {
                    return dataTable;
                }
            }
            throw new Exception(TextUtil.Format("Could find datatable '{0}'", fullName));
        }

        /// <summary>
        /// 获取表格
        /// </summary>
        /// <typeparam name="T">要获取的表格类型</typeparam>
        /// <returns>获取到的表格</returns>
        public T GetDataTable<T>() where T : IDataTable
        {
            string fullName = typeof(T).FullName;
            IDataTable dataTable = null;
            lock (m_DataTables)
            {
                if (m_DataTables.TryGetValue(fullName, out dataTable))
                {
                    return (T)dataTable;
                }
            }
            throw new Exception(TextUtil.Format("Could find datatable '{0}'", fullName));
        }



        /// <summary>
        /// 获取所有数据表
        /// </summary>
        /// <returns>所有数据表</returns>
        public IDataTable[] GetAllDataTables()
        {
            int index = 0;
            lock (m_DataTables)
            {
                IDataTable[] results = new IDataTable[m_DataTables.Count];
                foreach (KeyValuePair<string, IDataTable> dataTable in m_DataTables)
                {
                    results[index++] = dataTable.Value;
                }
                return results;
            }
        }

        /// <summary>
        /// 获取所有数据表
        /// </summary>
        /// <param name="results">所有数据表</param>
        public void GetAllDataTables(List<IDataTable> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            lock (m_DataTables)
            {
                foreach (KeyValuePair<string, IDataTable> dataTable in m_DataTables)
                {
                    results.Add(dataTable.Value);
                }
            }
        }

        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <typeparam name="T">要移除的表格类型</typeparam>
        /// <returns>是否销毁数据表成功</returns>
        public bool DestroyDataTable<T>() where T : IDataTable
        {
            return DestroyDataTable(typeof(T).FullName);
        }

        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <param name="dataRowType">数据表的类型</param>
        /// <returns>是否销毁数据表成功</returns>
        public bool DestroyDataTable(Type dataTableType)
        {
            if (dataTableType == null)
            {
                throw new Exception("Data row type is invalid.");
            }

            return DestroyDataTable(dataTableType.FullName);
        }

        /// <summary>
        /// 销毁数据表
        /// </summary>
        /// <param name="dataTableName">要销毁的数据表名称</param>
        /// <returns>是否销毁数据表成功</returns>
        private bool DestroyDataTable(string dataTableName)
        {
            IDataTable dataTable = null;
            lock (m_DataTables)
            {
                if (m_DataTables.TryGetValue(dataTableName, out dataTable))
                {
                    dataTable.Clear();
                    return m_DataTables.Remove(dataTableName);
                }
            }
            return false;
        }

        private bool CreateDataTable(string dataTableAssetName, UnityEngine.Object dataTableAsset, Type datatableType, object userData)
        {
            TextAsset textAsset = dataTableAsset as TextAsset;
            if (textAsset == null)
            {
                Log.Warning("Data table asset '{0}' is invalid.", dataTableAssetName);
                return false;
            }

            if (datatableType == null)
            {
                Log.Warning("Data row type is invalid.");
                return false;
            }

            if (HasDataTable(datatableType))
            {
                throw new Exception(TextUtil.Format("Already exist data table '{0}'.", datatableType.FullName));
            }

            IDataTable dataTable = (IDataTable)Activator.CreateInstance(datatableType);

            if (dataTable == null)
            {
                throw new Exception(TextUtil.Format("Can't createdatatable '{0}'.", datatableType.FullName));
            }

            dataTable.LoadData(textAsset.bytes);
            m_DataTables.Add(datatableType.FullName, dataTable);
            return true;
        }

        private void LoadDataTableSuccessCallback(string dataTableAssetName, UnityEngine.Object dataTableAsset, float duration, object userData)
        {
            LoadDataTableInfo loadDataTableInfo = (LoadDataTableInfo)userData;
            if (loadDataTableInfo == null)
            {
                throw new Exception("Load data table info is invalid.");
            }

            try
            {
                if (!CreateDataTable(dataTableAssetName, dataTableAsset, loadDataTableInfo.DataTableType, loadDataTableInfo.UserData))
                {
                    throw new Exception(TextUtil.Format("Load data table failure in helper, asset name '{0}'.", dataTableAssetName));
                }
            }
            catch (Exception exception)
            {
                if (LoadDataTableFailure != null)
                {
                    LoadDataTableFailure(dataTableAssetName, exception.ToString(), loadDataTableInfo.UserData);
                    return;
                }

                throw;
            }
            finally
            {
                GameEntry.Resource.UnloadAsset(dataTableAsset);
            }

            if (LoadDataTableSuccess != null)
            {
                LoadDataTableSuccess(dataTableAssetName, duration, loadDataTableInfo.UserData);
            }
        }

        private void LoadDataTableFailureCallback(string dataTableAssetName, string errorMessage, object userData)
        {
            LoadDataTableInfo loadDataTableInfo = (LoadDataTableInfo)userData;
            if (loadDataTableInfo == null)
            {
                throw new Exception("Load data table info is invalid.");
            }

            string appendErrorMessage = TextUtil.Format("Load data table failure, asset name '{0}', error message '{1}'.", dataTableAssetName, errorMessage);
            if (LoadDataTableFailure != null)
            {
                LoadDataTableFailure(dataTableAssetName, appendErrorMessage, loadDataTableInfo.UserData);
                return;
            }

            throw new Exception(appendErrorMessage);
        }

        private void LoadDataTableUpdateCallback(string dataTableAssetName, float progress, object userData)
        {
            LoadDataTableInfo loadDataTableInfo = (LoadDataTableInfo)userData;
            if (loadDataTableInfo == null)
            {
                throw new Exception("Load data table info is invalid.");
            }

            if (LoadDataTableUpdate != null)
            {
                LoadDataTableUpdate(dataTableAssetName, progress, loadDataTableInfo.UserData);
            }
        }

        private void LoadDataTableDependencyAssetCallback(string dataTableAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            LoadDataTableInfo loadDataTableInfo = (LoadDataTableInfo)userData;
            if (loadDataTableInfo == null)
            {
                throw new Exception("Load data table info is invalid.");
            }

            if (LoadDataTableDependencyAsset != null)
            {
                LoadDataTableDependencyAsset(dataTableAssetName, dependencyAssetName, loadedCount, totalCount, loadDataTableInfo.UserData);
            }
        }
    }
}
