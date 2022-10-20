using System;
using System.Collections.Generic;
using GameFramework;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Animations;
using DG.Tweening;
using GameFrameworkPackage;

namespace GameFrameworkPackage
{
    public class CUILogoLoading : CLogicUI
    {
        [SerializeField]
        private Animator Anim;
        [SerializeField]
        private Button BtnClick;
        [SerializeField]
        private Text LbContent;

        private bool m_bStartLoad;

        private COpenParamCommonLoading m_Param;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            BtnClick.onClick.AddListener(_OnClick);
        }

        protected override void OnUpdateShow(object userData)
        {
            base.OnUpdateShow(userData);
            _InitData(userData);
            _InitShow();
        }

        private void _InitData(object userData)
        {
            m_Param = userData as COpenParamCommonLoading;
            m_bStartLoad = false;
        }

        private void _InitShow()
        {
            m_Param.OnLoadStart();
            Anim.PlayAnim("fadeIn", _StartLoad);
            BtnClick.enabled = false;
            LbContent.gameObject.SetActive(false);
        }

        private void _StartLoad()
        {
            m_Param.OnLoadStartEnd();
            _ResetText();
            Sequence seq = DOTween.Sequence();
            seq.Append(LbContent.DOText(CGameEntryMgr.Localization.GetStringEX(m_Param.GetLoadTips()), 0.1f * m_Param.GetLoadTips().Length))
                .AppendCallback(() => BtnClick.enabled = true);
        }

        protected override void OnUpdate(float a_fElapseSeconds, float a_fRealElapeSeconds)
        {
            base.OnUpdate(a_fElapseSeconds, a_fRealElapeSeconds);
            if (!m_bStartLoad)
            {
                return;
            }

            if (m_Param.CheckIsLoadEnd())
            {
                _OnLoadEnd();
            }
        }

        private void _OnLoadEnd()
        {
            if (!m_bStartLoad)
            {
                return;
            }
            m_bStartLoad = false;
            Anim.PlayAnim("fadeout", _OnFadeOutEnd);
        }

        private void _OnClick()
        {
            m_Param.OnLoadEndStart();
            LbContent.gameObject.SetActive(false);
            BtnClick.enabled = false;
            m_bStartLoad = true;
        }

        private void _ResetText()
        {
            LbContent.gameObject.SetActive(true);
            LbContent.GetComponent<ContentSizeFitter>().enabled = true;
            LbContent.text = CGameEntryMgr.Localization.GetStringEX(m_Param.GetLoadTips());
            LayoutRebuilder.ForceRebuildLayoutImmediate(LbContent.rectTransform);
            LbContent.GetComponent<ContentSizeFitter>().enabled = false;
            LbContent.text = string.Empty;
        }

        private void _OnFadeOutEnd()
        {
            Close();
            m_Param.OnLoadEnd();
        }

    }
}
