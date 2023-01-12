using GameFrameworkPackageEditor;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;

namespace Tools
{
    //工具菜单
    public sealed partial class ToolMenu
    {

        [MenuItem("MyTools/DataTable/只生成客户端配置", false, 10)]
        public static void CreateConfig_Client()
        {
            CSDataTableCreator.CreateConfig_ClientByExcelFloder();
        }

        [MenuItem("MyTools/DataTable/一键生成本地服务器和客户端配置", false, 11)]
        public static void CreateCofig_CS_Local()
        {
            CSDataTableCreator.CreateCofig_CS_Local();
        }

        [MenuItem("MyTools/DataTable/打开Excel文件夹", false, 12)]
        public static void ViewExcelPath()
        {
            CSDataTableCreator.ViewExcelPath();
        }

        [MenuItem("MyTools/DataTable/打开txt文件夹", false, 13)]
        public static void ViewTxtPath()
        {
            CSDataTableCreator.ViewTxtPath();
        }

        [MenuItem("MyTools/Protobuf/生成.cs文件", false, 20)]
        public static void Proto2CS()
        {
            //todo
        }

        [MenuItem("MyTools/Protobuf/生成.cs文件，选择生成路径", false, 21)]
        public static void BuildAssetBundle()
        {
            //todo
        }
        [MenuItem("MyTools/Protobuf/打开proto文件夹", false, 22)]

        public static void ViewInputDataPath()
        {
            //todo
        }

        [MenuItem("MyTools/Protobuf/打开生成路径", false, 23)]
        public static void ViewOutputDataPath()
        {
            //todo
        }

        //[MenuItem("MyTools/图集/按文件夹制作图集", false, 30)]
        //public static void CreateAtlasByAllFolders()
        //{
        //    AtlasMaker.CreateAtlasByAllFolders();
        //}

        [MenuItem("MyTools/图集/按文件夹递归制作图集", false, 31)]
        public static void CreateAtlasByAllFoldersRecursive()
        {
            AtlasMaker.CreateAtlasByAllFoldersRecursive();
        }

        [MenuItem("MyTools/字体/创建字体(sprite)", false, 38)]
        public static void FontCreateOpen()
        {
            CreateFont.Open();
        }
        [MenuItem("MyTools/字体/编辑字体数据", false, 38)]
        public static void TextToolEditorOpen()
        {
            TextToolDataEditor.Open();
        }

        private static string[] HotFixServerType = { "HotfixServerLocal", "HotfixServerRemoteTest", "HotfixServerRemote", "HotfixServerRemoteTestNoSDk" };  //热更服务器的类型
        [MenuItem("MyTools/热更/设置热更服务器/本地", false, 51)]
        private static void SetHotfixServer_Local()
        {
            for (int i = 0; i < HotFixServerType.Length; i++)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(HotFixServerType[i]);
            }
            ScriptingDefineSymbols.AddScriptingDefineSymbol(HotFixServerType[0]);
        }

        [MenuItem("MyTools/热更/设置热更服务器/远程测试资源服", false, 52)]
        private static void SetHotfixServer_RemoteTest()
        {
            for (int i = 0; i < HotFixServerType.Length; i++)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(HotFixServerType[i]);
            }
            ScriptingDefineSymbols.AddScriptingDefineSymbol(HotFixServerType[1]);
        }

        [MenuItem("MyTools/热更/设置热更服务器/远程测试资源服没有SDK版本", false, 53)]
        private static void SetHotfixServer_RemoteTestNoSDK()
        {
            for (int i = 0; i < HotFixServerType.Length; i++)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(HotFixServerType[i]);
            }
            ScriptingDefineSymbols.AddScriptingDefineSymbol(HotFixServerType[3]);
        }

        [MenuItem("MyTools/热更/设置热更服务器/远程正式资源服", false, 54)]
        private static void SetHotfixServer_Remote()
        {
            for (int i = 0; i < HotFixServerType.Length; i++)
            {
                ScriptingDefineSymbols.RemoveScriptingDefineSymbol(HotFixServerType[i]);
            }
            ScriptingDefineSymbols.AddScriptingDefineSymbol(HotFixServerType[2]);
        }


        [MenuItem("MyTools/热更/HybridCLR/复制所需Dll到目录中", false, 55)]
        private static void CopyHybridCLRDll()
        {
            CHotFixTool.CopyHybridDllToTarget();
        }



        [MenuItem("Assets/MyTool/Utility/复制文件名", false, 1)]
        public static void CreateLocalizationConfig()
        {

            StringBuilder names = new StringBuilder();
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                names.AppendLine(Selection.objects[i].name);
            }
            GUIUtility.systemCopyBuffer = names.ToString();
        }


        [MenuItem("MyTools/测试/测试", false, 1001)]
        public static void Test()
        {
            UnityEngine.ScreenCapture.CaptureScreenshot("guideMainSceneBg.png");//截图并保存截图文件

            //string szPath = "I:\\WorkProjects\\GridBattle\\_ABRes\\PcTest";
            //CBuildABUtilityTools.GenerateVersionInfos(Application.version, 13, szPath);
            //CBuildABUtilityTools.GenerateVersionInfo(Application.version, 12, UnityGameFramework.Editor.AssetBundleTools.Platform.Windows64, szPath);

            //DirectoryInfo dicWork = new DirectoryInfo(Application.dataPath);
            //szPath = Path.Combine(dicWork.Parent.FullName, "_ABRes");
            //GameFrameworkPackageEditor.CBuildEventHandlerHotFix.CopyPackedResToAABPad(szPath, Path.Combine(szPath, "Packed", "0_0_1_12", "Android"));
        }
    }
}
