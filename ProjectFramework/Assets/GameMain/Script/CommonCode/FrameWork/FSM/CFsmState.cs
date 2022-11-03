using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework.Fsm;

namespace GameFrameworkPackage
{

    public class CFsmState<T> : FsmState<T> where T : class
    {
        protected IFsm<T> m_refFsm = null;

        protected virtual void _SubscribeEvent()
        {
        }

        protected virtual void _UnSubsribeEvent()
        {
        }


        protected override void OnInit(IFsm<T> a_fsm)
        {
            base.OnInit(a_fsm);
            m_refFsm = a_fsm;
        }

        protected override void OnEnter(IFsm<T> a_fsm)
        {
            base.OnEnter(a_fsm);
            _SubscribeEvent();
        }

        protected override void OnUpdate(IFsm<T> a_fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(a_fsm, elapseSeconds, realElapseSeconds);
        }

        public virtual void OnPause()
        {

        }

        public virtual void OnContinue()
        {

        }

        protected override void OnLeave(IFsm<T> a_fsm, bool isShutdown)
        {
            base.OnLeave(a_fsm, isShutdown);
            _UnSubsribeEvent();
        }

        protected override void OnDestroy(IFsm<T> a_fsm)
        {
            base.OnDestroy(a_fsm);
        }

    }
}
