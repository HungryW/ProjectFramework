using GameFramework;
using GameFramework.DataTable;
using GameFramework.Sound;
using UnityGameFramework.Runtime;
using Defines.DataTable;
using UnityEngine;

namespace GameFrameworkPackage
{
    public static class CSoundExtension
    {
        public static string ms_szGroupName_Music = "Music";
        public static string ms_szGroupName_Sound = "Sound";
        public static string ms_szGroupName_UISound = "UISound";


        public static float mc_fFadeVolmueDuration = 1f;
        public static float mc_fDontBGMTime = 3f;
        private static int? ms_nMusicId = null;

        public static int? PlayMusic(this SoundComponent soundComponent, int a_nMusicId, object userData = null, bool a_bIsLoop = false, float a_fFadeIn = 1f)
        {
            if (!CGameEntryMgr.DataTable.HasDataRow<DRMusic>(a_nMusicId))
            {
                return null;
            }
            soundComponent.StopMusic();
            DRMusic drMusic = CGameEntryMgr.DataTable.GetDataRow<DRMusic>(a_nMusicId);
            if (null == drMusic)
            {

                Log.Warning("Can not load music '{0}' from data table.", a_nMusicId.ToString());
                return null;
            }

            PlaySoundParams param = new PlaySoundParams
            {
                Priority = 64,
                Loop = a_bIsLoop,
                FadeInSeconds = a_fFadeIn,
                SpatialBlend = 0f,
            };
            ms_nMusicId = soundComponent.PlaySound(CAssestPathUtility.GetMusicAsset(drMusic.AssetName), ms_szGroupName_Music, CConstAssetPriority.MusicAsset, param, null, userData);
            return ms_nMusicId;
        }

        public static void StopMusic(this SoundComponent soundComponent, float a_fFadeOut = 1f)
        {
            if (!ms_nMusicId.HasValue || ms_nMusicId == -1)
            {
                return;
            }
            soundComponent.StopSound(ms_nMusicId.Value, a_fFadeOut);
            ms_nMusicId = null;
        }

        public static int PlaySound(this SoundComponent soundComponent, int a_nSoundId, object userData = null)
        {
            return soundComponent.PlaySound(a_nSoundId, Vector3.zero, userData);
        }

        public static int PlaySound(this SoundComponent soundComponent, int a_nSoundId, Vector3 a_v3WorldPos, object userData = null)
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

            return soundComponent.PlaySound(CAssestPathUtility.GetSoundAsset(drSound.AssetName), ms_szGroupName_Sound, CConstAssetPriority.SoundAsset, playSoundParams, a_v3WorldPos, userData);
        }


        public static int PlayUISound(this SoundComponent soundComponent, int a_nSoundId, object userData = null)
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

            return soundComponent.PlaySound(CAssestPathUtility.GetUISoundAsset(drSound.AssetName), ms_szGroupName_UISound, CConstAssetPriority.UISoundAsset, playSoundParams, userData);
        }

        public static bool IsMuted(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return true;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return true;
            }

            return soundGroup.Mute;
        }

        public static void Mute(this SoundComponent soundComponent, string soundGroupName, bool mute)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }
            soundGroup.Mute = mute;
        }

        public static float GetVolume(this SoundComponent soundComponent, string soundGroupName)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return 0f;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return 0f;
            }

            return soundGroup.Volume;
        }

        public static void SetVolume(this SoundComponent soundComponent, string soundGroupName, float volume)
        {
            if (string.IsNullOrEmpty(soundGroupName))
            {
                Log.Warning("Sound group is invalid.");
                return;
            }

            ISoundGroup soundGroup = soundComponent.GetSoundGroup(soundGroupName);
            if (soundGroup == null)
            {
                Log.Warning("Sound group '{0}' is invalid.", soundGroupName);
                return;
            }

            soundGroup.Volume = volume;
        }

        public static void MuteSound(this SoundComponent soundComponent, bool a_bMute)
        {
            soundComponent.Mute(ms_szGroupName_Sound, a_bMute);
            soundComponent.Mute(ms_szGroupName_UISound, a_bMute);
        }

        public static void MuteMusic(this SoundComponent soundComponent, bool a_bMute)
        {
            soundComponent.Mute(ms_szGroupName_Music, a_bMute);
        }


        public static void SetSoundVolume(this SoundComponent soundComponent, float a_fVolume)
        {
            soundComponent.SetVolume(ms_szGroupName_Sound, a_fVolume);
            soundComponent.SetVolume(ms_szGroupName_UISound, a_fVolume);
        }

        public static float GetSoundVolume(this SoundComponent soundComponent)
        {
            return soundComponent.GetVolume(ms_szGroupName_UISound);
        }

        public static void SetMusicVolume(this SoundComponent soundComponent, float a_fVolume)
        {
            soundComponent.SetVolume(ms_szGroupName_Music, a_fVolume);
        }

        public static float GetMusicVolume(this SoundComponent soundComponent)
        {
            return soundComponent.GetVolume(ms_szGroupName_Music);
        }
    }
}

