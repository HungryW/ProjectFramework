using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GameFramework.Localization;

namespace GameFrameworkPackage
{
    public class CProcedureLaunch : CProcedureBase
    {
        protected override void OnEnter(ProcedureOwner a_pProcedureOwner)
        {
            CSDKMgr.Instance.Init(_OnSdkInitSuccess, _OnSdkInitFail);
            base.OnEnter(a_pProcedureOwner);
            CSettingMgr.Instance.Init();
            CGameEntryMgr.PreloadComponent.InitData();
            _InitCurrentVariant();
        }

        protected override void OnUpdate(ProcedureOwner a_pProcedureOwner, float a_fElapseSecond, float a_fRealElapseSecond)
        {
            ChangeState<CProcedureSplash>(a_pProcedureOwner);
        }

        private void _InitCurrentVariant()
        {
            if (CGameEntryMgr.Base.EditorResourceMode)
            {
                // 编辑器资源模式不使用 AssetBundle，也就没有变体了
                return;
            }

            string currentVariant = null;
            switch (CGameEntryMgr.Localization.Language)
            {
                case Language.English:
                    currentVariant = "en-us";
                    break;

                case Language.ChineseSimplified:
                    currentVariant = "zh-cn";
                    break;

                case Language.ChineseTraditional:
                    currentVariant = "zh-tw";
                    break;

                case Language.Korean:
                    currentVariant = "ko-kr";
                    break;

                default:
                    currentVariant = "zh-cn";
                    break;
            }

            CGameEntryMgr.Resource.SetCurrentVariant(currentVariant);
            Log.Info("Init current variant complete.");
        }


        private void _OnSdkInitSuccess()
        {

        }

        private void _OnSdkInitFail(string a_szErrorMsg)
        {

        }
    }
}