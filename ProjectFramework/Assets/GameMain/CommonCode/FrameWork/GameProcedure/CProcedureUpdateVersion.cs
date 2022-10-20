using GameFramework.Procedure;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameFrameworkPackage
{
    public class CProcedureUpdateVersion : CProcedureBase
    {
        private bool m_bUpdateVersionComplete = false;
        private UpdateVersionListCallbacks m_UpdateVersionListCallbacks = null;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            m_UpdateVersionListCallbacks = new UpdateVersionListCallbacks(_OnUpdateVersionListSuccess, OnUpdateVersionListFailure);
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_bUpdateVersionComplete = false;
            CGameEntryMgr.Resource.UpdateVersionList(procedureOwner.GetData<VarInt32>("VersionListLength")
                                                    , procedureOwner.GetData<VarInt32>("VersionListHashCode")
                                                    , procedureOwner.GetData<VarInt32>("VersionListCompressedLength")
                                                    , procedureOwner.GetData<VarInt32>("VersionListCompressedHashCode")
                                                    , m_UpdateVersionListCallbacks
                                                    );
            procedureOwner.RemoveData("VersionListLength");
            procedureOwner.RemoveData("VersionListHashCode");
            procedureOwner.RemoveData("VersionListCompressedLength");
            procedureOwner.RemoveData("VersionListCompressedHashCode");

        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_bUpdateVersionComplete)
            {
                return;
            }

            ChangeState<CProcedureVerifyResources>(procedureOwner);
        }

        private void _OnUpdateVersionListSuccess(string a_szDownloadPath, string a_szDownloadUri)
        {
            m_bUpdateVersionComplete = true;
            Log.Info("Update version list from '{0}' success.", a_szDownloadUri);
        }

        private void OnUpdateVersionListFailure(string downloadUri, string errorMessage)
        {
            CUIPreloadConfirm.TipNetErrorRestart();
            Log.Warning("Update version list from '{0}' failure, error message is '{1}'.", downloadUri, errorMessage);
        }
    }
}

