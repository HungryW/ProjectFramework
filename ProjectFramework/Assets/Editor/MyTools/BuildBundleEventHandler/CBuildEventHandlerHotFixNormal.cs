
using GameFramework;
using GameFrameworkPackage;
using LitJson;
using System;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;
using Aliyun.OSS;
using UnityGameFramework.Editor.ResourceTools;
using Tools;

namespace GameFrameworkPackageEditor
{
    public class CBuildEventHandlerHotFixNormal : IBuildEventHandler
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
            CHotFixTool.CopyHybridDllToTarget();
        }

        public void OnPreprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath)
        {
        }

        public void OnBuildAssetBundlesComplete(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, AssetBundleManifest assetBundleManifest)
        {
        }

        public void OnOutputUpdatableVersionListData(Platform platform, string versionListPath, int versionListLength, int versionListHashCode, int versionListCompressedLength, int versionListCompressedHashCode)
        {
            //生成上传的version文件, 放在versio文件夹下
            CBuildABUtilityTools.GenerateVersionInfos(szAppGameVersion, nInternalResVersion, szABOutputRootPath, platform, versionListLength, versionListHashCode, versionListCompressedLength, versionListCompressedHashCode);
        }

        public void OnPostprocessPlatform(Platform platform, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, bool isSuccess)
        {
            if (!isSuccess)
            {
                return;
            }
            //把packed的资源拷贝到stream文件夹下

#if UNITY_STANDALONE_WIN
            if (platform == Platform.Windows64)
            {
                CBuildABUtilityTools.CopyPackedResToStreamingAssets(outputPackedPath);
            }
#endif

#if UNITY_ANDROID
            if (platform == Platform.Android)
            {
                CBuildABUtilityTools.CopyPackedResToStreamingAssets(outputPackedPath);
            }
#endif

#if UNITY_IOS
             if (platform == Platform.IOS)
            {
                CBuildABUtilityTools.CopyPackedResToStreamingAssets(outputPackedPath);
            }
#endif
        }

        public void OnPostprocessAllPlatforms(string productName, string companyName, string gameIdentifier, string gameFrameworkVersion, string unityVersion, string applicableGameVersion, int internalResourceVersion, Platform platforms, AssetBundleCompressionType assetBundleCompression, string compressionHelperTypeName, bool additionalCompressionSelected, bool forceRebuildAssetBundleSelected, string buildEventHandlerTypeName, string outputDirectory, BuildAssetBundleOptions buildAssetBundleOptions, string workingPath, bool outputPackageSelected, string outputPackagePath, bool outputFullSelected, string outputFullPath, bool outputPackedSelected, string outputPackedPath, string buildReportPath)
        {

            //将要上传资源服务器的Full文件夹下的文件拷贝到res文件夹下
            CBuildABUtilityTools.CopyFullResToUploadServerDic(szABOutputRootPath, outputFullPath);

            ////把full的资源上传到资源服务器
            //CBuildABUtilityTools.UploadFullResToOss(outputDirectory, COssMgr.Instance.GetObjectNameRoot(CABResConfig.GetHotFixResRootUrl()));

            ////将versioninfo文件夹下的文件整体替换资源服务器的文件夹
            //CBuildABUtilityTools.UploadFullVersionInfoToOss(outputDirectory, COssMgr.Instance.GetObjectNameRoot(CABResConfig.GetHotFixResRootUrl()));

            //CBuildABUtilityTools.SetVersionInfoFileACL(CPackageUtility.Path.GetCombinePath(COssMgr.Instance.GetObjectNameRoot(CABResConfig.GetHotFixResRootUrl()), CABResConfig.ms_szHotFixVersionDicName), CannedAccessControlList.PublicRead);
        }
    }
}

