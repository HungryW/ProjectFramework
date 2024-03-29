﻿using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
using Defines;

namespace HotFixEntry.UI
{
    public class CUICommonConfirm : CUIBase
    {
        private GameObject GoContent;
        private Button BtnClose;
        private Image ImgBg;
        private Text LbContent;
        private Button BtnConfirm;
        private Text LbConfirm;
        private Button BtnCancel;
        private Text LbCancel;

        private COpenParamCommonConfirm m_Param;
        protected override void _InitComponents()
        {
            GoContent = transform.Find("GoContent").gameObject;
            BtnClose = transform.Find("GoContent/BtnClose").GetComponent<Button>();
            ImgBg = transform.Find("GoContent/ImgBg").GetComponent<Image>();
            LbContent = transform.Find("GoContent/LbContent").GetComponent<Text>();
            BtnConfirm = transform.Find("GoContent/BtnConfirm").GetComponent<Button>();
            LbConfirm = transform.Find("GoContent/BtnConfirm/LbConfirm").GetComponent<Text>();
            BtnCancel = transform.Find("GoContent/BtnCancel").GetComponent<Button>();
            LbCancel = transform.Find("GoContent/BtnCancel/LbCancel").GetComponent<Text>();
        }


        protected override void _OnInit(object userData)
        {
            base._OnInit(userData);

            BtnConfirm.onClick.AddListener(_OnComfirmClick);
            BtnCancel.onClick.AddListener(_OnCancelClick);
            BtnClose.onClick.AddListener(_OnCloseClick);
        }

        protected override void _OnOpenShow(object userData)
        {
            base._OnOpenShow(userData);
            _InitData(userData);
            _InitShow();
        }

        private void _InitData(object userData)
        {
            m_Param = userData as COpenParamCommonConfirm;
        }

        private void _InitShow()
        {
            BtnCancel.gameObject.SetActive(m_Param.IsShowCancel());
            LbContent.text = m_Param.GetContent();
            LbConfirm.text = string.IsNullOrEmpty(m_Param.GetConfirmBtnName()) ? CGameEntryMgr.Localization.GetStringEX("confirm") : m_Param.GetConfirmBtnName();
            LbCancel.text = string.IsNullOrEmpty(m_Param.GetCancelBtnName()) ? CGameEntryMgr.Localization.GetStringEX("cancel") : m_Param.GetCancelBtnName();
        }

        private void _OnComfirmClick()
        {
            _Close();
            if (null != m_Param)
            {
                m_Param.InvokeConfirm();
            }
        }

        private void _OnCancelClick()
        {
            _Close();
            if (null != m_Param)
            {
                m_Param.InvokeCancel();
            }
        }

        private void _OnCloseClick()
        {
            _OnCancelClick();
        }

        public static void TipConfirm(string a_szContent, UnityAction a_fnOnConfirm = null, UnityAction a_fnCanCel = null)
        {
            COpenParamCommonConfirm param = new COpenParamCommonConfirm(a_szContent);
            param.SetConfirmCallback(a_fnOnConfirm);
            param.SetCancelCallback(a_fnCanCel);
            CHotFixEntry.UI.OpenUINoHideAnim(EUIFormID.UICommonConfirm, param);
        }
    }
}
