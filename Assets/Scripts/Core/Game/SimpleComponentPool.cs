using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core.Pool
{
    public class SimpleComponentPool<T> where T : Component
    {
        // 对象池列表，在构造函数时初始化大小，每次获取物件从末尾，大小减1，每次返还对象添加至末尾，大小加1.
        internal readonly List<T> m_List;   

        private readonly GameObject m_ComponentPrefab;

        private readonly Action<T> m_ActionOnGet;

        private readonly Action<T> m_ActionOnRelease;

        private readonly int m_MaxSize;

        public int CountAll { get; private set; }

        public int CountInactive => m_List.Count;

        public int CountActive => CountAll - CountInactive;

        public SimpleComponentPool(GameObject componentPrefab, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, int defaultCapacity = 10, int maxSize = 10000)
        {
            m_ComponentPrefab = componentPrefab;

            if (m_ComponentPrefab == null)
            {
                throw new ArgumentNullException("componentPrefab");
            }

            if (maxSize <= 0)
            {
                throw new ArgumentException("对象池最大大小必须大于 0", "maxSize");
            }

            m_List = new List<T>(defaultCapacity);
            m_MaxSize = maxSize;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public SimpleComponentPool(string componentPrefabPath, Action<T> actionOnGet = null, Action<T> actionOnRelease = null, int defaultCapacity = 10, int maxSize = 10000)
        {
            m_ComponentPrefab = Resources.Load<GameObject>(componentPrefabPath);

            if (m_ComponentPrefab == null)
            {
                throw new Exception($"从该路径加载对象池预制件失败: {componentPrefabPath}");
            }

            if (maxSize <= 0)
            {
                throw new ArgumentException("对象池最大大小必须大于 0", "maxSize");
            }

            m_List = new List<T>(defaultCapacity);
            m_MaxSize = maxSize;
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T val;
            if (m_List.Count == 0)
            {
                val = GameObject.Instantiate(m_ComponentPrefab).GetComponent<T>();
                CountAll++;
            }
            else
            {
                int index = m_List.Count - 1;
                val = m_List[index];
                m_List.RemoveAt(index);
            }

            m_ActionOnGet?.Invoke(val);

            val.gameObject.SetActive(true);
            return val;
        }

        public void Release(T element)
        {
            element.gameObject.SetActive(false);

            m_ActionOnRelease?.Invoke(element);
            if (CountInactive < m_MaxSize)  // 其实就是列表的数量没超过最大限制
            {
                m_List.Add(element);
                return;
            }

            // 超过最大数量限制时，直接删除元素
            CountAll--;
            GameObject.Destroy(element.gameObject);
        }

        public void Clear()
        {
            foreach (T item in m_List)
            {
                GameObject.Destroy(item.gameObject);
            }

            m_List.Clear();
            CountAll = 0;
        }
    }
}
