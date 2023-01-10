using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.Fsm;
using GameFrameworkPackage;
using Defines;
using UnityGameFramework.Runtime;
using GameFramework.Event;
using DG.Tweening;
using System;

namespace HotFixLogic
{

    public class ELoadingUIType
    {
        public const int BlackFade = 1;
        public const int Cloud = 2;
    }

    public class CChangeGameStateParam
    {
        public static CChangeGameStateParam ms_empty = new CChangeGameStateParam();
        private int m_nLoadingUIType;
        private Action m_fnChangeStateEnter;
        private Action m_fnChangeStateStart;
        private Action m_fnChangeStateEnd;
        private Action m_fnChangeStateExit;
        private Func<bool> m_fnCheckChangeEnd;

        public CChangeGameStateParam()
        {
            m_nLoadingUIType = ELoadingUIType.BlackFade;
            m_fnChangeStateEnter = null;
            m_fnChangeStateStart = null;
            m_fnChangeStateEnd = null;
            m_fnChangeStateExit = null;
            m_fnCheckChangeEnd = null;
        }

        public CChangeGameStateParam SetUIType(int a_eUIType)
        {
            m_nLoadingUIType = a_eUIType;
            return this;
        }

        public CChangeGameStateParam SetChangeStateEnterCallback(Action a_fn)
        {
            m_fnChangeStateEnter = a_fn;
            return this;
        }

        public CChangeGameStateParam SetChangeStateStartCallback(Action a_fn)
        {
            m_fnChangeStateStart = a_fn;
            return this;
        }

        public CChangeGameStateParam SetChangeStateEndCallback(Action a_fn)
        {
            m_fnChangeStateEnd = a_fn;
            return this;
        }

        public CChangeGameStateParam SetChangeStateExitCallback(Action a_fn)
        {
            m_fnChangeStateExit = a_fn;
            return this;
        }

        public CChangeGameStateParam SetCheckChangeEndFunc(Func<bool> a_fn)
        {
            m_fnCheckChangeEnd = a_fn;
            return this;
        }

        public int GetUIType()
        {
            return m_nLoadingUIType;
        }

        public void InvokeChangeStateEnter()
        {
            m_fnChangeStateEnter.SafeInvoke();
        }

        public void InvokeChangeStateStart()
        {
            m_fnChangeStateStart.SafeInvoke();
        }

        public void InvokeChangeStateEnd()
        {
            m_fnChangeStateEnd.SafeInvoke();
        }

        public void InvokeChangeStateExit()
        {
            m_fnChangeStateExit.SafeInvoke();
        }

        public bool IsChangeEnd()
        {
            if (m_fnCheckChangeEnd == null)
            {
                return true;
            }
            return m_fnCheckChangeEnd.Invoke();
        }
    }

    public class CGameStateBase : CFsmState<CGameStateMgr>
    {
        private EGameStateID m_eState;
        private CGameStateChangeSceneTool m_changeSceneTool;

        private bool m_bIsChangeAllEnd = false;
        public CGameStateBase(EGameStateID a_eState)
        {
            m_eState = a_eState;
            m_changeSceneTool = new CGameStateChangeSceneTool();
        }

        public EGameStateID GetStateId()
        {
            return m_eState;
        }

        protected virtual bool _CheckCanEnter()
        {
            return true;
        }

        protected virtual bool _EnterNeedLoading()
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
            //CGameEntryMgr.Sound.PlayMusic(GetBGMId(), m_eState, true);
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventImmediateGameStateEnterArgs>().Fill(m_eState));
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventGameStateEnterArgs>().Fill(m_eState));
        }

        protected override void OnUpdate(IFsm<CGameStateMgr> a_fsm, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(a_fsm, elapseSeconds, realElapseSeconds);
        }

        protected override void OnLeave(IFsm<CGameStateMgr> a_fsm, bool isShutdown)
        {
            base.OnLeave(a_fsm, isShutdown);
            CGameEntryMgr.Event.Fire(null, ReferencePool.Acquire<CEventGameStateLevelArgs>().Fill(m_eState));
            CGameEntryMgr.Event.FireNow(null, ReferencePool.Acquire<CEventImmediateGameStateLevelArgs>().Fill(m_eState));
        }

        protected override void OnDestroy(IFsm<CGameStateMgr> a_fsm)
        {
            base.OnDestroy(a_fsm);
        }

        public bool ChangeState(EGameStateID a_eState, CChangeGameStateParam a_param)
        {
            if (a_eState == m_eState)
            {
                return false;
            }
            CGameStateBase stateBase = CGameStateMgr.Instance.GetState(a_eState);
            if (stateBase == null || !stateBase._CheckCanEnter())
            {
                return false;
            }
            a_param = a_param == null ? CChangeGameStateParam.ms_empty : a_param;
            if (stateBase._EnterNeedLoading())
            {
                m_bIsChangeAllEnd = false;
                //COpenParamCommonLoading paramLoading = new COpenParamCommonLoading();
                //paramLoading.SetCheckLoadEndFn(() => { return _CheckSceneChangeEnd() && a_param.IsChangeEnd(); });
                //paramLoading.SetStartLoadCallback(() => { a_param.InvokeChangeStateEnter(); });
                //paramLoading.SetStartLoadEndCallback(() => { _StartChangeState(a_eState); a_param.InvokeChangeStateStart(); });
                //paramLoading.SetEndLoadStartCallback(() => { a_param.InvokeChangeStateEnd(); });
                //paramLoading.SetEndLoadCallBack(() => { a_param.InvokeChangeStateExit(); });
                //EUIFormID eUI = a_param.GetUIType() == ELoadingUIType.Cloud ? EUIFormID.UICloudLoading : EUIFormID.UICommonLoading;
                //CGameEntryMgr.UI.OpenUIForm(eUI, paramLoading);
            }
            else
            {
                a_param.InvokeChangeStateEnter();
                a_param.InvokeChangeStateStart();
                _StartChangeState(a_eState);
                a_param.InvokeChangeStateEnd();
                a_param.InvokeChangeStateExit();
            }
            return true;
        }

        private bool _CheckSceneChangeEnd()
        {
            return m_bIsChangeAllEnd;
        }
        //
        private void _StartChangeState(EGameStateID a_eState)
        {
            m_changeSceneTool.StartChangeState(m_eState, a_eState, _OnChangeSceneEnd);
        }

        private void _OnChangeSceneEnd(EGameStateID a_eState)
        {
            DRGameState dr = CGameEntryMgr.DataTable.GetDataRow<DRGameState>((int)a_eState);
            Type stateType = Type.GetType("GameFrameworkPackage." + dr.GameStateName);
            ChangeState(m_refFsm, stateType);
            Sequence seq = DOTween.Sequence();
            seq.AppendInterval(0.05f)
                .AppendCallback(() => { m_bIsChangeAllEnd = true; });
        }
    }
}

