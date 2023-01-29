using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework;
using GameFramework.Resource;
using System;
using HybridCLR;

namespace GameFrameworkPackage
{
    public static class CHotFixSetting
    {
        public static string ms_szHotFixEntryNamespace = "HotFixEntry";

        public static string ms_szHotFixRootPath = "Assets/GameMain/HotFix";
        public static string ms_szDllResSuffix = "bytes";
        public static string ms_szDllSuffix = "dll";

        public static string ms_szDllTypeHotFix = "HotFix";
        public static string ms_szDllTypeAOT = "AOT";
        public static string[] ms_arrHotFixDllName = {
                                                        "Define"
                                                       ,"HotFixEntry"
                                                       ,"HotFixLogic"
                                                       ,"Assembly-CSharp"
                                                     };

        public static string[] ms_arrAOTDllName = {  "mscorlib"
                                                    ,"System.Core"
                                                    ,"GameFramework"
                                                    ,"UnityGameFramework.Runtime"
                                                    ,"GameMain"
                                                  };

        public static string GetHotFixEntryClassFullName(string a_szClassName)
        {
            return _GetHotFixClassFullName(ms_szHotFixEntryNamespace, a_szClassName);
        }

        private static string _GetHotFixClassFullName(string a_szNameSpace, string a_szClassName)
        {
            return Utility.Text.Format("{0}.{1}", a_szNameSpace, a_szClassName);

        }
        public static string GetHotFixDefineDllName()
        {
            return ms_arrHotFixDllName[0];
        }
        public static string GetHotFixEntryDllName()
        {
            return ms_arrHotFixDllName[1];
        }

        public static string GetHotFixLogicDllName()
        {
            return ms_arrHotFixDllName[2];
        }

        public static string GetDllName(string a_szName)
        {
            return Utility.Text.Format("{0}.{1}", a_szName, ms_szDllSuffix);
        }

        public static string GetDllResName(string a_szName)
        {
            return Utility.Text.Format("{0}.{1}", GetDllName(a_szName), ms_szDllResSuffix);
        }

        public static string GetDllResFullPathName(string a_szDllTypeName, string a_szName)
        {
            return Utility.Text.Format("{0}/{1}", _GetHotFixDllRootPath(a_szDllTypeName), GetDllResName(a_szName));
        }

        public static string GetHotFixDllRootPath()
        {
            return _GetHotFixDllRootPath(ms_szDllTypeHotFix);
        }

        public static string GetAOTDllRootPath()
        {
            return _GetHotFixDllRootPath(ms_szDllTypeAOT);
        }

        private static string _GetHotFixDllRootPath(string a_szDllTypeName)
        {
            return Utility.Text.Format("{0}/Dll/{1}", ms_szHotFixRootPath, a_szDllTypeName);
        }

        public static string[] GetAllHotFixResFullPath()
        {
            return GetAllResFullPath(ms_szDllTypeHotFix, ms_arrHotFixDllName);
        }

        public static string[] GetAllAOTResFullPath()
        {
            return GetAllResFullPath(ms_szDllTypeAOT, ms_arrAOTDllName);
        }

        private static string[] GetAllResFullPath(string a_szDllTypeName, string[] a_arrDllName)
        {
            string[] arrPath = new string[a_arrDllName.Length];
            for (int i = 0; i < arrPath.Length; i++)
            {
                arrPath[i] = GetDllResFullPathName(a_szDllTypeName, a_arrDllName[i]);
            }
            return arrPath;
        }
    }
}

