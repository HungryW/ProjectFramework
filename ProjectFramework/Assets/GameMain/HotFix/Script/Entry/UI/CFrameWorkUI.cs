using System.Collections;
using System.Collections.Generic;
using Defines;
using GameFramework;
using GameFramework.UI;
using GameFrameworkPackage;
using UnityGameFramework.Runtime;

namespace HotFixEntry
{
    public class CFrameWorkUI : CFrameworkBase
    {
        private UIComponent refUIComponent
        {
            get;
            set;
        }

        public CFrameWorkUI() : base()
        {
            refUIComponent = CGameEntryMgr.UI;
        }

        public override void Clean()
        {
            refUIComponent = null;
        }

        public void CloseUIForm(EUIFormID a_eUIId)
        {
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>((int)a_eUIId);
            if (null == drUI)
            {
                Log.Warning("CloseUIForm Fail Id ='{0}' ", a_eUIId);
                return;
            }
            string szAssetName = drUI.GetAssetFullName();
            if (refUIComponent.IsLoadingUIForm(szAssetName))
            {
                return;
            }
            CHotFixLogicUI uiLogic = _GetOpenedFrameworkLogicUI(a_eUIId);
            if (uiLogic == null)
            {
                return;
            }
            refUIComponent.CloseUIForm(uiLogic);
        }

        public void CloseAllUIByGroupName(string a_szGroupName)
        {
            refUIComponent.CloseAllUIByGroupName(a_szGroupName);
        }

        public CUIBase GetTopUIFormInGroup(string a_szGroupName)
        {
            IUIGroup uiGroup = refUIComponent.GetUIGroup(a_szGroupName);
            if (uiGroup == null)
            {
                return null;
            }
            IUIForm topUI = uiGroup.CurrentUIForm;
            if (null == topUI || !(topUI is UIForm))
            {
                return null;
            }
            CHotFixLogicUI topUILogic = ((UIForm)topUI).Logic as CHotFixLogicUI;
            if (null == topUILogic)
            {
                Log.Warning("GetTopUIFormInGroup Fail a_szGroupName ='{0}' is Not HotFix UI", a_szGroupName);
                return null;
            }
            return (CUIBase)topUILogic.m_AgentHotFixUI;
        }

        public bool CheckUIVisible(EUIFormID a_eUIID)
        {
            CHotFixLogicUI uiLogic = _GetOpenedFrameworkLogicUI(a_eUIID);
            if (null == uiLogic)
            {
                return false;
            }
            return uiLogic.Visible;
        }
        public bool CheckUIOpened(EUIFormID a_eUIID)
        {
            return _GetOpenedFrameworkLogicUI(a_eUIID) != null;
        }
        public void OpenUINoHideAnim(EUIFormID a_eUIID, object a_oUserData = null)
        {
            _OpenUIWithAnim(a_eUIID, a_eUIID, a_oUserData, false);
        }
        public void OpenUI(EUIFormID a_eUIID, object a_oUserData = null)
        {
            //默认隐藏group内最上层的UI
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>((int)a_eUIID);
            if (null == drUI)
            {
                Log.Warning("OpenUI Fail Id ='{0}' ", a_eUIID);
                return;
            }
            CUIBase uiTopLogic = GetTopUIFormInGroup(drUI.GroupName);
            bool bNeedHideAnim = uiTopLogic != null;
            _OpenUIWithAnim((EUIFormID)uiTopLogic.GetConfigId(), a_eUIID, a_oUserData, bNeedHideAnim);
        }
        private void _OpenUIWithAnim(EUIFormID a_eCurHideFormId, EUIFormID a_eNextOpenFormId, object a_oUserData, bool a_bCurUINeedHideAnim)
        {
            if (!a_bCurUINeedHideAnim)
            {
                _OpenUIForm(a_eNextOpenFormId, a_oUserData);
                return;
            }

            CUIBase uiHide = _GetOpenedUIBase(a_eCurHideFormId);
            if (null == uiHide)
            {
                Log.Warning("UI_NOT_OPEN:{0}", a_eCurHideFormId);
                _OpenUIForm(a_eNextOpenFormId, a_oUserData);
                return;
            }
            uiHide.StartPlayHideAnim(() =>
            {
                _OpenUIForm(a_eNextOpenFormId, a_oUserData);
            });
        }

        private int? _OpenUIForm(EUIFormID a_eUIID, object a_oUserData = null)
        {
            int a_nUIId = (int)a_eUIID;
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>(a_nUIId);
            if (null == drUI)
            {
                Log.Warning("_OpenUIForm Fail Id ='{0}' ", a_nUIId);
                return null;
            }
            string szAssetName = drUI.GetAssetFullName();
            if (!drUI.AllowMultiInstance)
            {
                if (refUIComponent.IsLoadingUIForm(szAssetName))
                {
                    return null;
                }

                if (refUIComponent.HasUIForm(szAssetName))
                {
                    return null;
                }
            }
            COpenUIParam param = COpenUIParam.Create(a_nUIId, a_oUserData);
            return refUIComponent.OpenHotFixUIForm(szAssetName, drUI.GroupName, drUI.PauseCoveredUIForm, drUI.DllName, drUI.GetClassName(), param);
  
        }

        private CUIBase _GetOpenedUIBase(EUIFormID a_eUIID)
        {
            CHotFixLogicUI uiHotFix = _GetOpenedFrameworkLogicUI(a_eUIID);
            if (null == uiHotFix)
            {
                Log.Warning("_GetUI Fail Id ='{0}', is not hotFix UI ", a_eUIID);
                return null;
            }
            return (CUIBase)uiHotFix.m_AgentHotFixUI;
        }
        private CHotFixLogicUI _GetOpenedFrameworkLogicUI(EUIFormID a_eUIID)
        {
            int a_nUIId = (int)a_eUIID;
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>(a_nUIId);
            if (null == drUI)
            {
                Log.Warning("_GetUI Fail Id ='{0}' ", a_nUIId);
                return null;
            }
            string szAssetName = drUI.GetAssetFullName();
            IUIGroup uiGroup = refUIComponent.GetUIGroup(drUI.GroupName);
            if (null == uiGroup)
            {
                Log.Warning("_GetUI Fail null == uiGroup Id ='{0}' ", a_nUIId);
                return null;
            }

            UIForm ui = (UIForm)uiGroup.GetUIForm(szAssetName);
            if (null == ui)
            {
                return null;
            }
            return (CHotFixLogicUI)ui.Logic;
        }
    }
}

