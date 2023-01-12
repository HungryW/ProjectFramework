using GameFrameworkPackage;
using HybridCLR.Editor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Tools
{
    public static class CHotFixTool
    {
        public static void CopyHybridDllToTarget()
        {
            _CopyDllFileToTarget(SettingsUtil.GetHotUpdateDllsOutputDirByTarget(EditorUserBuildSettings.activeBuildTarget), CHotFixSetting.ms_arrHotFixDllName, CHotFixSetting.GetHotFixDllRootPath());
        }

        private static void _CopyDllFileToTarget(string sourceFolder, string[] fileNames, string targetFolder)
        {
            foreach (string fileName in fileNames)
            {
                // Construct the source and target file paths.
                string sourceFile = Path.Combine(sourceFolder, string.Format("{0}.dll", fileName));
                string targetFile = Path.Combine(targetFolder, string.Format("{0}.dll.bytes", fileName));
                // Check if the file exists in the source folder.
                if (File.Exists(sourceFile))
                {
                    // Check if the target folder exists. If not, create it.
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }
                    // Copy the file to the target folder.
                    File.Copy(sourceFile, targetFile, true);
                    //print the log 
                    Debug.Log(string.Format("Copied {0} to {1}", sourceFile, targetFile));
                }
            }
        }
    }
}
