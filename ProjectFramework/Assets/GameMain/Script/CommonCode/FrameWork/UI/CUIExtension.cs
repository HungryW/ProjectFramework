using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.DataTable;
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

        public static bool HasUIForm(this UIComponent a_uiComponent, string a_szUIAssetName, string a_szGroupName)
        {
            if (string.IsNullOrEmpty(a_szGroupName))
            {
                return a_uiComponent.HasUIForm(a_szUIAssetName);
            }

            IUIGroup uiGroup = a_uiComponent.GetUIGroup(a_szGroupName);
            if (uiGroup == null)
            {
                return false;
            }

            return uiGroup.HasUIForm(a_szUIAssetName);
        }

        public static UIForm GetUIForm(this UIComponent a_uiComponent, string a_szUIAssetName, string a_szGroupName)
        {
            UIForm ui = null;
            if (string.IsNullOrEmpty(a_szGroupName))
            {
                ui = a_uiComponent.GetUIForm(a_szUIAssetName);
                return ui;
            }

            IUIGroup uiGroup = a_uiComponent.GetUIGroup(a_szGroupName);
            if (null == uiGroup)
            {
                return null;
            }

            ui = (UIForm)uiGroup.GetUIForm(a_szUIAssetName);
            return ui;
        }

        public static CLogicUI GetUILogic(this UIComponent a_uiComponent, string a_szUIAssetName, string a_szGroupName)
        {
            UIForm uiForm = a_uiComponent.GetUIForm(a_szUIAssetName, a_szGroupName);
            if (uiForm == null)
            {
                return null;
            }
            return (CLogicUI)uiForm.Logic;
        }

        public static bool GetUIFormVisible(this UIComponent a_uiComponent, string a_szUIAssetName, string a_szGroupName)
        {
            CLogicUI uiLogic = GetUILogic(a_uiComponent, a_szUIAssetName, a_szGroupName);
            if (null == uiLogic)
            {
                return false;
            }
            return uiLogic.Visible;
        }


        public static void CloseUIForm(this UIComponent a_uiComponent, CLogicUI a_uiLogic)
        {
            a_uiComponent.CloseUIForm(a_uiLogic.UIForm);
        }

        private static int? OpenHotFixUIForm(this UIComponent a_uiComponent, string a_szAssetName, string a_szGroupName, bool a_bIsauseCoveredUIForm, string a_szHotFixDllName, string a_szHotFixClassName, object a_oUserData)
        {
            COpenHotFixLogicUIParam param = COpenHotFixLogicUIParam.Create(a_szHotFixDllName, a_szHotFixClassName, a_oUserData);
            return a_uiComponent.OpenUIForm(a_szAssetName, a_szGroupName, CConstAssetPriority.UIFormAsset, a_bIsauseCoveredUIForm, param);
        }

        private static int? OpenUIForm(this UIComponent a_uiComponent, string a_szAssetName, string a_szGroupName, bool a_bIsauseCoveredUIForm, object a_oUserData)
        {
            return a_uiComponent.OpenUIForm(a_szAssetName, a_szGroupName, CConstAssetPriority.UIFormAsset, a_bIsauseCoveredUIForm, a_oUserData);
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

    }
}
