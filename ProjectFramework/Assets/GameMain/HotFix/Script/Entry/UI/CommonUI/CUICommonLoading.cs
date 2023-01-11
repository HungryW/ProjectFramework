using GameFrameworkPackage;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace HotFixEntry.UI
{
    public class COpenParamCommonLoading
    {
        private float m_fFadeInTime;
        private float m_fMinWaitTime;
        private float m_fFadeOutTime;

        private Action m_fnLoadingStart;
        private Action m_fnLoadingStartEnd;
        private Action m_fnLoadingEndStart;
        private Action m_fnLoadingEnd;
        private Func<bool> m_fnCheckLoadEnd;

        private int m_nLoadType;
        private string m_szLoadTips;
        private string m_szAnimName;

        public COpenParamCommonLoading()
        {
            m_fFadeInTime = 0.8f;
            m_fMinWaitTime = 0.3f;
            m_fFadeOutTime = 0.8f;
            m_fnLoadingStart = null;
            m_fnLoadingStartEnd = null;
            m_fnLoadingEndStart = null;
            m_fnLoadingEnd = null;
            m_szLoadTips = "";
        }
        public float GetFadeInTime()
        {
            return m_fFadeInTime;
        }

        public float GetFadeOutTime()
        {
            return m_fFadeOutTime;
        }

        public string GetLoadTips()
        {
            return m_szLoadTips;
        }

        public string GetAnimName()
        {
            return m_szAnimName;
        }

        public void SetLoadTips(string a_szTips)
        {
            m_szLoadTips = a_szTips;
        }

        public void SetAnim(string a_szAnim)
        {
            m_szAnimName = a_szAnim;
        }

        public void SetFadeInTime(float a_fTime)
        {
            m_fFadeInTime = a_fTime;
        }

        public void SetFadeOutTime(float a_fTime)
        {
            m_fFadeOutTime = a_fTime;
        }

        public void SetStartLoadCallback(Action a_fn)
        {
            m_fnLoadingStart = a_fn;
        }

        public void SetStartLoadEndCallback(Action a_fn)
        {
            m_fnLoadingStartEnd = a_fn;
        }

        public void SetEndLoadStartCallback(Action a_fn)
        {
            m_fnLoadingEndStart = a_fn;
        }

        public void SetEndLoadCallBack(Action a_fn)
        {
            m_fnLoadingEnd = a_fn;
        }

        public void SetCheckLoadEndFn(Func<bool> a_fnCheckLoadEnd)
        {
            m_fnCheckLoadEnd = a_fnCheckLoadEnd;
        }

        public void OnLoadStart()
        {
            m_fnLoadingStart.SafeInvoke();
        }

        public void OnLoadStartEnd()
        {
            m_fnLoadingStartEnd.SafeInvoke();
        }

        public void OnLoadEndStart()
        {
            m_fnLoadingEndStart.SafeInvoke();
        }

        public void OnLoadEnd()
        {
            m_fnLoadingEnd.SafeInvoke();
        }

        public bool CheckIsLoadEnd()
        {
            if (m_fnCheckLoadEnd != null)
            {
                return m_fnCheckLoadEnd.Invoke();
            }
            return true;
        }
    }

    public class CUICommonLoading : CUIBase
    {
        private Image ImgBg;

        private COpenParamCommonLoading m_Param;
        private bool m_bStartLoad;

        protected override void _InitComponents()
        {
            ImgBg = transform.Find("ImgBg").GetComponent<Image>();
        }

        protected override void _OnInit(object userData)
        {
            base._OnInit(userData);
        }

        protected override void _OnOpenShow(object userData)
        {
            base._OnOpenShow(userData);
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
            ImgBg.color = new Color(0, 0, 0, 0);
            Sequence seq = DOTween.Sequence();
            seq.Append(ImgBg.DOFade(1, m_Param.GetFadeInTime()))
                .AppendCallback(_StartLoad);
        }

        private void _StartLoad()
        {
            if (m_bStartLoad)
            {
                return;
            }
            m_bStartLoad = true;
            m_Param.OnLoadStartEnd();
        }

        public override void OnUpdate(float a_fElapseSeconds, float a_fRealElapeSeconds)
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
            m_Param.OnLoadEndStart();
            m_bStartLoad = false;
            Sequence seq = DOTween.Sequence();
            seq.Append(ImgBg.DOFade(0, m_Param.GetFadeOutTime()))
                .AppendCallback(_OnFadeOutEnd);
        }
        private void _OnFadeOutEnd()
        {
            _Close();
            m_Param.OnLoadEnd();
        }
    }
}
