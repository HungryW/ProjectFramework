using GameFramework;
using System.IO;
using UnityEngine;

namespace GameFrameworkPackage
{
    public static class CAssestPathUtility
    {

        //热更新程序文件
        public const string HotfixPath = "Assets/GameMain/HotFixDll";  //热更新路径
        public const string HotfixDllName = "Game.Hotfix.dll.bytes";
        public const string HotfixPdbName = "Game.Hotfix.pdb.bytes";
        public const string HotfixDataTableListName = "DataTableList.txt";
        public const string HotfixUISpriteListName = "UISpriteList.txt";
        public const string HotfixLocalizationListName = "Localization.txt";

        //获取热更新资源内置路径
        public static string GetHotfixAsset(string assetName)
        {
            return Utility.Text.Format("{0}/{1}", HotfixPath, assetName);
        }

        public static string GetConfigAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/Configs/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDataTableAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/DataTables/{0}.{1}", assetName, fromBytes ? "bytes" : "txt");
        }

        public static string GetDictionaryAsset(string assetName, bool fromBytes)
        {
            return Utility.Text.Format("Assets/GameMain/Localization/{0}/Dictionaries/{1}.{2}", CGameEntryMgr.Localization.Language, assetName, fromBytes ? "bytes" : "xml");
        }

        public static string GetFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Fonts/{0}.ttf", assetName);
        }

        public static string GetSceneAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Scenes/{0}.unity", assetName);
        }

        public static string GetMusicAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Music/{0}.mp3", assetName);
        }

        public static string GetSoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Sounds/{0}.wav", assetName);
        }

        public static string GetEntityAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Entities/{0}.prefab", assetName);
        }

        public static string GetUIFormAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UIForms/{0}.prefab", assetName);
        }

        public static string GetUISoundAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UI/UISounds/{0}.wav", assetName);
        }


        public static string GetCustomFontAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/Font/Custom/{0}", assetName);
        }

        public static string GetUISpriteAsset(string assetName)
        {
            return Utility.Text.Format("Assets/GameMain/UISprite/{0}.prefab", assetName);
        }


        public static string GetScriptAsset(string a_ssAssetName)
        {
            return Utility.Text.Format("Assets/GameMain/_Logic/Scripts/{0}.cs", a_ssAssetName);
        }

        public static string GetResToolPrefab(string a_szName)
        {
            return Utility.Text.Format("Tool/{0}", a_szName);
        }

        public static string GetParticleAsset(string particleName)
        {
            return Utility.Text.Format("Assets/GameMain/Particle/Prefab/{0}.prefab", particleName);
        }
    }
}


