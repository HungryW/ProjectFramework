using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public static class CABResConfig
    {
        public static string ms_szHotFixUrl_Local = "http://192.168.0.111";
        public static string ms_szHotFixUrl_PublicTestNoSDK = "http://unusualbroadcast.s3.ap-southeast-1.amazonaws.com/Variant/NoSDK";
        public static string ms_szHotFixUrl_PublicTest = "https://unusual-southeast-base.oss-ap-southeast-1.aliyuncs.com/unusual/HotFix/dev2207";
        public static string ms_szHotFixUrl_Public = "https://unusual-southeast-base.oss-ap-southeast-1.aliyuncs.com/unusual/HotFix/PublicTest2207";

        
        public static string ms_szHotFixResDicName = "res";
        public static string ms_szHotFixVersionDicName = "version";

        public static string GetHotFixResRootUrl()
        {
            string szRootUrl = ms_szHotFixUrl_PublicTest;
#if HotfixServerLocal
            szRootUrl = ms_szHotFixUrl_Local;
#elif HotfixServerRemoteTest
            szRootUrl = ms_szHotFixUrl_PublicTest;
#elif HotfixServerRemoteTestNoSDk
            szRootUrl = ms_szHotFixUrl_PublicTestNoSDK;
#elif HotfixServerRemote
            szRootUrl = ms_szHotFixUrl_Public;
#endif
            return szRootUrl;
        }

        public static string GetHotFixResVersionFileUrl()
        {
            string szRootUrl = GetHotFixResRootUrl();
            string szUrl = Utility.Text.Format("{0}/{1}/{2}_version.txt", szRootUrl, ms_szHotFixVersionDicName, _GetPlatformVersionPath());
            string szVersionInfoPath = Utility.Path.GetRegularPath(szUrl);
            return szVersionInfoPath;
        }

        private static string _GetPlatformVersionPath()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return "Windows64";
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
