using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.Fsm;
using GameFrameworkPackage;
using Defines;
using UnityGameFramework.Runtime;
using System;

namespace HotFixEntry
{
    public class ELoadingUIType
    {
        public const int Null = -1;
        public const int BlackFade = 1;

        public static EUIFormID GetUIId(int a_eType)
        {
            if (a_eType == BlackFade)
            {
                return EUIFormID.UICommonLoading;
            }
            else
            {
                return (EUIFormID)(-1);
            }
        }
    }
    public class CChangeGameStateParam
    {
        public static CChangeGameStateParam ms_empty = new CChangeGameStateParam();
        private EGameStateID m_eNextState;
        private int m_nLoadingUIType;
        private Action m_fnChangeStateEnter;
        private Action m_fnChangeStateStart;
        private Action m_fnChangeStateEnd;
        private Action m_fnChangeStateExit;
        private Func<bool> m_fnCheckChangeEnd;

        public CChangeGameStateParam()
        {
            m_nLoadingUIType = ELoadingUIType.Null;
            m_eNextState = (EGameStateID)(-1);
            m_fnChangeStateEnter = null;
            m_fnChangeStateStart = null;
            m_fnChangeStateEnd = null;
            m_fnChangeStateExit = null;
            m_fnCheckChangeEnd = null;
        }

        public static CChangeGameStateParam Create(EGameStateID a_eStateId)
        {
            CChangeGameStateParam param = new CChangeGameStateParam();
            param.SetNextGameState(a_eStateId);
            return param;
        }
        public static void Del(CChangeGameStateParam a_refParam)
        {
            a_refParam = null;
        }

        public void SetNextGameState(EGameStateID a_eStateId)
        {
            m_eNextState = a_eStateId;
        }
        public EGameStateID GetNextGameStateId()
        {
            return m_eNextState;
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

        public EUIFormID GetUIId()
        {
            return ELoadingUIType.GetUIId(m_nLoadingUIType);
        }

        public bool CheckNeedLoadingUI()
        {
            return (int)GetUIId() != -1;
        }

        public void DirectInvokeCallback()
        {
            InvokeChangeStateEnter();
            InvokeChangeStateStart();
            InvokeChangeStateEnd();
            InvokeChangeStateExit();
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

}


