using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public class CUIToolVisibleStackItem : MonoBehaviour
    {
        private CUIToolVisibleStackMgr m_refMgr = null;

        public virtual void Open(CUIToolVisibleStackMgr a_refMgr, object a_oUserData = null)
        {
            m_refMgr = a_refMgr;
            Show();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Close()
        {
            Hide();
            m_refMgr = null;
        }

        public void Visible( bool a_bVisible )
        {
            gameObject.SetActive(a_bVisible);
        }

        protected void _ShowOther(CUIToolVisibleStackItem a_go )
        {
            if (null == m_refMgr )
            {
                return;
            }

            m_refMgr.PushShow(a_go);
        }

        protected void _HideSelf()
        {
            if(null == m_refMgr)
            {
                return; 
            }
            m_refMgr.PopHide();
        }
    }

    public class CUIToolVisibleStackMgr
    {
        private List<CUIToolVisibleStackItem> m_stackItem;

        public CUIToolVisibleStackMgr()
        {
            m_stackItem = new List<CUIToolVisibleStackItem>();
        }

        public void Init()
        {
            foreach( var item in m_stackItem )
            {
                item.Visible(false);
            }

            m_stackItem.Clear();
        }

        public void PushShow(CUIToolVisibleStackItem a_go, object a_oUserData = null)
        {
            if( !_CheckIsEmpty() )
            {
                m_stackItem[0].Hide();
            }
            a_go.Open(this, a_oUserData);
            m_stackItem.Insert(0, a_go);
        }

        public void PopHide()
        {
            if (_CheckIsEmpty())
            {
                return;
            }
            m_stackItem[0].Close();
            m_stackItem.RemoveAt(0);
            if (!_CheckIsEmpty())
            {
                m_stackItem[0].Show();
            }
        }

        private bool _CheckIsEmpty()
        {
            return m_stackItem.Count == 0;
        }
    }
}


