using UnityEngine;
using GameFramework;
using UnityGameFramework.Runtime;
using System.IO;
using System;
using System.Collections.Generic;

namespace GameFrameworkPackage
{
    public partial class CSDKMgr
    {
        public CSDKBase GetLogic()
        {
            return m_SdkLogic;
        }

        public void InitIap()
        {
            m_SdkLogic.InitIap();
        }
    }

    public partial class CSDKBase
    {
        public abstract void InitIap();
    }

}
