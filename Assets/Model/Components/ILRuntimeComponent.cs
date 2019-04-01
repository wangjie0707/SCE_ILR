using GameFramework.Resource;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Myth
{
    /// <summary>
    /// 热更新组件
    /// </summary>
    public class ILRuntimeComponent : GameBaseComponent
    {
        /// <summary>
        /// ILRuntime入口对象
        /// </summary>
        public AppDomain AppDomain
        {
            get;
            private set;
        }

        private Assembly assembly;

        private bool m_DLLLoaded;
        private bool m_PDBLoaded;
        private ILInstanceMethod m_Update;
        private ILInstanceMethod m_ShutDown;
        private MemoryStream dllStream;
        private MemoryStream pdbStream;
        private byte[] assBytes;
        private byte[] pdbBytes;
        private LoadAssetCallbacks m_LoadAssetCallbacks;


        protected override void OnAwake()
        {
            base.OnAwake();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadHotfixDLLSuccess, OnLoadHotfixDLLFailure);
            m_DLLLoaded = false;
            m_PDBLoaded = false;
            m_Update = null;
            m_ShutDown = null;
            AppDomain = null;
            assBytes = null;
            pdbBytes = null;
            dllStream = null;
            pdbStream = null;
        }

        

        protected override void OnStart()
        {
            base.OnStart();
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_Update?.Invoke(deltaTime, unscaledDeltaTime);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_ShutDown?.Invoke();

            m_LoadAssetCallbacks = null;
            m_DLLLoaded = false;
            m_PDBLoaded = false;
            m_Update = null;
            m_ShutDown = null;
            AppDomain = null;
            assBytes = null;
            pdbBytes = null;
            dllStream = null;
            pdbStream = null;
        }

        /// <summary>
        /// 初始化ILRuntime
        /// </summary>
        public void InitILRuntime()
        {
            AppDomain = new AppDomain();
            GameEntry.Resource.LoadAsset("Assets/DownLoad/Hotfix/Hotfix.dll.bytes", typeof(TextAsset), Constant.AssetPriority.ConfigAsset, m_LoadAssetCallbacks, 0);
            GameEntry.Resource.LoadAsset("Assets/DownLoad/Hotfix/Hotfix.pdb.bytes", typeof(TextAsset), Constant.AssetPriority.ConfigAsset, m_LoadAssetCallbacks, 1);

        }

        /// <summary>
        /// 开始执行热更新层代码
        /// </summary>
        public void HotfixStart()
        {

            
        }

        private void OnLoadHotfixDLLFailure(string assetName, string errorMessage, object userData)
        {

        }

        private void OnLoadHotfixDLLSuccess(string assetName, UnityEngine.Object asset, float duration, object userData)
        {
            if ((int)userData == 0)
            {
                Log.Info("Hotfix.dll加载成功");
                m_DLLLoaded = true;
                assBytes = ((TextAsset)asset).bytes;
            }
            else
            {
                Log.Info("Hotfix.pdb加载成功");
                m_PDBLoaded = true;
                pdbBytes = (asset as TextAsset).bytes;
            }

            RefreshILStatus();
        }

        private void RefreshILStatus()
        {
            if (!m_DLLLoaded || !m_PDBLoaded)
            {
                return;
            }
#if !ILRuntime
            Log.Info($"当前使用的是ILRuntime模式");

            ILHelper.InitILRuntime(AppDomain);

            this.dllStream = new MemoryStream(assBytes);
            this.pdbStream = new MemoryStream(pdbBytes);
            this.AppDomain.LoadAssembly(this.dllStream, this.pdbStream, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider());

            string typeFullName = "Hotfix.HotfixEntry";
            IType type = AppDomain.LoadedTypes[typeFullName];
            object hotfixInstance = ((ILType)type).Instantiate();
            
            AppDomain.Invoke(typeFullName, "Start", hotfixInstance, null);

            m_Update = new ILInstanceMethod(hotfixInstance, typeFullName, "Update", 2);
            m_ShutDown = new ILInstanceMethod(hotfixInstance, typeFullName, "ShutDown", 0);

#else
            Log.Info($"当前使用的是Mono模式");

            this.assembly = Assembly.Load(assBytes, pdbBytes);

            Type hotfixInit = this.assembly.GetType("ETHotfix.Init");
            this.start = new MonoStaticMethod(hotfixInit, "Start");

            this.hotfixTypes = this.assembly.GetTypes().ToList();
#endif

            

        }
    }
}
