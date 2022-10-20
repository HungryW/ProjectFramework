using GameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    /// <summary>
    /// 默认版本号辅助器。
    /// </summary>
    public class CVersionHelper : Version.IVersionHelper
    {
        /// <summary>
        /// 获取游戏版本号。
        /// </summary>
        public string GameVersion
        {
            get
            {
                return Application.version;
            }
        }

        /// <summary>
        /// 获取内部游戏版本号。
        /// </summary>
        public int InternalGameVersion
        {
            get
            {
                return 0;
            }
        }
    }
}
