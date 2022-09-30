using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using GameFramework.Event;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GameFramework.Procedure;

namespace GameFrameworkPackage
{
    public class CProcedureCheckVersion : CProcedureBase
    {
        private bool m_bCheckVersionComplete = false;
        private bool m_bNeedUpdateVersion = false;
        private CVersionInfo m_VersionInfo = null;


        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_bCheckVersionComplete = false;
            m_bNeedUpdateVersion = false;
            m_VersionInfo = null;

            CGameEntryMgr.Event.Subscribe(WebRequestSuccessEventArgs.EventId, _OnWebRequestSuccess);
            CGameEntryMgr.Event.Subscribe(WebRequestFailureEventArgs.EventId, _OnWebRequestFail);
            string szVersionInfoPath = CABResConfig.GetHotFixResVersionFileUrl();
            CGameEntryMgr.WebRequest.AddWebRequest(szVersionInfoPath, this);
        }

        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            CGameEntryMgr.Event.Unsubscribe(WebRequestSuccessEventArgs.EventId, _OnWebRequestSuccess);
            CGameEntryMgr.Event.Unsubscribe(WebRequestFailureEventArgs.EventId, _OnWebRequestFail);

            base.OnLeave(procedureOwner, isShutdown);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            if (!m_bCheckVersionComplete)
            {
                return;
            }

            if (m_bNeedUpdateVersion)
            {
                procedureOwner.SetData<VarInt32>("VersionListLength", m_VersionInfo.VersionListLength);
                procedureOwner.SetData<VarInt32>("VersionListHashCode", m_VersionInfo.VersionListHashCode);
                procedureOwner.SetData<VarInt32>("VersionListCompressedLength", m_VersionInfo.VersionListCompressedLength);
                procedureOwner.SetData<VarInt32>("VersionListCompressedHashCode", m_VersionInfo.VersionListCompressedHashCode);
                ChangeState<CProcedureUpdateVersion>(procedureOwner);
            }
            else
            {
                ChangeState<CProcedureVerifyResources>(procedureOwner);
            }
        }

        private void _OnWebRequestSuccess(object sender, GameEventArgs e)
        {
            WebRequestSuccessEventArgs ne = e as WebRequestSuccessEventArgs;
            if (ne.UserData != this)
            {
                return;
            }

            byte[] versionInfoBytes = ne.GetWebResponseBytes();
            string szVersionInfo = Utility.Converter.GetString(versionInfoBytes);
            m_VersionInfo = Utility.Json.ToObject<CVersionInfo>(szVersionInfo);
            if (null == m_VersionInfo)
            {
                Log.Error("Parse VersionInfo failure.");
                return;
            }
            Log.Info("Latest game version is '{0} ({1})', local game version is '{2} ({3})'.", m_VersionInfo.LatestGameVersion, m_VersionInfo.InternalGameVersion, Version.GameVersion, Version.InternalGameVersion);

            // 设置资源更新下载地址
            CGameEntryMgr.Resource.UpdatePrefixUri = Utility.Path.GetRegularPath(Utility.Text.Format(m_VersionInfo.UpdatePrefixUri, m_VersionInfo.InternalResourceVersion, _GetPlatformPath()));

            m_bNeedUpdateVersion = CGameEntryMgr.Resource.CheckVersionList(m_VersionInfo.InternalResourceVersion) == CheckVersionListResult.NeedUpdate;

            if (CGameEntryMgr.PreloadComponent.GetBuildInfo().InternalGameVersion < m_VersionInfo.InternalGameVersion && m_VersionInfo.ForceUpdateGame)
            {
                COpenParamCommonConfirm param = new COpenParamCommonConfirm(CGameEntryMgr.Localization.GetStringEX("ForceUpdate.Message"));
                param.SetConfirmBtnName(CGameEntryMgr.Localization.GetStringEX("ForceUpdate.UpdateButton"));
                param.SetCancelBtnName(CGameEntryMgr.Localization.GetStringEX("ForceUpdate.QuitButton"));
                param.SetConfirmCallback(() =>
                {
                    _GotoUpdateApp();
                    GameEntry.Shutdown(ShutdownType.Quit);
                });
                param.SetCancelCallback(() =>
                {
                    GameEntry.Shutdown(ShutdownType.Quit);
                });
                CUIPreloadConfirm.Tip(param);
            }
            else
            {
                m_bCheckVersionComplete = true;
            }
        }

        private void _OnWebRequestFail(object sender, GameEventArgs e)
        {
            WebRequestFailureEventArgs ne = e as WebRequestFailureEventArgs;
            if (ne.UserData != this)
            {
                return;
            }
            CUIPreloadConfirm.TipNetErrorRestart();
        }

        private void _GotoUpdateApp()
        {
            string url = null;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            url = CGameEntryMgr.PreloadComponent.GetBuildInfo().WindowsAppUrl;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            url = CGameEntryMgr.PreloadComponent.GetBuildInfo().MacOSAppUrl;
#elif UNITY_IOS
            url = CGameEntryMgr.PreloadComponent.GetBuildInfo().IOSAppUrl;
#elif UNITY_ANDROID
            url = CGameEntryMgr.PreloadComponent.GetBuildInfo().AndroidAppUrl;
#endif
            if (!string.IsNullOrEmpty(url))
            {
                Application.OpenURL(url);
            }
        }

        private string _GetPlatformPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows";
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                    return "MacOS";
                case RuntimePlatform.IPhonePlayer:
                    return "IOS";
                case RuntimePlatform.Android:
                    return "Android";
                default:
                    throw new System.NotSupportedException(Utility.Text.Format("Platform '{0}' is not supported.", Application.platform));
            }
        }
    }
}
