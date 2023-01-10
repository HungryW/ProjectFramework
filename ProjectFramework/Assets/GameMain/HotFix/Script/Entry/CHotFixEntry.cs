using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameworkPackage;
using UnityGameFramework.Runtime;

namespace HotFixEntry
{
    public static partial class CHotFixEntry
    {
        public static void Enter()
        {
            _InitFramework();
            GameStateMgr.Start();
        }

        public static void Shutdown(ShutdownType a_eType)
        {
            GameEntry.Shutdown(a_eType);
            _CleanFrameWork();
        }

    }

    public abstract class CFrameworkBase
    {
        public CFrameworkBase()
        {
            CHotFixEntry.RegisterFramework(this);
        }
        public abstract void Clean();
    }

    public static partial class CHotFixEntry
    {
        private static List<CFrameworkBase> m_listFrameWork = new List<CFrameworkBase>();

        public static void RegisterFramework(CFrameworkBase a_refFramework)
        {
            m_listFrameWork.Add(a_refFramework);
        }
        private static void _CleanFrameWork()
        {
            foreach (var item in m_listFrameWork)
            {
                item.Clean();
            }
            m_listFrameWork.Clear();
        }

        public static LocalizationComponent Localization { private set; get; }
        public static EventComponent Event { private set; get; }
        public static DataTableComponent DataTable { private set; get; }
        public static CFrameworkSound Sound { private set; get; }
        public static CFrameWorkUI UI { private set; get; }
        public static CGameStateMgr GameStateMgr { private set; get; }
        private static void _InitFramework()
        {
            Localization = CGameEntryMgr.Localization;
            Event = CGameEntryMgr.Event;
            DataTable = CGameEntryMgr.DataTable;

            Sound = new CFrameworkSound();
            UI = new CFrameWorkUI();
            GameStateMgr = new CGameStateMgr();
        }
    }
}

