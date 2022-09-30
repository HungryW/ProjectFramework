using GameFramework;
using GameFramework.Procedure;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public class CAnalyticsMainProject : ISingleton<CAnalyticsMainProject>
    {
        private Dictionary<string, bool> m_mapRecordedGameProgress = new Dictionary<string, bool>();
        private int m_nGameProgressIdx = 0;


        public void LogGameInitInfo()
        {
            string szResVersion = CGameEntryMgr.Base.EditorResourceMode ? "Unavailable in editor resource mode" : (string.IsNullOrEmpty(CGameEntryMgr.Resource.ApplicableGameVersion) ? "Unknown" : Utility.Text.Format("{0} ({1})", CGameEntryMgr.Resource.ApplicableGameVersion, CGameEntryMgr.Resource.InternalResourceVersion.ToString()));
            CSDKMgr.Instance.LogEvent("GameAppInfo", "ResVersion", szResVersion, null);
            string szSystemLanguage = Application.systemLanguage.ToString();
            CSDKMgr.Instance.LogEvent("GameAppInfo", "SystemLanguage", szSystemLanguage, null);
            string szMemorySize = Application.systemLanguage.ToString();
            CSDKMgr.Instance.LogEvent("GameAppInfo", "MemorySize", szMemorySize, null);
        }

        public void LogProducePassTime(ProcedureBase a_procedure, float a_fTime)
        {
            CSDKMgr.Instance.LogEvent("GameProducePassTime", a_procedure.GetType().Name, a_fTime.ToString(), null);
        }

        public void LogGameProgress(string a_szProgressName, string a_szProgressIdx, Hashtable a_hashParam)
        {
            if (m_mapRecordedGameProgress.ContainsKey(a_szProgressName))
            {
                return;
            }
            m_mapRecordedGameProgress.Add(a_szProgressName, true);
            if (string.IsNullOrEmpty(a_szProgressIdx))
            {
                a_szProgressIdx = (m_nGameProgressIdx * 10).ToString();
                m_nGameProgressIdx++;
            }

            CSDKMgr.Instance.LogEvent("GameProgress", a_szProgressName, a_szProgressIdx, a_hashParam);
        }
    }
}

