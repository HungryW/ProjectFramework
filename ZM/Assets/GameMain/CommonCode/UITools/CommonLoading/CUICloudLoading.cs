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
    public class CUICloudLoading : CLogicUI
    {
        [SerializeField]
        private Animator Anim;

        private COpenParamCommonLoading m_Param;
        private bool m_bStartLoad;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
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
            CGameEntryMgr.Sound.PlayUISound(10143);
            Anim.PlayAnim("close", _StartLoad);
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
            m_Param.OnLoadEndStart();
            CGameEntryMgr.Sound.PlayUISound(10142);
            Anim.PlayAnim("open", _OnFadeOutEnd);
        }

        private void _OnFadeOutEnd()
        {
            Close();
            m_Param.OnLoadEnd();
        }
    }
}
