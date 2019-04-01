using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    /// <summary>
    /// 界面组
    /// </summary>
    public class UIGroup
    {
        private readonly string m_Name;
        private int m_Depth;
        private bool m_Pause;
        private readonly Transform uiGroupTransform;
        private readonly LinkedList<UIFormInfo> m_UIFormInfos;

        /// <summary>
        /// 初始化界面组的新实例
        /// </summary>
        /// <param name="name">界面组名称</param>
        /// <param name="depth">界面组深度</param>
        /// <param name="uiGroupTransform">界面组变换</param>
        public UIGroup(string name, int depth, Transform uiGroupTransform)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception("UI group name is invalid.");
            }

            if (uiGroupTransform == null)
            {
                throw new Exception("UI grouptransform is invalid.");
            }

            m_Name = name;
            m_Pause = false;
            this.uiGroupTransform = uiGroupTransform;
            m_UIFormInfos = new LinkedList<UIFormInfo>();
            Depth = depth;
        }

        /// <summary>
        /// 获取界面组名称
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 获取或设置界面组深度
        /// </summary>
        public int Depth
        {
            get
            {
                return m_Depth;
            }
            set
            {
                if (m_Depth == value)
                {
                    return;
                }

                m_Depth = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取或设置界面组是否暂停
        /// </summary>
        public bool Pause
        {
            get
            {
                return m_Pause;
            }
            set
            {
                if (m_Pause == value)
                {
                    return;
                }

                m_Pause = value;
                Refresh();
            }
        }

        /// <summary>
        /// 获取界面组中界面数量
        /// </summary>
        public int UIFormCount
        {
            get
            {
                return m_UIFormInfos.Count;
            }
        }

        /// <summary>
        /// 获取当前界面
        /// </summary>
        public UIFormBase CurrentUIForm
        {
            get
            {
                return m_UIFormInfos.First != null ? m_UIFormInfos.First.Value.UIFormBase : null;
            }
        }

        /// <summary>
        /// 获取界面组变换
        /// </summary>
        public Transform UIGroupTransform
        {
            get
            {
                return uiGroupTransform;
            }
        }

        /// <summary>
        /// 界面组轮询
        /// </summary>
        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            LinkedListNode<UIFormInfo> current = m_UIFormInfos.First;
            while (current != null)
            {
                if (current.Value.Paused)
                {
                    break;
                }

                LinkedListNode<UIFormInfo> next = current.Next;
                current.Value.UIFormBase.OnUpdate(deltaTime, unscaledDeltaTime);
                current = next;
            }
        }

        /// <summary>
        /// 界面组中是否存在界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>界面组中是否存在界面</returns>
        public bool HasUIForm(int serialId)
        {
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIFormBase.SerialId == serialId)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 界面组中是否存在界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>界面组中是否存在界面</returns>
        public bool HasUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UI form asset name is invalid.");
            }

            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIFormBase.UIFormAssetName == uiFormAssetName)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 从界面组中获取界面
        /// </summary>
        /// <param name="serialId">界面序列编号</param>
        /// <returns>要获取的界面</returns>
        public UIFormBase GetUIForm(int serialId)
        {
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIFormBase.SerialId == serialId)
                {
                    return uiFormInfo.UIFormBase;
                }
            }

            return null;
        }

        /// <summary>
        /// 从界面组中获取界面
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称</param>
        /// <returns>要获取的界面</returns>
        public UIFormBase GetUIForm(string uiFormAssetName)
        {
            if (string.IsNullOrEmpty(uiFormAssetName))
            {
                throw new Exception("UI form asset name is invalid.");
            }

            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIFormBase.UIFormAssetName == uiFormAssetName)
                {
                    return uiFormInfo.UIFormBase;
                }
            }

            return null;
        }

        /// <summary>
        /// 从界面组中获取界面
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
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIFormBase.UIFormAssetName == uiFormAssetName)
                {
                    results.Add(uiFormInfo.UIFormBase);
                }
            }

            return results.ToArray();
        }

        /// <summary>
        /// 从界面组中获取界面
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
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIFormBase.UIFormAssetName == uiFormAssetName)
                {
                    results.Add(uiFormInfo.UIFormBase);
                }
            }
        }

        /// <summary>
        /// 从界面组中获取所有界面
        /// </summary>
        /// <returns>界面组中的所有界面</returns>
        public UIFormBase[] GetAllUIForms()
        {
            List<UIFormBase> results = new List<UIFormBase>();
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                results.Add(uiFormInfo.UIFormBase);
            }

            return results.ToArray();
        }

        /// <summary>
        /// 从界面组中获取所有界面
        /// </summary>
        /// <param name="results">界面组中的所有界面</param>
        public void GetAllUIForms(List<UIFormBase> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                results.Add(uiFormInfo.UIFormBase);
            }
        }

        /// <summary>
        /// 往界面组增加界面
        /// </summary>
        /// <param name="uiFormBase">要增加的界面</param>
        public void AddUIForm(UIFormBase uiFormBase)
        {
            UIFormInfo uiFormInfo = new UIFormInfo(uiFormBase);
            m_UIFormInfos.AddFirst(uiFormInfo);
        }

        /// <summary>
        /// 从界面组移除界面
        /// </summary>
        /// <param name="uiFormBase">要移除的界面</param>
        public void RemoveUIForm(UIFormBase uiFormBase)
        {
            UIFormInfo uiFormInfo = GetUIFormInfo(uiFormBase);
            if (uiFormInfo == null)
            {
                throw new Exception(TextUtil.Format("Can not find UI form info for serial id '{0}', UI form asset name is '{1}'.", uiFormBase.SerialId.ToString(), uiFormBase.UIFormAssetName));
            }

            if (!uiFormInfo.Covered)
            {
                uiFormInfo.Covered = true;
                uiFormBase.OnCover();
            }

            if (!uiFormInfo.Paused)
            {
                uiFormInfo.Paused = true;
                uiFormBase.OnPause();
            }

            m_UIFormInfos.Remove(uiFormInfo);
        }

        /// <summary>
        /// 激活界面
        /// </summary>
        /// <param name="uiFormBase">要激活的界面</param>
        /// <param name="userData">用户自定义数据</param>
        public void RefocusUIForm(UIFormBase uiFormBase, object userData)
        {
            UIFormInfo uiFormInfo = GetUIFormInfo(uiFormBase);
            if (uiFormInfo == null)
            {
                throw new Exception("Can not find UI form info.");
            }

            m_UIFormInfos.Remove(uiFormInfo);
            m_UIFormInfos.AddFirst(uiFormInfo);
        }

        /// <summary>
        /// 刷新界面组
        /// </summary>
        public void Refresh()
        {
            LinkedListNode<UIFormInfo> current = m_UIFormInfos.First;
            bool pause = m_Pause;
            bool cover = false;
            int depth = UIFormCount;
            while (current != null)
            {
                LinkedListNode<UIFormInfo> next = current.Next;
                current.Value.UIFormBase.OnDepthChanged(Depth, depth--);
                if (pause)
                {
                    if (!current.Value.Covered)
                    {
                        current.Value.Covered = true;
                        current.Value.UIFormBase.OnCover();
                    }

                    if (!current.Value.Paused)
                    {
                        current.Value.Paused = true;
                        current.Value.UIFormBase.OnPause();
                    }
                }
                else
                {
                    if (current.Value.Paused)
                    {
                        current.Value.Paused = false;
                        current.Value.UIFormBase.OnResume();
                    }

                    if (current.Value.UIFormBase.PauseCoveredUIForm)
                    {
                        pause = true;
                    }

                    if (cover)
                    {
                        if (!current.Value.Covered)
                        {
                            current.Value.Covered = true;
                            current.Value.UIFormBase.OnCover();
                        }
                    }
                    else
                    {
                        if (current.Value.Covered)
                        {
                            current.Value.Covered = false;
                            current.Value.UIFormBase.OnReveal();
                        }

                        cover = true;
                    }
                }

                current = next;
            }
        }

        internal void InternalGetUIForms(string uiFormAssetName, List<UIFormBase> results)
        {
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIFormBase.UIFormAssetName == uiFormAssetName)
                {
                    results.Add(uiFormInfo.UIFormBase);
                }
            }
        }

        internal void InternalGetAllUIForms(List<UIFormBase> results)
        {
            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                results.Add(uiFormInfo.UIFormBase);
            }
        }

        private UIFormInfo GetUIFormInfo(UIFormBase uiFormBase)
        {
            if (uiFormBase == null)
            {
                throw new Exception("UI form is invalid.");
            }

            foreach (UIFormInfo uiFormInfo in m_UIFormInfos)
            {
                if (uiFormInfo.UIFormBase == uiFormBase)
                {
                    return uiFormInfo;
                }
            }

            return null;
        }
    }
}