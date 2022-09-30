using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameFramework
{
    public class CUIButtonAnim : UIBehaviour,IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private Animator m_Animator;

        public void OnPointerDown(PointerEventData eventData)
        {
            m_Animator.SetBool("Down", true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            m_Animator.SetBool("Down", false);
        }
    }
}
