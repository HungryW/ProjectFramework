using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
