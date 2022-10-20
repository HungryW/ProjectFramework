using Defines;
using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFrameworkPackage
{
    public class COpenParamNotice
    {
        private string m_szContent;
        private UnityAction m_fnConfirm;

        public COpenParamNotice(string a_szContent)
        {
            m_szContent = a_szContent;
            m_fnConfirm = null;
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


        public void InvokeConfirm()
        {
            if (m_fnConfirm != null)
            {
                m_fnConfirm.Invoke();
            }
        }
    }

    public class CUINotice : CLogicUI
    {
        [SerializeField]
        private Button BtnCloseBg;
        [SerializeField]
        private Text LbContent;
        [SerializeField]
        private Button BtnConfirm;

        private COpenParamCommonConfirm m_Param;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            BtnConfirm.onClick.AddListener(_OnComfirmClick);
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
            LbContent.text = m_Param.GetContent();
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
    }
}
