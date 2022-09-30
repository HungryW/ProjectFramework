using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Animations;
using DG.Tweening;

namespace GameFrameworkPackage
{
    public class CUIBlinkLoading : CLogicUI
    {
        [SerializeField]
        CScreenBlinkEffect BlinkCtrl;

        private bool m_bStartLoad;

        private COpenParamCommonLoading m_Param;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            BlinkCtrl = CCamera.GetUICamera().GetComponent<CScreenBlinkEffect>();
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
            BlinkCtrl.progress = 1f;
            BlinkCtrl.enabled = true;
            Sequence seq = DOTween.Sequence();
            seq.Append(DOTween.To(() => 1f, p => BlinkCtrl.progress = p, 0f, 0.2f))
                .AppendCallback(_StartLoad);
        }

        private void _StartLoad()
        {
            m_Param.OnLoadStartEnd();
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.4f)
                .AppendCallback(() => m_bStartLoad = true)
                .AppendCallback(m_Param.OnLoadEndStart);
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
            Sequence seq = DOTween.Sequence();
            seq.Append(DOTween.To(() => 0f, p => BlinkCtrl.progress = p, 0.3f, 0.9f))
                .Append(DOTween.To(() => 0.3f, p => BlinkCtrl.progress = p, 0f, 0.4f))
                .Append(DOTween.To(() => 0f, p => BlinkCtrl.progress = p, 0.6f, 0.6f))
                .Append(DOTween.To(() => 0.6f, p => BlinkCtrl.progress = p, 0f, 0.4f))
                .Append(DOTween.To(() => 0f, p => BlinkCtrl.progress = p, 1f, 0.4f))
                .AppendCallback(_OnBlinkLoadEnd);
        }

        private void _OnBlinkLoadEnd()
        {
            Close();
            m_Param.OnLoadEnd();
        }
    }
}
