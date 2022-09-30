using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;
using System.IO;
using System;

namespace GameFrameworkPackage
{
    public enum ELoginSDK
    {
        DummyLoginSDKInfo = 0,
        HuaWeiLoginSDKInfo = 1,
        DouYuLoginSDKInfo = 2,
        WeChatLoginSDKInfo = 3,
        YiJieLoginSDKInfo = 4,
        SMSLoginSDKInfo = 5,
        MsdkLoginSDKInfo = 6,
        DeviceLoginSDKInfo = 7,
        FacebookLoginSDKInfo = 8,
        YsdkLoginSDKInfo = 9,
        OppoOLLoginSDKInfo = 10,
        XiaoMiLoginSDKInfo = 11,
        UCOLLoginSDKInfo = 12,
        BilibiliLoginSDKInfo = 13,
        MeiZuOLLoginSDKInfo = 14,
        M4399OLLoginSDKInfo = 15,
        VivoOLLoginSDKInfo = 16,
        ToutiaoLoginSDKInfo = 17,
        GoogleLoginSDKInfo = 18,
        GuoPanLoginSDKInfo = 19,
        WeGameLoginSDKInfo = 20,
        SteamLoginSDKInfo = 21,
        AppleLoginSDKInfo = 22,
        UsernameLoginSDKInfo = 23,
        QQLoginSDKInfo = 24,
        TapTapLoginSDKInfo = 25,
        RocketCNLoginSDKInfo = 26,
        KwaiLoginSDKInfo = 27,
        TwitterLoginSDKInfo = 28,
        TapTapSDKLoginSDKInfo = 29,
    }

    public class CSDKLoginInfo
    {
        public string userId;
        public string session;

        public CSDKLoginInfo(string a_szUserid, string a_szSession)
        {
            userId = a_szUserid;
            session = a_szSession;
        }
    }
}
