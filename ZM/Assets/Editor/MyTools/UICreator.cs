using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;

namespace UnityGameFramework.Editor
{
    public static class CUICreator
    {
        private class CUIItemCode
        {
            private string m_szPrefixName;
            private string m_szTypeName;
            private string m_szInitCodeFormat;

            public CUIItemCode(string a_szPrefixName, string a_szTypeName, string a_szInitCodeFormat)
            {
                m_szPrefixName = a_szPrefixName;
                m_szTypeName = a_szTypeName;
                m_szInitCodeFormat = a_szInitCodeFormat;
            }

            public string GetDeclareCode(string a_szGameObjectName)
            {
                return Utility.Text.Format("private {0} {1};", m_szTypeName, a_szGameObjectName);
            }

            public string GetInitCode(string a_szGameObjectName, string a_szParentPath)
            {
                return Utility.Text.Format(m_szInitCodeFormat, a_szGameObjectName, a_szParentPath, m_szTypeName);
            }

            private static Dictionary<string, CUIItemCode> ms_mapCodeConfig = new Dictionary<string, CUIItemCode>();

            public CUIItemCode GetItemCode(string a_szPrefixName)
            {
                if (ms_mapCodeConfig.Count == 0)
                {
                    _InitItemCode();
                }

                if (ms_mapCodeConfig.ContainsKey(a_szPrefixName))
                {
                    return ms_mapCodeConfig[a_szPrefixName];
                }
                return null;
            }

            private static void _InitItemCode()
            {
                _AddNormalUIItem("Lb", "Text");
                _AddNormalUIItem("Btn", "Button");
                _AddNormalUIItem("Img", "Image");
                _AddNormalUIItem("Scl", "ScrollRect");
                _AddNormalUIItem("Input", "InputField");

                _AddItem("Prefab", "GameObject", "{0} = transform.Find(\"{1}{0}\").gameObject;");
                _AddItem("Go", "GameObject", "{0} = transform.Find(\"{1}{0}\").gameObject;");
                _AddItem("Content", "CUIFormItemContent", "{0} = CreateChildItemCtrl<CUIFormItemContent>(this,\"{1}{0}\");");
                _AddItem("Ctrl", "CUICtrl", "{0} = CreateChildItemCtrl<CUICtrl>(this,\"{1}{0}\");");
            }

            private static void _AddNormalUIItem(string a_szPrefixName, string a_szTypeName)
            {
                _AddItem(a_szPrefixName, a_szTypeName, "{0} = transform.Find(\"{1}{0}\").GetComponent<{2}>();");
            }

            private static void _AddItem(string a_szPrefixName, string a_szTypeName, string a_szInitCodeFormat)
            {
                ms_mapCodeConfig.Add(a_szPrefixName, new CUIItemCode(a_szPrefixName, a_szTypeName, a_szInitCodeFormat));
            }
        }

    }
}

