using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Runtime;
using GameFramework.Localization;

namespace GameFrameworkPackage
{
    [Serializable]
    public class CSettingData
    {
        public bool m_bMuteMusic = false;
        public bool m_bMuteSound = false;
        public double m_fMusicVolume = 0.5f;
        public double m_fSoundVolume = 0.5f;
        public int m_nLanguageType = (int)Language.Unspecified;

        public void SetLanguage(int a_eLan)
        {
            m_nLanguageType = a_eLan;
        }

        public int GetLanaguae()
        {
            if (m_nLanguageType == (int)Language.Unspecified)
            {
                m_nLanguageType = (int)CGameEntryMgr.Localization.Language;
            }
            return m_nLanguageType;
        }

        public void SetMusicVolume(double a_fmusicV)
        {
            m_fMusicVolume = a_fmusicV;
        }

        public void SetSoundVolume(double a_fsoundV)
        {
            m_fSoundVolume = a_fsoundV;
        }
        public void SetMuteMusic(bool a_bmusicM)
        {
            m_bMuteMusic = a_bmusicM;
        }

        public void SetMuteSound(bool a_bsoundM)
        {
            m_bMuteSound = a_bsoundM;
        }

        public float GetMusicVolume()
        {
            return (float)m_fMusicVolume;
        }

        public float GetSoundVolume()
        {
            return (float)m_fSoundVolume;
        }
        public bool GetMuteMusic()
        {
            return m_bMuteMusic;
        }

        public bool GetMuteSound()
        {
            return m_bMuteSound;
        }
    }
    public class CSettingMgr : ISingleton<CSettingMgr>
    {
        public static string ms_szSavaKeySettingData = "SettingSave";
        public CSettingData m_saveSetting;
        public void Init()
        {
            _LoadFile();
            _SetSettingData();
        }
        public CSettingData GetSettingData()
        {
            if (m_saveSetting == null)
            {
                m_saveSetting = new CSettingData();
            }

            return m_saveSetting;
        }

        public void SaveSetting()
        {
            GetSettingData();
            string szData = Utility.Json.ToJson(m_saveSetting);
            CGameEntryMgr.Setting.SetString(ms_szSavaKeySettingData, szData);
            CGameEntryMgr.Setting.Save();
        }

        private void _SetSettingData()
        {
            CGameEntryMgr.Sound.SetMusicVolume(GetSettingData().GetMusicVolume());
            CGameEntryMgr.Sound.SetSoundVolume(GetSettingData().GetSoundVolume());
            CGameEntryMgr.Sound.MuteMusic(GetSettingData().GetMuteMusic());
            CGameEntryMgr.Sound.MuteSound(GetSettingData().GetMuteSound());
            CGameEntryMgr.Localization.Language = (Language)m_saveSetting.GetLanaguae();
        }

        private void _LoadFile()
        {
            GetSettingData();
            string szData = CGameEntryMgr.Setting.GetString("SettingSave", "");
            m_saveSetting = Utility.Json.ToObject<CSettingData>(szData);
        }
    }
}
