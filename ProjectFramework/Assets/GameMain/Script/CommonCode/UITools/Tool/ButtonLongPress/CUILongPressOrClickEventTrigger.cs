using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameFrameworkPackage
{
    public class CUILongPressOrClickEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler
    {
        public float durationThreshold = 1.0f;
        public UnityEvent onLongPress = new UnityEvent();
        public UnityEvent onLongPressEnd = new UnityEvent();
        public UnityEvent onClick = new UnityEvent();
        private bool isPointerDown = false;
        private bool longPressTriggered = false;
        private float timePressStarted;
        private void Update()
        {
            if (isPointerDown && !longPressTriggered)
            {
                if (Time.time - timePressStarted > durationThreshold)
                {
                    longPressTriggered = true;
                    onLongPress.Invoke();
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            timePressStarted = Time.time;
            isPointerDown = true;
            longPressTriggered = false;
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            _TryInvokeLongPressEnd();
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            _TryInvokeLongPressEnd();
            Debug.Log("OnPointerExit");
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!longPressTriggered)
            {
                onClick.Invoke();
            }
        }

        private void _TryInvokeLongPressEnd()
        {
            isPointerDown = false;
            if (longPressTriggered)
            {
                Debug.Log("ON_LONGPRESS_END");
                onLongPressEnd.Invoke();
            }
        }

        protected override void OnDisable()
        {
            if (!isPointerDown)
            {
                return;
            }
            _TryInvokeLongPressEnd();
        }

        protected override void OnDestroy()
        {
            if (!isPointerDown)
            {
                return;
            }
            _TryInvokeLongPressEnd();
        }

    }
}

