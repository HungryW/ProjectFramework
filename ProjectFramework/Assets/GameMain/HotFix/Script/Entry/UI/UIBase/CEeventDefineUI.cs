using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFrameworkPackage;
using UnityEngine.UI;
using Defines;
using UnityGameFramework.Runtime;
using System;
using GameFramework.Event;
using GameFramework;

namespace HotFixEntry
{
    public class CEventUIOpenStartArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventUIOpenStartArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int eUI
        {
            get;
            private set;
        }

        public CEventUIOpenStartArgs Fill(int a_eUI)
        {
            eUI = a_eUI;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventUIOpenEndArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventUIOpenEndArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int eUI
        {
            get;
            private set;
        }

        public CEventUIOpenEndArgs Fill(int a_eUI)
        {
            eUI = a_eUI;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventUICloseStartArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventUICloseStartArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int eUI
        {
            get;
            private set;
        }

        public CEventUICloseStartArgs Fill(int a_eUI)
        {
            eUI = a_eUI;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventUICloseEndArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventUICloseEndArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public int eUI
        {
            get;
            private set;
        }

        public CEventUICloseEndArgs Fill(int a_eUI)
        {
            eUI = a_eUI;
            return this;
        }

        public override void Clear()
        {
        }
    }
}