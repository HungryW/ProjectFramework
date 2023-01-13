using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public class CBuildInfo
    {
        public string GameVersion
        {
            get;
            set;
        }

        public int InternalGameVersion
        {
            get;
            set;
        }

        public string WindowsAppUrl
        {
            get;
            set;
        }

        public string MacOSAppUrl
        {
            get;
            set;
        }

        public string IOSAppUrl
        {
            get;
            set;
        }

        public string AndroidAppUrl
        {
            get;
            set;
        }

    }
}
