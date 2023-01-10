using Defines;
using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public class CEventGameStateEnterArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventGameStateEnterArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public EGameStateID StateId
        {
            get;
            private set;
        }

        public CEventGameStateEnterArgs Fill(EGameStateID a_eStateId)
        {
            StateId = a_eStateId;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventGameStateStartChangeSceneArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventGameStateStartChangeSceneArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public EGameStateID LevelStateId
        {
            get;
            private set;
        }

        public EGameStateID EnterStateId
        {
            get;
            private set;
        }

        public CEventGameStateStartChangeSceneArgs Fill(EGameStateID a_eLevelStateId, EGameStateID a_eEnterStateId)
        {
            LevelStateId = a_eLevelStateId;
            EnterStateId = a_eEnterStateId;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventGameImmediatelyStateStartChangeSceneArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventGameImmediatelyStateStartChangeSceneArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public EGameStateID LevelStateId
        {
            get;
            private set;
        }

        public EGameStateID EnterStateId
        {
            get;
            private set;
        }

        public CEventGameImmediatelyStateStartChangeSceneArgs Fill(EGameStateID a_eLevelStateId, EGameStateID a_eEnterStateId)
        {
            LevelStateId = a_eLevelStateId;
            EnterStateId = a_eEnterStateId;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventGameStateEndChangeSceneArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventGameStateEndChangeSceneArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public EGameStateID LevelStateId
        {
            get;
            private set;
        }

        public EGameStateID EnterStateId
        {
            get;
            private set;
        }

        public CEventGameStateEndChangeSceneArgs Fill(EGameStateID a_eLevelStateId, EGameStateID a_eEnterStateId)
        {
            LevelStateId = a_eLevelStateId;
            EnterStateId = a_eEnterStateId;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventGameStateLevelArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventGameStateLevelArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public EGameStateID StateId
        {
            get;
            private set;
        }

        public CEventGameStateLevelArgs Fill(EGameStateID a_eStateId)
        {
            StateId = a_eStateId;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventImmediateGameStateLevelArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventImmediateGameStateLevelArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public EGameStateID StateId
        {
            get;
            private set;
        }

        public CEventImmediateGameStateLevelArgs Fill(EGameStateID a_eStateId)
        {
            StateId = a_eStateId;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventImmediateGameStateEnterArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventImmediateGameStateEnterArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public EGameStateID StateId
        {
            get;
            private set;
        }

        public CEventImmediateGameStateEnterArgs Fill(EGameStateID a_eStateId)
        {
            StateId = a_eStateId;
            return this;
        }

        public override void Clear()
        {
        }
    }

    public class CEventInclassStateDayPassArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventInclassStateDayPassArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public override void Clear()
        {
        }
    }

    public class CEventUIDialogOpenArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventUIDialogOpenArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public override void Clear()
        {
        }
    }

    public class CEventUIDialogCloseArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(CEventUIDialogCloseArgs).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public override void Clear()
        {
        }
    }
}
