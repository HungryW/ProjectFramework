using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.Fsm;
using GameFrameworkPackage;
using Defines;
using UnityGameFramework.Runtime;
using Defines.DataTable;
using System;

namespace GameFrameworkPackage
{
    public static class CConstGameStateMgr
    {
        public static string szNextState = "NextStateID";
    }

    public partial class CGameStateMgr : ISingleton<CGameStateMgr>
    {
        GameFramework.Fsm.IFsm<CGameStateMgr> m_fsmGameState;

        public void Init()
        {
            if (m_fsmGameState != null)
            {
                return;
            }
            _InitGameFsm();
        }

        public void Clean()
        {
            if (m_fsmGameState == null)
            {
                return;
            }
            CGameEntryMgr.Fsm.DestroyFsm<CGameStateMgr>();
            m_fsmGameState = null;
        }


        public EGameStateID GetCurStateId()
        {
            return (m_fsmGameState.CurrentState as CGameStateBase).GetStateId();
        }

        public CGameStateBase GetCurState()
        {
            return m_fsmGameState.CurrentState as CGameStateBase;
        }

        public CGameStateBase GetState(EGameStateID a_eStateType)
        {
            DRGameState dr = CGameEntryMgr.DataTable.GetDataRow<DRGameState>((int)a_eStateType);
            return m_fsmGameState.GetState(Type.GetType("GameFrameworkPackage." + dr.GameStateName)) as CGameStateBase;
        }

        public void ChangeState(EGameStateID a_eState, CChangeGameStateParam a_param)
        {
            GetCurState().ChangeState(a_eState, a_param);
        }

        private void _InitGameFsm()
        {
            m_fsmGameState = CGameEntryMgr.Fsm.CreateFsm<CGameStateMgr>(this, new CGameStateBase[]
                                                                                    {
                                                                                        new CGameStateLogin(),
                                                                                        new CGameStateChangeScene(),
                                                                                        new CGameStateMain()
        });

            m_fsmGameState.SetData<VarInt32>(CConstGameStateMgr.szNextState, (int)EGameStateID.CGameStateLogin);
            m_fsmGameState.Start<CGameStateChangeScene>();
        }
    }
}


