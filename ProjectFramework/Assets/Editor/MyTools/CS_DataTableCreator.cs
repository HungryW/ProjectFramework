
using GameFramework;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Data;
using ExcelDataReader;
using System.Xml;
using GameFrameworkPackage;

namespace UnityGameFramework.Editor
{
    public static partial class CSDataTableCreator
    {
        public static void ViewExcelPath()
        {
            EditorUtility.OpenWithDefaultApp(_GetExcelsPath());
        }

        public static void ViewTxtPath()
        {
            EditorUtility.OpenWithDefaultApp(mc_szTxtPath);
        }
    }

    public static partial class CSDataTableCreator
    {
        public static char MC_cSplitRow = '∮';
        public static string mc_szConfigPath = "Editor/MyTools/Config/DataTabelGenerateConfig.xml";
        public static string mc_szTxtPath = Application.dataPath + "/GameMain/DataTables";
        public static string mc_szCsDefinePath = Application.dataPath + "/GameMain/_Logic/Scripts/Defines/DataTableDefine";

        public static void CreateCofig_CS_Local()
        {
            CreateConfig_ClientByExcelFloder();
            CreateConfigSever();
        }

        public static void CreateConfigSever()
        {
            string szGenerateServerConfigBatPath = CPackageUtility.Path.GetCombinePath(_GetLocalSeverConfigBatPath(), "run.bat");
            Application.OpenURL(szGenerateServerConfigBatPath);
        }

        public static void CreateConfig_ClientByExcelFloder()
        {
            int nIdx = 0;
            string szExcelsPath = _GetExcelsPath();
            Dictionary<string, string> mapResultTxt = _ParseToTxtByExcelFloder(szExcelsPath);
            foreach (var item in mapResultTxt)
            {
                _CreateDataTableScript(item.Key, item.Value);
                nIdx++;
                EditorUtility.DisplayProgressBar("生成中", item.Key, (float)nIdx / Selection.objects.Length);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/MyTool/DataTable/ChangeExcelToTxtAndCs", false, 1)]
        public static void CreateConfig_ClientBySelection()
        {
            int nIdx = 0;
            Dictionary<string, string> mapResultTxt = _ParseToTxtBySelectionExcelFile();
            foreach (var item in mapResultTxt)
            {
                _CreateDataTableScript(item.Key, item.Value);
                nIdx++;
                EditorUtility.DisplayProgressBar("生成中", item.Key, (float)nIdx / Selection.objects.Length);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }


        private static string _GetLocalSeverConfigBatPath()
        {
            return _GetConfig("LocalGenerateSeverConfigPath", "请选择生成服务器配置的.bat文件所在路径");
        }


        private static string _GetExcelsPath()
        {
            return _GetConfig("ExcelsPath", "请选择Excel文件路径");
        }


        private static string _GetConfig(string a_szConfigName, string a_szTipTxt)
        {
            string szConfigFilePath = CPackageUtility.Path.GetCombinePath(Application.dataPath, mc_szConfigPath); ;
            if (File.Exists(szConfigFilePath))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(szConfigFilePath);
                XmlNode xmlRootOld = xmlDocument.SelectSingleNode("Root");
                if (null != xmlRootOld)
                {
                    XmlNode xmlNode = xmlRootOld.SelectSingleNode(a_szConfigName);
                    if (null != xmlNode)
                    {
                        return xmlNode.InnerText;
                    }
                    else
                    {
                        string szPathNew = EditorUtility.OpenFolderPanel(a_szTipTxt, Application.dataPath, "");
                        XmlElement xmlElementNew = xmlDocument.CreateElement(a_szConfigName);
                        xmlElementNew.InnerText = szPathNew;
                        xmlRootOld.AppendChild(xmlElementNew);

                        xmlDocument.Save(szConfigFilePath);
                        AssetDatabase.Refresh();
                        return szPathNew;
                    }
                }
            }


            if (!File.Exists(szConfigFilePath))
            {
                using (File.Create(szConfigFilePath)) { }
            }

            XmlDocument newXmlDocument = new XmlDocument();
            string szPath = EditorUtility.OpenFolderPanel(a_szTipTxt, Application.dataPath, "");
            XmlElement xmlRoot = newXmlDocument.CreateElement("Root");
            newXmlDocument.AppendChild(xmlRoot);
            XmlElement xmlElement = newXmlDocument.CreateElement(a_szConfigName);
            xmlElement.InnerText = szPath;
            xmlRoot.AppendChild(xmlElement);

            newXmlDocument.Save(szConfigFilePath);
            return szPath;
        }

        private static Dictionary<string, string> _ParseToTxtByExcelFloder(string a_szExcelsPath)
        {
            DirectoryInfo direction = new DirectoryInfo(a_szExcelsPath);
            FileInfo[] _fileList = direction.GetFiles("*.xlsx", SearchOption.AllDirectories);
            Dictionary<string, string> mapResult = new Dictionary<string, string>();
            for (int i = 0; i < _fileList.Length; i++)
            {
                Dictionary<string, string> mapOneFile = _ParseExcelFileToTxts(_fileList[i].FullName);
                foreach (var item in mapOneFile)
                {
                    mapResult.Add(item.Key, item.Value);
                }
                EditorUtility.DisplayProgressBar("生成中", _fileList[i].Name, (float)i / _fileList.Length);
            }
            EditorUtility.ClearProgressBar();
            return mapResult;
        }

        private static Dictionary<string, string> _ParseToTxtBySelectionExcelFile()
        {
            Dictionary<string, string> mapResult = new Dictionary<string, string>();
            int nIdx = 0;
            foreach (var obj in Selection.objects)
            {
                Dictionary<string, string> mapOneFile = _ParseExcelFileToTxts(AssetDatabase.GetAssetPath(obj));
                foreach (var item in mapOneFile)
                {
                    mapResult.Add(item.Key, item.Value);
                }
                nIdx++;
                EditorUtility.DisplayProgressBar("生成中", obj.name, (float)nIdx / Selection.objects.Length);
            }
            EditorUtility.ClearProgressBar();
            return mapResult;
        }

        private static Dictionary<string, string> _ParseExcelFileToTxts(string a_szFileFullName)
        {
            Dictionary<string, string> mapTxtResult = new Dictionary<string, string>();
            string szFileName = a_szFileFullName.Substring(a_szFileFullName.LastIndexOf('/') + 1);
            if (szFileName.Contains("$"))
            {
                return mapTxtResult;
            }
            if (szFileName.Contains("ignore"))
            {
                return mapTxtResult;
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
                    //是常量格式表
                    if (_TryParseConstantExcelFileToTxts(dtSheep))
                    {
                        continue;
                    }
                    if (dtSheep.Rows[0].IsNull(0) || dtSheep.Rows[2].IsNull(0))
                    {
                        continue;
                    }
                    if (!dtSheep.Rows[0][0].ToString().Contains("c") && dtSheep.Rows[2][0].ToString() != "Id")
                    {
                        //无效sheet
                        continue;
                    }

                    List<int> listClientUseColIdx = new List<int>();
                    for (int columnIndex = 0; columnIndex < dtSheep.Columns.Count; columnIndex++)
                    {
                        string colObj = dtSheep.Rows[0][columnIndex].ToString();
                        if (colObj.Contains("c") || colObj.Contains("C"))
                        {
                            listClientUseColIdx.Add(columnIndex);
                        }
                    }

                    StringBuilder txt = new StringBuilder();
                    for (int rowIndex = 0; rowIndex < dtSheep.Rows.Count; rowIndex++)
                    {
                        DataRow rowData = dtSheep.Rows[rowIndex];
                        if (rowIndex == 0 || rowIndex == 1 || rowIndex == 4)
                        {
                            continue;
                        }
                        string szRowTxt = "";
                        if (rowIndex == 2)
                        {
                            szRowTxt = "!";
                        }
                        else if (rowIndex == 3)
                        {
                            szRowTxt = "@";
                        }
                        if (string.IsNullOrEmpty(rowData[0].ToString()))
                        {
                            continue;
                        }

                        foreach (int columnIndex in listClientUseColIdx)
                        {
                            szRowTxt = szRowTxt + MC_cSplitRow + dtSheep.Rows[rowIndex][columnIndex].ToString();
                        }
                        txt.AppendLine(szRowTxt);
                    }

                    string szPath = mc_szTxtPath + "/" + dtSheep.TableName + ".txt";
                    ByteTool.WriteStringByFile(szPath, txt.ToString());
                    mapTxtResult.Add(dtSheep.TableName, txt.ToString());
                    Debug.Log(string.Format("ChangeExcelToTxt success excel name = {0}", dtSheep.TableName));
                }
                catch
                {
                    Debug.LogError(string.Format(" {0} in {1} CreateDataTableScript fail", res.Tables[nSheet].TableName, a_szFileFullName));
                }

            }
            return mapTxtResult;
        }



        private static void _CreateDataTableScript(string a_szName, string a_szContent)
        {
            string szClassName = "DR" + a_szName;
            string[] arr_names = { };
            string[] arr_types = { };
            string[] arr_sz_row_text = CPackageUtility.Text.SplitToLines(a_szContent);
            for (int i = 0; i < arr_sz_row_text.Length; i++)
            {
                if (arr_sz_row_text[i].Length <= 0)
                {
                    continue;
                }

                if (arr_sz_row_text[i][0] == '!')
                {
                    arr_names = arr_sz_row_text[i].Split(MC_cSplitRow);
                }
                else if (arr_sz_row_text[i][0] == '@')
                {
                    arr_types = arr_sz_row_text[i].Split(MC_cSplitRow);
                }
            }

            if (arr_names.Length == 0)
            {
                Debug.LogError(string.Format("{0} CreateDataTableScript fail, don`t have name row, please Add ! on row head", a_szName));
                return;
            }

            if (arr_types.Length == 0)
            {
                Debug.LogError(string.Format("{0} CreateDataTableScript fail, don`t have type row, please Add @ on row head", a_szName));
                return;
            }

            if (arr_names.Length != arr_types.Length)
            {
                Debug.LogError(string.Format("{0} CreateDataTableScript fail arr_names.Length != arr_types.Length, arr_names.Length = {1}, arr_types.Length = {2}", a_szName, arr_names.Length, arr_types.Length));
                return;
            }

            //if (arr_names[1] != "Id" || arr_types[1] != "int")
            //{
            //    Debug.LogError(string.Format("{0} CreateDataTableScript fail arr_names[1] != Id || arr_types[1] != int", a_szName));
            //    return;
            //}

            List<DataTableCreatorFeildInfo> list_feild = new List<DataTableCreatorFeildInfo>();
            for (int i = 1; i < arr_names.Length; i++)
            {
                string szType = arr_types[i];
                if (arr_types[i].Contains("[") || arr_types[i].Contains("{"))
                {
                    szType = "string";
                }

                if (arr_types[i].Equals("array"))
                {
                    szType = "JsonData";
                }

                if (arr_types[i].Equals("json"))
                {
                    szType = "JsonData";
                }

                string szName = arr_names[i];
                if (szName == "id")
                {
                    szName = "Id";
                }
                szName.Replace("(", string.Empty);
                szName.Replace(")", string.Empty);
                szName.Replace(",", string.Empty);

                list_feild.Add(new DataTableCreatorFeildInfo(szName, szType));
            }

            StringBuilder code = new StringBuilder();
            code.AppendLine("using GameFramework.DataTable;");
            code.AppendLine("using GameFrameworkPackage;");
            code.AppendLine("using LitJson;");
            code.AppendLine("namespace Defines.DataTable");
            code.AppendLine("{");
            code.AppendLine(string.Format("\tpublic class {0} : IDataRow", szClassName));
            code.AppendLine("\t{");

            foreach (DataTableCreatorFeildInfo info in list_feild)
            {
                if (info.szType == "desc")
                {
                    continue;
                }
                if (string.IsNullOrEmpty(info.szType) || string.IsNullOrEmpty(info.szName))
                {
                    continue;
                }
                code.AppendLine(string.Format("\t\tpublic {0} {1}", info.szType, info.szName));
                code.AppendLine("\t\t{");
                code.AppendLine("\t\t\tget;");
                if (info.szName == "Id")
                {
                    code.AppendLine("\t\t\tprotected set;");
                }
                else
                {
                    code.AppendLine("\t\t\tprivate set;");
                }
                code.AppendLine("\t\t}");
                code.AppendLine("\t\t");
            }

            code.AppendLine("\t\tpublic void ParseDataRow(string dataRowText)");
            code.AppendLine("\t\t{");
            code.AppendLine("\t\t\tstring[] text = CDataTableExtension.SplitDataRow(dataRowText);");
            code.AppendLine("\t\t\tint index = 1;");
            foreach (DataTableCreatorFeildInfo info in list_feild)
            {
                if (info.szType == "desc")
                {
                    code.AppendLine("\t\t\tindex++;");
                }
                else if (info.szType == "string")
                {
                    code.AppendLine(string.Format("\t\t\t{0} = text[index++];", info.szName));
                }
                else if (info.szType == "JsonData")
                {
                    code.AppendLine(string.Format("\t\t\t{0} = JsonMapper.ToObject(text[index++]);", info.szName));
                }
                else if (info.szType == "int")
                {
                    code.AppendLine(string.Format("\t\t\t{0}= int.Parse(text[index++]);", info.szName));
                }
                else if (info.szType == "bool")
                {
                    code.AppendLine(string.Format("\t\t\t{0}= bool.Parse(text[index++]);", info.szName));
                }
                else if (info.szType == "float")
                {
                    code.AppendLine(string.Format("\t\t\t{0}= float.Parse(text[index++]);", info.szName));
                }
            }
            code.AppendLine("\t\t}");

            code.AppendLine("\t}");
            code.AppendLine("}");

            string szPath = mc_szCsDefinePath + "/" + szClassName + ".cs";
            ByteTool.WriteStringByFile(szPath, code.ToString());

            Debug.Log(string.Format("CreateDataTableScript success ClassName = {0}", szClassName));
        }

        public static void CreateTextAndEnumScript()
        {
            CreateEnumScript();
        }


        public static void CreateEnumScript()
        {
            int nIdx = 0;
            foreach (var obj in Selection.objects)
            {
                FileStream fileStream = File.Open(AssetDatabase.GetAssetPath(obj), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                DataSet res = excelDataReader.AsDataSet();

                StringBuilder code = new StringBuilder();
                code.AppendLine("namespace Defines");
                code.AppendLine("{");
                code.AppendLine(string.Format("\tpublic enum E{0}ID", obj.name));
                code.AppendLine("\t{");

                for (int rowIndex = 3; rowIndex < res.Tables[0].Rows.Count; rowIndex++)
                {
                    if (res.Tables[0].Rows[rowIndex].IsNull(0))
                    {
                        continue;
                    }

                    code.AppendLine(string.Format("\t\t{0} = {1},", res.Tables[0].Rows[rowIndex][2], res.Tables[0].Rows[rowIndex][0]));
                }

                code.AppendLine("\t}");
                code.AppendLine("}");


                string szPath = string.Format("{0}/GameMain/_Logic/Scripts/Defines/{1}Define.cs", Application.dataPath, obj.name);
                ByteTool.WriteStringByFile(szPath, code.ToString());
                Debug.Log(string.Format("CreateUIEnumScript success"));
                nIdx++;
                EditorUtility.DisplayProgressBar("生成中", obj.name, (float)nIdx / Selection.objects.Length);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }


        #region 常量表解析
        private static bool _TryParseConstantExcelFileToTxts(DataTable dtSheep)
        {
            if (dtSheep.Rows[0].IsNull(0) || dtSheep.Rows[2].IsNull(0))
            {
                return false;
            }
            if (dtSheep.Rows[2][0].ToString().ToLower() != "id"
                || dtSheep.Rows[2][1].ToString().ToLower() != "cls"
                || dtSheep.Rows[2][2].ToString().ToLower() != "content")
            {
                return false;
            }

            StringBuilder txt = new StringBuilder();
            for (int rowIndex = 5; rowIndex < dtSheep.Rows.Count; rowIndex++)
            {
                DataRow rowData = dtSheep.Rows[rowIndex];
                if (rowIndex == 0 || rowIndex == 1 || rowIndex == 4)
                {
                    continue;
                }
                string szRowTxt = "";
                if (rowIndex == 2)
                {
                    szRowTxt = "!";
                }
                else if (rowIndex == 3)
                {
                    szRowTxt = "@";
                }
                if (string.IsNullOrEmpty(rowData[0].ToString()))
                {
                    continue;
                }
                for (int colIdx = 0; colIdx < 3; colIdx++)
                {
                    szRowTxt = szRowTxt + MC_cSplitRow + dtSheep.Rows[rowIndex][colIdx].ToString();
                }
                txt.AppendLine(szRowTxt);
            }

            string szPath = mc_szTxtPath + "/" + dtSheep.TableName + ".txt";
            ByteTool.WriteStringByFile(szPath, txt.ToString());
            Debug.Log(string.Format("ChangeConstantExcelToTxt success excel name = {0}", dtSheep.TableName));
            return true;
        }
    }
    #endregion
}