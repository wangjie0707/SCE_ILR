using System;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 资源池
    /// </summary>
    public class AssetPool
    {
        private readonly LinkedList<AssetBundleInfo> m_AssetBundleInfos;
        private readonly LinkedList<AssetInfo> m_AssetInfos;

        private readonly List<object> m_CachedCanReleaseObjects;

        /// <summary>
        /// 自动释放时间
        /// </summary>
        private float m_AutoReleaseTime;

        /// <summary>
        /// 自动释放间隔
        /// </summary>
        private float m_AutoReleaseInterval;

        /// <summary>
        /// 过期时间
        /// </summary>
        private float m_ExpireTime;

        public AssetPool()
        {
            m_AssetBundleInfos = new LinkedList<AssetBundleInfo>();
            m_AssetInfos = new LinkedList<AssetInfo>();
            m_AutoReleaseTime = 0f;
            m_AutoReleaseInterval = 60f;
            m_ExpireTime = 300f;
        }

        /// <summary>
        /// 资源池轮询
        /// </summary>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            m_AutoReleaseTime += Time.deltaTime;
            if (m_AutoReleaseTime < m_AutoReleaseInterval)
            {
                return;
            }

            Log.Info("Asset pool auto release start.");
            Release();
            Log.Info("Asset pool auto release complete.");
        }

        /// <summary>
        /// 获取或设置自动释放资源间隔
        /// </summary>
        public float AutoReleaseInterval
        {
            get
            {
                return m_AutoReleaseInterval;
            }
            set
            {
                m_AutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 获取或设置资源过期时间
        /// </summary>
        public  float ExpireTime
        {
            get
            {
                return m_ExpireTime;
            }
            set
            {
                m_ExpireTime = value;
            }
        }


        /// <summary>
        /// 释放对象池中的可释放对象
        /// </summary>
        public void Release()
        {
            m_AutoReleaseTime = 0f;

            LinkedListNode<AssetBundleInfo> currentAssetBundleInfo = m_AssetBundleInfos.First;
            while (currentAssetBundleInfo != null)
            {
                if ((currentAssetBundleInfo.Value.IsUsed && (float)(DateTime.Now - currentAssetBundleInfo.Value.LastUseTime).TotalSeconds < m_ExpireTime) || currentAssetBundleInfo.Value.IsLock)
                {
                    currentAssetBundleInfo = currentAssetBundleInfo.Next;
                    continue;
                }

                LinkedListNode<AssetBundleInfo> next = currentAssetBundleInfo.Next;
                m_AssetBundleInfos.Remove(currentAssetBundleInfo);
                currentAssetBundleInfo.Value.Release(!currentAssetBundleInfo.Value.IsUsed);
                Log.Info("Asset pool release assetbundle the name is '{0}'.", currentAssetBundleInfo.Value.AssetBundleName);
                currentAssetBundleInfo = next;
            }


            LinkedListNode<AssetInfo> currentAssetInfo = m_AssetInfos.First;
            while (currentAssetInfo != null)
            {
                if (currentAssetInfo.Value.IsUsed)
                {
                    currentAssetInfo = currentAssetInfo.Next;
                    continue;
                }

                LinkedListNode<AssetInfo> next = currentAssetInfo.Next;
                m_AssetInfos.Remove(currentAssetInfo);
                currentAssetInfo.Value.Release(currentAssetInfo.Value.IsUsed);
                Log.Info("Asset pool release asset the name is '{0}'.", currentAssetInfo.Value.AssetName);
                currentAssetInfo = next;
            }
        }

        /// <summary>
        /// 创建asset对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="spawned">对象是否已被获取</param>
        public void RegisterAssetInfo(AssetInfo assetInfo, bool spawned)
        {
            if (assetInfo == null)
            {
                throw new Exception("assetInfo is invalid.");
            }
            
            m_AssetInfos.AddLast(assetInfo);

            Release();
        }

        /// <summary>
        /// 创建asset对象
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="spawned">对象是否已被获取</param>
        public void RegisterAssetBundleInfo(AssetBundleInfo assetBundleInfo, bool spawned)
        {
            if (assetBundleInfo == null)
            {
                throw new Exception("assetInfo is invalid.");
            }

            m_AssetBundleInfos.AddLast(assetBundleInfo);

            Release();
        }


        /// <summary>
        /// 获取AssetBundle对象
        /// </summary>
        /// <param name="assetPath">Assetbundle对象名称</param>
        /// <returns>要获取的对象</returns>
        public AssetBundleInfo SpawnAssetBundle(string assetBundleName)
        {
            foreach (AssetBundleInfo assetInfo in m_AssetBundleInfos)
            {
                if (assetInfo.AssetBundleName != assetBundleName)
                {
                    continue;
                }

                return assetInfo.Spawn();
            }
            return null;
        }

        /// <summary>
        /// 检查Assetbundle对象
        /// </summary>
        /// <param name="assetBundleName">Assetbundle对象名称</param>
        /// <returns>要检查的对象是否存在</returns>
        public bool CanSpawnAssetbundle(string assetBundleName)
        {
            foreach (AssetBundleInfo assetInfo in m_AssetBundleInfos)
            {
                if (assetInfo.AssetBundleName != assetBundleName)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 回收AssetBundle对象
        /// </summary>
        /// <param name="target">要回收的AssetBundle</param>
        public void UnspawnAssetBundle(AssetBundle target)
        {
            if (target == null)
            {
                throw new Exception("AssetBundleInfo is invalid.");
            }

            foreach (AssetBundleInfo assetBundleInfo in m_AssetBundleInfos)
            {
                if (assetBundleInfo.AssetBundle == target)
                {
                    Log.Info("Assetpool unspawn assetbundle the name is'{0}'.", assetBundleInfo.AssetBundleName);
                    assetBundleInfo.Unspawn();
                    return;
                }
            }

            throw new Exception("Can not find assetbundle in assetpool .");
        }


        /// <summary>
        /// 获取Asset对象
        /// </summary>
        /// <param name="assetName">对象名称</param>
        /// <returns>要获取的对象</returns>
        public AssetInfo SpawnAsset(string assetName)
        {
            foreach (AssetInfo assetInfo in m_AssetInfos)
            {
                if (assetInfo.AssetName != assetName)
                {
                    continue;
                }

                return assetInfo.Spawn();
            }
            return null;
        }

        /// <summary>
        /// 检查Asset对象
        /// </summary>
        /// <param name="assetName">对象名称</param>
        /// <returns>要检查的对象是否存在</returns>
        public bool CanSpawnAsset(string assetName)
        {
            foreach (AssetInfo assetInfo in m_AssetInfos)
            {
                if (assetInfo.AssetName != assetName)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 回收Asset对象
        /// </summary>
        /// <param name="target">要回收的Asset</param>
        public void UnspawnAsset(UnityEngine.Object target)
        {
            if (target == null)
            {
                throw new Exception("AssetBundleInfo is invalid.");
            }

            foreach (AssetInfo assetInfo in m_AssetInfos)
            {
                if (assetInfo.Asset == target)
                {
                    Log.Info("Assetpool unspawn asset the name is'{0}'.", assetInfo.AssetName);
                    assetInfo.Unspawn();
                    return;
                }
            }

            //throw new Exception("Can not find target in assetpool");
        }

        /// <summary>
        /// 释放未使用的资源
        /// </summary>
        public void UnloadUnusedAssets()
        {
            LinkedListNode<AssetInfo> currentAssetInfo = m_AssetInfos.First;
            while (currentAssetInfo != null)
            {
                LinkedListNode<AssetInfo> next = currentAssetInfo.Next;
                m_AssetInfos.Remove(currentAssetInfo);
                currentAssetInfo.Value.Release(false);
                Log.Info("Assetpool release asset '{0}'.", currentAssetInfo.Value.AssetName);
                currentAssetInfo = next;
            }
        }

        internal void Shutdown()
        {
            LinkedListNode<AssetBundleInfo> currentAssetBundleInfo = m_AssetBundleInfos.First;
            while (currentAssetBundleInfo != null)
            {
                LinkedListNode<AssetBundleInfo> next = currentAssetBundleInfo.Next;
                m_AssetBundleInfos.Remove(currentAssetBundleInfo);
                currentAssetBundleInfo.Value.Release(true);
                Log.Info("Assetpool release assetbundle '{0}'.", currentAssetBundleInfo.Value.AssetBundleName);
                currentAssetBundleInfo = next;
            }

            LinkedListNode<AssetInfo> currentAssetInfo = m_AssetInfos.First;
            while (currentAssetInfo != null)
            {
                LinkedListNode<AssetInfo> next = currentAssetInfo.Next;
                m_AssetInfos.Remove(currentAssetInfo);
                currentAssetInfo.Value.Release(true);
                Log.Info("Assetpool release asset '{0}'.", currentAssetInfo.Value.AssetName);
                currentAssetInfo = next;
            }
        }
    }
}
