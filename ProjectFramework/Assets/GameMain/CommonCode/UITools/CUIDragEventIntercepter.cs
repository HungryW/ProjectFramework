using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFrameworkPackage
{
    public class CUIDragEventIntercepter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private Action<PointerEventData> m_fnInstanceDragBegin;
        private Action<PointerEventData> m_fnInstanceDrag;
        private Action<PointerEventData> m_fnInstanceDragEnd;
        private Action m_fnOnBeginDrag;
        private Action m_fnOnDrag;
        private Action m_fnOnEndDrag;

        public CUIDragEventIntercepter SetOnBeginFn(Action a_fn)
        {
            m_fnOnBeginDrag = a_fn;
            return this;
        }
        public CUIDragEventIntercepter SetOnEndFn(Action a_fn)
        {
            m_fnOnEndDrag = a_fn;
            return this;
        }
        public CUIDragEventIntercepter SetOnDragFn(Action a_fn)
        {
            m_fnOnDrag = a_fn;
            return this;
        }

        public CUIDragEventIntercepter SetBeginInstance(Action<PointerEventData> a_fn)
        {
            m_fnInstanceDragBegin = a_fn;
            return this;
        }

        public CUIDragEventIntercepter SetDragInstance(Action<PointerEventData> a_fn)
        {
            m_fnInstanceDrag = a_fn;
            return this;
        }

        public CUIDragEventIntercepter SetDragEndInstance(Action<PointerEventData> a_fn)
        {
            m_fnInstanceDragEnd = a_fn;
            return this;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (m_fnInstanceDragBegin != null)
            {
                m_fnInstanceDragBegin.Invoke(eventData);
            }
            if (m_fnOnBeginDrag != null)
            {
                m_fnOnBeginDrag.SafeInvoke();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_fnInstanceDrag != null)
            {
                m_fnInstanceDrag.Invoke(eventData);
            }
            if (m_fnOnDrag != null)
            {
                m_fnOnDrag.SafeInvoke();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (m_fnInstanceDragEnd != null)
            {
                m_fnInstanceDragEnd.Invoke(eventData);
            }
            if (m_fnOnEndDrag != null)
            {
                m_fnOnEndDrag.SafeInvoke();
            }
        }
    }
}
