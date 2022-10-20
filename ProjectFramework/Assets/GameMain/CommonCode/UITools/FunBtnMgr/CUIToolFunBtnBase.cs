using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameFrameworkPackage;

namespace GameFrameworkPackage
{
    public class CUIParamToolFunBtnBase
    {
        public delegate bool fnCheck();

        public CUIParamToolFunBtnBase(int a_nId)
        {
            m_nId = a_nId;
            m_szName = "";
            m_fnClick = null;
            m_fnIsShow = null;
            m_fnHaveCorner = null;
        }

        private int m_nId;
        private string m_szName;
        private UnityAction m_fnClick;
        private fnCheck m_fnIsShow;
        private fnCheck m_fnHaveCorner;

        public CUIParamToolFunBtnBase SetName(string a_szName)
        {
            m_szName = a_szName;
            return this;
        }

        public CUIParamToolFunBtnBase SetClickFun(UnityAction a_fnOnClick)
        {
            m_fnClick = a_fnOnClick;
            return this;
        }

        public CUIParamToolFunBtnBase SetIsShowFun(fnCheck a_fnIsShow)
        {
            m_fnIsShow = a_fnIsShow;
            return this;
        }

        public CUIParamToolFunBtnBase SetHaveCornerFun(fnCheck a_fnHaveCorner)
        {
            m_fnHaveCorner = a_fnHaveCorner;
            return this;
        }

        public int GetId()
        {
            return m_nId;
        }

        public string GetName()
        {
            return CGameEntryMgr.Localization.GetString(m_szName);
        }

        public void InvokeClick()
        {
            if (m_fnClick == null)
            {
                return;
            }
            m_fnClick.Invoke();
        }

        public bool IsShow()
        {
            if (m_fnIsShow == null)
            {
                return false;
            }
            return m_fnIsShow.Invoke();
        }

        public bool HaveCorner()
        {
            if (m_fnHaveCorner == null)
            {
                return false;
            }
            return m_fnHaveCorner.Invoke();
        }
    }

    public class CUIToolFunBtnBase : MonoBehaviour
    {
        [SerializeField]
        private GameObject Go;
        [SerializeField]
        private Button Btn;
        [SerializeField]
        private Text LbTxt;
        [SerializeField]
        private GameObject GoCorner;

        private CUIParamToolFunBtnBase m_param;

        private void Start()
        {
            Btn.onClick.AddListener(_OnClick);
        }

        public virtual void Init(CUIParamToolFunBtnBase a_param)
        {
            m_param = a_param;
        }

        public virtual void UpdateShow()
        {
            LbTxt.text =  m_param.GetName();
            Go.SetActive(m_param.IsShow());
            GoCorner.SetActive(m_param.HaveCorner());
        }

        public int GetId()
        {
            return m_param.GetId();
        }

        public CUIParamToolFunBtnBase GetParam()
        {
            return m_param;
        }

        private void _OnClick()
        {
            m_param.InvokeClick();
        }
    }

}
