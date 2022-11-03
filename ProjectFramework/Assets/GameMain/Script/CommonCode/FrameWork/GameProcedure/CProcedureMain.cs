using Logic;
using GameFramework;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityEngine;

namespace GameFrameworkPackage
{
    public class CProcedureMain : CProcedureBase
    {
        protected override void OnEnter(ProcedureOwner a_pProcedureOwner)
        {
            base.OnEnter(a_pProcedureOwner);
            _InitData();
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            _CleanData();
        }

        private void _InitData()
        {
            //DOTween.SetTweensCapacity(200, 100);
            CSDKMgr.Instance.InitIap();
            CTimeStampMgr.Instance.Init();
        }

        protected void _CleanData()
        {
            //DOTween.Clear();
            if(CTimeStampMgr.Instance != null) CTimeStampMgr.Instance.Clean();
        }
    }
}