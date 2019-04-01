using System;
using System.Collections.Generic;
using UnityEngine;
using Myth;

namespace Hotfix
{
    /// <summary>
    /// 热更新层入口
    /// </summary>
    public class HotfixEntry
    {
        private static readonly LinkedList<IHotfixComponent> m_GameComponentList = new LinkedList<IHotfixComponent>();

        /// <summary>
        /// 实体
        /// </summary>
        public static EntityComponent Entity
        {
            get;
            private set;
        }

        public void Start()
        {
            Debug.Log("热更新层启动!");
            //热修复
            //Fiaxed.Init();

            //todo 初始化组件在这里进行
            Entity = GetComponent<EntityComponent>();

            
           
        }

        
        
        /// <summary>
        /// 更新方法
        /// </summary>
        /// <param name="deltaTime">逻辑流逝时间，以秒为单位</param>
        /// <param name="unscaledDeltaTime">真实流逝时间，以秒为单位</param>
        public void Update(float deltaTime, float unscaledDeltaTime)
        {
            for (LinkedListNode<IHotfixComponent> curr = m_GameComponentList.First; curr != null; curr = curr.Next)
            {
                curr.Value.OnUpdate(deltaTime, unscaledDeltaTime);
            }

            //if (Input.GetKeyUp(KeyCode.B))
            //{
            //    GameEntry.Resource.LoadAsset("Assets/DownLoad/Prefab/CreateRole/104000.prefab", typeof(GameObject), 0,
            // new LoadAssetCallbacks((assetName, asset, duration, userData) =>
            // {
            //     Debug.Log(assetName);
            //     UnityEngine.Object.Instantiate(asset);
            //     Debug.Log(duration);
            //     Debug.Log(userData);
            // }), 1);
            //}

            //if (Input.GetKeyUp(KeyCode.C))
            //{
            //    HotfixEntry.Entity.ShowEntity<TestEntity>(0, "Assets/DownLoad/Prefab/CreateRole/104000.prefab", "ABC", 0, "123456789");
            //}

            //if (Input.GetKeyUp(KeyCode.E))
            //{
            //    HotfixEntry.Entity.HideEntity(0);
            //}
            //if (Input.GetKeyUp(KeyCode.D))
            //{
            //    TestEntity testEntity = (TestEntity)HotfixEntry.Entity.GetEntity(0);
            //    if (testEntity!=null)
            //    {
            //        Debug.Log("获取到了！");
            //    }
            //}

            if (Input.GetKeyUp(KeyCode.B))
            {
                int id = GameEntry.UI.OpenUIForm("Assets/DownLoad/UI/UIPrefab/SelectRole/SelectRoleForm.prefab", "Default");
                //GameEntry.UI.CloseUIForm(id);
            }
        }

        /// <summary>
        /// 关闭热更新
        /// </summary>
        public void ShutDown()
        {
            Debug.Log("热更新关闭!");
            for (LinkedListNode<IHotfixComponent> curr = m_GameComponentList.First; curr != null; curr = curr.Next)
            {
                curr.Value.Shutdown();
            }
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="T">要获取的组件类型</typeparam>
        /// <returns>获取到的组件类型</returns>
        public static T GetComponent<T>() where T : IHotfixComponent
        {
            Type type = typeof(T);
            return (T)GetComponent(type);
        }

        /// <summary>
        /// 获取组件
        /// </summary>
        /// <typeparam name="componentType">要获取的组件类型</typeparam>
        /// <returns>获取到的组件类型</returns>
        private static IHotfixComponent GetComponent(Type componentType)
        {
            foreach (IHotfixComponent hotfixComponent in m_GameComponentList)
            {
                if (hotfixComponent.GetType() == componentType)
                {
                    return hotfixComponent;
                }
            }

            return CreateComponent(componentType);
        }

        /// <summary>
        /// 创建组件
        /// </summary>
        /// <param name="componentType">要创建的组件类型</param>
        /// <returns>要创建的组件类型</returns>
        private static IHotfixComponent CreateComponent(Type componentType)
        {
            IHotfixComponent hotfixComponent = (IHotfixComponent)Activator.CreateInstance(componentType);
            if (hotfixComponent == null)
            {
                throw new Exception(TextUtil.Format("Can not create component '{0}'.", componentType.FullName));
            }
            hotfixComponent.Init();
            LinkedListNode<IHotfixComponent> current = m_GameComponentList.First;

            m_GameComponentList.AddLast(hotfixComponent);

            return hotfixComponent;
        }
    }
}

