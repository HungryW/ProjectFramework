using UnityEngine;
using GameFrameworkPackage;
using System;
using System.Collections.Generic;
using GameFramework;
using UnityGameFramework.Runtime;

namespace HotFixEntry
{
    public class CUIModuleContentCtrl : CUIModuleCtrlBase
    {
        private List<CUIModuleCtrlBase> m_listChildren;
        public CUIModuleContentCtrl()
        {
            m_listChildren = new List<CUIModuleCtrlBase>();
        }

        public T AddChild<T>(GameObject a_prefab) where T : CUIModuleCtrlBase
        {
            CUIModuleCtrlBase itemCtrl = AddChild(a_prefab, typeof(T));
            return (T)itemCtrl;
        }
        public CUIModuleCtrlBase AddChild(GameObject a_prefab)
        {
            return AddChild(a_prefab, typeof(CUIModuleCtrlBase));
        }

        public CUIModuleCtrlBase AddChild(GameObject a_prefab, Type a_tChildLogicType)
        {
            GameObject goChild = GameObject.Instantiate(a_prefab, transform, false);
            CUIModuleCtrlBase itemCtrl = AttachMoudleCtrl(goChild.transform, a_tChildLogicType);
            itemCtrl.transform.name = Utility.Text.Format("{0}_{1}", a_prefab.name, m_listChildren.Count);
            m_listChildren.Add(itemCtrl);
            return itemCtrl;
        }

        public T AttachChild<T>(Transform a_childTransform) where T : CUIModuleCtrlBase
        {
            CUIModuleCtrlBase itemCtrl = AttachMoudleCtrl(a_childTransform, typeof(T));
            m_listChildren.Add(itemCtrl);
            return (T)itemCtrl;
        }

        public void DelChild(CUIModuleCtrlBase a_refChild)
        {
            if (null == a_refChild)
            {
                return;
            }
            m_listChildren.Remove(a_refChild);
            UnityEngine.Object.Destroy(a_refChild.gameObject);
        }

        public void DestoryAllChildren()
        {
            m_listChildren.Clear();
            transform.DestoryAllChildren();
        }

        public T FindChild<T>(Func<T, bool> a_fnCheck) where T : CUIModuleCtrlBase
        {
            if (null == a_fnCheck)
            {
                return null;
            }
            List<CUIModuleCtrlBase> listChild = m_listChildren;
            int nCount = listChild.Count;
            for (int i = 0; i < nCount; i++)
            {
                if (a_fnCheck((T)listChild[i]))
                {
                    return (T)listChild[i];
                }
            }
            return null;
        }

        public List<T> FindChildList<T>(Func<T, bool> a_fnCheck) where T : CUIModuleCtrlBase
        {
            List<T> listChild = new List<T>();
            if (null == a_fnCheck)
            {
                return listChild;
            }
            List<CUIModuleCtrlBase> listAllChild = m_listChildren;
            int nCount = listAllChild.Count;
            for (int i = 0; i < nCount; i++)
            {
                if (a_fnCheck((T)listAllChild[i]))
                {
                    listChild.Add((T)listAllChild[i]);
                }
            }
            return listChild;
        }

        public void TraversingChild<T>(Action<T> a_fn) where T : CUIModuleCtrlBase
        {
            if (a_fn == null)
            {
                return;
            }
            List<CUIModuleCtrlBase> listAllChild = m_listChildren;
            int nCount = listAllChild.Count;
            for (int i = 0; i < nCount; i++)
            {
                a_fn.Invoke((T)listAllChild[i]);
            }
        }

        public List<CUIModuleCtrlBase> GetAllChildren()
        {
            return m_listChildren;
        }
        public CUIModuleCtrlBase GetChild(int a_nIdx)
        {
            if (a_nIdx < 0 || a_nIdx > m_listChildren.Count)
            {
                Log.Error("CUIModuleCtrlBase.GetChild() Fail a_nIdx ={0} m_listChildren.Count = {1}", a_nIdx, m_listChildren.Count);
                return null;
            }
            return m_listChildren[a_nIdx];
        }
        public int GetChildCount()
        {
            return m_listChildren.Count;
        }
    }
}