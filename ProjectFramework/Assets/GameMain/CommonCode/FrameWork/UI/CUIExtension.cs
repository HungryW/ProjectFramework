using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.DataTable;
using Defines;
using Defines.DataTable;
using GameFramework.UI;
using UnityEngine.UI;
using DG.Tweening;

namespace GameFrameworkPackage
{
    public static class CUIExtension
    {
        #region 表现扩展
        public static void SetSizeWidth(this RectTransform a_thisTransform, float a_fWidth)
        {
            Vector2 v2Size = a_thisTransform.sizeDelta;
            v2Size.x = a_fWidth;
            a_thisTransform.sizeDelta = v2Size;
        }

        public static void SetSizeHeight(this RectTransform a_thisTransform, float a_fHeight)
        {
            Vector2 v2Size = a_thisTransform.sizeDelta;
            v2Size.y = a_fHeight;
            a_thisTransform.sizeDelta = v2Size;
        }

        public static float GetSizeWidth(this Text a_lb)
        {
            return (a_lb.transform as RectTransform).sizeDelta.x;
        }

        public static float GetSizeHeight(this Text a_lb)
        {
            return (a_lb.transform as RectTransform).sizeDelta.y;
        }

        public static void FitSizeCenter(this Text a_lb, string a_szContent, float a_fMaxWidth)
        {
            a_lb.FitHorizontalSize(a_szContent);
            if (a_lb.GetSizeWidth() > a_fMaxWidth)
            {
                (a_lb.transform as RectTransform).SetSizeWidth(a_fMaxWidth);
            }
            a_lb.FitVerticalSize(a_szContent);
        }

        public static void FitHorizontalSize(this Text a_lb, string a_szContent)
        {
            a_lb.gameObject.SetActive(true);
            a_lb.gameObject.GetOrAddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            a_lb.text = a_szContent;
            LayoutRebuilder.ForceRebuildLayoutImmediate(a_lb.transform as RectTransform);
            a_lb.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            a_lb.text = "";
        }

        public static void FitVerticalSize(this Text a_lb, string a_szContent)
        {
            a_lb.gameObject.SetActive(true);
            a_lb.gameObject.GetOrAddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            a_lb.text = a_szContent;
            LayoutRebuilder.ForceRebuildLayoutImmediate(a_lb.transform as RectTransform);
            a_lb.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.Unconstrained;
            a_lb.text = "";
        }

        public static void DoTextFitHorizontal(this Text a_lb, string a_szContent, float a_fTime)
        {
            a_lb.FitHorizontalSize(a_szContent);
            a_lb.DOText(a_szContent, a_fTime).SetEase(Ease.Linear);
        }

        public static void DoTextFitVertical(this Text a_lb, string a_szContent, float a_fTime)
        {
            a_lb.FitVerticalSize(a_szContent);
            a_lb.DOText(a_szContent, a_fTime).SetEase(Ease.Linear);
        }
        #endregion

        public static IEnumerator FadeToAlpha(this CanvasGroup a_canvasGroup, float a_fAlpha, float a_fDuration)
        {
            float fTime = 0f;
            float fStartA = a_canvasGroup.alpha;
            while (fTime < a_fDuration)
            {
                fTime += Time.deltaTime;
                a_canvasGroup.alpha = Mathf.Lerp(fStartA, a_fAlpha, fTime / a_fDuration);
                yield return new WaitForEndOfFrame();
            }

            a_canvasGroup.alpha = a_fAlpha;
        }

        public static void DestoryAllChildren(this Transform a_tranParent)
        {
            foreach (Transform item in a_tranParent)
            {
                GameObject.Destroy(item.gameObject);
            }
            a_tranParent.DetachChildren();
        }

        public static void ActiveAllChildren(this Transform a_tranParent, bool a_bIsActive)
        {
            foreach (Transform item in a_tranParent)
            {
                item.gameObject.SetActive(a_bIsActive);
            }
        }

        public static void SetUIInstanceLocked(this UIComponent a_uiComponet, CLogicUI a_uiLogic, bool a_bLocked)
        {
            a_uiComponet.SetUIFormInstanceLocked(a_uiLogic.UIForm, a_bLocked);
        }

        public static bool HasUIForm(this UIComponent a_uiComponent, EUIFormID a_eUIId, string a_szGroupName = null)
        {
            return HasUIForm(a_uiComponent, (int)a_eUIId, a_szGroupName);
        }

        public static bool HasUIForm(this UIComponent a_uiComponent, int a_nUIId, string a_szGroupName = null)
        {
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>(a_nUIId);
            if (drUI == null)
            {
                return false;
            }

            string szAssetName = CAssestPathUtility.GetUIFormAsset(drUI.AssetName);
            if (string.IsNullOrEmpty(a_szGroupName))
            {
                return a_uiComponent.HasUIForm(szAssetName);
            }

            IUIGroup uiGroup = a_uiComponent.GetUIGroup(a_szGroupName);
            if (uiGroup == null)
            {
                return false;
            }

            return uiGroup.HasUIForm(szAssetName);
        }

        public static bool GetUIFormVisible(this UIComponent a_uiComponent, EUIFormID a_eUIId, string a_szGroupName = null)
        {
            CLogicUI uiLogic = GetUIForm(a_uiComponent, a_eUIId, a_szGroupName);
            if (null == uiLogic)
            {
                return false;
            }

            return uiLogic.Visible;
        }

        public static void RefocusUI(this UIComponent a_uiComponent, EUIFormID a_eUIId)
        {
            UIForm UIForm = a_uiComponent.GetForm(a_eUIId);
            if (null == UIForm)
            {
                return;
            }
            ((CLogicUI)UIForm.Logic).BeforeRefocus();
            a_uiComponent.RefocusUIForm(UIForm);
        }

        public static void ReLosefocusUI(this UIComponent a_uiComponent, EUIFormID a_eUIId)
        {
            CLogicUI UIForm = a_uiComponent.GetUIForm(a_eUIId);
            if (null == UIForm)
            {
                return;
            }
            UIForm.ReLoseFocus();
        }

        public static UIForm GetForm(this UIComponent a_uiComponent, EUIFormID a_nUIId, string a_szGroupName = null)
        {
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>((int)a_nUIId);
            if (null == drUI)
            {
                return null;
            }

            string szAssetName = CAssestPathUtility.GetUIFormAsset(drUI.AssetName);
            UIForm ui = null;
            if (string.IsNullOrEmpty(a_szGroupName))
            {
                ui = a_uiComponent.GetUIForm(szAssetName);
                return ui;
            }

            IUIGroup uiGroup = a_uiComponent.GetUIGroup(a_szGroupName);
            if (null == uiGroup)
            {
                return null;
            }

            ui = (UIForm)uiGroup.GetUIForm(szAssetName);
            return ui;
        }

        public static CLogicUI GetUIForm(this UIComponent a_uiComponent, EUIFormID a_eUIId, string a_szGroupName = null)
        {
            return GetUIForm(a_uiComponent, (int)a_eUIId, a_szGroupName);
        }

        public static CLogicUI GetUIForm(this UIComponent a_uiComponent, int a_nUIId, string a_szGroupName = null)
        {
            if (CGameEntryMgr.DataTable.GetDataTable<DRUIForm>() == null)
            {
                return null;
            }
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>(a_nUIId);
            if (null == drUI)
            {
                return null;
            }

            string szAssetName = CAssestPathUtility.GetUIFormAsset(drUI.AssetName);
            UIForm ui = null;
            if (string.IsNullOrEmpty(a_szGroupName))
            {
                ui = a_uiComponent.GetUIForm(szAssetName);
                if (ui == null)
                {
                    return null;
                }

                return (CLogicUI)ui.Logic;
            }

            IUIGroup uiGroup = a_uiComponent.GetUIGroup(a_szGroupName);
            if (null == uiGroup)
            {
                return null;
            }

            ui = (UIForm)uiGroup.GetUIForm(szAssetName);
            if (null == ui)
            {
                return null;
            }

            return (CLogicUI)ui.Logic;
        }

        public static void CloseUIForm(this UIComponent a_uiComponent, CLogicUI a_uiLogic)
        {
            a_uiComponent.CloseUIForm(a_uiLogic.UIForm);
        }

        public static void CloseUIForm(this UIComponent a_uiComponent, EUIFormID a_eUIId)
        {
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>((int)a_eUIId);
            if (null == drUI)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", (int)a_eUIId);
                return;
            }
            string szAssetName = CAssestPathUtility.GetUIFormAsset(drUI.AssetName);
            if (a_uiComponent.IsLoadingUIForm(szAssetName))
            {
                return;
            }
            if (!a_uiComponent.HasUIForm(szAssetName))
            {
                return;
            }
            CloseUIForm(a_uiComponent, GetUIForm(a_uiComponent, a_eUIId));
        }

        public static int? OpenUIForm(this UIComponent a_uiComponent, EUIFormID a_eUIID, object a_oUserData = null)
        {
            return OpenUIForm(a_uiComponent, (int)a_eUIID, a_oUserData);
        }

        public static int? OpenUIForm(this UIComponent a_uiComponent, int a_nUIId, object a_oUserData = null)
        {
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>(a_nUIId);
            if (null == drUI)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", a_nUIId);
                return null;
            }
            string szAssetName = CAssestPathUtility.GetUIFormAsset(drUI.AssetName);
            if (!drUI.AllowMultiInstance)
            {
                if (a_uiComponent.IsLoadingUIForm(szAssetName))
                {
                    return null;
                }

                if (a_uiComponent.HasUIForm(szAssetName))
                {
                    return null;
                }
            }

            return a_uiComponent.OpenUIForm(szAssetName, a_nUIId, true, a_oUserData);
        }

        public static int? OpenHotUIForm(this UIComponent a_uiComponent, EUIFormID a_eUIID, object a_oUserData = null, bool a_bPlayOpenAnim = true)
        {
            return OpenHotUIForm(a_uiComponent, (int)a_eUIID, a_oUserData, a_bPlayOpenAnim);
        }

        public static int? OpenHotUIForm(this UIComponent a_uiComponent, int a_nUIId, object a_oUserData = null, bool a_bPlayOpenAnim = true)
        {
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>(a_nUIId);
            if (null == drUI)
            {
                Log.Warning("Can not load UI form '{0}' from data table.", a_nUIId);
                return null;
            }
            string szAssetName = CAssestPathUtility.GetUIFormAsset(drUI.AssetName);
            if (!drUI.AllowMultiInstance)
            {
                if (a_uiComponent.IsLoadingUIForm(szAssetName))
                {
                    return null;
                }

                if (a_uiComponent.HasUIForm(szAssetName))
                {
                    return null;
                }
            }
            return a_uiComponent.OpenUIForm(szAssetName, a_nUIId, a_bPlayOpenAnim, a_oUserData);
        }

        private static int? OpenUIForm(this UIComponent a_uiComponent, string a_szAssetName, int a_nConfigId, bool a_bPlayOpenAnim, object a_oUserData)
        {
            DRUIForm drUI = CGameEntryMgr.DataTable.GetDataRow<DRUIForm>(a_nConfigId);
            CUIPackageUserData uiData = new CUIPackageUserData(a_nConfigId, drUI.AssetName, drUI.NeedBlurBg, a_bPlayOpenAnim, a_oUserData);
            return a_uiComponent.OpenUIForm(a_szAssetName, drUI.GroupName, CConstAssetPriority.UIFormAsset, drUI.PauseCoveredUIForm, uiData);
        }

        public static void CloseAllUIByGroupName(this UIComponent a_uiComponent, string a_szGroupName)
        {
            IUIGroup uiGroup = a_uiComponent.GetUIGroup(a_szGroupName);
            if (null == uiGroup)
            {
                return;
            }

            IUIForm[] arrUIForm = uiGroup.GetAllUIForms();
            for (int i = 0; i < arrUIForm.Length; i++)
            {
                a_uiComponent.CloseUIForm((UIForm)arrUIForm[i]);
            }
        }

        #region
        public static void ShowTip(this UIComponent uiComponent, string tip)
        {
            CUICommonTextTip.ShowTip(tip);
        }
        #endregion
    }
}
