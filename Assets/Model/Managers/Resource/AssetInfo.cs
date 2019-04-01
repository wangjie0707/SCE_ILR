using System;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// Asset资源信息
    /// </summary>
    public class AssetInfo
    {
        private string m_AssetName;
        private UnityEngine.Object m_Asset;
        private int m_SpawnCount;
        private DateTime m_LastUseTime;

        /// <summary>
        /// 初始化内部对象的新实例
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="spawned">对象是否已被获取</param>
        public AssetInfo(string assetName, UnityEngine.Object asset, bool spawned)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new Exception("assetName is invalid.");
            }
            if (asset == null)
            {
                throw new Exception("asset is invalid.");
            }
            m_AssetName = assetName;
            m_Asset = asset;
            m_SpawnCount = spawned ? 1 : 0;
            m_LastUseTime = DateTime.Now;
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
        /// 获取Asset对象
        /// </summary>
        public UnityEngine.Object Asset
        {
            get
            {
                return m_Asset;
            }
        }

        /// <summary>
        /// 获取Asset对象的名称
        /// </summary>
        public string AssetName
        {
            get
            {
                return m_AssetName;
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public AssetInfo Spawn()
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
        /// 释放对象
        /// </summary>
        /// <param name="isShutdown">是否完全释放</param>
        public void Release(bool isShutdown)
        {
            //Debug.Log("Unity 当前 Resources.UnloadAsset 在 iOS 设备上会导致一些诡异问题，先不用这部分");

            /*if (m_Asset is GameObject || m_Asset is MonoBehaviour)
            {
                // UnloadAsset may only be used on individual assets and can not be used on GameObject's / Components or AssetBundles.
                throw new Exception("UnloadAsset may only be used on individual assets and can not be used on GameObject's / Components or AssetBundles.");
            }
            Resources.UnloadAsset(m_Asset);
            */
        }
    }
}
