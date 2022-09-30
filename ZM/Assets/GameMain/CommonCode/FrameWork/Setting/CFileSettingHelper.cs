using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System;
using UnityGameFramework.Runtime;
using System.Text;
using System.IO;
using Logic;

namespace GameFrameworkPackage
{
    public class CFileSettingHelper : SettingHelperBase
    {
        private const string ms_szEncryptKey = "clientSaveDatakey";
        public const int mc_nCurVersion = 1;
        public const string mc_szVersionKey = "VersionId";
        public static string ms_szFileName = ".save";
        private Dictionary<string, object> m_mapAllSave = new Dictionary<string, object>();
        private string m_szFilePath;

        public override int Count => throw new NotImplementedException();

        private string _GetRootPath()
        {
#if UNITY_EDITOR
            return Application.dataPath;
#else
            return Application.persistentDataPath;
#endif
        }
        /// <summary>
        /// 加载配置。
        /// </summary>
        /// <returns>是否加载配置成功。</returns>
        public override bool Load()
        {
            m_mapAllSave = new Dictionary<string, object>();
            m_szFilePath = Utility.Path.GetRegularPath(Path.Combine(_GetRootPath(), ms_szFileName));
            if (!File.Exists(m_szFilePath))
            {
                return true;
            }

            string szContentJson;
            using (StreamReader sr = new StreamReader(m_szFilePath))
            {
                szContentJson = sr.ReadToEnd();
            }
            m_mapAllSave = Json.Deserialize(szContentJson) as Dictionary<string, object>;
            if (null == m_mapAllSave)
            {
                m_mapAllSave = new Dictionary<string, object>();
            }

            if (!_CheckVersionVaild())
            {
                m_mapAllSave.Clear();
                m_mapAllSave.Add(mc_szVersionKey, mc_nCurVersion);
            }
            return true;
        }

        private bool _CheckVersionVaild()
        {
            int nSaveVersion = GetInt(mc_szVersionKey, -1);
            return nSaveVersion >= mc_nCurVersion;
        }

        /// <summary>
        /// 保存配置。
        /// </summary>
        /// <returns>是否保存配置成功。</returns>
        public override bool Save()
        {
            string sz = Json.Serialize(m_mapAllSave);
            if (string.IsNullOrEmpty(sz) || m_mapAllSave == null)
            {
                Log.Error("Save fail");
                return false;
            }
            using (StreamWriter streamWriter = new StreamWriter(m_szFilePath, false))
            {
                streamWriter.Write(sz);
            }
#if UNITY_EDITOR
            _DebugSaveFile(sz);
#endif
            return true;
        }

        private void _DebugSaveFile(string sz)
        {
            string szDirPath = Path.Combine(Application.dataPath, "..save");
            string szFilePath = Path.Combine(szDirPath, "save");
            if (!Directory.Exists(szDirPath))
            {
                Directory.CreateDirectory(szDirPath);
            }
            using (StreamWriter streamWriter = new StreamWriter(szFilePath, false))
            {
                streamWriter.Write(sz);
            }
        }

        /// <summary>
        /// 检查是否存在指定配置项。
        /// </summary>
        /// <param name="settingName">要检查配置项的名称。</param>
        /// <returns>指定的配置项是否存在。</returns>
        public override bool HasSetting(string settingName)
        {
            if (m_mapAllSave == null)
            {
                return false;
            }
            return m_mapAllSave.ContainsKey(settingName);
        }

        /// <summary>
        /// 移除指定配置项。
        /// </summary>
        /// <param name="settingName">要移除配置项的名称。</param>
        public override bool RemoveSetting(string settingName)
        {
            if (HasSetting(settingName))
            {
                m_mapAllSave.Remove(settingName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 清空所有配置项。
        /// </summary>
        public override void RemoveAllSettings()
        {
            m_mapAllSave.Clear();
        }

        private object _GetValue(string settingName)
        {
            if (!HasSetting(settingName))
            {
                return null;
            }
            return m_mapAllSave[settingName];
        }

        private void _SetValue(string settingName, object obj)
        {
            if (!HasSetting(settingName))
            {
                m_mapAllSave.Add(settingName, obj);
            }
            else
            {
                m_mapAllSave[settingName] = obj;
            }
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的布尔值。</returns>
        public override bool GetBool(string settingName)
        {
            return GetBool(settingName, false);
        }

        /// <summary>
        /// 从指定配置项中读取布尔值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的布尔值。</returns>
        public override bool GetBool(string settingName, bool defaultValue)
        {
            object val = _GetValue(settingName);
            if (null == val)
            {
                return defaultValue;
            }
            Type t = val.GetType();
            if (t != typeof(bool))
            {
                return defaultValue;
            }
            return Convert.ToBoolean(val);
        }

        /// <summary>
        /// 向指定配置项写入布尔值。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="value">要写入的布尔值。</param>
        public override void SetBool(string settingName, bool value)
        {
            _SetValue(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的整数值。</returns>
        public override int GetInt(string settingName)
        {
            return GetInt(settingName, 0);
        }

        /// <summary>
        /// 从指定配置项中读取整数值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的整数值。</returns>
        public override int GetInt(string settingName, int defaultValue)
        {
            object val = _GetValue(settingName);
            if (null == val)
            {
                return defaultValue;
            }
            Type t = val.GetType();
            if (t == typeof(int))
            {
                return defaultValue;
            }
            return Convert.ToInt32(val);
        }

        /// <summary>
        /// 向指定配置项写入整数值。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="value">要写入的整数值。</param>
        public override void SetInt(string settingName, int value)
        {
            _SetValue(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的浮点数值。</returns>
        public override float GetFloat(string settingName)
        {
            return GetFloat(settingName, 0);
        }

        /// <summary>
        /// 从指定配置项中读取浮点数值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的浮点数值。</returns>
        public override float GetFloat(string settingName, float defaultValue)
        {
            object val = _GetValue(settingName);
            if (null == val)
            {
                return defaultValue;
            }
            Type t = val.GetType();
            if (t != typeof(float) && t != typeof(double))
            {
                return defaultValue;
            }
            return (float)Convert.ToDouble(val);
        }

        /// <summary>
        /// 向指定配置项写入浮点数值。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="value">要写入的浮点数值。</param>
        public override void SetFloat(string settingName, float value)
        {
            _SetValue(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的字符串值。</returns>
        public override string GetString(string settingName)
        {
            return GetString(settingName, "");
        }

        /// <summary>
        /// 从指定配置项中读取字符串值。
        /// </summary>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultValue">当指定的配置项不存在时，返回此默认值。</param>
        /// <returns>读取的字符串值。</returns>
        public override string GetString(string settingName, string defaultValue)
        {
            object val = _GetValue(settingName);
            if (null == val)
            {
                return defaultValue;
            }
            string v = val as string;
            if (string.IsNullOrEmpty(v))
            {
                return defaultValue;
            }
            return v;
        }

        /// <summary>
        /// 向指定配置项写入字符串值。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="value">要写入的字符串值。</param>
        public override void SetString(string settingName, string value)
        {
            _SetValue(settingName, value);
        }

        /// <summary>
        /// 从指定配置项中读取对象。
        /// </summary>
        /// <typeparam name="T">要读取对象的类型。</typeparam>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns>读取的对象。</returns>
        public override T GetObject<T>(string settingName)
        {
            return Utility.Json.ToObject<T>(CPackageUtility.Text.RijndaelDecrypt(GetString(settingName), ms_szEncryptKey));
        }

        /// <summary>
        /// 从指定配置项中读取对象。
        /// </summary>
        /// <param name="objectType">要读取对象的类型。</param>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <returns></returns>
        public override object GetObject(Type objectType, string settingName)
        {
            return Utility.Json.ToObject(objectType, CPackageUtility.Text.RijndaelDecrypt(GetString(settingName), ms_szEncryptKey));
        }

        /// <summary>
        /// 从指定配置项中读取对象。
        /// </summary>
        /// <typeparam name="T">要读取对象的类型。</typeparam>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultObj">当指定的配置项不存在时，返回此默认对象。</param>
        /// <returns>读取的对象。</returns>
        public override T GetObject<T>(string settingName, T defaultObj)
        {
            string json = GetString(settingName, null);
            if (string.IsNullOrEmpty(json))
            {
                return defaultObj;
            }

            return Utility.Json.ToObject<T>(CPackageUtility.Text.RijndaelDecrypt(json, ms_szEncryptKey));
        }

        /// <summary>
        /// 从指定配置项中读取对象。
        /// </summary>
        /// <param name="objectType">要读取对象的类型。</param>
        /// <param name="settingName">要获取配置项的名称。</param>
        /// <param name="defaultObj">当指定的配置项不存在时，返回此默认对象。</param>
        /// <returns></returns>
        public override object GetObject(Type objectType, string settingName, object defaultObj)
        {
            string json = GetString(settingName, null);
            if (string.IsNullOrEmpty(json))
            {
                return defaultObj;
            }

            return Utility.Json.ToObject(objectType, CPackageUtility.Text.RijndaelDecrypt(json, ms_szEncryptKey));
        }

        /// <summary>
        /// 向指定配置项写入对象。
        /// </summary>
        /// <typeparam name="T">要写入对象的类型。</typeparam>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        public override void SetObject<T>(string settingName, T obj)
        {
            string sz = CPackageUtility.Text.RijndaelEncrypt(Utility.Json.ToJson(obj), ms_szEncryptKey);
            SetString(settingName, sz);
        }

        /// <summary>
        /// 向指定配置项写入对象。
        /// </summary>
        /// <param name="settingName">要写入配置项的名称。</param>
        /// <param name="obj">要写入的对象。</param>
        public override void SetObject(string settingName, object obj)
        {

            string sz = CPackageUtility.Text.RijndaelEncrypt(Utility.Json.ToJson(obj), ms_szEncryptKey);
            SetString(settingName, sz);
        }

        public override string[] GetAllSettingNames()
        {
            throw new NotImplementedException();
        }

        public override void GetAllSettingNames(List<string> results)
        {
            throw new NotImplementedException();
        }
    }

}