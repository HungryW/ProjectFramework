using GameFramework;
using GameFramework.Event;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GameFrameworkPackage
{
    public class CSubscribeEventTool
    {
        private class CListenEventData
        {
            public int m_nEventId = -1;
            public EventHandler<GameEventArgs> m_handlerEvent = null;

            public CListenEventData(int a_nId, EventHandler<GameEventArgs> a_handler)
            {
                m_nEventId = a_nId;
                m_handlerEvent = a_handler;
            }
        }

        public static CSubscribeEventTool Create()
        {
            return new CSubscribeEventTool();
        }

        public static void Del(CSubscribeEventTool a_refTool)
        {
            if (a_refTool != null)
            {
                if (a_refTool.m_bIsSubscribing)
                {
                    a_refTool.UnSubscribeAllEvent();
                }
                a_refTool.m_listEeventData.Clear();
                a_refTool = null;
            }
        }

        private List<CListenEventData> m_listEeventData;
        private bool m_bIsSubscribing;
        public CSubscribeEventTool()
        {
            m_listEeventData = new List<CListenEventData>();
            m_bIsSubscribing = false;
        }
        public void RegistEvent(int a_nEventId, EventHandler<GameEventArgs> a_handler)
        {
            m_listEeventData.Add(new CListenEventData(a_nEventId, a_handler));
        }

        public void SubscribeAllEvent()
        {
            if (m_bIsSubscribing)
            {
                return;
            }
            m_bIsSubscribing = true;
            for (int i = 0; i < m_listEeventData.Count; i++)
            {
                CGameEntryMgr.Event.Subscribe(m_listEeventData[i].m_nEventId, m_listEeventData[i].m_handlerEvent);
            }
        }

        public void UnSubscribeAllEvent()
        {
            if (!m_bIsSubscribing)
            {
                return;
            }
            m_bIsSubscribing = false;
            for (int i = 0; i < m_listEeventData.Count; i++)
            {
                CGameEntryMgr.Event.Unsubscribe(m_listEeventData[i].m_nEventId, m_listEeventData[i].m_handlerEvent);
            }
        }
    }
}


