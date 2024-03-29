﻿using System;
using GameFramework.DataTable;
using UnityGameFramework.Runtime;
using System.Collections.Generic;
using System.Reflection;
using GameFramework;

namespace GameFrameworkPackage
{

    public static class CDataTableExtension
    {
        private const string DataRowClassPrefixName = "Defines.DR";
        private static readonly string[] ColumnSplit = new string[] { "∮" };

        public static void LoadDataTable(this DataTableComponent dataTableComponent, string dataTableName, string dataTableAssetName, object userData)
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                Log.Warning("Data table name is invalid.");
                return;
            }

            string dataRowClassName = Utility.Text.Format("{0}{1}", DataRowClassPrefixName, dataTableName);
            Assembly assDefine = CGameEntryMgr.HotFixComponent.GetAsmByName(CHotFixSetting.GetHotFixDefineDllName());
            Type dataRowType = assDefine.GetType(dataRowClassName);
            if (dataRowType == null)
            {
                Log.Warning("Can not get data row type with class name '{0}'.", dataRowClassName);
                return;
            }

            DataTableBase dataTable = dataTableComponent.CreateDataTable(dataRowType);
            dataTable.ReadData(dataTableAssetName, CConstAssetPriority.DataTableAsset, userData);
        }

        public static T GetDataRow<T>(this DataTableComponent a_componentDataTable, int a_nRowId) where T : IDataRow
        {
            IDataTable<T> dt = CGameEntryMgr.DataTable.GetDataTable<T>();
            if (dt == null)
            {
                Log.Warning("Can not get data dt={0} row id ={1}.", typeof(T), a_nRowId);
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

