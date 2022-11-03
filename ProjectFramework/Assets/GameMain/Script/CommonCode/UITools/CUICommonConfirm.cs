using Defines;
using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public class COpenParamCommonConfirm
    {
        private string m_szContent;
        private UnityAction m_fnConfirm;
        private UnityAction m_fnCancel;
        private bool m_bShowCancel;
        private string m_sCancelBtnName;
        private string m_sConfirmBtnName;

        public COpenParamCommonConfirm(string a_szContent)
        {
            m_szContent = a_szContent;
            m_fnCancel = null;
            m_fnConfirm = null;
            m_bShowCancel = true;
            m_sCancelBtnName = "";
            m_sConfirmBtnName = "";
        }

        public string GetContent()
        {
            return m_szContent;
        }

        public void SetContent(string a_szContent)
        {
            m_szContent = a_szContent;
        }

        public void SetConfirmCallback(UnityAction a_fnConfirmCallback)
        {
            m_fnConfirm = a_fnConfirmCallback;
        }

        public void SetCancelCallback(UnityAction a_fnCancelCallback)
        {
            m_fnCancel = a_fnCancelCallback;
        }

        public void SetShowCancel(bool a_bIsShow)
        {
            m_bShowCancel = a_bIsShow;
        }

        public void InvokeConfirm()
        {
            if (m_fnConfirm != null)
            {
                m_fnConfirm.Invoke();
            }
        }

        public void InvokeCancel()
        {
            if (m_fnCancel != null)
            {
                m_fnCancel.Invoke();
            }
        }

        public bool IsShowCancel()
        {
            return m_bShowCancel;
        }

        public void SetCancelBtnName(string a_sName)
        {
            m_sCancelBtnName = a_sName;
        }
        public string GetCancelBtnName()
        {
            return m_sCancelBtnName;
        }
        public void SetConfirmBtnName(string a_sName)
        {
            m_sConfirmBtnName = a_sName;
        }
        public string GetConfirmBtnName()
        {
            return m_sConfirmBtnName;
        }
    }

    public class CUICommonConfirm : CLogicUI
    {
        [SerializeField]
        private Button BtnCloseBg;
        [SerializeField]
        private Button BtnCancel;
        [SerializeField]
        private Text LbContent;
        [SerializeField]
        private Button BtnConfirm;
        [SerializeField]
        private Button BtnClose;
        [SerializeField]
        private Animator GoContent;
        [SerializeField]
        private Text LbComfirm;
        [SerializeField]
        private Text LbCancel;

        private COpenParamCommonConfirm m_Param;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            BtnConfirm.onClick.AddListener(_OnComfirmClick);
            BtnCancel.onClick.AddListener(_OnCancelClick);
            BtnClose.onClick.AddListener(_OnCloseClick);
            BtnCloseBg.onClick.AddListener(_OnCloseClick);
        }

        protected override void OnUpdateShow(object userData)
        {
            base.OnUpdateShow(userData);
            _InitData(userData);
            _InitShow();
        }

        private void _InitData(object userData)
        {
            m_Param = userData as COpenParamCommonConfirm;
        }

        private void _InitShow()
        {
            GoContent.SetTrigger("Show");
            BtnCancel.gameObject.SetActive(m_Param.IsShowCancel());
            LbContent.text = m_Param.GetContent();
            LbComfirm.text = string.IsNullOrEmpty(m_Param.GetConfirmBtnName()) ? CGameEntryMgr.Localization.GetStringEX("confirm") : m_Param.GetConfirmBtnName();
            LbCancel.text = string.IsNullOrEmpty(m_Param.GetCancelBtnName()) ? CGameEntryMgr.Localization.GetStringEX("cancel") : m_Param.GetCancelBtnName();
        }

        private void _OnComfirmClick()
        {
            Close();
            if (null != m_Param)
            {
                m_Param.InvokeConfirm();
            }
        }

        private void _OnCancelClick()
        {
            Close();
            if (null != m_Param)
            {
                m_Param.InvokeCancel();
            }
        }

        private void _OnCloseClick()
        {
            GoContent.SetTrigger("Hide");
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(GoContent.GetClipLength("HideWindow"))
                .AppendCallback(Close);

        }

        public static void TipConfirm(string a_szContent, UnityAction a_fnOnConfirm = null, UnityAction a_fnCanCel = null)
        {
            COpenParamCommonConfirm parma = new COpenParamCommonConfirm(a_szContent);
            parma.SetConfirmCallback(a_fnOnConfirm);
            parma.SetCancelCallback(a_fnCanCel);
            CGameEntryMgr.UI.OpenUIForm(EUIFormID.UICommonConfirm, parma);
        }
    }
}
