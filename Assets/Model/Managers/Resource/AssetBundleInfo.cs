using System;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// AssetBundle资源信息
    /// </summary>
    public class AssetBundleInfo
    {
        private string m_AssetBundleName;
        private AssetBundle m_AssetBundle;
        private int m_SpawnCount;
        private DateTime m_LastUseTime;
        private bool m_IsLock;

        /// <summary>
        /// 初始化Assetbundle对象的新实例
        /// </summary>
        /// <param name="assetPath">路径</param>
        /// <param name="obj">Assetbundle对象</param>
        /// <param name="spawned">对象是否已被获取</param>
        public AssetBundleInfo(string assetBundleName, AssetBundle assetBundle, bool spawned)
        {
            if (string.IsNullOrEmpty(assetBundleName))
            {
                throw new Exception("assetName is invalid.");
            }
            if (assetBundle == null)
            {
                throw new Exception("Object is invalid.");
            }

            m_AssetBundleName = assetBundleName;
            m_AssetBundle = assetBundle;
            m_SpawnCount = spawned ? 1 : 0;
            m_LastUseTime = DateTime.Now;
            m_IsLock = false;
        }

        public AssetBundle AssetBundle
        {
            get
            {
                return m_AssetBundle;
            }
        }


        /// <summary>
        /// 获取对象的获取计数
        /// </summary>
        public int SpawnCount
        {
            get
            {
                return m_SpawnCount;
            }
        }

        /// <summary>
        /// 是否被加锁
        /// </summary>
        public bool IsLock
        {
            get
            {
                return m_IsLock;
            }
        }

        /// <summary>
        /// 是否被使用
        /// </summary>
        public bool IsUsed
        {
            get
            {
                return SpawnCount > 0;
            }
        }

        /// <summary>
        /// 获取AssetBundle对象的名称
        /// </summary>
        public string AssetBundleName
        {
            get
            {
                return m_AssetBundleName;
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public AssetBundleInfo Spawn()
        {
            m_SpawnCount++;
            m_LastUseTime = DateTime.Now;
            return this;
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Unspawn()
        {
            m_LastUseTime = DateTime.Now;
            m_SpawnCount--;
            if (m_SpawnCount < 0)
            {
                throw new Exception("Spawn count is less than 0.");
            }
        }


        /// <summary>
        /// 获取对象上次使用时间
        /// </summary>
        public DateTime LastUseTime
        {
            get
            {
                return m_LastUseTime;
            }
            set
            {
                m_LastUseTime = value;
            }
        }

        /// <summary>
        /// 给对象枷锁
        /// </summary>
        public void Lock()
        {
            m_IsLock = true;
        }

        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="isShutdown">是否完全释放</param>
        public void Release(bool isShutdown)
        {
            m_AssetBundle.Unload(isShutdown);
        }
    }
}
