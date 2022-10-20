
using GameFramework;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Data;
using ExcelDataReader;
using GameFrameworkPackage;

namespace UnityGameFramework.Editor
{
    public class DataTableCreatorFeildInfo
    {
        public string szName
        {
            get;
            private set;
        }

        public string szType
        {
            get;
            private set;
        }

        public DataTableCreatorFeildInfo(string a_szFieldName, string a_szFieldType)
        {
            szName = a_szFieldName;
            szType = a_szFieldType;
        }
    }
    public static class DataTableCreator
    {
        const char MC_cSplitRow = '∮';

        [MenuItem("Assets/GF/DataTable/ChangeExcelToTxt", false, 1)]
        public static Dictionary<string, string> ChangeExcelToTxt()
        {
            Dictionary<string, string> mapResult = new Dictionary<string, string>();
            int nIdx = 0;
            foreach (var obj in Selection.objects)
            {
                FileStream fileStream = File.Open(AssetDatabase.GetAssetPath(obj), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                IExcelDataReader excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(fileStream);
                DataSet res = excelDataReader.AsDataSet();
                DataTable dtSheep0 = res.Tables[0];

                int nVaildColNum = 0;
                for (int columnIndex = 0; columnIndex < res.Tables[0].Columns.Count; columnIndex++)
                {
                    string colObj = dtSheep0.Rows[0][columnIndex].ToString();
                    if (string.IsNullOrEmpty(colObj))
                    {
                        break;
                    }
                    nVaildColNum++;
                }

                StringBuilder txt = new StringBuilder();
                for (int rowIndex = 0; rowIndex < dtSheep0.Rows.Count; rowIndex++)
                {
                    DataRow rowData = dtSheep0.Rows[rowIndex];
                    if (rowData.IsNull(0))
                    {
                        continue;
                    }
                    if (rowIndex == 2)
                    {
                        continue;
                    }
                    string szRowTxt = "";
                    if (rowIndex == 0)
                    {
                        szRowTxt = "!";
                    }
                    else if (rowIndex == 1)
                    {
                        szRowTxt = "@";
                    }
                    else if (rowIndex == 2)
                    {
                        szRowTxt = "#";
                    }

                    for (int columnIndex = 0; columnIndex < res.Tables[0].Columns.Count; columnIndex++)
                    {
                        if (columnIndex >= nVaildColNum)
                        {
                            break;
                        }
                        szRowTxt = szRowTxt + MC_cSplitRow + res.Tables[0].Rows[rowIndex][columnIndex].ToString();
                    }
                    txt.AppendLine(szRowTxt);
                }

                string szPath = Application.dataPath + "/GameMain/DataTables/" + obj.name + ".txt";
                ByteTool.WriteStringByFile(szPath, txt.ToString());
                mapResult.Add(obj.name, txt.ToString());
                Debug.Log(string.Format("ChangeExcelToTxt success excel name = {0}", obj.name));
                nIdx++;
                EditorUtility.DisplayProgressBar("生成中", obj.name, (float)nIdx / Selection.objects.Length);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            return mapResult;
        }

        public static void CreateDataTableScript()
        {
            int nIdx = 0;
            foreach (var obj in Selection.objects)
            {
                TextAsset asset_text = obj as TextAsset;
                _CreateDataTableScript(asset_text.name, asset_text.text);
                nIdx++;
                EditorUtility.DisplayProgressBar("生成中", obj.name, (float)nIdx / Selection.objects.Length);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/GF/DataTable/CreateToTxtAddScript", false, 3)]
        public static void ChangeExcelToTxtAndScript()
        {
            int nIdx = 0;
            Dictionary<string, string> mapResultTxt = ChangeExcelToTxt();
            foreach (var item in mapResultTxt)
            {
                _CreateDataTableScript(item.Key, item.Value);
                nIdx++;
                EditorUtility.DisplayProgressBar("生成中", item.Key, (float)nIdx / Selection.objects.Length);
            }
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
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

            if (arr_names[1] != "Id" || arr_types[1] != "int")
            {
                Debug.LogError(string.Format("{0} CreateDataTableScript fail arr_names[1] != Id || arr_types[1] != int", a_szName));
                return;
            }

            List<DataTableCreatorFeildInfo> list_feild = new List<DataTableCreatorFeildInfo>();
            for (int i = 1; i < arr_names.Length; i++)
            {
                list_feild.Add(new DataTableCreatorFeildInfo(arr_names[i], arr_types[i]));
            }

            StringBuilder code = new StringBuilder();
            code.AppendLine("using GameFramework.DataTable;");
            code.AppendLine("using GameFrameworkPackage;");
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

            string szPath = Application.dataPath + "/GameMain/_Logic/Scripts/Defines/DataTableDefine/" + szClassName + ".cs";
            ByteTool.WriteStringByFile(szPath, code.ToString());

            Debug.Log(string.Format("CreateDataTableScript success ClassName = {0}", szClassName));
        }

        [MenuItem("Assets/GF/DataTable/CreateToTxtAddEnum", false, 4)]
        public static void CreateTextAndEnumScript()
        {
            ChangeExcelToTxt();
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

        public static void TempPreloadDataTable()
        {
            StringBuilder txt = new StringBuilder();
            foreach (var obj in Selection.objects)
            {
                txt.AppendLine(string.Format("_LoadDataTable(\"{0}\");", obj.name));
            }
            string szPath = Application.dataPath + "/GameMain/DataTables/_temp_preload.txt";
            ByteTool.WriteStringByFile(szPath, txt.ToString());
        }
    }
}