using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFrameworkPackage
{
    public class CDragEventParam
    {
        private Camera m_Camera;
        private Vector3 m_v3WorldPos;
        public CDragEventParam(Camera a_Camera, Vector3 a_v3WorldPos)
        {
            m_Camera = a_Camera;
            m_v3WorldPos = a_v3WorldPos;
        }

        public Camera GetCamera()
        {
            return m_Camera;
        }

        public Vector3 GetWorldPos()
        {
            return m_v3WorldPos;
        }
    }

    public class CUIDragInputCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public delegate void DragEvent(CDragEventParam eventData);

        [SerializeField]
        private Transform TranMove;

        private DragEvent m_fnOnDragBegin;
        private DragEvent m_fnOnDraging;
        private DragEvent m_fnOnDragEnd;
        private bool m_bEnable = true;

        public CUIDragInputCtrl SetBeginCallback(DragEvent a_fn)
        {
            m_fnOnDragBegin = a_fn;
            return this;
        }

        public CUIDragInputCtrl SetDragingCallback(DragEvent a_fn)
        {
            m_fnOnDraging = a_fn;
            return this;
        }

        public CUIDragInputCtrl SetEndCallback(DragEvent a_fn)
        {
            m_fnOnDragEnd = a_fn;
            return this;
        }

        public void SetEnable(bool a_bIsEnable)
        {
            m_bEnable = a_bIsEnable;
        }

        private Vector3 m_v3StartDragPos;
        private Vector3 m_v3OffsetPos;  //临时记录点击点与UI的相对位置

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!m_bEnable)
            {
                return;
            }
            _SafeInvokeEventCallback(m_fnOnDragBegin, eventData);
            _SetDraggOffset(eventData);
            m_v3StartDragPos = TranMove.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!m_bEnable)
            {
                return;
            }
            _SetDraggedPosition(eventData);
            _SafeInvokeEventCallback(m_fnOnDraging, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!m_bEnable)
            {
                return;
            }
            _SafeInvokeEventCallback(m_fnOnDragEnd, eventData);
        }

        private void _SetDraggOffset(PointerEventData eventData)
        {
            Vector3 tWorldPos;
            //UI屏幕坐标转换为世界坐标
            RectTransformUtility.ScreenPointToWorldPointInRectangle(TranMove as RectTransform, eventData.position, eventData.pressEventCamera, out tWorldPos);
            //计算偏移量   
            m_v3OffsetPos = TranMove.position - tWorldPos;
        }

        private void _SetDraggedPosition(PointerEventData eventData)
        {
            //存储当前鼠标所在位置
            Vector3 globalMousePos;
            //UI屏幕坐标转换为世界坐标
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(TranMove as RectTransform, eventData.position, eventData.pressEventCamera, out globalMousePos))
            {
                //设置位置及偏移量
                TranMove.position = globalMousePos + m_v3OffsetPos;
            }
        }

        private void _SafeInvokeEventCallback(DragEvent a_fn, PointerEventData eventData)
        {
            if (a_fn != null)
            {
                CDragEventParam param = new CDragEventParam(eventData.pressEventCamera, TranMove.position);
                a_fn.Invoke(param);
            }
        }

    }
}

