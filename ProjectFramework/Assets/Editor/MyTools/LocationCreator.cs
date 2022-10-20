
using GameFramework;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Data;
using ExcelDataReader;
using System.Xml;
using System;
using GameFrameworkPackage;

namespace UnityGameFramework.Editor
{
    public static class CLocationCreator
    {
        public static string mc_szLocaliztionRootPath = Application.dataPath + "/GameMain/Localization";
        public static string mc_szXMLDirName = "Dictionaries";



        [MenuItem("Assets/MyTool/Localization/生成多语言配置", false, 1)]
        public static void CreateLocalizationConfig()
        {
            for (int i = 0; i < Selection.objects.Length; i++)
            {
                var path = AssetDatabase.GetAssetPath(Selection.objects[i]);
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                if (System.IO.Directory.Exists(path))
                {
                    _ChangeDirAllExcelToAllLanguageXmls(path);
                }
                else if (System.IO.File.Exists(path))
                {
                    EditorUtility.DisplayProgressBar("生成中", Selection.objects[i].name, (float)i / Selection.objects.Length);
                    _ChangeOneExcelToAllLanguageXmls(path);
                    EditorUtility.ClearProgressBar();
                }
            }
            AssetDatabase.Refresh();
        }


        private static void _ChangeDirAllExcelToAllLanguageXmls(string a_szPath)
        {
            DirectoryInfo direction = new DirectoryInfo(a_szPath);
            FileInfo[] _fileList = direction.GetFiles("*.xlsx", SearchOption.AllDirectories);
            Dictionary<string, string> mapResult = new Dictionary<string, string>();
            for (int i = 0; i < _fileList.Length; i++)
            {
                EditorUtility.DisplayProgressBar("生成中", _fileList[i].Name, (float)i / _fileList.Length);
                _ChangeOneExcelToAllLanguageXmls(_fileList[i].FullName);
            }
            EditorUtility.ClearProgressBar();
        }

        private static void _ChangeOneExcelToAllLanguageXmls(string a_szFileFullName)
        {
            string szFileName = FileTool.GetFileNameByPath(a_szFileFullName);
            szFileName = FileTool.RemoveExpandName(szFileName);
            Dictionary<string, Dictionary<string, string>> mapAllLanKeyVal = _ParseExcelFileToLanguagesKeyValue(a_szFileFullName);
            foreach (var lanKeyVal in mapAllLanKeyVal)
            {
                string szLanName = lanKeyVal.Key;
                Dictionary<string, string> keyVals = lanKeyVal.Value;
                _CreateOneLanguageConfig(keyVals, szLanName, szFileName);
            }
        }

        private static Dictionary<string, Dictionary<string, string>> _ParseExcelFileToLanguagesKeyValue(string a_szFileFullName)
        {
            Dictionary<string, Dictionary<string, string>> mapResult = new Dictionary<string, Dictionary<string, string>>();
            string szFileName = a_szFileFullName.Substring(a_szFileFullName.LastIndexOf('/') + 1);
            if (szFileName.Contains("$"))
            {
                return mapResult;
            }
            if (szFileName.Contains("ignore"))
            {
                return mapResult;
            }
            FileStream fileStream = File.Open(a_szFileFullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
            DataSet res = excelDataReader.AsDataSet();

            for (int nSheet = 0; nSheet < res.Tables.Count; nSheet++)
            {
                try
                {
                    DataTable dtSheep = res.Tables[nSheet];
                    if (dtSheep.TableName.Contains("$"))
                    {
                        continue;
                    }

                    for (int col = 1; col < dtSheep.Columns.Count; col++)
                    {
                        string szLanguageName = dtSheep.Rows[0][col].ToString();
                        if (string.IsNullOrEmpty(szLanguageName))
                        {
                            continue;
                        }
                        Dictionary<string, string> mapOneLanguage = null;
                        if (!mapResult.ContainsKey(szLanguageName))
                        {
                            mapOneLanguage = new Dictionary<string, string>();
                            mapResult.Add(szLanguageName, mapOneLanguage);
                        }
                        mapOneLanguage = mapResult[szLanguageName];

                        for (int row = 0; row < dtSheep.Rows.Count; row++)
                        {
                            DataRow rowData = dtSheep.Rows[row];
                            if (row == 0 || row == 1)
                            {
                                continue;
                            }
                            string szKey = dtSheep.Rows[row][0].ToString();
                            if (string.IsNullOrEmpty(szKey))
                            {
                                continue;
                            }
                            string szValue = dtSheep.Rows[row][col].ToString();
                            if (string.IsNullOrEmpty(szValue))
                            {
                                szValue = szKey;
                            }
                            if (mapOneLanguage.ContainsKey(szKey))
                            {
                                continue;
                            }
                            mapOneLanguage.Add(szKey, szValue);
                        }
                    }
                }
                catch (Exception exception)
                {
                    Debug.LogError(string.Format(" {0} in {1} _ParseExcelFileToLanguagesKeyValue fail error={2}", res.Tables[nSheet].TableName, a_szFileFullName, exception.ToString()));
                }

            }
            return mapResult;
        }

        private static void _CreateOneLanguageConfig(Dictionary<string, string> a_mapKeyValue, string a_szLanguageName, string a_szConfigName)
        {
            string szLocalizationPath = CPackageUtility.Path.GetCombinePath(mc_szLocaliztionRootPath, a_szLanguageName);
            DirectoryInfo dirLocalizationPath = new DirectoryInfo(szLocalizationPath);
            if (!dirLocalizationPath.Exists)
            {
                dirLocalizationPath = Directory.CreateDirectory(szLocalizationPath);
            }

            string szXMLPath = CPackageUtility.Path.GetCombinePath(szLocalizationPath, mc_szXMLDirName);
            DirectoryInfo dirXMLPath = new DirectoryInfo(szXMLPath);
            if (!dirXMLPath.Exists)
            {
                dirXMLPath = Directory.CreateDirectory(szXMLPath);
            }

            string szConfigFileFullPath = CPackageUtility.Path.GetCombinePath(szXMLPath, a_szConfigName);
            szConfigFileFullPath = Utility.Text.Format("{0}.{1}", szConfigFileFullPath, "xml");

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null));
            XmlElement xmlRoot = xmlDocument.CreateElement("Dictionaries");
            xmlDocument.AppendChild(xmlRoot);

            XmlElement xmlRootKeyValue = xmlDocument.CreateElement("Dictionary");
            XmlAttribute xmlAttribute = xmlDocument.CreateAttribute("Language");
            xmlAttribute.Value = a_szLanguageName;
            xmlRootKeyValue.Attributes.SetNamedItem(xmlAttribute);
            xmlRoot.AppendChild(xmlRootKeyValue);

            foreach (var Var in a_mapKeyValue)
            {
                string szKey = Var.Key;
                string szValue = Var.Value;


                XmlElement xmlElement = xmlDocument.CreateElement("String");
                XmlAttribute xmlAttributeKey = xmlDocument.CreateAttribute("Key");
                xmlAttributeKey.Value = szKey;
                xmlElement.Attributes.SetNamedItem(xmlAttributeKey);
                XmlAttribute xmlAttributeValue = xmlDocument.CreateAttribute("Value");
                xmlAttributeValue.Value = szValue;
                xmlElement.Attributes.SetNamedItem(xmlAttributeValue);
                xmlRootKeyValue.AppendChild(xmlElement);
            }

            xmlDocument.Save(szConfigFileFullPath);
        }

    }
}