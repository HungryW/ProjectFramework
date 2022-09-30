using Defines;
using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic;
using UnityEngine.Events;

namespace GameFrameworkPackage
{
    public class CUIToolTipTxtListener : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Transform TranPos;
        [SerializeField]
        private EPosDirect EPivotDirect;
        [SerializeField]
        private float ContentWidth = 380;

        private string m_szContent;
        private string m_szTitle;
        private UnityAction m_fnOnEnter;
        private UnityAction m_fnOnExit;

        public void SetContent(string a_szContent)
        {
            m_szContent = a_szContent;
        }

        public void SetTitle(string a_szTitle)
        {
            m_szTitle = a_szTitle;
        }

        public void SetEnterCallback(UnityAction a_fnOnEnter)
        {
            m_fnOnEnter = a_fnOnEnter;
        }

        public void SetExitCallback(UnityAction a_fnOnExit)
        {
            m_fnOnExit = a_fnOnExit;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_fnOnEnter != null)
            {
                m_fnOnEnter.Invoke();
            }
            COpenUITipTxtParam param = new COpenUITipTxtParam(m_szContent, EPivotDirect, TranPos.position, ContentWidth);
            param.m_szTitle = m_szTitle;
            CUITipTxt.OpenTipInfo(param);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (m_fnOnExit != null)
            {
                m_fnOnExit.Invoke();
            }
            CUITipTxt.CloseTipInfo();
        }

        private void OnDestroy()
        {
            CUITipTxt.CloseTipInfo();
        }
    }

}
