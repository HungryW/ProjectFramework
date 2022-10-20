using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

namespace GameFrameworkPackage
{
    public class CUIPackageUserData
    {
       
        public object UserData { get; set; }
        public bool NeedBlurBg { get; set; }
        public int nConfigId { get; set; }
        public bool PlayOpenAnim { get; set; }

        public CUIPackageUserData(int a_nConfigId, string a_szAssetName, bool a_bNeedBlurBg, bool a_bPlayOpenAnim, object userData)
        {
            NeedBlurBg = a_bNeedBlurBg;
            UserData = userData;
            nConfigId = a_nConfigId;
            PlayOpenAnim = a_bPlayOpenAnim;
        }

    }
}
