using System;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public class COpenHotFixLogicUIParam : IReference
    {
        public string m_szUILogicDllName { get; private set; }
        public string m_szUILogicClassName { get; private set; }
        public object m_oUserData { get; private set; }

        public COpenHotFixLogicUIParam()
        {
            Clear();
        }
        public void Clear()
        {
            m_szUILogicDllName = null;
            m_szUILogicClassName = null;
            m_oUserData = null;
        }

        public static COpenHotFixLogicUIParam Create(string a_szUILogicDllName, string a_szUILogicClassName, object a_oUserData)
        {
            COpenHotFixLogicUIParam param = ReferencePool.Acquire<COpenHotFixLogicUIParam>();
            param.m_szUILogicDllName = a_szUILogicDllName;
            param.m_szUILogicClassName = a_szUILogicClassName;
            param.m_oUserData = a_oUserData;
            return param;
        }
        public static void Del(COpenHotFixLogicUIParam a_refParam)
        {
            if (null != a_refParam)
            {
                ReferencePool.Release(a_refParam);
                a_refParam = null;
            }
        }
    }
    public class CHotFixLogicUI : CLogicUI
    {
        public CHotFixUILogicAgentBase m_AgentHotFixUI { private set; get; }
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            COpenHotFixLogicUIParam param = userData as COpenHotFixLogicUIParam;
            if (param == null)
            {
                Log.Error("CHotFixLogicUI COpenHotFixLogicUIParam is Invalid");
                return;
            }
            m_AgentHotFixUI = (CHotFixUILogicAgentBase)CGameEntryMgr.HotFixComponent.CreateHotFixInstance(param.m_szUILogicDllName, param.m_szUILogicClassName, null);
            m_AgentHotFixUI.Init(this, param.m_oUserData);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            COpenHotFixLogicUIParam param = userData as COpenHotFixLogicUIParam;
            m_AgentHotFixUI.OnOpen(param.m_oUserData);
            COpenHotFixLogicUIParam.Del(param);
        }
        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            m_AgentHotFixUI.OnClose(isShutdown, userData);
        }
        protected override void OnPause()
        {
            base.OnPause();
            m_AgentHotFixUI.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            m_AgentHotFixUI.OnResume();
        }

        protected override void OnCover()
        {
            base.OnCover();
            m_AgentHotFixUI.OnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
            m_AgentHotFixUI.OnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
            m_AgentHotFixUI.OnRefocus(userData);
        }

        protected override void OnUpdate(float a_fElapseSeconds, float a_fRealElapeSeconds)
        {
            base.OnUpdate(a_fElapseSeconds, a_fRealElapeSeconds);
            m_AgentHotFixUI.OnUpdate(a_fElapseSeconds, a_fRealElapeSeconds);
        }

        protected override void OnDepthChanged(int a_nUIGroupDepth, int a_nDepthInUIGroup)
        {
            base.OnDepthChanged(a_nUIGroupDepth, a_nDepthInUIGroup);
            m_AgentHotFixUI.OnDepthChanged(a_nUIGroupDepth, a_nDepthInUIGroup);
        }
    }
}