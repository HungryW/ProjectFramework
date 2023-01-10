//using Defines;
//using GameFrameworkPackage;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;
//using Logic;
//using UnityEngine.Events;

//namespace GameFrameworkPackage
//{
//    public class CUIToolTipTxtListenerFlexible : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//    {
//        [SerializeField]
//        private List<Transform> ListTranPos;
//        [SerializeField]
//        private List<EPosDirect> ListEPivotDirect;
//        [SerializeField]
//        private float ContentWidth = 380;

//        private string m_szContent;
//        private string m_szTitle;
//        private UnityAction m_fnOnEnter;
//        private UnityAction m_fnOnExit;
//        private List<Vector3> m_listWorldPos;
//        private bool m_bNeedPointerHandler = true;

//        public void Awake()
//        {
//            m_listWorldPos = new List<Vector3>();
//        }

//        private void OnDestroy()
//        {
//            m_szContent = null;
//            m_szTitle = null;
//            m_fnOnEnter = null;
//            m_fnOnExit = null;
//            m_listWorldPos.Clear();
//            CUITipTxt.CloseTipInfo();
//        }

//        public void SetContent(string a_szContent)
//        {
//            m_szContent = a_szContent;
//        }

//        public void SetTitle(string a_sz)
//        {
//            m_szTitle = a_sz;
//        }

//        public void SetNeedPointerHandler(bool a_bNeedPointerHandler)
//        {
//            m_bNeedPointerHandler = a_bNeedPointerHandler;
//        }

//        public void SetEnterCallback(UnityAction a_fnOnEnter)
//        {
//            m_fnOnEnter = a_fnOnEnter;
//        }

//        public void SetExitCallback(UnityAction a_fnOnExit)
//        {
//            m_fnOnExit = a_fnOnExit;
//        }

//        public void OnPointerEnter(PointerEventData eventData)
//        {
//            if (!m_bNeedPointerHandler)
//            {
//                return;
//            }
//            if (m_fnOnEnter != null)
//            {
//                m_fnOnEnter.Invoke();
//            }
//            OpenTipTxt();
//        }

//        public void OnPointerExit(PointerEventData eventData)
//        {
//            if (!m_bNeedPointerHandler)
//            {
//                return;
//            }
//            if (m_fnOnExit != null)
//            {
//                m_fnOnExit.Invoke();
//            }
//            CloseTipTxt();
//        }

//        public void OpenTipTxt()
//        {
//            m_listWorldPos.Clear();
//            foreach (var item in ListTranPos)
//            {
//                m_listWorldPos.Add(item.position);
//            }
//            COpenUITipTxtParam param = new COpenUITipTxtParam(m_szContent, ContentWidth, ListEPivotDirect, m_listWorldPos);
//            param.m_szTitle = m_szTitle;
//            CUITipTxt.OpenTipInfo(param);
//        }

//        public void CloseTipTxt()
//        {
//            m_listWorldPos.Clear();
//            CUITipTxt.CloseTipInfo();
//        }
//    }

//}
