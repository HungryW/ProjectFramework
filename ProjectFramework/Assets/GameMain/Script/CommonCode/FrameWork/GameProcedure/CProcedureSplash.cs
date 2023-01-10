
using GameFramework;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using System.Collections.Generic;
using GameFramework.Event;
using GameFramework.Resource;
using UnityEngine;
using Logic;

namespace GameFrameworkPackage
{
    public class CProcedureSplash : CProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            CGameEntryMgr.PreloadComponent.CreateUIBg();
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            _ChangeState(procedureOwner);
        }

        private void _ChangeState(ProcedureOwner procedureOwner)
        {
            if (CGameEntryMgr.Base.EditorResourceMode)
            {
                // 编辑器模式
                ChangeState<CProcedureMain>(procedureOwner);
            }
            else if (CGameEntryMgr.Resource.ResourceMode == ResourceMode.Package)
            {
                // 单机模式
                ChangeState<CProcedureInitResources>(procedureOwner);
            }
            else
            {
                // 可更新模式
                ChangeState<CProcedureCheckVersion>(procedureOwner);
            }
        }
    }
}