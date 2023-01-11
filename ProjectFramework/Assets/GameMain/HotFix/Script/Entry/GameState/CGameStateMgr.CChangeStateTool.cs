using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.Fsm;
using GameFrameworkPackage;
using Defines;
using UnityGameFramework.Runtime;
using System;
using DG.Tweening;
using HotFixEntry.UI;

namespace HotFixEntry
{
    public partial class CGameStateMgr
    {
        private class CChangeStateTool
        {
            private bool m_bIsChangingState;
            private bool m_bIsChangeAllEnd;
            private CGameStateMgr m_refStateMgr;
            private CGameStateChangeSceneTool m_changeSceneTool;

            public CChangeStateTool(CGameStateMgr a_refStateMgr)
            {
                m_refStateMgr = a_refStateMgr;
                m_bIsChangeAllEnd = false;
                m_bIsChangingState = false;
                m_changeSceneTool = new CGameStateChangeSceneTool();
            }
            public void ChangeState(CChangeGameStateParam a_param)
            {
                CGameStateBase curState = m_refStateMgr.GetCurState();
                if (a_param.GetNextGameStateId() == curState.GetStateId())
                {
                    a_param.DirectInvokeCallback();
                    return;
                }
                CGameStateBase nextState = m_refStateMgr._GetState(a_param.GetNextGameStateId());
                if (nextState == null)
                {
                    a_param.DirectInvokeCallback();
                    return;
                }
                if (!nextState.CheckCanEnter())
                {
                    a_param.DirectInvokeCallback();
                    return;
                }
                if (m_bIsChangingState)
                {
                    Log.Error("ChangeGameState  失败, 当前正在切换状态.同一时间只能调用一次状态切换接口,本次调用将被忽略");
                    return;
                }
                m_bIsChangingState = true;
                m_bIsChangeAllEnd = false;
                a_param = a_param == null ? CChangeGameStateParam.ms_empty : a_param;
                if (a_param.CheckNeedLoadingUI())
                {
                    COpenParamCommonLoading paramLoading = new COpenParamCommonLoading();
                    paramLoading.SetCheckLoadEndFn(() => { return m_bIsChangeAllEnd && a_param.IsChangeEnd(); });
                    paramLoading.SetStartLoadCallback(() => { a_param.InvokeChangeStateEnter(); });
                    paramLoading.SetStartLoadEndCallback(() => { _StartChangeState(a_param.GetNextGameStateId()); a_param.InvokeChangeStateStart(); });
                    paramLoading.SetEndLoadStartCallback(() => { a_param.InvokeChangeStateEnd(); });
                    paramLoading.SetEndLoadCallBack(() => { a_param.InvokeChangeStateExit(); });
                    CHotFixEntry.UI.OpenUINoHideAnim(a_param.GetUIId(), paramLoading);
                }
                else
                {
                    a_param.InvokeChangeStateEnter();
                    a_param.InvokeChangeStateStart();
                    _StartChangeState(a_param.GetNextGameStateId());
                    a_param.InvokeChangeStateEnd();
                    a_param.InvokeChangeStateExit();
                }

            }
            private void _StartChangeState(EGameStateID a_eNextStateId)
            {
                m_changeSceneTool.StartChangeState(m_refStateMgr.GetCurStateId(), a_eNextStateId, _OnChangeSceneEnd);
            }

            private void _OnChangeSceneEnd(EGameStateID a_eState)
            {
                m_refStateMgr.GetCurState().ChangeGameState(a_eState);
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(0.1f)
                    .AppendCallback(_OnGameStateChangeEnd);
            }
            private void _OnGameStateChangeEnd()
            {
                m_bIsChangeAllEnd = true;
                m_bIsChangingState = false;
            }
        }
    }
}


