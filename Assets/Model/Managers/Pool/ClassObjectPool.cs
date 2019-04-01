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
    /// 类对象池 (不支持带参数构造函数的类，必须使用Init之类的初始化方法)
    /// </summary>
    public class ClassObjectPool : IDisposable
    {
        /// <summary>
        /// 下次释放时间
        /// </summary>
        private float m_NextRunTime;

        /// <summary>
        /// 释放间隔
        /// </summary>
        private float m_ClearClassObjectInterval = 60f;

        /// <summary>
        /// 类对象在池中的常驻数量
        /// </summary>
        public Dictionary<int, byte> ClassObjectCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 类对象池字典
        /// </summary>
        private Dictionary<int, Queue<object>> m_ClassObjetPoolDic;


        /// <summary>
        /// 在监类对象池的信息
        /// </summary>
        private Dictionary<Type, int> m_ObjectTypeDic = new Dictionary<Type, int>();


        public ClassObjectPool()
        {
            ClassObjectCount = new Dictionary<int, byte>();
            m_ClassObjetPoolDic = new Dictionary<int, Queue<object>>();
            m_NextRunTime = 0f;
        }

        /// <summary>
        /// 在监类对象池的信息
        /// </summary>
        public Dictionary<Type, int> ObjectTypeDic
        {
            get
            {
                return m_ObjectTypeDic;
            }
        }
     


        /// <summary>
        /// 获取或设置类对象池是否间隔
        /// </summary>
        public float ClearClassObjectInterval
        {
            get
            {
                return m_ClearClassObjectInterval;
            }
            set
            {
                m_ClearClassObjectInterval = value;
            }
        }

        public void OnUpdate(float deltaTime, float unscaledDeltaTime)
        {
            if (Time.time > m_NextRunTime + m_ClearClassObjectInterval)
            {
                //释放
                m_NextRunTime = Time.time;
                Clear();//释放类对象池
            }
        }

        #region SetResideCount 设置类常驻数量
        /// <summary>
        /// 设置类常驻数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="count"></param>
        public void SetResideCount<T>(byte count) where T : class
        {
            int key = typeof(T).GetHashCode();
            ClassObjectCount[key] = count;
        }
        #endregion

        #region Dequeue 取出一个对象
        /// <summary>
        /// 取出一个对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Dequeue<T>() where T : class, new()
        {
            lock (m_ClassObjetPoolDic)
            {
                //先找到这个类的哈希值
                int key = typeof(T).GetHashCode();

                Queue<object> queue = null;
                m_ClassObjetPoolDic.TryGetValue(key, out queue);

                if (queue == null)
                {
                    queue = new Queue<object>();
                    m_ClassObjetPoolDic[key] = queue;
                }

                //开始获取对象
                if (queue.Count > 0)
                {
                    //说明队列中有闲置的
                    //Debug.Log("对象" + key + "存在 从池中获取");
                    object obj = queue.Dequeue();

                    Type t = obj.GetType();
                    if (m_ObjectTypeDic.ContainsKey(t))
                    {
                        m_ObjectTypeDic[t]--;
                    }
                    else
                    {
                        m_ObjectTypeDic[t] = 0;
                    }

                    return (T)obj;
                }
                else
                {
                    //如果队列中没有 才实例化一个
                    return new T();
                }
            }
        }
        #endregion

        #region Enqueue 对象回池
        /// <summary>
        /// 对象回池
        /// </summary>
        /// <param name="obj"></param>
        public void Enqueue(object obj)
        {
            lock (m_ClassObjetPoolDic)
            {
                int key = obj.GetType().GetHashCode();
                //Debug.Log("对象" + key + "回池了");

                Queue<object> queue = null;
                m_ClassObjetPoolDic.TryGetValue(key, out queue);

                Type t = obj.GetType();
                if (m_ObjectTypeDic.ContainsKey(t))
                {
                    m_ObjectTypeDic[t]++;
                }
                else
                {
                    m_ObjectTypeDic[t] = 1;
                }


                if (queue != null)
                {
                    queue.Enqueue(obj);
                }
            }
        }
        #endregion


        /// <summary>
        /// 释放类对象池
        /// </summary>
        public void Clear()
        {
            lock (m_ClassObjetPoolDic)
            {
                int queueCount = 0;//队列数量
                
                //1.定义迭代器
                var enumerator = m_ClassObjetPoolDic.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    int key = enumerator.Current.Key;
                    
                    //拿到队列
                    Queue<object> queue = m_ClassObjetPoolDic[key];

                    Type t = null;

                    queueCount = queue.Count;

                    //用于释放的时候判断
                    byte resideCount = 0;
                    ClassObjectCount.TryGetValue(key, out resideCount);
                    while (queueCount > resideCount)
                    {
                        //队列中有可释放的对象
                        queueCount--;
                        object obj = queue.Dequeue();//从队列中取出一个 这个对象没有任何引用，就变成了野指针 等待GC回收

                        t = obj.GetType();
                        m_ObjectTypeDic[t]--;
                    }

                    if (queueCount == 0)
                    {
                        if (t != null)
                        {
                            m_ObjectTypeDic.Remove(t);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            m_ClassObjetPoolDic.Clear();
        }
    }
}
