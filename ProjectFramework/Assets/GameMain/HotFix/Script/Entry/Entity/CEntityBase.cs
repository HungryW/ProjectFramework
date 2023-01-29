using GameFramework.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using GameFrameworkPackage;
using UnityEngine;

namespace HotFixEntry
{
    public class CEntityData
    {
        public int m_nId { protected set; get; }
        public CEntityData(int a_nId)
        {
            m_nId = a_nId;
        }
        protected virtual void _CleanData()
        {
            m_nId = -1;
        }

        private static int ms_IdSeed;

        public static int GenerateId()
        {
            return ms_IdSeed++;
        }
    }

    public class CEntityBase : CHotFixLogicEntityAgentBase, IGameObj
    {
        private CEntityData m_data;
        private CSubscribeEventTool m_eventTool;

        public Transform transform => EntityLogic.CachedTransform;

        public GameObject gameObject => transform.gameObject;

        public int GetId()
        {
            return m_data.m_nId;
        }
        protected override void _OnInit(object userData)
        {
            m_eventTool = _CreateEventTool();
            _InitComponents();
        }

        protected virtual void _InitComponents()
        {

        }

        private CSubscribeEventTool _CreateEventTool()
        {
            CSubscribeEventTool.Del(m_eventTool);
            return CSubscribeEventTool.Create();
        }
        public override void OnRecycle()
        {

        }

        public override void OnShow(object userData)
        {
            m_data = userData as CEntityData;
        }

        public override void OnHide(bool isShutdown, object userData)
        {

        }

        public override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
        protected override void _OnVisibleChange(bool a_bVisible)
        {
            base._OnVisibleChange(a_bVisible);
            if (a_bVisible)
            {
                m_eventTool.SubscribeAllEvent();
            }
            else
            {
                m_eventTool.UnSubscribeAllEvent();
            }
        }

        protected void _SubscribeEvent(int a_nEventId, EventHandler<GameEventArgs> a_handler)
        {
            m_eventTool.RegistEvent(a_nEventId, a_handler);
        }
    }
}