
using GameFramework;
using GameFrameworkPackage;
using LitJson;
using System;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;
using System.Text;
using Aliyun.OSS;
using Aliyun.OSS.Common;

namespace GameFrameworkPackageEditor
{
    public static class CBuildABUtilityTools
    {
        private const string ms_szPackageResListFileName = "GameFrameworkVersion.dat";
        private const string ms_szPackedResListFileName = "GameFrameworkList.dat";

        private static string ms_szRecordName = "GameResourceVersion";

        private static string ms_szPathAABPadRes = "_AABPadRes";
        private static string ms_szPathUploadServerRoot = "_UploadToServerRes";

        private static string ms_szPathUploadRecordPath = "VersionRecored";


        public static void GenerateVersionInfos(string szGameVersion, int nInternalResVersion, string a_szABRootDic)
        {
            string szRecordPath = CPackageUtility.Path.GetCombinePath(a_szABRootDic, Utility.Text.Format("{0}_{1}.xml", ms_szRecordName, szGameVersion.Replace('.', '_')));
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(szRecordPath);
            XmlNode xmlRoot = xmlDocument.SelectSingleNode("ResourceVersionInfo");

            XmlNodeList xmlPlatformList = xmlRoot.ChildNodes;
            for (int i = 0; i < xmlPlatformList.Count; i++)
            {
                XmlNode xmlPlatform = xmlPlatformList.Item(i);
                string szPlatform = xmlPlatform.Name;

                CVersionInfo version = new CVersionInfo();
                version.ForceUpdateGame = false;
                version.InternalGameVersion = CBuildInfoTools.ReadBuildInternalGameVersion();
                version.InternalResourceVersion = nInternalResVersion;
                version.LatestGameVersion = szGameVersion;
                version.UpdatePrefixUri = _GetHotFixResUrl(szGameVersion, nInternalResVersion, szPlatform);

                version.VersionListLength = int.Parse(xmlPlatform.Attributes.GetNamedItem("Length").Value);
                version.VersionListHashCode = int.Parse(xmlPlatform.Attributes.GetNamedItem("HashCode").Value);
                version.VersionListCompressedLength = int.Parse(xmlPlatform.Attributes.GetNamedItem("ZipLength").Value);
                version.VersionListCompressedHashCode = int.Parse(xmlPlatform.Attributes.GetNamedItem("ZipHashCode").Value);
                string szVersionInfo = JsonMapper.ToJson(version);

                string szUploadSeverRoot = CPackageUtility.Path.GetCombinePath(a_szABRootDic, ms_szPathUploadServerRoot);
                if (!Directory.Exists(szUploadSeverRoot))
                {
                    Directory.CreateDirectory(szUploadSeverRoot);
                }
                string szUploadVersionDic = CPackageUtility.Path.GetCombinePath(szUploadSeverRoot, CABResConfig.ms_szHotFixVersionDicName);
                if (!Directory.Exists(szUploadVersionDic))
                {
                    Directory.CreateDirectory(szUploadVersionDic);
                }

                string szRecordDic = CPackageUtility.Path.GetCombinePath(szUploadSeverRoot, ms_szPathUploadRecordPath);
                if (!Directory.Exists(szRecordDic))
                {
                    Directory.CreateDirectory(szRecordDic);
                }
                string szVersionPath = CPackageUtility.Path.GetCombinePath(szUploadVersionDic, Utility.Text.Format("{0}_version.txt", szPlatform));
                if (File.Exists(szVersionPath))
                {
                    using (TextReader txtR = File.OpenText(szVersionPath))
                    {
                        CVersionInfo oldVersionInfo = JsonMapper.ToObject<CVersionInfo>(new JsonReader(txtR));
                        string szRecordVersionPath = CPackageUtility.Path.GetCombinePath(szRecordDic, Utility.Text.Format("{0}_version_{1}.txt", szPlatform, oldVersionInfo.InternalResourceVersion));
                        File.Copy(szVersionPath, szRecordVersionPath);
                    }
                }
                using (StreamWriter streamWriter = new StreamWriter(szVersionPath, false))
                {
                    streamWriter.Write(szVersionInfo);
                }
            }
        }

        private static string _GetHotFixResUrl(string szGameVersion, int nInternalResVersion, string szPlatform)
        {
            string szRootUrl = CABResConfig.GetHotFixResRootUrl();
            string szVersionPath = Utility.Text.Format("{0}_{1}", szGameVersion.Replace('.', '_'), nInternalResVersion);
            string szResUrl = Utility.Text.Format("{0}/{1}/{2}/{3}", szRootUrl, CABResConfig.ms_szHotFixResDicName, szVersionPath, szPlatform);
            return szResUrl;
        }

        public static void CopyPackageResToAABPad(string a_szOutputABRootPath, string a_szOutputPackedPath)
        {
            _CopyResToAABPad(a_szOutputABRootPath, a_szOutputPackedPath, ms_szPackageResListFileName);
        }

        public static void CopyPackedResToAABPad(string a_szOutputABRootPath, string a_szOutputPackedPath)
        {
            _CopyResToAABPad(a_szOutputABRootPath, a_szOutputPackedPath, ms_szPackedResListFileName);
        }

        private static void _CopyResToAABPad(string a_szOutputABRootPath, string a_szOutputPackedPath, string a_szResListFileName)
        {
            string szAABResRootPath = Path.Combine(a_szOutputABRootPath, ms_szPathAABPadRes);
            if (Directory.Exists(szAABResRootPath))
            {
                Directory.Delete(szAABResRootPath, true);
            }
            string szAABResPath = Path.Combine(szAABResRootPath, "Android");
            FileTool.CopyDirectory(a_szOutputPackedPath, szAABResPath);

            string szResListFileName =(a_szResListFileName);
            string szPathSourceResListFile = Path.Combine(a_szOutputPackedPath, szResListFileName);
            string szPathTargetResListFile = Path.Combine(Application.streamingAssetsPath, szResListFileName);
            File.Delete(szPathTargetResListFile);
            File.Copy(szPathSourceResListFile, szPathTargetResListFile);
        }

        public static void CopyPackedResToStreamingAssets(string a_szOutputPackedPath)
        {
            FileTool.DeleteDirectory(Application.streamingAssetsPath);
            FileTool.CopyDirectory(a_szOutputPackedPath, Application.streamingAssetsPath);
        }

        public static void CopyPackageResToStreamingAssets(string a_szOutputPackagePath)
        {
            FileTool.DeleteDirectory(Application.streamingAssetsPath);
            FileTool.CopyDirectory(a_szOutputPackagePath, Application.streamingAssetsPath);
        }

        public static void CopyFullResToUploadServerDic(string a_szOutputABRootPath, string a_szFullABRootPath)
        {
            string szUploadSeverRoot = CPackageUtility.Path.GetCombinePath(a_szOutputABRootPath, ms_szPathUploadServerRoot);
            if (!Directory.Exists(szUploadSeverRoot))
            {
                Directory.CreateDirectory(szUploadSeverRoot);
            }
            string szUploadResRoot = CPackageUtility.Path.GetCombinePath(szUploadSeverRoot, CABResConfig.ms_szHotFixResDicName);
            if (Directory.Exists(szUploadResRoot))
            {
                Directory.Delete(szUploadResRoot, true);
            }
            Directory.CreateDirectory(szUploadResRoot);

            DirectoryInfo dic = new DirectoryInfo(a_szFullABRootPath);
            string szTargetResDic = CPackageUtility.Path.GetCombinePath(szUploadResRoot, dic.Name);
            if (Directory.Exists(szTargetResDic))
            {
                Directory.Delete(szTargetResDic, true);
            }
            Directory.CreateDirectory(szTargetResDic);

            FileTool.CopyDirectory(a_szFullABRootPath, szTargetResDic);
        }

        public static void UploadFileToOss(string objectName, string a_szUploadRootPath)
        {
            DirectoryInfo info = new DirectoryInfo(a_szUploadRootPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                try
                {
                    if (fsi is FileInfo)//如果是文件，复制文件
                    {
                        // 上传文件。
                        Debug.Log(string.Format("ObjectName:{0}  RootPath:{1}", CPackageUtility.Path.GetCombinePath(objectName, fsi.Name), CPackageUtility.Path.GetCombinePath(a_szUploadRootPath, fsi.Name)));
                        var result = COssMgr.Instance.GetClient().PutObject(COssMgr.Instance.GetBucketName(CABResConfig.ms_szHotFixUrl_Public), CPackageUtility.Path.GetCombinePath(objectName, fsi.Name), CPackageUtility.Path.GetCombinePath(a_szUploadRootPath, fsi.Name));
                        Console.WriteLine("Put object succeeded versionid : {0}", result.VersionId);
                    }
                    else
                    {
                        UploadFileToOss(CPackageUtility.Path.GetCombinePath(objectName, fsi.Name), fsi.FullName);
                    }

                }
                catch (Exception ex)
                {
                    Debug.LogWarning(string.Format("Put object failed, {0}", ex.Message));
                }
            }
        }

        public static void UploadFullResToOss(string a_szOutputABRootPath, string a_szObjectName)
        {
            string szUploadSeverRoot = CPackageUtility.Path.GetCombinePath(a_szOutputABRootPath, ms_szPathUploadServerRoot);
            if (!Directory.Exists(szUploadSeverRoot))
            {
                Debug.LogWarning(string.Format("Put object failed:Upload Sever Root Not Exist, {0}", szUploadSeverRoot));
                return;
            }
            string szUploadResRoot = CPackageUtility.Path.GetCombinePath(szUploadSeverRoot, CABResConfig.ms_szHotFixResDicName);
            if (!Directory.Exists(szUploadResRoot))
            {
                Debug.LogWarning(string.Format("Put object failed:Upload Res Root Not Exist, {0}", szUploadSeverRoot));
                return;
            }
            UploadFileToOss(CPackageUtility.Path.GetCombinePath(a_szObjectName, CABResConfig.ms_szHotFixResDicName), szUploadResRoot);
        }

        public static void UploadFullVersionInfoToOss(string a_szOutputABRootPath, string a_szObjectName)
        {
            string szUploadSeverRoot = CPackageUtility.Path.GetCombinePath(a_szOutputABRootPath, ms_szPathUploadServerRoot);
            if (!Directory.Exists(szUploadSeverRoot))
            {
                Debug.LogWarning(string.Format("Put object failed:Upload Sever Root Not Exist, {0}", szUploadSeverRoot));
                return;
            }
            string szUploadResRoot = CPackageUtility.Path.GetCombinePath(szUploadSeverRoot, CABResConfig.ms_szHotFixVersionDicName);
            if (!Directory.Exists(szUploadResRoot))
            {
                Debug.LogWarning(string.Format("Put object failed:Upload Version Info Root Not Exist, {0}", szUploadSeverRoot));
                return;
            }
            UploadFileToOss(CPackageUtility.Path.GetCombinePath(a_szObjectName, CABResConfig.ms_szHotFixVersionDicName), szUploadResRoot);
        }

        public static void SetVersionInfoFileACL(string a_szPrefix, CannedAccessControlList a_acl)
        {
            var listObjectsRequest = new ListObjectsRequest(COssMgr.Instance.GetBucketName(CABResConfig.ms_szHotFixUrl_Public))
            {
                Marker = string.Empty,
                Prefix = a_szPrefix,
            };
            ObjectListing result = COssMgr.Instance.GetClient().ListObjects(listObjectsRequest);
            foreach (var summary in result.ObjectSummaries)
            {
                COssMgr.Instance.GetClient().SetObjectAcl(COssMgr.Instance.GetBucketName(CABResConfig.ms_szHotFixUrl_Public), summary.Key, a_acl);
            }
        }
    }
}

