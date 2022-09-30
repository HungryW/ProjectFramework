using Defines;
using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFrameworkPackage
{
    public class CUICommonConfirmHaveCheckBox : CLogicUI
    {
        [SerializeField]
        private Button BtnCloseBg;
        [SerializeField]
        private Button BtnCancel;
        [SerializeField]
        private Text LbContent;
        [SerializeField]
        private Text LbCheckTip;
        [SerializeField]
        private Button BtnConfirm;
        [SerializeField]
        private Button BtnCheckBox;
        [SerializeField]
        private GameObject GoSelect;

        private bool m_bIsSelect;

        public void ShowConfirm(string a_szContent, string a_szCheckTip, bool a_bCheck, UnityAction a_fnOnCheck, UnityAction a_fnOnConfirm = null, UnityAction a_fnCanCel = null)
        {
            m_bIsSelect = a_bCheck;

            BtnConfirm.onClick.RemoveAllListeners();
            BtnCancel.onClick.RemoveAllListeners();
            BtnCheckBox.onClick.RemoveAllListeners();

            LbContent.text = a_szContent;
            LbCheckTip.text = a_szCheckTip;
            GoSelect.SetActive(m_bIsSelect);

            BtnCheckBox.onClick.AddListener(() =>
            {
                m_bIsSelect = !m_bIsSelect;
                a_fnOnCheck.Invoke();
                GoSelect.SetActive(m_bIsSelect);
            });

            BtnConfirm.onClick.AddListener(() =>
            {
                if( a_fnOnConfirm != null)
                {
                    a_fnOnConfirm.Invoke();
                }
                Close();
            });

            BtnCancel.onClick.AddListener(() =>
            {
                if (a_fnCanCel != null)
                {
                    a_fnCanCel.Invoke();
                }
                Close();
            });
           
        }

        public static void TipConfirm(string a_szContent, string a_szCheckTip, bool a_bCheck, UnityAction a_fnOnCheck, UnityAction a_fnOnConfirm = null, UnityAction a_fnCanCel = null)
        {
            if (!CGameEntryMgr.UI.GetUIFormVisible(EUIFormID.UICommonConfirmHaveCheckBox))
            {
                CGameEntryMgr.UI.OpenUIForm(EUIFormID.UICommonConfirmHaveCheckBox);
            }

           (CGameEntryMgr.UI.GetUIForm(EUIFormID.UICommonConfirmHaveCheckBox) as CUICommonConfirmHaveCheckBox).ShowConfirm(a_szContent, a_szCheckTip, a_bCheck, a_fnOnCheck, a_fnOnConfirm, a_fnCanCel);
        }
    }
}
