
using GameFramework;
using GameFrameworkPackage;
using LitJson;
using System;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;
using System.Text;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace GameFrameworkPackageEditor
{
    public static class CBuildInfoTools
    {
        private static string ms_szBuildInfoPath = Application.dataPath + "/FrameWork/GameframeWork/Configs/BuildInfo.txt";
        public static int ReadBuildInternalGameVersion()
        {
            if (!File.Exists(ms_szBuildInfoPath))
            {
                return _GetExceptionVersion();
            }
            using (StreamReader streamReader = File.OpenText(ms_szBuildInfoPath))
            {
                JsonReader jsonReader = new JsonReader(streamReader);
                CBuildInfo buildInfo = JsonMapper.ToObject<CBuildInfo>(jsonReader);
                if (null == buildInfo)
                {
                    return _GetExceptionVersion();
                }
                return buildInfo.InternalGameVersion;
            }
        }

        private static int _GetExceptionVersion()
        {
            UnityEngine.Debug.LogError("ReadBuildInternalGameVersion Fail");
#if UNITY_ANDROID
                return PlayerSettings.Android.bundleVersionCode;
#elif UNITY_IOS
                return int.Parse(PlayerSettings.iOS.buildNumber);
#else
            return 0;
#endif
        }

        public static void AutoIncrementInternalGameVersion()
        {
            int nCur = ReadBuildInternalGameVersion();
            nCur++;
            WriteBuildInternalGameVersion(nCur);
        }

        public static void WriteBuildInternalGameVersion(int a_nVersion)
        {
            if (!File.Exists(ms_szBuildInfoPath))
            {
                return;
            }
            CBuildInfo buildInfo = null;
            using (StreamReader streamReader = File.OpenText(ms_szBuildInfoPath))
            {
                JsonReader jsonReader = new JsonReader(streamReader);
                buildInfo = JsonMapper.ToObject<CBuildInfo>(jsonReader);
                if (null == buildInfo)
                {
                    return;
                }
            }
            buildInfo.InternalGameVersion = a_nVersion;
            using (StreamWriter streamWriter = File.CreateText(ms_szBuildInfoPath))
            {
                JsonWriter jw = new JsonWriter();
                jw.PrettyPrint = true;
                JsonMapper.ToJson(buildInfo, jw);
                streamWriter.Write(jw.ToString());
            }
        }
    }
}

