using GameFramework.Fsm;
using Defines;
using GameFramework.Event;
using Logic;

namespace HotFixEntry
{
    public class CGameStateMain : CGameStateBase
    {

        public override EGameStateID GetStateId()
        {
            return EGameStateID.CGameStateMain;
        }

        public CGameStateMain() : base()
        {

        }

        protected override void OnInit(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnInit(a_fsm);
        }

        protected override void OnEnter(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnEnter(a_fsm);
            CHotFixEntry.UI.OpenUINoHideAnim(EUIFormID.UIMain);
        }

        protected override void OnUpdate(IFsm<CGameStateMgr> a_fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(a_fsm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(IFsm<CGameStateMgr> a_fsm, bool isShutdown)
        {
            base.OnLeave(a_fsm, isShutdown);
        }

        protected override void OnDestroy(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnDestroy(a_fsm);
        }


        protected override void _SubscribeEvent()
        {
            base._SubscribeEvent();
        }
        protected override void _UnSubsribeEvent()
        {
            base._UnSubsribeEvent();
        }


    }
}

