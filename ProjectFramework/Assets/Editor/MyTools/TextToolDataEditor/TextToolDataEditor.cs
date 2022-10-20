using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using UnityEngine.UI;
using System.Windows;
using System.Runtime.InteropServices;

[Serializable]
public class ColorData
{
    public string name;
    public string color;

    public ColorData()
    {

    }

    public ColorData(string a_name,string a_val)
    {
        name = a_name;
        color = a_val;
    }
}

public class TextToolDataEditor : EditorWindow
{
    private const string m_szDataPath = "Assets/Editor/MyTools/TextToolDataEditor/FontStyle.xml";

    private SerializedObject serObj;

    public List<ColorData> m_listColor = new List<ColorData>();
    public List<ColorData> m_listOutLineColor = new List<ColorData>();
    public List<int> m_listSize = new List<int>();
    public List<int> m_listOutLineSize = new List<int>();

    public ReorderableList m_rListColor;
    public ReorderableList m_rListOutLineColor;
    public ReorderableList m_rSize;
    public ReorderableList m_rListOutLineSize;
    

    public static void Open()
    {
        GetWindow<TextToolDataEditor>("编辑字体数据");
    }

    private void _InitData()
    {
        TextHelperData.InitData();

        m_listColor.Clear();
        m_listOutLineColor.Clear();
        m_listSize.Clear();
        m_listOutLineSize.Clear();

        foreach (var item in TextHelperData.m_listColor)
        {
            m_listColor.Add(new ColorData(item.szName, item.szValue));
        }

        foreach (var item in TextHelperData.m_listSize)
        {
            m_listSize.Add(item.nValue);
        }

        foreach (var item in TextHelperData.m_listOutLineColor)
        {
            m_listOutLineColor.Add(new ColorData(item.szName,item.szValue));
        }

        foreach (var item in TextHelperData.m_listOutLineSize)
        {
            m_listOutLineSize.Add(item);
        }

        serObj.Update();
    }

    public void OnEnable()
    {
        serObj = new SerializedObject(this);
        _InitData();
        
        m_rListColor = new ReorderableList(m_listColor, typeof(ColorData));
        m_rListOutLineColor = new ReorderableList(m_listOutLineColor, typeof(ColorData));
        m_rSize = new ReorderableList(serObj, serObj.FindProperty("m_listSize"), true, true, true, true);
        m_rListOutLineSize = new ReorderableList(serObj, serObj.FindProperty("m_listOutLineSize"), true, true, true, true);
        
        m_rListColor.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, "FontColors");
        };
        m_rListColor.drawElementCallback = _DrawColorElement;

        m_rSize.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, "FontSize");
        };
        m_rSize.drawElementCallback = _DrawSizeElement;

        m_rListOutLineColor.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, "OutLineColor");
        };
        m_rListOutLineColor.drawElementCallback = _DrawOutLineColorElement;

        m_rListOutLineSize.drawHeaderCallback = (Rect rect) =>
        {
            GUI.Label(rect, "OutLineSize");
        };
        m_rListOutLineSize.drawElementCallback = _DrawOutLineSizeElement;
    }
    Vector2 scrollPos;
    private void OnGUI()
    {
        serObj.Update();
        minSize = new Vector2(400, 300);

        GUIStyle titleStyle = new GUIStyle();
        titleStyle.fontStyle = FontStyle.Bold;
        titleStyle.fontSize = 20;
        titleStyle.richText = true;

        GUILayout.Label("<color=#c5c5c5>字体数据添加修改</color>", titleStyle);
        
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width - 10), GUILayout.Height(position.height - 60));
        m_rListColor.DoLayoutList();
        m_rSize.DoLayoutList();
        m_rListOutLineColor.DoLayoutList();
        m_rListOutLineSize.DoLayoutList();
        
        EditorGUILayout.EndScrollView();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("刷新数据", new GUILayoutOption[] { GUILayout.Height(30) }))
        {
            _InitData();
        }
        if (GUILayout.Button("保存并刷新数据", new GUILayoutOption[] { GUILayout.Height(30) }))
        {
            _SaveDataByXml();
            EditorUtility.DisplayDialog("保存数据","保存成功","确定");
            _InitData();
        }
        EditorGUILayout.EndHorizontal();

        serObj.ApplyModifiedProperties();
    }
    
    private void _SaveDataBinary()
    {
        IFormatter formatter = new BinaryFormatter();

        Stream stream = new FileStream("Assets\\GameMain\\Font\\FontStyle.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        foreach (var color in m_listColor)
        {
            formatter.Serialize(stream, color);
        }
        foreach (var color in m_listOutLineColor)
        {
            formatter.Serialize(stream, color);
        }
        foreach (var size in m_listSize)
        {
            formatter.Serialize(stream, size);
        }
        foreach (var size in m_listOutLineSize)
        {
            formatter.Serialize(stream, size);
        }
        stream.Close();
    }

    private void _SaveDataByXml()
    {
        XmlDocument xmlDoc = new XmlDocument();
        XmlElement fontStyle = xmlDoc.CreateElement("fontStyle");
        fontStyle.SetAttribute("name", "fontStyleData");

        XmlElement color = xmlDoc.CreateElement("colorData");
        foreach (var item in m_listColor)
        {
            XmlElement colorData = xmlDoc.CreateElement("fColorData");
            XmlAttribute name = xmlDoc.CreateAttribute("name");
            name.Value = item.name;
            XmlAttribute value = xmlDoc.CreateAttribute("value");
            value.Value = item.color;
            colorData.Attributes.SetNamedItem(name);
            colorData.Attributes.SetNamedItem(value);
            color.AppendChild(colorData);
        }
        XmlElement outLineColor = xmlDoc.CreateElement("outLineColor");
        foreach (var item in m_listOutLineColor)
        {
            XmlElement colorData = xmlDoc.CreateElement("oColorData");
            XmlAttribute name = xmlDoc.CreateAttribute("name");
            name.Value = item.name;
            XmlAttribute value = xmlDoc.CreateAttribute("value");
            value.Value = item.color;
            colorData.Attributes.SetNamedItem(name);
            colorData.Attributes.SetNamedItem(value);
            outLineColor.AppendChild(colorData);
        }
        XmlElement outLineSize = xmlDoc.CreateElement("outLineSize");
        foreach (var item in m_listOutLineSize)
        {
            XmlElement num = xmlDoc.CreateElement("fsize_num");
            num.InnerText = item.ToString();
            outLineSize.AppendChild(num);
        }
        XmlElement size = xmlDoc.CreateElement("sizeData");
        foreach (var item in m_listSize)
        {
            XmlElement num = xmlDoc.CreateElement("osize_num");
            num.InnerText = item.ToString();
            size.AppendChild(num);
        }
        fontStyle.AppendChild(color);
        fontStyle.AppendChild(size);
        fontStyle.AppendChild(outLineColor);
        fontStyle.AppendChild(outLineSize);
        xmlDoc.AppendChild(fontStyle);
        xmlDoc.Save(m_szDataPath);
    }
    
    private void _DrawColorElement(Rect rect, int index, bool selected, bool focused)
    {
        if (m_listColor.Count <= 0)
            return;
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, 20), "name");
        m_listColor[index].name = EditorGUI.TextField(new Rect(rect.x+50, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), m_listColor[index].name);
        EditorGUI.LabelField(new Rect(rect.x+rect.width / 2, rect.y, 50, 20), "value");
        m_listColor[index].color = EditorGUI.TextField(new Rect(rect.x+(rect.width / 2)+50, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), m_listColor[index].color);
    }

    private void _DrawOutLineColorElement(Rect rect, int index, bool selected, bool focused)
    {
        if (m_listOutLineColor.Count <= 0)
            return;
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 50, 20), "name");
        m_listOutLineColor[index].name = EditorGUI.TextField(new Rect(rect.x + 50, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), m_listOutLineColor[index].name);
        EditorGUI.LabelField(new Rect(rect.x + rect.width / 2, rect.y, 50, 20), "value");
        m_listOutLineColor[index].color = EditorGUI.TextField(new Rect(rect.x + (rect.width / 2) + 50, rect.y, rect.width / 3, EditorGUIUtility.singleLineHeight), m_listOutLineColor[index].color);
    }

    private void _DrawSizeElement(Rect rect, int index, bool selected, bool focused)
    {
        if (m_listSize.Count <= 0)
            return;
        SerializedProperty itemData = m_rSize.serializedProperty.GetArrayElementAtIndex(index);

        rect.y += 2;
        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, itemData, new GUIContent("size"));
    }

    private void _DrawOutLineSizeElement(Rect rect, int index, bool selected, bool focused)
    {
        if (m_listOutLineSize.Count <= 0)
            return;
        SerializedProperty itemData = m_rListOutLineSize.serializedProperty.GetArrayElementAtIndex(index);

        rect.y += 2;
        rect.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(rect, itemData, new GUIContent("size"));
    }
}
