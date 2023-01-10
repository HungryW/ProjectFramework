using GameFramework.Procedure;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameFrameworkPackage
{
    public class CProcedureCheckResources : CProcedureBase
    {
        private bool m_bCheckResComplete = false;
        private bool m_bNeedUpdateRes = false;
        private int m_nUpdateResCount = 0;
        private long m_nUpdateResTotalCompressLen = 0;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            m_bCheckResComplete = false;
            m_bNeedUpdateRes = false;
            m_nUpdateResCount = 0;
            m_nUpdateResTotalCompressLen = 0;

            CGameEntryMgr.Resource.CheckResources(_OnCheckResComplete);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (!m_bCheckResComplete)
            {
                return;
            }
            if (m_bNeedUpdateRes)
            {
                procedureOwner.SetData<VarInt32>("UpdateResourceCount", m_nUpdateResCount);
                procedureOwner.SetData<VarInt64>("UpdateResourceTotalCompressedLength", m_nUpdateResTotalCompressLen);
                ChangeState<CProcedureUpdateResources>(procedureOwner);
            }
            else
            {
                ChangeState<CProcedureMain>(procedureOwner);
            }
        }

        private void _OnCheckResComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalCompressedLength)
        {
            m_bCheckResComplete = true;
            m_bNeedUpdateRes = updateCount > 0;
            m_nUpdateResCount = updateCount;
            m_nUpdateResTotalCompressLen = updateTotalCompressedLength;
        }
    }
}

