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
    /// UI管理器
    /// </summary>
    public class UIManager : ManagerBase
    {
        private const int DefaultPriority = 0;

        private readonly Dictionary<string, UIGroup> m_UIGroups;
        private readonly List<int> m_UIFormsBeingLoaded;
        private readonly List<string> m_UIFormAssetNamesBeingLoaded;
        private readonly HashSet<int> m_UIFormsToReleaseOnLoad;
        private readonly LinkedList<UIFormBase> m_RecycleQueue;
        private readonly LoadAssetCallbacks m_LoadAssetCallbacks;
        private IObjectPool<UIFormInstanceObject> m_InstancePool;
        private int m_Serial;
        private OpenUIFormSuccessEvent m_OpenUIFormSuccessEventHandler;
        private OpenUIFormFailureEvent m_OpenUIFormFailureEventHandler;
        private OpenUIFormUpdateEvent m_OpenUIFormUpdateEventHandler;
        private OpenUIFormDependencyAssetEvent m_OpenUIFormDependencyAssetEventHandler;
        private CloseUIFormCompleteEvent m_CloseUIFormCompleteEventHandler;

        /// <summary>
        /// 初始化界面管理器的新实例
        /// </summary>
        public UIManager()
        {
            m_UIGroups = new Dictionary<string, UIGroup>();
            m_UIFormsBeingLoaded = new List<int>();
            m_UIFormAssetNamesBeingLoaded = new List<string>();
            m_UIFormsToReleaseOnLoad = new HashSet<int>();
            m_RecycleQueue = new LinkedList<UIFormBase>();
            m_LoadAssetCallbacks = new LoadAssetCallbacks(LoadUIFormSuccessCallback, LoadUIFormFailureCallback, LoadUIFormUpdateCallback, LoadUIFormDependencyAssetCallback);
            m_InstancePool = null;
            m_Serial = 0;
            m_OpenUIFormSuccessEventHandler = null;
            m_OpenUIFormFailureEventHandler = null;
            m_OpenUIFormUpdateEventHandler = null;
            m_OpenUIFormDependencyAssetEventHandler = null;
            m_CloseUIFormCompleteEventHandler = null;
        }

        /// <summary>
        /// 获取界面组数量
        /// </summary>
        public int UIGroupCount
        {
            get
            {
                return m_UIGroups.Count;
            }
        }

        /// <summary>
        /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get
            {
                return m_InstancePool.AutoReleaseInterval;
            }
            set
            {
                m_InstancePool.AutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 获取或设置界面实例对象池的容量
        /// </summary>
        public int InstanceCapacity
        {
            get
            {
                return m_InstancePool.Capacity;
            }
            set
            {
                m_InstancePool.Capacity = value;
            }
        }

        /// <summary>
        /// 获取或设置界面实例对象池对象过期秒数
        /// </summary>
        public float InstanceExpireTime
        {
            get
            {
                return m_InstancePool.ExpireTime;
            }
            set
            {
                m_InstancePool.ExpireTime = value;
            }
        }

        /// <summary>
        /// 获取或设置界面实例对象池的优先级
        /// </summary>
        public int InstancePriority
        {
            get
            {
                return m_InstancePool.Priority;
            }
            set
            {
                m_InstancePool.Priority = value;
            }
        }

        /// <summary>
        /// 打开界面成功事件
        /// </summary>
        public OpenUIFormSuccessEvent OpenUIFormSuccess
        {
            get
            {
                return m_OpenUIFormSuccessEventHandler;
            }
            set
            {
                m_OpenUIFormSuccessEventHandler = value;
            }
        }

        /// <summary>
        /// 打开界面失败事件
        /// </summary>
        public OpenUIFormFailureEvent OpenUIFormFailure
        {
            get
            {
                return m_OpenUIFormFailureEventHandler;
            }
            set
            {
                m_OpenUIFormFailureEventHandler = value;
            }
        }

        /// <summary>
        /// 打开界面更新事件
        /// </summary>
        public OpenUIFormUpdateEvent OpenUIFormUpdate
        {
            get
            {
                return m_OpenUIFormUpdateEventHandler;
            }
            set
            {
                m_OpenUIFormUpdateEventHandler = value;
            }
        }

        /// <summary>
        /// 打开界面时加载依赖资源事件
        /// </summary>
        public OpenUIFormDependencyAssetEvent OpenUIFormDependencyAsset
        {
            get
            {
                return m_OpenUIFormDependencyAssetEventHandler;
            }
            set
            {
                m_OpenUIFormDependencyAssetEventHandler = value;
            }
        }

        /// <summary>
        /// 关闭界面完成事件
        /// </summary>
        public CloseUIFormCompleteEvent CloseUIFormComplete
        {
            get
            {
                return m_CloseUIFormCompleteEventHandler;
            }
            set
            {
                m_CloseUIFormCompleteEventHandler = value;
            }
        }

        /// <summary>
        /// 界面管理器轮询
        /// </summary>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            while (m_RecycleQueue.Count > 0)
            {
                UIFormBase uiFormBase = m_RecycleQueue.First.Value;
                m_RecycleQueue.RemoveFirst();
                uiFormBase.OnRecycle();
                m_InstancePool.Unspawn(uiFormBase.Handle);
            }

            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                uiGroup.Value.OnUpdate(deltaTime, unscaledDeltaTime);
            }
        }

        /// <summary>
        /// 关闭并清理界面管理器
        /// </summary>
        public override void Dispose()
        {
            CloseAllLoadedUIForms();
            m_UIGroups.Clear();
            m_UIFormsBeingLoaded.Clear();
            m_UIFormAssetNamesBeingLoaded.Clear();
            m_UIFormsToReleaseOnLoad.Clear();
            m_RecycleQueue.Clear();
        }

        /// <summary>
        /// 设置对象池管理器
        /// </summary>
        public void SetObjectPoolManager()
        {
            m_InstancePool = GameEntry.Pool.CreateSingleSpawnObjectPool<UIFormInstanceObject>("UI Instance Pool");
        }


        /// <summary>
        /// 是否存在界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>是否存在界面组</returns>
        public bool HasUIGroup(string uiGroupName)
        {
            if (string.IsNullOrEmpty(uiGroupName))
            {
                throw new Exception("UI group name is invalid.");
            }

            return m_UIGroups.ContainsKey(uiGroupName);
        }

        /// <summary>
        /// 获取界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>要获取的界面组</returns>
        public UIGroup GetUIGroup(string uiGroupName)
        {
            if (string.IsNullOrEmpty(uiGroupName))
            {
                throw new Exception("UI group name is invalid.");
            }

            UIGroup uiGroup = null;
            if (m_UIGroups.TryGetValue(uiGroupName, out uiGroup))
            {
                return uiGroup;
            }

            return null;
        }

        /// <summary>
        /// 获取所有界面组
        /// </summary>
        /// <returns>所有界面组</returns>
        public UIGroup[] GetAllUIGroups()
        {
            int index = 0;
            UIGroup[] results = new UIGroup[m_UIGroups.Count];
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                results[index++] = uiGroup.Value;
            }

            return results;
        }

        /// <summary>
        /// 获取所有界面组
        /// </summary>
        /// <param name="results">所有界面组</param>
        public void GetAllUIGroups(List<UIGroup> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                results.Add(uiGroup.Value);
            }
        }

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="uiGroupDepth">界面组深度</param>
        /// <param name="uiGroupTransform">界面组变换</param>
        /// <returns>是否增加界面组成功</returns>
        public bool AddUIGroup(string uiGroupName, int uiGroupDepth, Transform uiGroupTransform)
        {
            if (string.IsNullOrEmpty(uiGroupName))
            {
                throw new Exception("UI group name is invalid.");
            }

            if (uiGroupTransform == null)
            {
                throw new Exception("UI group helper is invalid.");
            }

            if (HasUIGroup(uiGroupName))
            {
                return false;
            }

            m_UIGroups.Add(uiGroupName, new UIGroup(uiGroupName, uiGroupDepth, uiGroupTransform));

            return true;
        }

        /// <summary>
        /// 是否存在界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>是否存在界面</returns>
        public bool HasUIForm(int serialId)
        {
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                if (uiGroup.Value.HasUIForm(serialId))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 是否存在界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>是否存在界面</returns>
        public bool HasUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UI form asset name is invalid.");
            }

            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                if (uiGroup.Value.HasUIForm(uiFormAssetName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>要获取的界面</returns>
        public UIFormBase GetUIForm(int serialId)
        {
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                UIFormBase uiForm = uiGroup.Value.GetUIForm(serialId);
                if (uiForm != null)
                {
                    return uiForm;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIFormBase GetUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UI form asset name is invalid.");
            }

            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                UIFormBase uiFormBase = uiGroup.Value.GetUIForm(uiFormAssetName);
                if (uiFormBase != null)
                {
                    return uiFormBase;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIFormBase[] GetUIForms(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UI form asset name is invalid.");
            }

            List<UIFormBase> results = new List<UIFormBase>();
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                results.AddRange(uiGroup.Value.GetUIForms(uiFormAssetName));
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="results">要获取的界面</param>
        public void GetUIForms(string uiFormAssetName, List<UIFormBase> results)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UI form asset name is invalid.");
            }

            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                uiGroup.Value.InternalGetUIForms(uiFormAssetName, results);
            }
        }

        /// <summary>
        /// 获取所有已加载的界面
        /// </summary>
        /// <returns>所有已加载的界面</returns>
        public UIFormBase[] GetAllLoadedUIForms()
        {
            List<UIFormBase> results = new List<UIFormBase>();
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                results.AddRange(uiGroup.Value.GetAllUIForms());
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取所有已加载的界面
        /// </summary>
        /// <param name="results">所有已加载的界面</param>
        public void GetAllLoadedUIForms(List<UIFormBase> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (KeyValuePair<string, UIGroup> uiGroup in m_UIGroups)
            {
                uiGroup.Value.InternalGetAllUIForms(results);
            }
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号
        /// </summary>
        /// <returns>所有正在加载界面的序列编号</returns>
        public int[] GetAllLoadingUIFormSerialIds()
        {
            return m_UIFormsBeingLoaded.ToArray();
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号
        /// </summary>
        /// <param name="results">所有正在加载界面的序列编号</param>
        public void GetAllLoadingUIFormSerialIds(List<int> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            results.AddRange(m_UIFormsBeingLoaded);
        }

        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingUIForm(int serialId)
        {
            return m_UIFormsBeingLoaded.Contains(serialId);
        }

        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UI form asset name is invalid.");
            }

            return m_UIFormAssetNamesBeingLoaded.Contains(uiFormAssetName);
        }

        /// <summary>
        /// 是否是合法的界面
        /// </summary>
        /// <param name="uiForm">界面</param>
        /// <returns>界面是否合法</returns>
        public bool IsValidUIForm(UIFormBase uiForm)
        {
            if (uiForm == null)
            {
                return false;
            }

            return HasUIForm(uiForm.SerialId);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="priority">加载界面资源的优先级</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName,   string uiGroupName, int priority, bool pauseCoveredUIForm, object userData)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UI form asset name is invalid.");
            }

            if (string.IsNullOrEmpty(uiGroupName))
            {
                throw new Exception("UI group name is invalid.");
            }

            UIGroup uiGroup = GetUIGroup(uiGroupName);
            if (uiGroup == null)
            {
                throw new Exception(TextUtil.Format("UI group '{0}' is not exist.", uiGroupName));
            }

            int serialId = m_Serial++;
            UIFormInstanceObject uiFormInstanceObject = m_InstancePool.Spawn(uiFormAssetName);
            if (uiFormInstanceObject == null)
            {
                m_UIFormsBeingLoaded.Add(serialId);
                m_UIFormAssetNamesBeingLoaded.Add(uiFormAssetName);
                GameEntry.Resource.LoadAsset(uiFormAssetName,  typeof(GameObject), priority, m_LoadAssetCallbacks, new OpenUIFormInfo(serialId, uiGroup, pauseCoveredUIForm, userData));
            }
            else
            {
                InternalOpenUIForm(serialId, uiFormAssetName, uiGroup, uiFormInstanceObject.Target, pauseCoveredUIForm, false, 0f, userData);
            }

            return serialId;
        }
        

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="serialId">要关闭界面的序列编号</param>
        public void CloseUIForm(int serialId)
        {
            CloseUIForm(serialId, null);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="serialId">要关闭界面的序列编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void CloseUIForm(int serialId, object userData)
        {
            if (IsLoadingUIForm(serialId))
            {
                m_UIFormsToReleaseOnLoad.Add(serialId);
                return;
            }

            UIFormBase uiFormBase = GetUIForm(serialId);
            if (uiFormBase == null)
            {
                throw new Exception(TextUtil.Format("Can not find UI form '{0}'.", serialId.ToString()));
            }

            CloseUIForm(uiFormBase, userData);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiFormBase">要关闭的界面</param>
        public void CloseUIForm(UIFormBase uiFormBase)
        {
            CloseUIForm(uiFormBase, null);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiFormBase">要关闭的界面</param>
        /// <param name="userData">用户自定义数据</param>
        public void CloseUIForm(UIFormBase uiFormBase, object userData)
        {
            if (uiFormBase == null)
            {
                throw new Exception("UI form is invalid.");
            }

            UIGroup uiGroup = uiFormBase.UIGroup;
            if (uiGroup == null)
            {
                throw new Exception("UI group is invalid.");
            }

            uiGroup.RemoveUIForm(uiFormBase);
            uiFormBase.OnClose(userData);
            uiGroup.Refresh();

            if (m_CloseUIFormCompleteEventHandler != null)
            {
                m_CloseUIFormCompleteEventHandler(uiFormBase.SerialId, uiFormBase.UIFormAssetName, uiGroup, userData);
            }

            m_RecycleQueue.AddLast(uiFormBase);
        }

        /// <summary>
        /// 关闭所有已加载的界面
        /// </summary>
        public void CloseAllLoadedUIForms()
        {
            CloseAllLoadedUIForms(null);
        }

        /// <summary>
        /// 关闭所有已加载的界面
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void CloseAllLoadedUIForms(object userData)
        {
            UIFormBase[] uiForms = GetAllLoadedUIForms();
            foreach (UIFormBase uiForm in uiForms)
            {
                if (!HasUIForm(uiForm.SerialId))
                {
                    continue;
                }

                CloseUIForm(uiForm, userData);
            }
        }

        /// <summary>
        /// 关闭所有正在加载的界面
        /// </summary>
        public void CloseAllLoadingUIForms()
        {
            foreach (int serialId in m_UIFormsBeingLoaded)
            {
                m_UIFormsToReleaseOnLoad.Add(serialId);
            }
        }

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="uiForm">要激活的界面</param>
        public void RefocusUIForm(UIFormBase uiForm)
        {
            RefocusUIForm(uiForm, null);
        }

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="uiForm">要激活的界面</param>
        /// <param name="userData">用户自定义数据</param>
        public void RefocusUIForm(UIFormBase uiForm, object userData)
        {
            if (uiForm == null)
            {
                throw new Exception("UI form is invalid.");
            }

            UIGroup uiGroup = uiForm.UIGroup;
            if (uiGroup == null)
            {
                throw new Exception("UI group is invalid.");
            }

            uiGroup.RefocusUIForm(uiForm, userData);
            uiGroup.Refresh();
            uiForm.OnRefocus(userData);
        }

        /// <summary>
        /// 设置界面实例是否被加锁
        /// </summary>
        /// <param name="uiFormInstance">要设置是否被加锁的界面实例</param>
        /// <param name="locked">界面实例是否被加锁</param>
        public void SetUIFormInstanceLocked(object uiFormInstance, bool locked)
        {
            if (uiFormInstance == null)
            {
                throw new Exception("UI form instance is invalid.");
            }

            m_InstancePool.SetLocked(uiFormInstance, locked);
        }

        /// <summary>
        /// 设置界面实例的优先级
        /// </summary>
        /// <param name="uiFormInstance">要设置优先级的界面实例</param>
        /// <param name="priority">界面实例优先级</param>
        public void SetUIFormInstancePriority(object uiFormInstance, int priority)
        {
            if (uiFormInstance == null)
            {
                throw new Exception("UI form instance is invalid.");
            }

            m_InstancePool.SetPriority(uiFormInstance, priority);
        }

        private void InternalOpenUIForm(int serialId, string uiFormAssetName, UIGroup uiGroup, object uiFormInstance, bool pauseCoveredUIForm, bool isNewInstance, float duration, object userData)
        {
            try
            {
                GameObject gameObject = uiFormInstance as GameObject;
                if (gameObject == null)
                {
                    Log.Error("UI form instance is invalid.");
                }

                Transform transform = gameObject.transform;
                transform.SetParent(uiGroup.UIGroupTransform);
                transform.localScale = Vector3.one;

                UIFormBase uiFormBase = transform.GetComponent<UIFormBase>();
                if (uiFormBase == null)
                {
                    throw new Exception("Can't find UI form.");
                }


                uiFormBase.OnInit(serialId, uiFormAssetName, uiGroup, pauseCoveredUIForm, isNewInstance, userData);
                uiGroup.AddUIForm(uiFormBase);
                uiFormBase.OnOpen(userData);
                uiGroup.Refresh();

                if (m_OpenUIFormSuccessEventHandler != null)
                {
                    m_OpenUIFormSuccessEventHandler(uiFormBase, duration, userData);
                }
            }
            catch (Exception exception)
            {
                if (m_OpenUIFormFailureEventHandler != null)
                {
                    m_OpenUIFormFailureEventHandler(serialId, uiFormAssetName, uiGroup.Name, pauseCoveredUIForm, exception.ToString(), userData);
                    return;
                }

                throw;
            }
        }

        private void LoadUIFormSuccessCallback(string uiFormAssetName, UnityEngine.Object uiFormAsset, float duration, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new Exception("Open UI form info is invalid.");
            }

            m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
            m_UIFormAssetNamesBeingLoaded.Remove(uiFormAssetName);
            if (m_UIFormsToReleaseOnLoad.Contains(openUIFormInfo.SerialId))
            {
                Log.Info("Release UI form '{0}' on loading success.", openUIFormInfo.SerialId.ToString());
                m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
                GameEntry.Resource.UnloadAsset(uiFormAsset);
                return;
            }

            UIFormInstanceObject uiFormInstanceObject = new UIFormInstanceObject(uiFormAssetName, uiFormAsset, UnityEngine.Object.Instantiate(uiFormAsset));
            m_InstancePool.Register(uiFormInstanceObject, true);

            InternalOpenUIForm(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup, uiFormInstanceObject.Target, openUIFormInfo.PauseCoveredUIForm, true, duration, openUIFormInfo.UserData);
        }

        private void LoadUIFormFailureCallback(string uiFormAssetName, string errorMessage, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new Exception("Open UI form info is invalid.");
            }

            m_UIFormsBeingLoaded.Remove(openUIFormInfo.SerialId);
            m_UIFormAssetNamesBeingLoaded.Remove(uiFormAssetName);
            m_UIFormsToReleaseOnLoad.Remove(openUIFormInfo.SerialId);
            string appendErrorMessage = TextUtil.Format("Load UI form failure, asset name '{0}', error message '{1}'.", uiFormAssetName, errorMessage);
            if (m_OpenUIFormFailureEventHandler != null)
            {
                m_OpenUIFormFailureEventHandler(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup.Name, openUIFormInfo.PauseCoveredUIForm, appendErrorMessage, openUIFormInfo.UserData);
                return;
            }

            throw new Exception(appendErrorMessage);
        }

        private void LoadUIFormUpdateCallback(string uiFormAssetName, float progress, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new Exception("Open UI form info is invalid.");
            }

            if (m_OpenUIFormUpdateEventHandler != null)
            {
                m_OpenUIFormUpdateEventHandler(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup.Name, openUIFormInfo.PauseCoveredUIForm, progress, openUIFormInfo.UserData);
            }
        }

        private void LoadUIFormDependencyAssetCallback(string uiFormAssetName, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            OpenUIFormInfo openUIFormInfo = (OpenUIFormInfo)userData;
            if (openUIFormInfo == null)
            {
                throw new Exception("Open UI form info is invalid.");
            }

            if (m_OpenUIFormDependencyAssetEventHandler != null)
            {
                m_OpenUIFormDependencyAssetEventHandler(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup.Name, openUIFormInfo.PauseCoveredUIForm, dependencyAssetName, loadedCount, totalCount, openUIFormInfo.UserData);
            }
        }

    }
}
