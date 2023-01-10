// using GameFrameworkPackage;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Events;
// using UnityEngine.UI;
// using DG.Tweening;
// using HotFixEntry;
// using Defines;
// 
// namespace HotFixLogic.UI
// {
//     public class CUICommonConfirm : CHotFixUIBase
//     {
//         private GameObject GoContent;
//         private Button BtnClose;
//         private Image ImgBg;
//         private Text LbContent;
//         private Button BtnConfirm;
//         private Text LbConfirm;
//         private Button BtnCancel;
//         private Text LbCancel;
//         protected override void _InitComponents()
//         {
//             GoContent = transform.Find("GoContent").gameObject;
//             BtnClose = transform.Find("GoContent/BtnClose").GetComponent<Button>();
//             ImgBg = transform.Find("GoContent/ImgBg").GetComponent<Image>();
//             LbContent = transform.Find("GoContent/LbContent").GetComponent<Text>();
//             BtnConfirm = transform.Find("GoContent/BtnConfirm").GetComponent<Button>();
//             LbConfirm = transform.Find("GoContent/BtnConfirm/LbConfirm").GetComponent<Text>();
//             BtnCancel = transform.Find("GoContent/BtnCancel").GetComponent<Button>();
//             LbCancel = transform.Find("GoContent/BtnCancel/LbCancel").GetComponent<Text>();
//         }
// 
// 
// 
//         private COpenParamCommonConfirm m_Param;
//         protected override void OnInit(object userData)
//         {
//             base.OnInit(userData);
// 
//             BtnConfirm.onClick.AddListener(_OnComfirmClick);
//             BtnCancel.onClick.AddListener(_OnCancelClick);
//             BtnClose.onClick.AddListener(_OnCloseClick);
//         }
// 
//         protected override void OnUpdateShow(object userData)
//         {
//             base.OnUpdateShow(userData);
//             _InitData(userData);
//             _InitShow();
//         }
// 
//         private void _InitData(object userData)
//         {
//             m_Param = userData as COpenParamCommonConfirm;
//         }
// 
//         private void _InitShow()
//         {
//             BtnCancel.gameObject.SetActive(m_Param.IsShowCancel());
//             LbContent.text = m_Param.GetContent();
//             LbConfirm.text = string.IsNullOrEmpty(m_Param.GetConfirmBtnName()) ? CGameEntryMgr.Localization.GetStringEX("confirm") : m_Param.GetConfirmBtnName();
//             LbCancel.text = string.IsNullOrEmpty(m_Param.GetCancelBtnName()) ? CGameEntryMgr.Localization.GetStringEX("cancel") : m_Param.GetCancelBtnName();
//         }
// 
//         private void _OnComfirmClick()
//         {
//             Close();
//             if (null != m_Param)
//             {
//                 m_Param.InvokeConfirm();
//             }
//         }
// 
//         private void _OnCancelClick()
//         {
//             Close();
//             if (null != m_Param)
//             {
//                 m_Param.InvokeCancel();
//             }
//         }
// 
//         private void _OnCloseClick()
//         {
//             _OnCancelClick();
//         }
// 
//         public static void TipConfirm(string a_szContent, UnityAction a_fnOnConfirm = null, UnityAction a_fnCanCel = null)
//         {
//             COpenParamCommonConfirm parma = new COpenParamCommonConfirm(a_szContent);
//             parma.SetConfirmCallback(a_fnOnConfirm);
//             parma.SetCancelCallback(a_fnCanCel);
//             CGameEntryMgr.UI.OpenUIForm(EUIFormID.UICommonConfirm, parma);
//         }
//     }
// }
