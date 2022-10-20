
using GameFramework;
using GameFrameworkPackage;
using LitJson;
using System;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;
using UnityGameFramework.Editor.ResourceTools;

namespace GameFrameworkPackageEditor
{
    public class CBuildEventHandlerNotHotFixNormal : IBuildEventHandler
    {
        public string szABOutputRootPath
        {
            get;
            private set;
        }

        public string szAppGameVersion
        {
            get;
            private set;
        }

        public int nInternalResVersion
        {
            get;
            private set;
        }

        public bool ContinueOnFailure
        {
            get
            {
                return false;
            }
        }





        public void OnPreprocessAllPlatforms(string productName, string companyName, string gameIdentifier, string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion, Platform platforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName, bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName, string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
            szAppGameVersion = applicableGameVersion;
            nInternalResVersion = internalResourceVersion;
            szABOutputRootPath = outputDirectory;
        }

        public void OnPreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath)
        {
        }

        public void OnBuildAssetBundlesComplete(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, AssetBundleManifest assetBundleManifest)
        {
        }

        public void OnOutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength, int versionListHashCode, int versionListCompressedLength, int versionListCompressedHashCode)
        {
        }

        public void OnPostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, bool isSuccess)
        {
            if (!isSuccess)
            {
                return;
            }
            //把package的资源拷贝到stream文件夹下

#if UNITY_STANDALONE_WIN
            if (platform == Platform.Windows64)
            {
                CBuildABUtilityTools.CopyPackageResToStreamingAssets(outputPackagePath);
            }
#endif

#if UNITY_ANDROID
            if (platform == Platform.Android)
            {
                CBuildABUtilityTools.CopyPackageResToStreamingAssets(outputPackagePath);
            }
#endif

#if UNITY_IOS
             if (platform == Platform.IOS)
            {
                CBuildABUtilityTools.CopyPackageResToStreamingAssets(outputPackagePath);
            }
#endif
        }

        public void OnPostprocessAllPlatforms(string productName, string companyName, string gameIdentifier, string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion, Platform platforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName, bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName, string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {
        }
    }
}

