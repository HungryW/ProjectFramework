using UnityEngine;
using GameFrameworkPackage;
using Defines;
using UnityGameFramework.Runtime;
using System;
using GameFramework;

namespace HotFixEntry
{
    public class COpenUIParam : IReference
    {
        public object m_oUserData { get; private set; }
        public int m_nConfigId { get; private set; }

        public COpenUIParam()
        {
            m_oUserData = null;
        }
        public void Clear()
        {
            m_oUserData = null;
        }
        public static COpenUIParam Create(int a_nConfigId, object a_oUserData)
        {
            COpenUIParam param = ReferencePool.Acquire<COpenUIParam>();
            param.m_oUserData = a_oUserData;
            param.m_nConfigId = a_nConfigId;
            return param;
        }

        public static void Delete(COpenUIParam a_refParam)
        {
            ReferencePool.Release(a_refParam);
        }
    }
    public abstract partial class CUIBase : CHotFixUILogicAgentBase, IGameObj
    {
        private int m_nConfigId;
        private DRUIForm m_refConfig;
        private CUIShieldRaycast m_Raycast;
        private CUIBlurBg m_BulrBg;
        private bool m_bPlayedHideAnim;

        public int GetConfigId()
        {
            return m_nConfigId;
        }
        public Transform transform
        {
            get
            {
                return UILogic.transform;
            }
        }

        public GameObject gameObject
        {
            get
            {
                return transform.gameObject;
            }
        }

        protected DRUIForm _GetConfig()
        {
            if (m_refConfig == null)
            {
                m_refConfig = CHotFixEntry.DataTable.GetDataRow<DRUIForm>(m_nConfigId);
                if (null == m_refConfig)
                {
                    Log.Error("{0} InitFail Config is Null ConfigId={1}", transform.name, m_nConfigId);
                }
            }
            return m_refConfig;
        }

        protected override void _OnInit(object userData)
        {
            COpenUIParam param = userData as COpenUIParam;
            m_nConfigId = param.m_nConfigId;
            m_Raycast = new CUIShieldRaycast(transform);
            m_BulrBg = new CUIBlurBg(transform);
            m_bPlayedHideAnim = false;
            _InitComponents();
        }

        protected virtual void _InitComponents()
        {

        }
        public override void OnOpen(object userData)
        {
            COpenUIParam param = userData as COpenUIParam;
            if (_GetConfig().NeedBlurBg)
            {
                m_BulrBg.CreateBlurBg(() => { _OnOpen(param); });
            }
            else
            {
                _OnOpen(param);
            }
        }
        private void _OnOpen(COpenUIParam a_refOpenParam)
        {
            _SetCanRaycastTarget(false);
            _OnOpenShow(a_refOpenParam.m_oUserData);
            CHotFixEntry.Event.FireNow(null, ReferencePool.Acquire<CEventUIOpenStartArgs>().Fill(m_nConfigId));
            _PlayOpenAnim(_OnOpenAnimEnd);
            COpenUIParam.Delete(a_refOpenParam);
        }

        protected virtual void _PlayOpenAnim(Action a_fnOnAnimEnd)
        {
            a_fnOnAnimEnd.SafeInvoke();

        }
        private void _OnOpenAnimEnd()
        {
            _SetCanRaycastTarget(true);
            CHotFixEntry.Event.FireNow(null, ReferencePool.Acquire<CEventUIOpenEndArgs>().Fill(m_nConfigId));

        }
        protected virtual void _OnOpenShow(object userData)
        {

        }

        protected virtual void _OnUpdateData(object userData)
        {

        }

        protected void _Close(bool a_bPlayCloseAnim = true)
        {
            _SetCanRaycastTarget(false);
            CHotFixEntry.Event.FireNow(null, ReferencePool.Acquire<CEventUICloseStartArgs>().Fill(m_nConfigId));
            if (a_bPlayCloseAnim)
            {
                _PlayCloseAnim(__Close);
            }
            else
            {
                __Close();
            }
        }

        protected virtual void _PlayCloseAnim(Action a_fnOnCloseAnimEnd)
        {
            a_fnOnCloseAnimEnd.SafeInvoke();
        }

        private void __Close()
        {
            UILogic.Close();
            CHotFixEntry.Event.FireNow(null, ReferencePool.Acquire<CEventUICloseEndArgs>().Fill(m_nConfigId));
        }

        public override void OnClose(bool isShutdown, object userData)
        {
            _SetPlayedHideAnim(false);
            _SetCanRaycastTarget(true);
            m_BulrBg.DelBlurBg();
        }

        public override void OnReveal()
        {
            _TryPlayShowAnim();
        }

        private void _TryPlayShowAnim()
        {
            if (!m_bPlayedHideAnim)
            {
                return;
            }
            _SetPlayedHideAnim(false);
            _PlayShowAnim(_OnShowAnimEnd);
        }

        protected virtual void _PlayShowAnim(Action a_fnOnShowAnimEnd)
        {
            a_fnOnShowAnimEnd.SafeInvoke();
        }
        private void _OnShowAnimEnd()
        {
            _SetCanRaycastTarget(true);
        }

        public void StartPlayHideAnim(Action a_fnOnHidePlayEnd)
        {
            if (m_bPlayedHideAnim)
            {
                a_fnOnHidePlayEnd.SafeInvoke();
                return;
            }
            _SetPlayedHideAnim(true);
            _SetCanRaycastTarget(false);
            _PlayHideAnim(a_fnOnHidePlayEnd);
        }
        public virtual void _PlayHideAnim(Action a_fnOnHidePlayEnd)
        {
            a_fnOnHidePlayEnd.SafeInvoke();
        }
        private void _SetCanRaycastTarget(bool a_bIsCan)
        {
            m_Raycast.SetActive(!a_bIsCan);
        }
        protected void _SetPlayedHideAnim(bool a_b)
        {
            m_bPlayedHideAnim = a_b;
        }

        public override void OnCover()
        {
        }

        public override void OnPause()
        {
        }

        public override void OnResume()
        {
        }

        public override void OnRefocus(object userData)
        {

        }
    }
}