using System;
using GameFramework.DataTable;
using UnityGameFramework.Runtime;
using System.Collections.Generic;

namespace GameFrameworkPackage
{

    public static class CDataTableExtension
    {
        private const string DataRowClassPrefixName = "Defines.DataTable.DR";
        private static readonly string[] ColumnSplit = new string[] { "∮" };

        public static void LoadDataTable(this DataTableComponent dataTableComponent, string dataTableName, string dataTableAssetName, object userData)
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string[] splitedNames = dataTableName.Split('_');
            if (splitedNames.Length > 2)
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string dataRowClassName = DataRowClassPrefixName + splitedNames[0];
            Type dataRowType = Type.GetType(dataRowClassName);
            if (dataRowType == null)
            {
                Log.Warning("Can not get data row type with class name '{0}'.", dataRowClassName);
                return;
            }

            string name = splitedNames.Length > 1 ? splitedNames[1] : null;
            DataTableBase dataTable = dataTableComponent.CreateDataTable(dataRowType, name);
            dataTable.ReadData(dataTableAssetName, CConstAssetPriority.DataTableAsset, userData);
        }

        public static T GetDataRow<T>(this DataTableComponent a_componentDataTable, int a_nRowId) where T : IDataRow
        {
            IDataTable<T> dt = CGameEntryMgr.DataTable.GetDataTable<T>();
            if (dt == null)
            {
                Log.Warning("Can not get data dt={0} row id ={1}.", dt.GetType(), a_nRowId);
            }
            T dr = dt.GetDataRow(a_nRowId);
            if (dr == null)
            {
                Log.Warning("Can not get data dt={0} row id ={1}.", dt.GetType(), a_nRowId);
            }
            return dr;
        }

        public static bool HasDataRow<T>(this DataTableComponent a_componentDataTable, int a_nRowId) where T : IDataRow
        {
            IDataTable<T> dt = CGameEntryMgr.DataTable.GetDataTable<T>();
            return dt.HasDataRow(a_nRowId);
        }



        public static T[] GetAllDataRows<T>(this DataTableComponent a_componentDataTable) where T : IDataRow
        {
            IDataTable<T> dt = CGameEntryMgr.DataTable.GetDataTable<T>();
            return dt.GetDataRows((T a_Row1, T a_Row2) => { return a_Row1.Id.CompareTo(a_Row2.Id); });
        }

        public static List<int> GetAllDataId<T>(this DataTableComponent a_componentDataTable) where T : IDataRow
        {
            IDataTable<T> dt = CGameEntryMgr.DataTable.GetDataTable<T>();
            List<int> listId = new List<int>();
            foreach (var dr in dt.GetAllDataRows())
            {
                listId.Add(dr.Id);
            }
            return listId;
        }

        public static string[] SplitDataRow(string dataRowText)
        {
            return dataRowText.Split(ColumnSplit, StringSplitOptions.None);
        }

    }
}

