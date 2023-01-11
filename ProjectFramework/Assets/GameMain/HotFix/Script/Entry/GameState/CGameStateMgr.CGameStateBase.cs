using GameFramework;
using GameFramework.Fsm;
using GameFrameworkPackage;
using Defines;
using System;

namespace HotFixEntry
{
    public abstract class CGameStateBase : CFsmState<CGameStateMgr>
    {
        public abstract EGameStateID GetStateId();

        public virtual bool CheckCanEnter()
        {
            return true;
        }
        public virtual int GetBGMId()
        {
            return 0;
        }

        protected override void _SubscribeEvent()
        {
            base._SubscribeEvent();
        }

        protected override void _UnSubsribeEvent()
        {
            base._UnSubsribeEvent();
        }


        protected override void OnInit(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnInit(a_fsm);
        }

        protected override void OnEnter(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnEnter(a_fsm);
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventImmediateGameStateEnterArgs>().Fill(GetStateId()));
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventGameStateEnterArgs>().Fill(GetStateId()));
        }

        protected override void OnUpdate(IFsm<CGameStateMgr> a_fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(a_fsm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(IFsm<CGameStateMgr> a_fsm, bool isShutdown)
        {
            base.OnLeave(a_fsm, isShutdown);
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventGameStateLevelArgs>().Fill(GetStateId()));
            CGameEntryMgr.Event.FireNow(null, ReferencePool.Acquire<CEventImmediateGameStateLevelArgs>().Fill(GetStateId()));
        }

        protected override void OnDestroy(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnDestroy(a_fsm);
        }

        public void ChangeGameState(EGameStateID a_eState)
        {
            DRGameState dr = CGameEntryMgr.DataTable.GetDataRow<DRGameState>((int)a_eState);
            Type stateType = dr.GetStateType();
            ChangeState(m_refFsm, stateType);
        }
    }
}

