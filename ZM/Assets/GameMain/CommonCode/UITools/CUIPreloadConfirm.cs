using Defines;
using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public class CUIPreloadConfirm : MonoBehaviour
    {
        private GameObject GoContent;
        private Button BtnBgClose;
        private Button BtnClose;
        private Text LbContent;
        private Button BtnCancel;
        private Text LbCancel;
        private Button BtnConfirm;
        private Text LbComfirm;

        private COpenParamCommonConfirm m_Param;

        private void Awake()
        {
            GoContent = transform.Find("GoContent").gameObject;
            BtnBgClose = transform.Find("GoContent/BgMin/BtnBgClose").GetComponent<Button>();
            BtnClose = transform.Find("GoContent/BgMin/BtnClose").GetComponent<Button>();
            LbContent = transform.Find("GoContent/LbContent").GetComponent<Text>();
            BtnCancel = transform.Find("GoContent/GoFun/BtnCancel").GetComponent<Button>();
            LbCancel = transform.Find("GoContent/GoFun/BtnCancel/Lb_WhiteRed").GetComponent<Text>();
            BtnConfirm = transform.Find("GoContent/GoFun/BtnConfirm").GetComponent<Button>();
            LbComfirm = transform.Find("GoContent/GoFun/BtnConfirm/Lb_WhiteGreen").GetComponent<Text>();

            BtnConfirm.onClick.AddListener(_OnComfirmClick);
            BtnCancel.onClick.AddListener(_OnCancelClick);
            BtnClose.onClick.AddListener(_OnCancelClick);
            BtnBgClose.onClick.AddListener(_OnCancelClick);
        }

        public void Open(COpenParamCommonConfirm userData)
        {
            gameObject.SetActive(true);
            _InitData(userData);
            _InitShow();
        }

        private void _InitData(COpenParamCommonConfirm userData)
        {
            m_Param = userData;
        }

        private void _InitShow()
        {
            BtnCancel.gameObject.SetActive(m_Param.IsShowCancel());
            LbContent.text = m_Param.GetContent();
            LbComfirm.text = string.IsNullOrEmpty(m_Param.GetConfirmBtnName()) ? CGameEntryMgr.Localization.GetStringEX("confirm") : m_Param.GetConfirmBtnName();
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

        private void _Close()
        {
            gameObject.SetActive(false);
        }

        private static CUIPreloadConfirm ms_ui;

        public static void Tip(COpenParamCommonConfirm param)
        {
            CUIPreloadConfirm ui = _CreateUI();
            ui.Open(param);
        }

        private static CUIPreloadConfirm _CreateUI()
        {
            if (ms_ui != null)
            {
                return ms_ui;
            }
            GameObject perfabUI = Resources.Load<GameObject>("PreloadRes/UI/UIConfirm");
            GameObject ui = GameObject.Instantiate(perfabUI, CGameEntryMgr.PreloadComponent.transform);
            ms_ui = ui.GetOrAddComponent<CUIPreloadConfirm>();
            return ms_ui;
        }

        public static void TipNetErrorRestart()
        {
            COpenParamCommonConfirm param = new COpenParamCommonConfirm(CGameEntryMgr.Localization.GetStringEX("PreloadTipNetErrorRestart"));
            param.SetConfirmBtnName(CGameEntryMgr.Localization.GetStringEX("PreloadRestartImmediately"));
            param.SetConfirmCallback(() =>
            {
                GameEntry.Shutdown(ShutdownType.Restart);
            });
            param.SetCancelCallback(() =>
            {
                GameEntry.Shutdown(ShutdownType.Restart);
            });
            param.SetShowCancel(false);
            Tip(param);
        }
    }
}
