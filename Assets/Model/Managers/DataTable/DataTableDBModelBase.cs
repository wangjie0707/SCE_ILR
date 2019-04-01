//===================================================
//
//===================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Myth
{
    /// <summary>
    /// 数据表管理基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="P"></typeparam>
    public abstract class DataTableDBModelBase<T, P> : IDataTable
    where T : class, new()
    where P : DataTableEntityBase
    {
        protected List<P> m_List;
        protected Dictionary<int, P> m_Dic;

        public DataTableDBModelBase()
        {
            m_List = new List<P>();
            m_Dic = new Dictionary<int, P>();
        }

        /// <summary>
        /// 获取数据表的类型
        /// </summary>
        public Type Type
        {
            get
            {
                return typeof(T);
            }
        }


        #region 需要子类实现的属性或方法
        /// <summary>
        /// 数据表名
        /// </summary>
        public abstract string DataTableName { get; }

        /// <summary>
        /// 加载数据列表
        /// </summary>
        protected abstract void LoadList(MMO_MemoryStream ms);
        #endregion


        #region LoadData 加载数据表数据
        /// <summary>
        /// 加载数据表数据
        /// </summary>
        /// <param name="buffer"></param>
        public void LoadData(byte[] buffer)
        {
            //加载数据
            using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
            {
                LoadList(ms);
            }
        }
        #endregion

        #region EntityCount 实体数量
        /// <summary>
        /// 实体数量
        /// </summary>
        public int EntityCount
        {
            get
            {
                return m_List.Count;
            }
        }
        #endregion

        #region GetList 获取集合
        /// <summary>
        /// 获取集合
        /// </summary>
        /// <returns></returns>
        public List<P> GetList()
        {
            return m_List;
        }
        #endregion

        #region Get 根据编号获取实体
        /// <summary>
        /// 根据编号获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public P Get(int id)
        {
            if (m_Dic.ContainsKey(id))
            {
                return m_Dic[id];
            }
            return null;
        }
        #endregion


        public void Clear()
        {
            m_List.Clear();
            m_Dic.Clear();
        }
    }
}