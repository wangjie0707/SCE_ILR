using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// UI组件
    /// </summary>
    public class UIComponent : GameBaseComponent
    {
        private const int DefaultPriority = 0;

        private UIManager m_UIManager = null;

        private readonly List<UIFormBase> m_InternalUIFormResultsCache = new List<UIFormBase>();

        [SerializeField]
        private bool m_EnableOpenUIFormSuccessEvent = true;

        [SerializeField]
        private bool m_EnableOpenUIFormFailureEvent = true;

        [SerializeField]
        private bool m_EnableOpenUIFormUpdateEvent = false;

        [SerializeField]
        private bool m_EnableOpenUIFormDependencyAssetEvent = false;

        [SerializeField]
        private bool m_EnableCloseUIFormCompleteEvent = true;

        [SerializeField]
        private float m_InstanceAutoReleaseInterval = 60f;

        [SerializeField]
        private int m_InstanceCapacity = 16;

        [SerializeField]
        private float m_InstanceExpireTime = 60f;

        [SerializeField]
        private int m_InstancePriority = 0;

        [SerializeField]
        private Transform m_UIRoot = null;

        [SerializeField]
        private UIGroupInfo[] m_UIGroupInfos = null;

        /// <summary>
        /// 获取界面组数量
        /// </summary>
        public int UIGroupCount
        {
            get
            {
                return m_UIManager.UIGroupCount;
            }
        }

        /// <summary>
        /// 获取或设置界面实例对象池自动释放可释放对象的间隔秒数
        /// </summary>
        public float InstanceAutoReleaseInterval
        {
            get
            {
                return m_UIManager.InstanceAutoReleaseInterval;
            }
            set
            {
                m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval = value;
            }
        }

        /// <summary>
        /// 获取或设置界面实例对象池的容量
        /// </summary>
        public int InstanceCapacity
        {
            get
            {
                return m_UIManager.InstanceCapacity;
            }
            set
            {
                m_UIManager.InstanceCapacity = m_InstanceCapacity = value;
            }
        }

        /// <summary>
        /// 获取或设置界面实例对象池对象过期秒数
        /// </summary>
        public float InstanceExpireTime
        {
            get
            {
                return m_UIManager.InstanceExpireTime;
            }
            set
            {
                m_UIManager.InstanceExpireTime = m_InstanceExpireTime = value;
            }
        }

        /// <summary>
        /// 获取或设置界面实例对象池的优先级
        /// </summary>
        public int InstancePriority
        {
            get
            {
                return m_UIManager.InstancePriority;
            }
            set
            {
                m_UIManager.InstancePriority = m_InstancePriority = value;
            }
        }

        protected override void OnAwake()
        {
            base.OnAwake();
            m_UIManager = new UIManager();

            m_UIManager.OpenUIFormSuccess = OnOpenUIFormSuccess;
            m_UIManager.OpenUIFormFailure = OnOpenUIFormFailure;
            m_UIManager.OpenUIFormUpdate = OnOpenUIFormUpdate;
            m_UIManager.OpenUIFormDependencyAsset = OnOpenUIFormDependencyAsset;
            m_UIManager.CloseUIFormComplete = OnCloseUIFormComplete;
        }
        protected override void OnStart()
        {
            base.OnStart();


            m_UIManager.SetObjectPoolManager();
            m_UIManager.InstanceAutoReleaseInterval = m_InstanceAutoReleaseInterval;
            m_UIManager.InstanceCapacity = m_InstanceCapacity;
            m_UIManager.InstanceExpireTime = m_InstanceExpireTime;
            m_UIManager.InstancePriority = m_InstancePriority;

            if (m_UIRoot == null)
            {
                m_UIRoot = (new GameObject("UIRoot")).transform;
                m_UIRoot.SetParent(gameObject.transform);
                m_UIRoot.localScale = Vector3.one;
            }

            m_UIRoot.gameObject.layer = LayerMask.NameToLayer("UI");

            for (int i = 0; i < m_UIGroupInfos.Length; i++)
            {
                if (!AddUIGroup(m_UIGroupInfos[i].Name, m_UIGroupInfos[i].Depth))
                {
                    Log.Warning("Add UI group '{0}' failure.", m_UIGroupInfos[i].Name);
                    continue;
                }
            }
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);
            m_UIManager.OnUpdate(deltaTime, unscaledDeltaTime);
        }

        public override void Shutdown()
        {
            base.Shutdown();
            m_UIManager.Dispose();
        }




        /// <summary>
        /// 是否存在界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>是否存在界面组</returns>
        public bool HasUIGroup(string uiGroupName)
        {
            return m_UIManager.HasUIGroup(uiGroupName);
        }

        /// <summary>
        /// 获取界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>要获取的界面组</returns>
        public UIGroup GetUIGroup(string uiGroupName)
        {
            return m_UIManager.GetUIGroup(uiGroupName);
        }

        /// <summary>
        /// 获取所有界面组
        /// </summary>
        /// <returns>所有界面组</returns>
        public UIGroup[] GetAllUIGroups()
        {
            return m_UIManager.GetAllUIGroups();
        }

        /// <summary>
        /// 获取所有界面组
        /// </summary>
        /// <param name="results">所有界面组</param>
        public void GetAllUIGroups(List<UIGroup> results)
        {
            m_UIManager.GetAllUIGroups(results);
        }

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>是否增加界面组成功</returns>
        public bool AddUIGroup(string uiGroupName)
        {
            return AddUIGroup(uiGroupName, 0);
        }

        /// <summary>
        /// 增加界面组
        /// </summary>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="depth">界面组深度</param>
        /// <returns>是否增加界面组成功</returns>
        public bool AddUIGroup(string uiGroupName, int depth)
        {
            if (m_UIManager.HasUIGroup(uiGroupName))
            {
                return false;
            }

            GameObject uiGroup = new GameObject();
            uiGroup.name = TextUtil.Format("UI Group - {0}", uiGroupName);
            uiGroup.gameObject.layer = LayerMask.NameToLayer("UI");
            Transform uiGroupTransform = uiGroup.transform;
            uiGroupTransform.SetParent(m_UIRoot);
            uiGroupTransform.localScale = Vector3.one;
            RectTransform transform = uiGroup.AddComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            return m_UIManager.AddUIGroup(uiGroupName, depth, transform);
        }

        /// <summary>
        /// 是否存在界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>是否存在界面</returns>
        public bool HasUIForm(int serialId)
        {
            return m_UIManager.HasUIForm(serialId);
        }

        /// <summary>
        /// 是否存在界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>是否存在界面</returns>
        public bool HasUIForm(string uiFormAssetName)
        {
            return m_UIManager.HasUIForm(uiFormAssetName);
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>要获取的界面</returns>
        public UIFormBase GetUIForm(int serialId)
        {
            return m_UIManager.GetUIForm(serialId);
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIFormBase GetUIForm(string uiFormAssetName)
        {
            return m_UIManager.GetUIForm(uiFormAssetName);
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIFormBase[] GetUIForms(string uiFormAssetName)
        {
            UIFormBase[] uiForms = m_UIManager.GetUIForms(uiFormAssetName);
            UIFormBase[] uiFormImpls = new UIFormBase[uiForms.Length];
            for (int i = 0; i < uiForms.Length; i++)
            {
                uiFormImpls[i] = uiForms[i];
            }

            return uiFormImpls;
        }

        /// <summary>
        /// 获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="results">要获取的界面</param>
        public void GetUIForms(string uiFormAssetName, List<UIFormBase> results)
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            m_UIManager.GetUIForms(uiFormAssetName, m_InternalUIFormResultsCache);
            foreach (UIFormBase uiForm in m_InternalUIFormResultsCache)
            {
                results.Add(uiForm);
            }
        }

        /// <summary>
        /// 获取所有已加载的界面
        /// </summary>
        /// <returns>所有已加载的界面</returns>
        public UIFormBase[] GetAllLoadedUIForms()
        {
            UIFormBase[] uiForms = m_UIManager.GetAllLoadedUIForms();
            UIFormBase[] uiFormImpls = new UIFormBase[uiForms.Length];
            for (int i = 0; i < uiForms.Length; i++)
            {
                uiFormImpls[i] = uiForms[i];
            }

            return uiFormImpls;
        }

        /// <summary>
        /// 获取所有已加载的界面
        /// </summary>
        /// <param name="results">所有已加载的界面</param>
        public void GetAllLoadedUIForms(List<UIFormBase> results)
        {
            if (results == null)
            {
                Log.Error("Results is invalid.");
                return;
            }

            results.Clear();
            m_UIManager.GetAllLoadedUIForms(m_InternalUIFormResultsCache);
            foreach (UIFormBase uiForm in m_InternalUIFormResultsCache)
            {
                results.Add(uiForm);
            }
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号
        /// </summary>
        /// <returns>所有正在加载界面的序列编号</returns>
        public int[] GetAllLoadingUIFormSerialIds()
        {
            return m_UIManager.GetAllLoadingUIFormSerialIds();
        }

        /// <summary>
        /// 获取所有正在加载界面的序列编号
        /// </summary>
        /// <param name="results">所有正在加载界面的序列编号</param>
        public void GetAllLoadingUIFormSerialIds(List<int> results)
        {
            m_UIManager.GetAllLoadingUIFormSerialIds(results);
        }

        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingUIForm(int serialId)
        {
            return m_UIManager.IsLoadingUIForm(serialId);
        }

        /// <summary>
        /// 是否正在加载界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>是否正在加载界面</returns>
        public bool IsLoadingUIForm(string uiFormAssetName)
        {
            return m_UIManager.IsLoadingUIForm(uiFormAssetName);
        }

        /// <summary>
        /// 是否是合法的界面
        /// </summary>
        /// <param name="uiForm">界面</param>
        /// <returns>界面是否合法</returns>
        public bool IsValidUIForm(UIForm uiForm)
        {
            return m_UIManager.IsValidUIForm(uiForm);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName,  string uiGroupName)
        {
            return OpenUIForm(uiFormAssetName,  uiGroupName, DefaultPriority, false, null);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="priority">加载界面资源的优先级</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName,  string uiGroupName, int priority)
        {
            return OpenUIForm(uiFormAssetName,  uiGroupName, priority, false, null);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName,   string uiGroupName, bool pauseCoveredUIForm)
        {
            return OpenUIForm(uiFormAssetName,  uiGroupName, DefaultPriority, pauseCoveredUIForm, null);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName,  string uiGroupName, object userData)
        {
            return OpenUIForm(uiFormAssetName,  uiGroupName, DefaultPriority, false, userData);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="priority">加载界面资源的优先级</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName,  string uiGroupName, int priority, bool pauseCoveredUIForm)
        {
            return OpenUIForm(uiFormAssetName,  uiGroupName, priority, pauseCoveredUIForm, null);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="priority">加载界面资源的优先级</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName,   string uiGroupName, int priority, object userData)
        {
            return OpenUIForm(uiFormAssetName,  uiGroupName, priority, false, userData);
        }

        /// <summary>
        /// 打开界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <param name="uiGroupName">界面组名称</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面</param>
        /// <param name="userData">用户自定义数据</param>
        /// <returns>界面的序列编号</returns>
        public int OpenUIForm(string uiFormAssetName,  string uiGroupName, bool pauseCoveredUIForm, object userData)
        {
            return OpenUIForm(uiFormAssetName,  uiGroupName, DefaultPriority, pauseCoveredUIForm, userData);
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
        public int OpenUIForm(string uiFormAssetName,  string uiGroupName, int priority, bool pauseCoveredUIForm, object userData)
        {
            return m_UIManager.OpenUIForm(uiFormAssetName,  uiGroupName, priority, pauseCoveredUIForm, userData);
        }
        
        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="serialId">要关闭界面的序列编号</param>
        public void CloseUIForm(int serialId)
        {
            m_UIManager.CloseUIForm(serialId);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="serialId">要关闭界面的序列编号</param>
        /// <param name="userData">用户自定义数据</param>
        public void CloseUIForm(int serialId, object userData)
        {
            m_UIManager.CloseUIForm(serialId, userData);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiForm">要关闭的界面</param>
        public void CloseUIForm(UIForm uiForm)
        {
            m_UIManager.CloseUIForm(uiForm);
        }

        /// <summary>
        /// 关闭界面
        /// </summary>
        /// <param name="uiForm">要关闭的界面</param>
        /// <param name="userData">用户自定义数据</param>
        public void CloseUIForm(UIForm uiForm, object userData)
        {
            m_UIManager.CloseUIForm(uiForm, userData);
        }

        /// <summary>
        /// 关闭所有已加载的界面
        /// </summary>
        public void CloseAllLoadedUIForms()
        {
            m_UIManager.CloseAllLoadedUIForms();
        }

        /// <summary>
        /// 关闭所有已加载的界面
        /// </summary>
        /// <param name="userData">用户自定义数据</param>
        public void CloseAllLoadedUIForms(object userData)
        {
            m_UIManager.CloseAllLoadedUIForms(userData);
        }

        /// <summary>
        /// 关闭所有正在加载的界面
        /// </summary>
        public void CloseAllLoadingUIForms()
        {
            m_UIManager.CloseAllLoadingUIForms();
        }

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="uiForm">要激活的界面</param>
        public void RefocusUIForm(UIFormBase uiForm)
        {
            m_UIManager.RefocusUIForm(uiForm);
        }

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="uiForm">要激活的界面</param>
        /// <param name="userData">用户自定义数据</param>
        public void RefocusUIForm(UIFormBase uiForm, object userData)
        {
            m_UIManager.RefocusUIForm(uiForm, userData);
        }

        /// <summary>
        /// 设置界面是否被加锁
        /// </summary>
        /// <param name="uiForm">要设置是否被加锁的界面</param>
        /// <param name="locked">界面是否被加锁</param>
        public void SetUIFormInstanceLocked(UIForm uiForm, bool locked)
        {
            if (uiForm == null)
            {
                Log.Warning("UI form is invalid.");
                return;
            }

            m_UIManager.SetUIFormInstanceLocked(uiForm.gameObject, locked);
        }

        /// <summary>
        /// 设置界面的优先级
        /// </summary>
        /// <param name="uiForm">要设置优先级的界面</param>
        /// <param name="priority">界面优先级</param>
        public void SetUIFormInstancePriority(UIForm uiForm, int priority)
        {
            if (uiForm == null)
            {
                Log.Warning("UI form is invalid.");
                return;
            }

            m_UIManager.SetUIFormInstancePriority(uiForm.gameObject, priority);
        }

        private void OnOpenUIFormSuccess(UIFormBase uiForm, float duration, object userData)
        {
            Debug.Log(TextUtil.Format("Open UI form success, uiform name '{0}',duration time '{1}'.", uiForm.UIFormAssetName, duration));
            if (m_EnableOpenUIFormSuccessEvent)
            {
                //todo
                Debug.Log("加载UI资源成功"+ duration);
            }
        }

        private void OnOpenUIFormFailure(int serialId, string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, string errorMessage, object userData)
        {
            Log.Warning("Open UI form failure, asset name '{0}', UI group name '{1}', pause covered UI form '{2}', error message '{3}'.", uiFormAssetName, uiGroupName, pauseCoveredUIForm.ToString(), errorMessage);
            if (m_EnableOpenUIFormFailureEvent)
            {
                //todo
                Debug.Log("加载UI资源失败" + errorMessage);
            }
        }

        private void OnOpenUIFormUpdate(int serialId, string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, float progress, object userData)
        {
            if (m_EnableOpenUIFormUpdateEvent)
            {
                //todo
                Debug.Log("加载UI资源更新");
            }
        }

        private void OnOpenUIFormDependencyAsset(int serialId, string uiFormAssetName, string uiGroupName, bool pauseCoveredUIForm, string dependencyAssetName, int loadedCount, int totalCount, object userData)
        {
            if (m_EnableOpenUIFormDependencyAssetEvent)
            {
                //todo
                Debug.Log("加载UI资源依赖资源");
            }
        }

        private void OnCloseUIFormComplete(int serialId, string uiFormAssetName, UIGroup uiGroup, object userData)
        {
            if (m_EnableCloseUIFormCompleteEvent)
            {
                //todo
                Debug.Log("关闭UI");
            }
        }
    }
}
