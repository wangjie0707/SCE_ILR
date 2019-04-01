using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Myth
{
    /// <summary>
    /// 输入组件
    /// </summary>
    public class InputComponent : GameBaseComponent
    {
        [Range(1, 10)]
        [SerializeField]
        private int m_MaxFingerCount = 3;

        /// <summary>
        /// 是否启用鼠标输入
        /// </summary>
        [SerializeField]
        private bool m_EnableMouseInput = true;

        /// <summary>
        /// 是否启用重力感应
        /// </summary>
        [SerializeField]
        private bool m_EnableGravityInduction = true;

        /// <summary>
        /// 是否接受touch输入
        /// </summary>
        [SerializeField]
        private bool m_EnableTouch = true;

        /// <summary>
        /// 重力方向
        /// </summary>
        private Vector3 m_GravityDir = Vector3.zero;

        private Dictionary<int, Vector2> m_UIFingeridDic = new Dictionary<int, Vector2>();//按在UI上的手势
        private Dictionary<int, Vector2> m_SceneFingeridDic = new Dictionary<int, Vector2>();//按在场景上的手势

        /// <summary>
        /// 手指滑动方向
        /// </summary>
        private Vector2 m_FingerDir;

        private bool m_IsChangeSize = false;

        private int m_Moveid1 = -1;
        private int m_Moveid2 = -1;

        private Vector2 m_TempFinger1Pos;
        private Vector2 m_TempFinger2Pos;

        private Vector2 m_OldFinger1Pos;
        private Vector2 m_OldFinger2Pos;

        /// <summary>
        /// 手指鼠标的位置
        /// </summary>
        private Vector2 m_OldMousePos;

        /// <summary>
        /// 是否处于按下状态
        /// </summary>
        private bool m_EligibleForClick = false;

        /// <summary>
        /// 获取或设置是否接受重力感应
        /// </summary>
        public bool EnableMouseInput
        {
            get
            {
                return m_EnableMouseInput;
            }
            set
            {
                m_EnableMouseInput = value;
            }
        }


        /// <summary>
        /// 获取或设置是否接受touch输入
        /// </summary>
        public bool EnableTouch
        {
            get
            {
                return m_EnableTouch;
            }
            set
            {
                m_EnableTouch = value;
            }
        }

        /// <summary>
        /// 获取或设置是否接受重力感应
        /// </summary>
        public bool EnableGravityInduction
        {
            get
            {
                return m_EnableGravityInduction;
            }
            set
            {
                m_EnableGravityInduction = value;
            }
        }


        /// <summary>
        /// 获取或设置最大处理手指数
        /// </summary>
        public int MaxFingerCount
        {
            get
            {
                return m_MaxFingerCount;
            }
            set
            {
                m_MaxFingerCount = value;
            }
        }

        public override void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            base.OnUpdate(deltaTime, unscaledDeltaTime);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (m_EnableMouseInput && Input.mousePresent)
            {
                UpdateMouseInput();
            }
#endif

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
            if (m_EnableTouch)//如果能接受输入
            {
                UpdateTouch();//更新Touch输入
            }


            if (m_EnableGravityInduction)
            {
                UpdateGravity();//更新重力感应
            }
#endif
        }

        #region UpdateMouseInput 更新鼠标输入
        /// <summary>
        /// 更新鼠标输入
        /// </summary>
        private void UpdateMouseInput()
        {
            #region 滚轮
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                Debug.Log("缩小");
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                Debug.Log("放大");
            }
            #endregion

            #region 鼠标点击
            if (Input.GetMouseButtonDown(0))
            {
                if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                {
                    m_EligibleForClick = false;
                    //点在了UI上。
                    Debug.Log("点在了UI上" + (Vector2)Input.mousePosition);
                }
                else
                {
                    //点在了场景
                    Debug.Log("点在了场景" + (Vector2)Input.mousePosition);
                    m_EligibleForClick = true;
                    m_OldMousePos = Input.mousePosition;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_EligibleForClick = false;
                Debug.Log("抬起");

            }

            if (m_EligibleForClick)
            {
                m_FingerDir = (Vector2)Input.mousePosition - m_OldMousePos;

                if (m_FingerDir.sqrMagnitude < 0.01f)
                {
                    return;
                }
                if (m_FingerDir.y < m_FingerDir.x && m_FingerDir.y > -m_FingerDir.x)
                {
                    //向右
                    Debug.Log("向右");
                }
                else if (m_FingerDir.y > m_FingerDir.x && m_FingerDir.y < -m_FingerDir.x)
                {
                    //向左
                    Debug.Log("向左");
                }
                else if (m_FingerDir.y > m_FingerDir.x && m_FingerDir.y > -m_FingerDir.x)
                {
                    //向上
                    Debug.Log("向上");
                }
                else
                {
                    //向下
                    Debug.Log("向下");
                }
                m_OldMousePos = Input.mousePosition;
            }

            #endregion
        }
        #endregion

        #region UpdateGravity 更新重力感应
        private void UpdateGravity()
        {
            m_GravityDir.x = Input.acceleration.x;
            m_GravityDir.z = Input.acceleration.y;
            m_GravityDir.y = Input.acceleration.z;


            if (m_GravityDir.x < 0)
            {
                Debug.Log("向左 速度 = " + m_GravityDir.x);
            }
            else
            {
                Debug.Log("向右 速度 = " + m_GravityDir.x);
            }

            if (m_GravityDir.z < 0)
            {
                Debug.Log("向下 速度 = " + m_GravityDir.z);
            }
            else
            {
                Debug.Log("向上 速度 = " + m_GravityDir.z);
            }

        }
        #endregion


        #region  UpdateTouch 更新Touch输入
        private void UpdateTouch()
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch input = Input.GetTouch(i);

                    if (m_UIFingeridDic.ContainsKey(input.fingerId))
                    {//如果是按在UI上了 
                     //如果是UI上的 并且是取消了 则要从列表中移除
                        if (input.phase == TouchPhase.Ended || input.phase == TouchPhase.Canceled)
                        {
                            Debug.Log("结束并删除了UI的输入" + input.position + "    手势Id= " + input.fingerId);
                            m_UIFingeridDic.Remove(input.fingerId);

                        }
                        continue;//跳过在UI上的这个手势的判断
                    }

                    //按下的时候
                    if (input.phase == TouchPhase.Began)
                    {
                        //如果点在了UI上
                        if (IsPointerOverGameObject(input.position))
                        {
                            Debug.Log("点击在了UI上,位置=" + input.position + "    手势Id= " + input.fingerId);
                            m_UIFingeridDic[input.fingerId] = input.position;
                        }
                        else
                        {//没有点在UI上 
                            Debug.Log("没有点在UI上,位置=" + input.position + "    手势Id= " + input.fingerId);
                            m_SceneFingeridDic[input.fingerId] = input.position;
                        }
                    }
                    else if (input.phase == TouchPhase.Ended || input.phase == TouchPhase.Canceled)
                    {//结束了输入

                        Debug.Log("结束了输入" + input.position + "    手势Id= " + input.fingerId);
                        if (m_SceneFingeridDic.ContainsKey(input.fingerId))
                        {
                            m_SceneFingeridDic.Remove(input.fingerId);
                        }
                    }


                    //不处理第超过最大手指的移动判断
                    if (i >= m_MaxFingerCount)
                    {
                        break;
                    }

                    if (input.phase == TouchPhase.Moved)
                    {//移动中

                        //有2个及以上在屏幕中的手指
                        if ((Input.touchCount > m_MaxFingerCount ? m_MaxFingerCount : Input.touchCount) - m_UIFingeridDic.Count >= 2)
                        {
                            m_IsChangeSize = true;
                            if (m_SceneFingeridDic.ContainsKey(input.fingerId))
                            {
                                m_SceneFingeridDic[input.fingerId] = input.position;
                            }
                        }
                        else
                        {//只有一个
                            if (m_SceneFingeridDic.ContainsKey(input.fingerId))
                            {
                                m_FingerDir = input.position - m_SceneFingeridDic[input.fingerId];
                                if (m_FingerDir.y < m_FingerDir.x && m_FingerDir.y > -m_FingerDir.x)
                                {
                                    //向右
                                    Debug.Log("向右");
                                }
                                else if (m_FingerDir.y > m_FingerDir.x && m_FingerDir.y < -m_FingerDir.x)
                                {
                                    //向左
                                    Debug.Log("向左");
                                }
                                else if (m_FingerDir.y > m_FingerDir.x && m_FingerDir.y > -m_FingerDir.x)
                                {
                                    //向上
                                    Debug.Log("向上");
                                }
                                else
                                {
                                    //向下
                                    Debug.Log("向下");
                                }
                                m_SceneFingeridDic[input.fingerId] = input.position;
                            }
                        }

                    }
                }

                UpdateMoveTouch();
            }
        }

        /// <summary>
        /// 更新在移动中的touch
        /// </summary>
        private void UpdateMoveTouch()
        {
            if (m_SceneFingeridDic.Count >= 2 && m_IsChangeSize)
            {
                bool first = true;
                var fingerid = m_SceneFingeridDic.GetEnumerator();
                while (fingerid.MoveNext())
                {
                    if (first)
                    {
                        m_Moveid1 = fingerid.Current.Key;
                        first = false;
                    }
                    else
                    {
                        m_Moveid2 = fingerid.Current.Key;
                        break;
                    }
                    break;
                }

                if (m_SceneFingeridDic.TryGetValue(m_Moveid1, out m_TempFinger1Pos) && m_SceneFingeridDic.TryGetValue(m_Moveid2, out m_TempFinger2Pos))
                {
                    if (Vector2.Distance(m_OldFinger1Pos, m_OldFinger2Pos) < Vector2.Distance(m_TempFinger1Pos, m_TempFinger2Pos))
                    {
                        //放大
                        Debug.Log("放大");
                    }
                    else
                    {
                        //缩小
                        Debug.Log("缩小");
                    }
                    m_OldFinger1Pos = m_TempFinger1Pos;
                    m_OldFinger2Pos = m_TempFinger2Pos;
                }
                m_IsChangeSize = false;
            }
        }
        #endregion

        #region IsPointerOverGameObject 移动端判断是否点击在UI上
        /// <summary>
        /// 点击判断结果
        /// </summary>
        List<RaycastResult> results = new List<RaycastResult>();

        /// <summary>
        /// 移动端判断是否点击在UI上
        /// </summary>
        /// <param name="screenPosition">点击的屏幕坐标</param>
        /// <returns></returns>
        public bool IsPointerOverGameObject(Vector2 screenPosition)
        {
            //实例化点击事件
            PointerEventData eventDataCurrPosition = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
            //将点击位置的屏幕坐标赋值给点击事件
            eventDataCurrPosition.position = new Vector2(screenPosition.x, screenPosition.y);

            results.Clear();
            //向点击处发射射线
            EventSystem.current.RaycastAll(eventDataCurrPosition, results);

            return results.Count > 0;
        }

        #endregion

        
    }
}