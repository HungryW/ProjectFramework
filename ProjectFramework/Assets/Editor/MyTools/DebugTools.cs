using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace Tools
{
    //工具菜单
    public sealed partial class DebugTool
    {
        private static string ms_szNotPreloadUI = "NotPreloadUI";
        private static string ms_szNotPreloadSpine = "NotPreloadSpine";
        public static void EnablePreload()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(ms_szNotPreloadUI);
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(ms_szNotPreloadSpine);

        }

        public static void DisablePreload()
        {
            ScriptingDefineSymbols.AddScriptingDefineSymbol(ms_szNotPreloadUI);
            ScriptingDefineSymbols.AddScriptingDefineSymbol(ms_szNotPreloadSpine);
        }
    }
}
