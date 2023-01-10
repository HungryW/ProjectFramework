using GameFramework;
using GameFramework.DataTable;
using GameFramework.Sound;
using UnityGameFramework.Runtime;
using UnityEngine;

namespace GameFrameworkPackage
{
    public static class CSoundExtension
    {
        public static string ms_szGroupName_Music = "Music";
        public static string ms_szGroupName_Sound = "Sound";
        public static string ms_szGroupName_UISound = "UISound";

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

