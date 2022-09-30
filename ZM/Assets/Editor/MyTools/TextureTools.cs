using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

namespace UnityGameFramework.Editor
{
    public static class TextureTools
    {
        [MenuItem("Assets/GF/TextureTools/UITexturePacker", false, 1)]
        static public void MakeAtlas()
        {
            DirectoryInfo rootDirInfo = new DirectoryInfo(Application.dataPath + "/Atlas/UI");
            foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories())
            {
                foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
                {
                    string allPath = pngFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    textureImporter.spritePackingTag = "UI";
                    AssetDatabase.SaveAssets();
                }
            }
        }

        [MenuItem("Assets/GF/纹理设置/设置压缩")]
        public static void SetTextureCompress()
        {
            var path = AssetDatabase.GetAssetPath(Selection.objects[0]);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            DirectoryInfo rootDirInfo = new DirectoryInfo(path);
            foreach (FileInfo pngFile in rootDirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                string allPath = pngFile.FullName;
                string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                textureImporter.textureCompression = TextureImporterCompression.Compressed;
                textureImporter.SaveAndReimport();
                AssetDatabase.SaveAssets();
            }
        }
    }

}