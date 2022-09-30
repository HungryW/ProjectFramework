using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// 原理分为两步 
/// 1、需要知道的信息是贴图中每一个字符对应的ASCII码与该字符在图集中对应的位置（例如0的ASCII码为48 图片中的位置为x:0;y:0;w:55;h:76）。
/// 2、创建CustomFont，和字体材质。
///  3、设置CusmomFont的属性：名字，字间距，子图片信息，材质等
///  
/// 实际操作步骤
/// 1.设置图片格式，并用sprite editor切割成多个子图
/// 2.查找“//Tag” 修改子图片的index
/// 3.选择图片，名字和输出目录，点击创建就好了
/// </summary>
public class CreateFont : EditorWindow
{
    public static void Open()
    {
        GetWindow<CreateFont>("创建字体");
    }

    private Texture2D tex;
    private string fontName;
    private string fontPath;

    private void OnGUI()
    {
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("字体图片：");
        tex = (Texture2D)EditorGUILayout.ObjectField(tex, typeof(Texture2D), true);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("字体名称：");
        fontName = EditorGUILayout.TextField(fontName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(string.IsNullOrEmpty(fontPath) ? "选择路径" : fontPath))
        {
            fontPath = EditorUtility.OpenFolderPanel("字体路径", Application.dataPath + "/GameMain/Font", "");
            if (string.IsNullOrEmpty(fontPath))
            {
                Debug.Log("取消选择路径");
            }
            else
            {
                fontPath = fontPath.Replace(Application.dataPath, "") + "/";
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("创建"))
        {
            Create();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    private void Create()
    {
        if (tex == null)
        {
            Debug.LogWarning("创建失败，图片为空！");
            return;
        }

        if (string.IsNullOrEmpty(fontPath))
        {
            Debug.LogWarning("字体路径为空！");
            return;
        }
        if (fontName == null)
        {
            Debug.LogWarning("创建失败，字体名称为空！");
            return;
        }

        string selectionPath = AssetDatabase.GetAssetPath(tex);
        string selectionExt = Path.GetExtension(selectionPath);
        if (selectionExt.Length == 0)
        {
            Debug.LogError("创建失败！");
            return;
        }

        string fontPathName = fontPath + fontName + ".fontsettings";
        string matPathName = fontPath + fontName + ".mat";
        float lineSpace = 0.1f;
        Object[] arrSubObj = AssetDatabase.LoadAllAssetRepresentationsAtPath(selectionPath);
        List<Sprite> sprites = new List<Sprite>();
        for (int i = 0; i < arrSubObj.Length; i++)
        {
            sprites.Add(arrSubObj[i] as Sprite);
        }
        if (sprites.Count > 0)
        {
            Material mat = new Material(Shader.Find("GUI/Text Shader"));
            mat.SetTexture("_MainTex", tex);
            Font m_myFont = new Font();
            m_myFont.material = mat;
            CharacterInfo[] characterInfo = new CharacterInfo[sprites.Count];
            //Tag 修改子图片的index
            int[] arrIdx = {49, 50, 51, 52, 53, 54, 55, 56, 57, 48};
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].rect.height > lineSpace)
                {
                    lineSpace = sprites[i].rect.height;
                }
            }
            for (int i = 0; i < sprites.Count; i++)
            {
                Sprite spr = sprites[i];
                CharacterInfo info = new CharacterInfo();
                if (i >= arrIdx.Length)
                {
                    Debug.LogError(string.Format("创建失败，Sprite名称错误！name = {0}", i));
                    return;
                }
                info.index = arrIdx[i];
                Rect rect = spr.rect;
                float pivot = spr.pivot.y / rect.height - 0.5f;
                if (pivot > 0)
                {
                    pivot = -lineSpace / 2 - spr.pivot.y;
                }
                else if (pivot < 0)
                {
                    pivot = -lineSpace / 2 + rect.height - spr.pivot.y;
                }
                else
                {
                    pivot = -lineSpace / 2;
                }
                int offsetY = (int)(pivot + (lineSpace - rect.height) / 2);
                info.uvBottomLeft = new Vector2((float)rect.x / tex.width, (float)(rect.y) / tex.height);
                info.uvBottomRight = new Vector2((float)(rect.x + rect.width) / tex.width, (float)(rect.y) / tex.height);
                info.uvTopLeft = new Vector2((float)rect.x / tex.width, (float)(rect.y + rect.height) / tex.height);
                info.uvTopRight = new Vector2((float)(rect.x + rect.width) / tex.width, (float)(rect.y + rect.height) / tex.height);
                info.minX = 0;
                info.minY = -(int)rect.height - offsetY;
                info.maxX = (int)rect.width;
                info.maxY = -offsetY;
                info.advance = (int)rect.width;
                characterInfo[i] = info;
            }
            AssetDatabase.CreateAsset(mat, "Assets" + matPathName);
            AssetDatabase.CreateAsset(m_myFont, "Assets" + fontPathName);
            m_myFont.characterInfo = characterInfo;
            SerializedObject so = new SerializedObject(m_myFont);
            so.FindProperty("m_LineSpacing").floatValue = tex.height;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(m_myFont);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();//刷新资源
            Debug.Log("创建字体成功");

        }
        else
        {
            Debug.LogError("图集错误！");
        }
    }

}
