using GameFramework.Fsm;
using Defines;
using GameFramework.Event;
using Logic;
using GameFramework;
using GameFrameworkPackage;

namespace HotFixEntry
{

    public class CGameStateLogin : CGameStateBase
    {
        public override EGameStateID GetStateId()
        {
            return EGameStateID.CGameStateLogin;
        }
        public CGameStateLogin() : base()
        {

        }

        protected override void OnInit(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnInit(a_fsm);
        }

        protected override void OnEnter(IFsm<CGameStateMgr> a_fsm)
        {
            CAnalyticsMainProject.Instance.LogGameProgress("StartLogin", "", null);
            base.OnEnter(a_fsm);
            CGameEntryMgr.PreloadComponent.SetBgVisible(true);
            CHotFixEntry.UI.CloseAllUIByGroupName("Default");
            COpenParamCommonConfirm param = new COpenParamCommonConfirm(CHotFixEntry.Localization.GetStringEX("Im Login UI"));
            param.SetConfirmCallback(() =>
            {
                CChangeGameStateParam paramChangeState = new CChangeGameStateParam();
                paramChangeState.SetNextGameState(EGameStateID.CGameStateMain);
                paramChangeState.SetUIType(ELoadingUIType.BlackFade);
                CHotFixEntry.GameStateMgr.ChangeState(paramChangeState);
            });
            CHotFixEntry.UI.OpenUINoHideAnim(EUIFormID.UICommonConfirm, param);
        }

        protected override void OnUpdate(IFsm<CGameStateMgr> a_fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(a_fsm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(IFsm<CGameStateMgr> a_fsm, bool isShutdown)
        {
            base.OnLeave(a_fsm, isShutdown);
            CGameEntryMgr.PreloadComponent.SetBgVisible(false);
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

