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

namespace HotFixEntry
{
    public partial class CGameStateMgr : CFrameworkBase
    {
        public static string szNextState = "NextStateID";

        private IFsm<CGameStateMgr> m_fsmGameState;
        private CChangeStateTool m_changeStateTool;
        public CGameStateMgr()
        {
            m_changeStateTool = new CChangeStateTool(this);
            m_fsmGameState = CGameEntryMgr.Fsm.CreateFsm(this, new CGameStateBase[]
                                                                                 {
                                                                                        new CGameStatePreloadResource(),
                                                                                        new CGameStateLogin(),
                                                                                        new CGameStateMain()
                                                                                 });
        }
        public void Start()
        {
            m_fsmGameState.Start<CGameStatePreloadResource>();
        }
        public override void Clean()
        {
            if (m_fsmGameState == null)
            {
                return;
            }
            CGameEntryMgr.Fsm.DestroyFsm<CGameStateMgr>();
            m_fsmGameState = null;
        }

        public void ChangeState(CChangeGameStateParam a_param)
        {
            m_changeStateTool.ChangeState(a_param);
        }

        public EGameStateID GetCurStateId()
        {
            CGameStateBase curState = GetCurState();
            return (m_fsmGameState.CurrentState as CGameStateBase).GetStateId();
        }

        public CGameStateBase GetCurState()
        {
            return m_fsmGameState.CurrentState as CGameStateBase;
        }

        private CGameStateBase _GetState(EGameStateID a_eStateType)
        {
            DRGameState dr = CGameEntryMgr.DataTable.GetDataRow<DRGameState>((int)a_eStateType);
            return m_fsmGameState.GetState(dr.GetStateType()) as CGameStateBase;
        }

      
    }
}


