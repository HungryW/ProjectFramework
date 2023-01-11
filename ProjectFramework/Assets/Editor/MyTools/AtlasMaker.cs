using UnityEngine;

using UnityEngine.Events;

using UnityEngine.UI;

using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UI;
using System.IO;
using System.Linq;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor.PackageManager;
using UnityGameFramework.Editor;

namespace Tools
{
    public class AtlasMaker : EditorWindow
    {
        private static string ms_szBaseSrcDir = Application.dataPath + "/GameMain/Atlas";
        private static string ms_szBaseDesDir = Application.dataPath + "/GameMain/AtlasMap";

        public static void CreateAtlasByAllFolders()
        {
            CreateAtlasByFolders(ms_szBaseSrcDir + "/UI", ms_szBaseDesDir + "/UI");
            CreateAtlasByFolders(ms_szBaseSrcDir + "/UISprite", ms_szBaseDesDir + "/UISprite");
        }

        static void CreateAtlasByFolders(string a_szSrcDir, string a_szDesDir)
        {
            if (Directory.Exists(a_szDesDir))
            {
                Directory.Delete(a_szDesDir, true);
                AssetDatabase.Refresh();
            }
            DirectoryInfo rootDirInfo = new DirectoryInfo(a_szSrcDir);
            //add folders
            List<Object> folders = new List<Object>();
            foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
            {
                folders.Clear();
                if (dirInfo != null)
                {
                    string assetPath = dirInfo.FullName.Substring(dirInfo.FullName.IndexOf("Assets"));
                    var o = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPath);
                    if (IsPackable(o))
                        folders.Add(o);
                }
                SpriteAtlas sptAtlas = CreateAtlas(a_szDesDir, dirInfo.Name);
                Debug.Log(sptAtlas.tag);
                AddPackAtlas(sptAtlas, folders.ToArray());
            }

            //add texture by your self
        }


        public static void CreateAtlasByAllFoldersRecursive()
        {
            _CreateAtlasByFoldersRecursive(ms_szBaseSrcDir + "/UI", ms_szBaseDesDir + "/UI");
            _CreateAtlasByFoldersRecursive(ms_szBaseSrcDir + "/UISprite", ms_szBaseDesDir + "/UISprite");
        }


        static void _CreateAtlasByFoldersRecursive(string a_szSrcDir, string a_szDesDir)
        {
            if (Directory.Exists(a_szDesDir))
            {
                FileTool.DeleteDirectory(a_szDesDir);
                AssetDatabase.Refresh();
            }
            _CreateOneAtlas(a_szSrcDir, "", a_szDesDir);
        }

        private static void _CreateOneAtlas(string a_szSrcDir, string a_szName, string a_szDesDir)
        {
            DirectoryInfo rootDirInfo = new DirectoryInfo(a_szSrcDir);
            DirectoryInfo[] arrDr = rootDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
            a_szName = a_szName + rootDirInfo.Name;
            if (arrDr.Length == 0)
            {
                List<Object> folders = new List<Object>();
                string assetPath = rootDirInfo.FullName.Substring(rootDirInfo.FullName.IndexOf("Assets"));
                var o = AssetDatabase.LoadAssetAtPath<DefaultAsset>(assetPath);
                folders.Add(o);
                SpriteAtlas sptAtlas = CreateAtlas(a_szDesDir, a_szName);
                AddPackAtlas(sptAtlas, folders.ToArray());
            }
            else
            {
                FileInfo[] arrFile = rootDirInfo.GetFiles("*", SearchOption.TopDirectoryOnly);
                SpriteAtlas sptAtlas = null;
                List<Object> listTexture = new List<Object>();
                foreach (var file in arrFile)
                {
                    string szAssetPath = file.FullName.Substring(file.FullName.IndexOf("Assets"));
                    Object asset = AssetDatabase.LoadAssetAtPath<Object>(szAssetPath);
                    if (IsPackable(asset))
                    {
                        if (null == sptAtlas)
                        {
                            sptAtlas = CreateAtlas(a_szDesDir, a_szName);
                        }
                        listTexture.Add(asset);
                    }
                }

                if (sptAtlas != null)
                {
                    AddPackAtlas(sptAtlas, listTexture.ToArray());
                }

                foreach (var dr in arrDr)
                {
                    _CreateOneAtlas(dr.FullName, a_szName, a_szDesDir);
                }
            }
        }

        static bool IsPackable(Object o)
        {
            return o != null && (o.GetType() == typeof(Sprite) || o.GetType() == typeof(Texture2D) || (o.GetType() == typeof(DefaultAsset) && ProjectWindowUtil.IsFolder(o.GetInstanceID())));
        }

        static void AddPackAtlas(SpriteAtlas atlas, Object[] spt)
        {
            MethodInfo methodInfo = System.Type
                 .GetType("UnityEditor.U2D.SpriteAtlasExtensions, UnityEditor")
                 .GetMethod("Add", BindingFlags.Public | BindingFlags.Static);
            if (methodInfo != null)
                methodInfo.Invoke(null, new object[] { atlas, spt });
            else
                Debug.Log("methodInfo is null");
            PackAtlas(atlas);
        }

        static void PackAtlas(SpriteAtlas atlas)
        {
            UnityEditor.U2D.SpriteAtlasUtility.PackAtlases(new[] { atlas }, EditorUserBuildSettings.activeBuildTarget);
        }

        public static SpriteAtlas CreateAtlas(string a_szDesDir, string atlasName)
        {
            string szYamlFormat = @"%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!687078895 &4343727234628468602
SpriteAtlas:
  m_ObjectHideFlags: 0
  m_PrefabParentObject: {fileID: 0}
  m_PrefabInternal: {fileID: 0}
  m_Name: {0}
  m_EditorData:
    textureSettings:
      serializedVersion: 2
      anisoLevel: 1
      compressionQuality: 50
      maxTextureSize: 2048
      textureCompression: 1
      filterMode: 1
      generateMipMaps: 0
      readable: 0
      crunchedCompression: 0
      sRGB: 1
    platformSettings: []
    packingParameters:
      serializedVersion: 2
      padding: 4
      blockOffset: 1
      allowAlphaSplitting: 0
      enableRotation: 0
      enableTightPacking: 0
    variantMultiplier: 1
    packables: []
    totalSpriteSurfaceArea: 0
    bindAsDefault: 1
  m_MasterAtlas: {fileID: 0}
  m_PackedSprites: []
  m_PackedSpriteNamesToIndex: []
  m_Tag: New Sprite Atlas
  m_IsVariant: 0

";
            string szContent = szYamlFormat.Replace("{0}", atlasName);
            AssetDatabase.Refresh();
            if (!Directory.Exists(a_szDesDir))
            {
                Directory.CreateDirectory(a_szDesDir);
                AssetDatabase.Refresh();
            }
            string filePath = a_szDesDir + "/" + atlasName + ".spriteatlas";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                AssetDatabase.Refresh();
            }
            FileStream fs = new FileStream(filePath, FileMode.CreateNew);
            byte[] bytes = new UTF8Encoding().GetBytes(szContent);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
            AssetDatabase.Refresh();
            string szFullPath = (filePath).Substring((filePath).IndexOf("Assets"));
            return AssetDatabase.LoadAssetAtPath<SpriteAtlas>(szFullPath);
        }
    }
}
