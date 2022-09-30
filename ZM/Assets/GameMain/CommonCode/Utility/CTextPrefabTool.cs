using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Defines.DataTable;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Text;

public class FontSize
{
    public int Id;
    public int nValue;
    public FontSize(int id, int val)
    {
        Id = id;
        nValue = val;
    }
}

public class FontColor
{
    public string szName { get; set; }
    public string szValue { get; set; }
    public FontColor(string name, string value)
    {
        szName = name;
        szValue = value;
    }
}


public static class TextHelperData
{
    private const string m_szDataPath = "Assets/Editor/MyTools/TextToolDataEditor/FontStyle.xml";

    public static List<FontColor> m_listColor = new List<FontColor>();
    public static List<FontSize> m_listSize = new List<FontSize>();
    public static List<int> m_listOutLineSize = new List<int>();
    public static List<FontColor> m_listOutLineColor = new List<FontColor>();

    public static void InitData()
    {
        m_listColor.Clear();
        m_listSize.Clear();
        m_listOutLineSize.Clear();
        m_listOutLineColor.Clear();

        LoadXmlData();

        #region FONT_COLOR
        m_listColor.Sort((FontColor color1, FontColor color2) =>
        {
            return -Convert.ToInt32(color1.szValue, 16).CompareTo(Convert.ToInt32(color2.szValue, 16));
        });
        #endregion

        #region FONT_SIZE

        m_listSize.Sort((FontSize val1, FontSize val2) =>
        {
            return val1.nValue.CompareTo(val2.nValue);
        });
        #endregion

        #region FONT_TYPE
        #endregion

        #region OUTLINE_COLOR
        m_listOutLineColor.Sort((FontColor color1, FontColor color2) =>
        {
            return -Convert.ToInt32(color1.szValue, 16).CompareTo(Convert.ToInt32(color2.szValue, 16));
        });
        #endregion

        #region OUTLINE_SIZE
        m_listOutLineSize.Sort((int val1, int val2) =>
        {
            return val1.CompareTo(val2);
        });
        #endregion
    }

    public static string[] GetColorOptions()
    {
        string[] arrColor;
        List<string> listName = new List<string>();
        foreach (var item in m_listColor)
        {
            listName.Add(string.Format("{0}#{1}",item.szName,item.szValue));
        }
        arrColor = listName.ToArray();
        return arrColor;
    }

    public static string[] GetSizeOptions()
    {
        List<string> listName = new List<string>();
        string[] arrName;
        for (int i = 0; i < m_listSize.Count; i++)
        {
            listName.Add(m_listSize[i].nValue.ToString());
        }
        arrName = listName.ToArray();
        return arrName;
    }

    public static string[] GetOutLineColorOptions()
    {
        string[] arrColor;
        List<string> listName = new List<string>();
        foreach (var item in m_listOutLineColor)
        {
            listName.Add(string.Format("{0}#{1}", item.szName, item.szValue));
        }
        arrColor = listName.ToArray();
        return arrColor;
    }

    public static string[] GetOutLineSizeOptions()
    {
        List<string> listName = new List<string>();
        string[] arrName;
        for (int i = 0; i < m_listOutLineSize.Count; i++)
        {
            listName.Add(m_listOutLineSize[i].ToString());
        }
        arrName = listName.ToArray();
        return arrName;
    }

    public static void AddColor(string a_name,string a_color)
    {
        m_listColor.Add(new FontColor(a_name, a_color));
    }

    public static void AddSize(int id,int val)
    {
        m_listSize.Add(new FontSize(id, val));
    }

    public static void AddOutLineColor(string a_name, string a_color)
    {
        m_listOutLineColor.Add(new FontColor(a_name, a_color));
    }

    public static void AddOutLineSize(int val)
    {
        m_listOutLineSize.Add(val);
    }

    public static void LoadXmlData()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(m_szDataPath);
        XmlNode xmlRoot = xmlDoc.SelectSingleNode("fontStyle");
        XmlNode xmlColor = xmlRoot.SelectSingleNode("colorData");
        XmlNodeList nodeListColor = xmlColor.ChildNodes;
        for (int i = 0; i < nodeListColor.Count; i++)
        {
            XmlNode xmlNode = nodeListColor.Item(i);
            
            AddColor(xmlNode.Attributes.GetNamedItem("name").Value, xmlNode.Attributes.GetNamedItem("value").Value);
        }
        XmlNode xmlOutLineColor = xmlRoot.SelectSingleNode("outLineColor");
        XmlNodeList nodeListOClor = xmlOutLineColor.ChildNodes;
        for (int i = 0; i < nodeListOClor.Count; i++)
        {
            XmlNode xmlNode = nodeListOClor.Item(i);
            
            AddOutLineColor(xmlNode.Attributes.GetNamedItem("name").Value, xmlNode.Attributes.GetNamedItem("value").Value);
        }
        XmlNode xmlSize = xmlRoot.SelectSingleNode("sizeData");
        XmlNodeList nodeListSize = xmlSize.ChildNodes;
        for (int i = 0; i < nodeListSize.Count; i++)
        {
            XmlNode xmlNode = nodeListSize.Item(i);
            int size;
            if (int.TryParse(xmlNode.InnerText, out size))
            {
                AddSize(i, size);
            }
        }
        XmlNode xmlOutLineSize = xmlRoot.SelectSingleNode("outLineSize");
        XmlNodeList nodeListOSize = xmlOutLineSize.ChildNodes;
        for (int i = 0; i < nodeListOSize.Count; i++)
        {
            XmlNode xmlNode = nodeListOSize.Item(i);
            int size;
            if (int.TryParse(xmlNode.InnerText, out size))
            {
                AddOutLineSize(size);
            }
        }
    }
}

public class CTextPrefabTool : MonoBehaviour
{
    public int m_nSizeId = 0;
    public int m_nColorIdx = 0;
    public string m_nColorId = "";
    public int m_nFontIdx = 0;
    public bool m_bUseOutline = false;
    public bool m_bUseShadow = false;
    public int m_nOutlineColorIdx = 0;
    public string m_nOutlineColorId = "";
    public float m_nOutlineWidthId = 0f;
    public float m_fAlha = 1f;

    private void OnValidate()
    {
    }

    public void ChangeSize(int a_nSizeIdx)
    {
        Text text = gameObject.GetComponent<Text>();
        text.fontSize = TextHelperData.m_listSize[a_nSizeIdx].nValue;
        m_nSizeId = TextHelperData.m_listSize[a_nSizeIdx].nValue;
        OnValidate();
    }

    public void ChangeColor(int a_nColorIdx)
    {
        Text text = gameObject.GetComponent<Text>();
        Color color = new Color();
        ColorUtility.TryParseHtmlString(string.Format("#{0}", TextHelperData.m_listColor[a_nColorIdx].szValue), out color);
        text.color = color;
        m_nColorIdx = a_nColorIdx;
        m_nColorId = TextHelperData.m_listColor[a_nColorIdx].szName;
    }

    public void ChangeColor(int a_nColorIdx, string a_szKey)
    {
        Text text = gameObject.GetComponent<Text>();
        Color color = new Color();
        ColorUtility.TryParseHtmlString(string.Format("#{0}", TextHelperData.m_listColor[a_nColorIdx].szValue),out color);
        text.color = color;
        m_nColorIdx = a_nColorIdx;
        m_nColorId = TextHelperData.m_listColor[a_nColorIdx].szName;
    }

    public void ChangeColor(string a_szVal)
    {
        int nColorIdx = -1;
        Text text = gameObject.GetComponent<Text>();
        Color color = new Color();
        for (int i = 0; i < TextHelperData.m_listColor.Count; i++)
        {
            if (TextHelperData.m_listColor[i].szValue == a_szVal)
            {
                nColorIdx = i;
            }
        }
        if (nColorIdx < 0)
        {
            Debug.LogWarning("TEXT_TOOL_CHANGE_COLOR_FAILED:"+ a_szVal);
            return;
        }
        ColorUtility.TryParseHtmlString(string.Format("#{0}", TextHelperData.m_listColor[nColorIdx].szValue), out color);
        text.color = color;
        m_nColorIdx = nColorIdx;
        m_nColorId = TextHelperData.m_listColor[nColorIdx].szName;
    }

    public void ChangeFont(int a_nIdx)
    {

#if UNITY_EDITOR
        Text text = gameObject.GetComponent<Text>();
        if (a_nIdx == 0)
        {
            text.font = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/main.ttf");
        }
        else if (a_nIdx == 1)
        {
            text.font = UnityEditor.AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/light.ttf");
        }
        m_nFontIdx = a_nIdx;
#endif
    }

    public void ChangeUseOutLine(bool a_bIsUseOutline)
    {
        if (a_bIsUseOutline)
        {
            gameObject.GetOrAddComponent<BoxOutline>();
        }
        else
        {
            BoxOutline outLine = gameObject.GetComponent<BoxOutline>();
            if (outLine != null)
            {
                DestroyImmediate(outLine);
            }
        }
        m_bUseOutline = a_bIsUseOutline;
    }

    public void ChangeUseShadow(bool a_bIsUseShadow)
    {
        Shadow[] shadow = gameObject.GetComponents<Shadow>();

        if (a_bIsUseShadow)
        {
            bool hasShadow = false;
            if (shadow.Length <=0)
            {
                gameObject.AddComponent<Shadow>();
                m_bUseShadow = a_bIsUseShadow;
                return;
            }
            foreach (var item in shadow)
            {
                if (item != null && !(item is BoxOutline))
                {
                    hasShadow = true;
                    return;
                }
            }
            if (!hasShadow)
            {
                gameObject.AddComponent<Shadow>();
            }
        }
        else
        {
            foreach (var item in shadow)
            {
                if (item != null && !(item is BoxOutline))
                {
                    DestroyImmediate(item);
                }
            }
            
            
        }
        m_bUseShadow = a_bIsUseShadow;
    }

    public void ChangeOutlineColorIdx(int a_nColorIdx)
    {
        BoxOutline outLine = gameObject.GetComponent<BoxOutline>();
        if (null == outLine)
        {
            return;
        }
        Color color = new Color();
        ColorUtility.TryParseHtmlString(string.Format("#{0}", TextHelperData.m_listOutLineColor[a_nColorIdx].szValue), out color);
        //outLine.effectColor = color;
        outLine.ChangeOutlineColor(color);
        m_nOutlineColorId = TextHelperData.m_listOutLineColor[a_nColorIdx].szValue;
    }

    public void ChangeOutlineWidthIdx(int a_nWidthIdx)
    {
        BoxOutline outLine = gameObject.GetComponent<BoxOutline>();
        if (null == outLine)
        {
            return;
        }
        float width = TextHelperData.m_listOutLineSize[a_nWidthIdx];
        //outLine.effectDistance = new Vector2(width, width);
        outLine.ChageOutLineSize(width);
        
        m_nOutlineWidthId = width;
    }

    public void ChangeOutlineColor(string a_szVal)
    {
        int nColorIdx = -1;
        Text text = gameObject.GetComponent<Text>();
        Color color = new Color();
        for (int i = 0; i < TextHelperData.m_listColor.Count; i++)
        {
            if (TextHelperData.m_listOutLineColor[i].szValue == a_szVal)
            {
                nColorIdx = i;
            }
        }
        if (nColorIdx < 0)
        {
            Debug.LogWarning("TEXT_TOOL_CHANGE_COLOR_FAILED:" + a_szVal);
            return;
        }
        ColorUtility.TryParseHtmlString(string.Format("#{0}", TextHelperData.m_listOutLineColor[nColorIdx].szValue), out color);
        text.color = color;
        m_nOutlineColorIdx = nColorIdx;
        m_nOutlineColorId = TextHelperData.m_listOutLineColor[nColorIdx].szName;
    }

    public void ChageFontAlphaVal(float a_alpha)
    {
        Text text = gameObject.GetComponent<Text>();
        Color curColor = text.color;
        curColor.a = a_alpha;
        m_fAlha = a_alpha;
        text.color = curColor;
    }

    public int GetSizeIdx()
    {
        foreach (var item in TextHelperData.m_listSize)
        {
            if (item.nValue == m_nSizeId)
            {
                return TextHelperData.m_listSize.IndexOf(item);
            }
        }
        return 0;
    }

    public int GetColorIdx()
    {
        foreach (var item in TextHelperData.m_listColor)
        {
            if (item.szName == m_nColorId)
            {
                return TextHelperData.m_listColor.IndexOf(item);
            }
        }
        return 0;
    }

    public int GetOutLineColorIdx()
    {
        foreach (var item in TextHelperData.m_listOutLineColor)
        {
            if (item.szValue == m_nOutlineColorId)
            {
                return TextHelperData.m_listOutLineColor.IndexOf(item);
            }
        }
        return 0;
    }

    public int GetOutLineColorIdx(string value)
    {
        foreach (var item in TextHelperData.m_listOutLineColor)
        {
            if (item.szValue == value)
            {
                return TextHelperData.m_listOutLineColor.IndexOf(item);
            }
        }
        return 0;
    }

    public int GetOutLineSizeIdx()
    {
        foreach (var item in TextHelperData.m_listOutLineSize)
        {
            if (item == (int)m_nOutlineWidthId)
            {
                return TextHelperData.m_listOutLineSize.IndexOf(item);
            }
        }
        return 0;
    }

    public float GetAlpha()
    {
        Text text = gameObject.GetComponent<Text>();
        Color curColor = text.color;
        return m_fAlha;
    }
}
