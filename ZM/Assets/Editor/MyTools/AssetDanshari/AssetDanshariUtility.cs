using System;
using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;
using System.Collections.Generic;

namespace AssetDanshari
{
    public static class AssetDanshariUtility
    {
        public static string GetPathInAsssets(string path)
        {
            if (!path.Contains("Assets"))
            {
                return path;
            }
            path = path.Remove(0, path.LastIndexOf("Assets"));
            return path;
        }

        public static string[] PathStrToArray(string paths)
        {
            paths = paths.Trim('\"');
            return paths.Split(new[] { "\" || \"" }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static List<string> PathStrToList(string paths)
        {
            paths = paths.Trim('\"');
            string[] arrpath = paths.Split(new[] { "\" || \"" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> listPath = new List<string>();
            foreach (var item in arrpath)
            {
                listPath.Add(item);
            }
            return listPath;
        }

        public static string PathArrayToStr(string[] paths)
        {
            var pathStr = string.Join("\" || \"", paths);
            return pathStr;
        }

        public static string PathArrayToListUseStr(string[] paths)
        {
            var pathStr = string.Join("\" || \"", paths) + ';';
            return pathStr;
        }

        public static bool IsPlainTextExt(string ext)
        {
            ext = ext.ToLower();
            return ext.EndsWith(".prefab") || ext.EndsWith(".unity") ||
                   ext.EndsWith(".mat") || ext.EndsWith(".asset") ||
                   ext.EndsWith(".controller") || ext.EndsWith(".anim");
        }

        public static bool IsMetaExt(string ext)
        {
            ext = ext.ToLower();
            return ext.EndsWith(".meta") || ext.EndsWith(".cs");
        }

        public static string GetSaveFilePath(string key)
        {
            string path = EditorPrefs.GetString("RecentSaveFilePath" + key, Application.dataPath + key + ".csv");
            path = EditorUtility.SaveFilePanel("Save File ..", Path.GetDirectoryName(path), Path.GetFileName(path), "csv");
            path = path.Replace('\\', '/');
            if (!string.IsNullOrEmpty(path))
            {
                EditorPrefs.SetString("RecentSaveFilePath" + key, path);
            }
            return path;
        }

        public static void SaveFileText(string path, string text)
        {
            try
            {
                File.WriteAllText(path, text, Encoding.UTF8);
                System.Diagnostics.Process.Start(path);
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog(AssetDanshariStyle.Get().errorTitle, e.Message, AssetDanshariStyle.Get().sureStr);
                throw;
            }
        }

        public static void DisplayThreadProgressBar(int totalFiles, int filesFinished)
        {
            string msg = String.Format(@"{0} ({1}/{2})", AssetDanshariStyle.Get().progressTitle,
                (filesFinished + 1).ToString(), totalFiles.ToString());
            EditorUtility.DisplayProgressBar(AssetDanshariStyle.Get().progressTitle, msg, (filesFinished + 1) * 1f / totalFiles);
        }

        public static string GetFileSizeStr(long a_nSize)
        {
            string szSize = string.Empty;
            if (a_nSize >= (1 << 20))
            {
                szSize = string.Format("{0:F} MB", a_nSize / 1024f / 1024f);
            }
            else if (a_nSize >= (1 << 10))
            {
                szSize = string.Format("{0:F} KB", a_nSize / 1024f);
            }
            else
            {
                szSize = string.Format("{0:F} B", a_nSize);
            }
            return szSize;
        }

        public static string GetFileSizeStr(ulong a_nSize)
        {
            string szSize = string.Empty;
            if (a_nSize >= (1 << 20))
            {
                szSize = string.Format("{0:F} MB", a_nSize / 1024f / 1024f);
            }
            else if (a_nSize >= (1 << 10))
            {
                szSize = string.Format("{0:F} KB", a_nSize / 1024f);
            }
            else
            {
                szSize = string.Format("{0:F} B", a_nSize);
            }
            return szSize;
        }
    }
}