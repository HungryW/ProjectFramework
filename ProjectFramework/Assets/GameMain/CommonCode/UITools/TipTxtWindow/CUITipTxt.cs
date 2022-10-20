using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameworkPackage;
using GameFramework;
using UnityEngine.UI;
using DG.Tweening;
using Defines;
using GameFramework.Event;

namespace GameFrameworkPackage
{
    public class CUITipTxt : CLogicUI
    {
        [SerializeField]
        private GameObject GoBg;
        [SerializeField]
        private GameObject GoDetail;
        [SerializeField]
        private GameObject GoTitle;
        [SerializeField]
        private Text lbContent;
        [SerializeField]
        private Text LbTitle;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            SubscribeEvent(CEventUITipTxtCloseArgs.EventId, _Close);
        }

        protected override void OnUpdateShow(object userData)
        {
            base.OnUpdateShow(userData);
            m_Canvas.worldCamera = CCamera.GetUICamera();
            COpenUITipTxtParam param = userData as COpenUITipTxtParam;
            if(null != param)
            {
                ShowText(param);
            }
        }

        public void ShowText(COpenUITipTxtParam a_param)
        {
            lbContent.text = a_param.m_szContent;
            _SetTitle(a_param);
            _SetSize(a_param.m_fContentWidth);
            _SetPos(a_param);
            _FitBgSizeAndPos();
        }

        private void _SetTitle(COpenUITipTxtParam a_param)
        {
            bool bHaveTitle = !string.IsNullOrEmpty(a_param.m_szTitle);
            GoTitle.SetActive(bHaveTitle);
            (GoTitle.transform as RectTransform).SetSizeWidth(a_param.m_fContentWidth);
            LbTitle.text = !bHaveTitle ? CGameEntryMgr.Localization.GetString("Desc") : a_param.m_szTitle;
        }

        private void _FitBgSizeAndPos()
        {
            (GoBg.transform as RectTransform).sizeDelta = (GoDetail.transform as RectTransform).sizeDelta;
            (GoBg.transform as RectTransform).pivot = (GoDetail.transform as RectTransform).pivot;
            (GoBg.transform as RectTransform).anchorMin = (GoDetail.transform as RectTransform).anchorMin;
            (GoBg.transform as RectTransform).anchorMax = (GoDetail.transform as RectTransform).anchorMax;
            GoBg.transform.position = GoDetail.transform.position;
        }

        private void _SetSize(float a_fWidth)
        {
            lbContent.rectTransform.sizeDelta = new Vector2(a_fWidth, lbContent.rectTransform.sizeDelta.y);
            LayoutRebuilder.ForceRebuildLayoutImmediate(lbContent.transform as RectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GoDetail.transform as RectTransform);
        }

        private void _SetPos(COpenUITipTxtParam a_param)
        {
            Vector2 v2Size = (GoDetail.transform as RectTransform).sizeDelta;
            CUIToolFitPosParamItem fitParam = a_param.GetFitPosMgr().GetFitPosParam(v2Size, transform, GetCanvas().worldCamera);
            if (fitParam != null)
            {
                _UpdatePiovePos(fitParam.CalcPivotPos());
                GoDetail.transform.position = fitParam.m_v3WorldPos;
            }
        }

        private void _UpdatePiovePos(Vector2 a_v2)
        {
            (GoDetail.transform as RectTransform).pivot = a_v2;
            (GoDetail.transform as RectTransform).anchorMin = a_v2;
            (GoDetail.transform as RectTransform).anchorMax = a_v2;
        }

        private void _Close(object a_oSender, GameEventArgs a_args)
        {
            Close();
        }

        public static void OpenTipInfo(COpenUITipTxtParam a_param)
        {
            if (!a_param.IsVaild())
            {
                return;
            }
            if (!CGameEntryMgr.UI.GetUIFormVisible(EUIFormID.UITipTxt))
            {
                CGameEntryMgr.UI.OpenUIForm(EUIFormID.UITipTxt, a_param);
            }
            if (CGameEntryMgr.UI.GetUIForm(EUIFormID.UITipTxt) != null)
            {
                (CGameEntryMgr.UI.GetUIForm(EUIFormID.UITipTxt) as CUITipTxt).ShowText(a_param);
            }
        }

        public static void CloseTipInfo()
        {
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventUITipTxtCloseArgs>());
        }
    }

    public class COpenUITipTxtParam
    {
        public float m_fContentWidth
        {
            get;
            private set;
        }

        public string m_szContent
        {
            get;
            private set;
        }

        public string m_szTitle
        {
            get;
            set;
        }
        private CUIToolFitPosParamMgr m_fitPosMgr;

        public COpenUITipTxtParam(string a_szContent, EPosDirect a_ePivotDirect, Vector3 a_v3WorldPos, float a_fContentWidth)
        {
            List<EPosDirect> a_listDirect = new List<EPosDirect>();
            List<Vector3> a_listWorldPos = new List<Vector3>();
            m_szContent = a_szContent;
            m_fContentWidth = a_fContentWidth;
            m_szTitle = null;
            a_listDirect.Add(a_ePivotDirect);
            a_listWorldPos.Add(a_v3WorldPos);
            m_fitPosMgr = new CUIToolFitPosParamMgr(a_listDirect, a_listWorldPos);
        }

        public COpenUITipTxtParam(string a_szContent, float a_fContentWidth, List<EPosDirect> a_listDirect, List<Vector3> a_listWorldPos)
        {
            m_szContent = a_szContent;
            m_fContentWidth = a_fContentWidth;
            m_szTitle = null;
            m_fitPosMgr = new CUIToolFitPosParamMgr(a_listDirect, a_listWorldPos);
        }

        public CUIToolFitPosParamMgr GetFitPosMgr()
        {
            return m_fitPosMgr;
        }

        public bool IsVaild()
        {
            return GetFitPosMgr().IsVaild();
        }
    }

    public class CEventUITipTxtCloseArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventUITipTxtCloseArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public override void Clear()
        {
        }
    }
}

