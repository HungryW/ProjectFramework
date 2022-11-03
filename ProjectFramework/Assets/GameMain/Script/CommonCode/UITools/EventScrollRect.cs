using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameFrameworkPackage
{
    public class EventScrollRect : ScrollRect
    {
        public UnityEvent onBeginDrag = new UnityEvent();
        public UnityEvent onDrag = new UnityEvent();
        public UnityEvent onEndDrag = new UnityEvent();
        public UnityEvent onScroll = new UnityEvent();
        public UnityEvent onStopMovement = new UnityEvent();

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            onBeginDrag.Invoke();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            onDrag.Invoke();
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            onEndDrag.Invoke();
        }

        public override void OnScroll(PointerEventData data)
        {
            base.OnScroll(data);
            onScroll.Invoke();
        }

        public override void StopMovement()
        {
            base.StopMovement();
            onStopMovement.Invoke();
        }

    }
}
