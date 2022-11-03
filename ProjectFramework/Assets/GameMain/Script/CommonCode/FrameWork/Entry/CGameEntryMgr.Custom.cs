using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class CGameEntryMgr
    {
        /// <summary>
        /// 获取游戏基础组件。
        /// </summary>
        public static CPreloadDataComponent PreloadComponent
        {
            get;
            private set;
        }

        private static void InitCustomComponents()
        {
            PreloadComponent = GameEntry.GetComponent<CPreloadDataComponent>();
        }
    }
}
