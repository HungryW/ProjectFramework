using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;
using GameFramework;
using GameFrameworkPackage;

[CustomEditor(typeof(CTextPrefabTool))]
[ExecuteInEditMode]
[CanEditMultipleObjects]
public class TextHelper : Editor
{
    private string[] arrFont = new string[] { "Main", "Light" };
    private int nSizeIdx = 0;
    private int nColorIdx = 0;
    private int nFontIdx = 0;
    private bool bUseOutline = false;
    private bool bUseShadow = false;
    private float fFontAlpha = 1f;
    private int nOutlineColorIdx = 4;
    private int nOutlineWidthIdx = 0;

    public override void OnPreviewSettings()
    {
        base.OnPreviewSettings();
        CTextPrefabTool targetObj = (CTextPrefabTool)target;
        targetObj.ChangeSize(nSizeIdx);
        targetObj.ChangeColor(nColorIdx, TextHelperData.GetColorOptions()[nColorIdx]);
    }

    public void OnEnable()
    {
        TextHelperData.InitData();
    }

    void OnValidate()
    {
        //CTextPrefabTool targetObj = (CTextPrefabTool)target;

        //targetObj.ChangeSize(nSizeIdx);
        //targetObj.ChangeColor(nColorIdx, TextHelperData.GetColorOptions()[nColorIdx]);
        //if (bUseOutline)
        //{
        //    targetObj.ChangeOutlineColorIdx(nOutlineColorIdx);
        //    targetObj.ChangeOutlineWidthIdx(nOutlineWidthIdx);
        //} 
    }

    public override void OnInspectorGUI()
    {
        CTextPrefabTool targetObj = (CTextPrefabTool)target;
        
        nSizeIdx = EditorGUILayout.Popup("大小",targetObj.GetSizeIdx(), TextHelperData.GetSizeOptions());
        targetObj.ChangeSize(nSizeIdx);

        nColorIdx = EditorGUILayout.Popup("颜色",targetObj.GetColorIdx(), TextHelperData.GetColorOptions());
        targetObj.ChangeColor(nColorIdx,TextHelperData.GetColorOptions()[nColorIdx]);

        nFontIdx = EditorGUILayout.Popup("字体",targetObj.m_nFontIdx, arrFont);
        targetObj.ChangeFont(nFontIdx);

        fFontAlpha = EditorGUILayout.Slider("透明度",targetObj.GetAlpha(),0f,1f);
        targetObj.ChageFontAlphaVal(fFontAlpha);

        bUseOutline = EditorGUILayout.Toggle("描边", targetObj.m_bUseOutline);
        targetObj.ChangeUseOutLine(bUseOutline);
        if (bUseOutline)
        {
            nOutlineColorIdx = EditorGUILayout.Popup("描边颜色",targetObj.GetOutLineColorIdx(), TextHelperData.GetOutLineColorOptions());
            targetObj.ChangeOutlineColorIdx(nOutlineColorIdx);

            nOutlineWidthIdx = EditorGUILayout.Popup("描边宽度",targetObj.GetOutLineSizeIdx(), TextHelperData.GetOutLineSizeOptions());
            targetObj.ChangeOutlineWidthIdx(nOutlineWidthIdx);
        }
        bUseShadow = EditorGUILayout.Toggle("阴影", targetObj.m_bUseShadow);
        targetObj.ChangeUseShadow(bUseShadow);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    [MenuItem("GameObject/UI/MyText")]
    public static void CreateTextPlus()
    {
        GameObject root = new GameObject("Text", typeof(RectTransform), typeof(Text));
        root.transform.SetParent(Selection.activeTransform, false);
        (root.transform as RectTransform).sizeDelta = new Vector2(200, 45);
        Text text = root.GetComponent<Text>();
        text.color = Color.white;
        text.raycastTarget = false;
        text.resizeTextForBestFit = false;
        text.supportRichText = false;
        text.text = "0";
        text.alignment = TextAnchor.MiddleCenter;
        text.font = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/hszz.otf");
        text.fontSize = 20;

        root.AddComponent(typeof(CTextPrefabTool));
        Selection.activeGameObject = root;
    }
}