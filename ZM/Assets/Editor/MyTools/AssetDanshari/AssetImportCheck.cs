using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace Assets.AssetDanshari.Editor
{
    public class AssetImportCheck : AssetPostprocessor
    {
        // 这个函数必须为静态的，其他随意，甚至可以是私有的，类似于update，start等生命周期，名字正确即可
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                Debug.Log("现在导入的文件的路径是" + path);
            }
        }

        void OnPostprocessTexture(Texture2D texture)
        {

            float runtimeMemorySize = Profiler.GetRuntimeMemorySizeLong(texture) / 1024;
            if (runtimeMemorySize > 1024)
            {
                Debug.LogWarning(string.Format("资源:{0}，运行占用内存: {1} KB", AssetDatabase.GetAssetPath(texture), runtimeMemorySize));
            }
        }

        void OnPreprocessTexture()
        {

        }
    }
}
