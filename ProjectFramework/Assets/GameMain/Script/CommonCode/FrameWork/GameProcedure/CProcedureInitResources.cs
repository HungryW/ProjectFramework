using GameFramework.Procedure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameFrameworkPackage
{
    public class CProcedureInitResources : CProcedureBase
    {
        private bool m_bInitResComplete = false;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_bInitResComplete = false;
            CGameEntryMgr.Resource.InitResources(_OnInitResComplete);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSed, float realElapseSed)
        {
            base.OnUpdate(procedureOwner, elapseSed, realElapseSed);
            if (!m_bInitResComplete)
            {
                return;
            }
            ChangeState<CProcedureMain>(procedureOwner);
        }

        private void _OnInitResComplete()
        {
            m_bInitResComplete = true;
        }
    }
}
