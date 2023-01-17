using System;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public class CHotFixLogicEntityData : IReference
    {
        public string m_szEntityLogicDllName { get; private set; }
        public string m_szEntityLogicClassName { get; private set; }
        public object m_oUserData { get; private set; }

        public CHotFixLogicEntityData()
        {
            Clear();
        }
        public void Clear()
        {
            m_szEntityLogicDllName = null;
            m_szEntityLogicClassName = null;
            m_oUserData = null;
        }

        public static CHotFixLogicEntityData Create(string a_szDllName, string a_szClassName, object a_oUserData)
        {
            CHotFixLogicEntityData param = ReferencePool.Acquire<CHotFixLogicEntityData>();
            param.m_szEntityLogicDllName = a_szDllName;
            param.m_szEntityLogicClassName = a_szClassName;
            param.m_oUserData = a_oUserData;
            return param;
        }
        public static void Del(CHotFixLogicEntityData a_refParam)
        {
            if (null != a_refParam)
            {
                ReferencePool.Release(a_refParam);
                a_refParam = null;
            }
        }
    }
    public class CHotFixLogicEntity : EntityLogic
    {
        public CHotFixLogicEntityAgentBase m_AgentHotFix { private set; get; }
        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            CHotFixLogicEntityData param = userData as CHotFixLogicEntityData;
            if (param == null)
            {
                Log.Error("CHotFixLogicEntity OnInit is Invalid");
                return;
            }
            m_AgentHotFix = (CHotFixLogicEntityAgentBase)CGameEntryMgr.HotFixComponent.CreateHotFixInstance(param.m_szEntityLogicDllName, param.m_szEntityLogicClassName, null);
            m_AgentHotFix.Init(this, param.m_oUserData);
        }
        protected override void OnRecycle()
        {
            base.OnRecycle();
            m_AgentHotFix.OnRecycle();
        }

        protected override void OnShow(object userData)
        {
            base.OnShow(userData);
            CHotFixLogicEntityData param = userData as CHotFixLogicEntityData;
            m_AgentHotFix.OnShow(param.m_oUserData);
            CHotFixLogicEntityData.Del(param);
        }

        protected override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            m_AgentHotFix.OnHide(isShutdown, userData);
        }

        protected override void OnUpdate(float a_fElapseSeconds, float a_fRealElapeSeconds)
        {
            base.OnUpdate(a_fElapseSeconds, a_fRealElapeSeconds);
            m_AgentHotFix.OnUpdate(a_fElapseSeconds, a_fRealElapeSeconds);
        }
    }
}