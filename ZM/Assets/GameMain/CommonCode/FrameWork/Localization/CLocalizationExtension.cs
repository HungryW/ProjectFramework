using GameFramework;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public static class LocalizationExtension
    {
        public static void LoadDictionary(this LocalizationComponent a_component, string a_szFileName, object a_oUserData)
        {
            if (string.IsNullOrEmpty(a_szFileName))
            {
                Log.Warning("Dictionary name is invalid.");
                return;
            }
            CGameEntryMgr.Localization.ReadData(a_szFileName, a_oUserData);
        }

        public static string GetStringEX(this LocalizationComponent a_component, string a_szKey)
        {
            if (string.IsNullOrEmpty(a_szKey))
            {
                return string.Empty;
            }
            if (!a_component.HasRawString(a_szKey))
            {
                return a_szKey;
            }
            return a_component.GetString(a_szKey);
        }

        public static string GetStringEX(this LocalizationComponent a_component, string a_szKey, object arg0)
        {
            if (string.IsNullOrEmpty(a_szKey))
            {
                return string.Empty;
            }
            if (!a_component.HasRawString(a_szKey))
            {
                return string.Format(a_szKey, arg0);
            }
            return a_component.GetString(a_szKey, arg0);
        }

        public static string GetStringEX(this LocalizationComponent a_component, string a_szKey, object arg0, object arg1)
        {
            if (string.IsNullOrEmpty(a_szKey))
            {
                return string.Empty;
            }
            if (!a_component.HasRawString(a_szKey))
            {
                return string.Format(a_szKey, arg0, arg1);
            }
            return a_component.GetString(a_szKey, arg0, arg1);
        }


        public static string GetStringEX(this LocalizationComponent a_component, string a_szKey, object arg0, object arg1, object arg2)
        {
            if (string.IsNullOrEmpty(a_szKey))
            {
                return string.Empty;
            }
            if (!a_component.HasRawString(a_szKey))
            {
                return string.Format(a_szKey, arg0, arg1, arg2);
            }
            return a_component.GetString(a_szKey, arg0, arg1, arg2);
        }

        public static string GetStringEX(this LocalizationComponent a_component, string a_szKey, params object[] args)
        {
            if (string.IsNullOrEmpty(a_szKey))
            {
                return string.Empty;
            }
            if (!a_component.HasRawString(a_szKey))
            {
                return string.Format(a_szKey, args);
            }
            return a_component.GetString(a_szKey, args);
        }
    }
}

