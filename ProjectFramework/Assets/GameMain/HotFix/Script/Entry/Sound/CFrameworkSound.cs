using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFrameworkPackage;
using Defines;
using GameFramework.Sound;

namespace HotFixEntry
{
    public class CFrameworkSound : CFrameworkBase
    {
        public static float mc_fFadeVolmueDuration = 1f;
        public static float mc_fDontBGMTime = 3f;

        private SoundComponent soundComponent { set; get; }
        private int? m_nMusicId;

        public CFrameworkSound() : base()
        {
            soundComponent = CGameEntryMgr.Sound;
        }

        public override void Clean()
        {
            soundComponent = null;
            m_nMusicId = null;
        }

        public int? PlayMusic(int a_nMusicId, object userData = null, float a_fFadeIn = 1f)
        {
            if (!CGameEntryMgr.DataTable.HasDataRow<DRMusic>(a_nMusicId))
            {
                return null;
            }
            StopMusic();
            DRMusic drMusic = CGameEntryMgr.DataTable.GetDataRow<DRMusic>(a_nMusicId);
            if (null == drMusic)
            {

                Log.Warning("Can not load music '{0}' from data table.", a_nMusicId.ToString());
                return null;
            }

            PlaySoundParams param = new PlaySoundParams
            {
                Priority = 64,
                Loop = true,
                FadeInSeconds = a_fFadeIn,
                SpatialBlend = 0f,
            };
            m_nMusicId = soundComponent.PlaySound(CAssestPathUtility.GetMusicAsset(drMusic.Name), CSoundExtension.ms_szGroupName_Music, CConstAssetPriority.MusicAsset, param, null, userData);
            return m_nMusicId;
        }

        public void StopMusic(float a_fFadeOut = 1f)
        {
            if (!m_nMusicId.HasValue || m_nMusicId == -1)
            {
                return;
            }
            soundComponent.StopSound(m_nMusicId.Value, a_fFadeOut);
            m_nMusicId = null;
        }

        public int PlaySound(int a_nSoundId, object userData = null)
        {
            return PlaySoundWithWroldPos(a_nSoundId, Vector3.zero, userData);
        }

        public int PlaySoundWithWroldPos(int a_nSoundId, Vector3 a_v3WorldPos, object userData = null)
        {
            DRSound drSound = CGameEntryMgr.DataTable.GetDataRow<DRSound>(a_nSoundId);
            if (null == drSound)
            {
                Log.Warning("Can not load sound '{0}' from data table.", a_nSoundId.ToString());
                return -1;
            }

            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = drSound.Priority,
                Loop = drSound.Loop,
                VolumeInSoundGroup = drSound.Volume,
                SpatialBlend = drSound.SpatialBlend,
            };

            return soundComponent.PlaySound(CAssestPathUtility.GetSoundAsset(drSound.AssetName), CSoundExtension.ms_szGroupName_Sound, CConstAssetPriority.SoundAsset, playSoundParams, a_v3WorldPos, userData);
        }

        public int PlayUISound(int a_nSoundId, object userData = null)
        {
            DRUISound drSound = CGameEntryMgr.DataTable.GetDataRow<DRUISound>(a_nSoundId);
            if (null == drSound)
            {
                Log.Warning("Can not load sound '{0}' from data table.", a_nSoundId.ToString());
                return -1;
            }

            PlaySoundParams playSoundParams = new PlaySoundParams
            {
                Priority = drSound.Priority,
                Loop = false,
                VolumeInSoundGroup = drSound.Volume,
                SpatialBlend = 0f,
            };

            return soundComponent.PlaySound(CAssestPathUtility.GetUISoundAsset(drSound.AssetName), CSoundExtension.ms_szGroupName_UISound, CConstAssetPriority.UISoundAsset, playSoundParams, userData);
        }

        public void MuteSound(bool a_bMute)
        {
            soundComponent.MuteSound(a_bMute);
        }

        public void MuteMusic(bool a_bMute)
        {
            soundComponent.MuteMusic(a_bMute);
        }
        public void SetSoundVolume(float a_fVolume)
        {
            soundComponent.SetSoundVolume(a_fVolume);
        }

        public float GetSoundVolume()
        {
            return soundComponent.GetSoundVolume();
        }

        public void SetMusicVolume(float a_fVolume)
        {
            soundComponent.SetMusicVolume(a_fVolume);
        }

        public float GetMusicVolume()
        {
            return soundComponent.GetMusicVolume();
        }
    }
}

