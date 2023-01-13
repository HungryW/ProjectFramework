using GameFramework;
using System.Collections.Generic;
using LitJson;
using System;
using System.Diagnostics;
using UnityEngine.Assertions;

namespace GameFrameworkPackage
{
    public class CConstantsTableTypeVal<T> : CConstantsTableValBase
    {
        protected T m_val;
        public CConstantsTableTypeVal(T a_val)
        {
            m_val = a_val;
        }

        public T GetVal()
        {
            return m_val;
        }

        public override Type ValType()
        {
            return typeof(T);
        }
    }

    public abstract class CConstantsTableValBase
    {
        public abstract Type ValType();
    }

    public class CConstantsTableMgr : ISingleton<CConstantsTableMgr>
    {
        private Dictionary<string, CConstantsTableValBase> m_mapVal = new Dictionary<string, CConstantsTableValBase>();

        public T GetVal<T>(string a_szKey)
        {
            Assert.IsTrue(!string.IsNullOrEmpty(a_szKey), Utility.Text.Format("常量数据错误 key='{0}'", a_szKey));
            Assert.IsTrue(m_mapVal.ContainsKey(a_szKey), Utility.Text.Format("常量数据错误 key='{0}',无数据", a_szKey));
            Assert.IsTrue(m_mapVal[a_szKey].ValType() == typeof(T), Utility.Text.Format("常量数据错误 key='{0}',类型不匹配", a_szKey));

            return ((CConstantsTableTypeVal<T>)m_mapVal[a_szKey]).GetVal();
        }

        public void Load(string a_szText)
        {
            m_mapVal.Clear();
            string[] rowTexts = CPackageUtility.Text.SplitToLines(a_szText);
            foreach (string dataRowText in rowTexts)
            {
                string[] arrData = CDataTableExtension.SplitDataRow(dataRowText);
                if (arrData.Length < 4)
                {
                    continue;
                }
                string szKey = arrData[1];
                string szType = arrData[2].ToLower();
                string szVal = arrData[3];
                CConstantsTableValBase val = null;
                if (szType == "int")
                {
                    int nVal = 0;
                    int.TryParse(szVal, out nVal);
                    val = new CConstantsTableTypeVal<int>(nVal);
                }
                else if (szType == "float")
                {
                    float fVal = 0f;
                    float.TryParse(szVal, out fVal);
                    val = new CConstantsTableTypeVal<float>(fVal);
                }
                else if (szType == "string")
                {
                    val = new CConstantsTableTypeVal<string>(szVal);
                }
                else if (szType == "[int]")
                {
                    List<int> listVal = szVal.SplitStringToIntList(';');
                    val = new CConstantsTableTypeVal<List<int>>(listVal);
                }
                else if (szType == "[string]")
                {
                    List<string> listVal = szVal.SplitToStringList(';');
                    val = new CConstantsTableTypeVal<List<string>>(listVal);
                }
                else if (szType == "jsonarray" || szType == "json")
                {
                    JsonData jsonVal = JsonMapper.ToObject(szVal);
                    val = new CConstantsTableTypeVal<JsonData>(jsonVal);
                }

                if (null != val)
                {
                    m_mapVal.Add(szKey, val);
                }
            }
        }
    }

}
