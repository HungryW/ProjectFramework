using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using GameFramework;
using GameFrameworkPackage;
using System.Collections.Generic;

namespace AssetDanshari
{
    public class AssetDanshariWindow : EditorWindow
    {
        [MenuItem("MyTools/资源规范管理")]
        static void ShowWindow()
        {
            GetWindow<AssetDanshariWindow>();
        }

        private AssetDanshariSetting m_AssetDanshariSetting;
        private Vector2 m_ScrollViewVector2;
        private ReorderableList m_ReorderableList;
        private bool m_IsForceText;
        private AssetDanshariHandlerDemo m_AssetDanshariHandlerDemo;

        private void Awake()
        {
            titleContent.text = "资源规范管理";
        }

        private void OnGUI()
        {
            Init();
            var style = AssetDanshariStyle.Get();
            minSize = new Vector2(450f, 331f);
            if (!m_IsForceText)
            {
                EditorGUILayout.LabelField(style.forceText);
                return;
            }

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.FlexibleSpace();
            Rect toolBtnRect = GUILayoutUtility.GetRect(style.help, EditorStyles.toolbarDropDown, GUILayout.ExpandWidth(false));
            if (GUI.Button(toolBtnRect, style.help, EditorStyles.toolbarDropDown))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(style.about, false, OnContextAbout);
                menu.DropDown(toolBtnRect);
            }
            EditorGUILayout.EndHorizontal();

            if (m_ReorderableList != null)
            {
                m_ScrollViewVector2 = GUILayout.BeginScrollView(m_ScrollViewVector2);
                m_ReorderableList.DoLayoutList();
                GUILayout.EndScrollView();
            }
        }

        private void Init()
        {
            if (m_AssetDanshariSetting == null)
            {
                m_IsForceText = EditorSettings.serializationMode == SerializationMode.ForceText;
                if (!m_IsForceText)
                {
                    return;
                }

                m_AssetDanshariSetting = AssetDatabase.LoadAssetAtPath<AssetDanshariSetting>(
                    "Assets/Editor/AssetDanshari/AssetDanshariSetting.asset");
                if (m_AssetDanshariSetting == null)
                {
                    string[] guids = AssetDatabase.FindAssets("t:" + typeof(AssetDanshariSetting).Name);
                    if (guids.Length == 0)
                    {
                        Debug.LogError("Missing AssetDanshariSetting File");
                        return;
                    }

                    string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    m_AssetDanshariSetting = AssetDatabase.LoadAssetAtPath<AssetDanshariSetting>(path);
                }
            }

            if (m_ReorderableList == null)
            {
                m_ReorderableList = new ReorderableList(m_AssetDanshariSetting.assetReferenceInfos, null, true, true, true, true);
                m_ReorderableList.drawHeaderCallback = OnDrawHeaderCallback;
                m_ReorderableList.drawElementCallback = OnDrawElementCallback;
                m_ReorderableList.elementHeight += 55;
            }
        }

        private void OnDrawHeaderCallback(Rect rect)
        {
            EditorGUI.LabelField(rect, AssetDanshariStyle.Get().assetReferenceTitle);
        }

        private void OnDrawElementCallback(Rect rect, int index, bool isactive, bool isfocused)
        {
            if (m_AssetDanshariSetting == null || m_AssetDanshariSetting.assetReferenceInfos.Count < index)
            {
                return;
            }

            var style = AssetDanshariStyle.Get();
            var info = m_AssetDanshariSetting.assetReferenceInfos[index];
            rect.height = EditorGUIUtility.singleLineHeight;
            rect.y += 2;

            EditorGUI.BeginChangeCheck();
            info.title = EditorGUI.TextField(rect, info.title);
            rect.y += EditorGUIUtility.singleLineHeight + 2;

            Rect rect2 = new Rect(rect) { width = 50f };
            Rect rect3 = new Rect(rect) { x = rect2.x + rect2.width, width = rect.width - rect2.width - 220f };
            Rect rect4 = new Rect(rect) { x = rect3.x + rect3.width + 55f, width = 70f };
            Rect rect5 = new Rect(rect) { x = rect4.x + rect4.width + 5f, width = 70f };
            Rect rect6 = new Rect(rect) { x = rect3.x + rect3.width + 5f, width = 70f };
            Rect rect7 = new Rect(rect) { x = rect3.x + rect3.width + 5f, width = 20f };
            Rect rect8 = new Rect(rect) { x = rect7.x + rect7.width + 3f, width = 20f };
            EditorGUI.LabelField(rect2, style.assetReferenceReference);
            if (GUI.Button(rect7, style.addPath))
            {
                List<string> listPath = info.referenceFolder.SplitToStringList(';');
                string path = AssetDanshariUtility.GetPathInAsssets(EditorUtility.OpenFolderPanel("添加路径", Application.dataPath, "")) + ";";
                if (!info.referenceFolder.Contains(path))
                    info.referenceFolder = info.referenceFolder.Length == 0 ? path : info.referenceFolder + path;
            }
            if (GUI.Button(rect8, style.subPath))
            {
                List<string> listPath = info.referenceFolder.SplitToStringList(';');
                string path = AssetDanshariUtility.GetPathInAsssets(EditorUtility.OpenFolderPanel("删除路径", Application.dataPath, "")) + ";";
                if (info.referenceFolder.Contains(path))
                {
                    info.referenceFolder = info.referenceFolder.Length == 0 ? path : info.referenceFolder.Remove(info.referenceFolder.IndexOf(path), path.Length);
                }
            }
            info.referenceFolder = EditorGUI.TextField(rect3, info.referenceFolder);
            info.referenceFolder = OnDrawElementAcceptDrop(rect3, info.referenceFolder);
            info.referenceFolder = OnDrawElementAcceptDrop(rect7, info.referenceFolder);
            info.referenceFolder = OnDrawElementAcceptDropDel(rect8, info.referenceFolder);
            bool valueChanged = EditorGUI.EndChangeCheck();
            if (GUI.Button(rect4, style.assetReferenceCheckRef))
            {
                AssetBaseWindow.CheckPaths<AssetReferenceWindow>(info.referenceFolder, info.assetFolder, info.assetCommonFolder);
            }
            rect2.y += EditorGUIUtility.singleLineHeight + 2;
            rect3.y += EditorGUIUtility.singleLineHeight + 2;
            rect4.y += EditorGUIUtility.singleLineHeight + 2;
            rect5.y += EditorGUIUtility.singleLineHeight + 2;
            rect7.y += EditorGUIUtility.singleLineHeight + 2;
            rect8.y += EditorGUIUtility.singleLineHeight + 2;
            EditorGUI.LabelField(rect2, style.assetReferenceAsset);
            EditorGUI.BeginChangeCheck();
            info.assetFolder = EditorGUI.TextField(rect3, info.assetFolder);
            info.assetFolder = OnDrawElementAcceptDrop(rect7, info.assetFolder);
            info.assetFolder = OnDrawElementAcceptDrop(rect3, info.assetFolder);
            info.assetFolder = OnDrawElementAcceptDropDel(rect8, info.assetFolder);
            valueChanged |= EditorGUI.EndChangeCheck();
            if (GUI.Button(rect7, style.addPath))
            {
                List<string> listPath = info.assetFolder.SplitToStringList(';');
                string path = AssetDanshariUtility.GetPathInAsssets(EditorUtility.OpenFolderPanel("添加路径", Application.dataPath, "")) + ";";
                if (!info.assetFolder.Contains(path))
                    info.assetFolder = info.assetFolder.Length == 0 ? path : info.assetFolder + path;
            }
            if (GUI.Button(rect8, style.subPath))
            {
                List<string> listPath = info.assetFolder.SplitToStringList(';');
                string path = AssetDanshariUtility.GetPathInAsssets(EditorUtility.OpenFolderPanel("删除路径", Application.dataPath, "")) + ";";
                if (info.assetFolder.Contains(path))
                {
                    info.assetFolder = info.assetFolder.Length == 0 ? path : info.assetFolder.Remove(info.assetFolder.IndexOf(path), path.Length);
                }
            }
            if (GUI.Button(rect4, style.assetReferenceCheckDup))
            {
                AssetBaseWindow.CheckPaths<AssetDuplicateWindow>(info.referenceFolder, info.assetFolder, info.assetCommonFolder);
            }
            if (GUI.Button(rect5, style.assetReferenceDepend))
            {
                AssetBaseWindow.CheckPaths<AssetDependenciesWindow>(info.referenceFolder, info.assetFolder, info.assetCommonFolder);
            }
            rect2.width += 25f;
            rect2.y += EditorGUIUtility.singleLineHeight + 2;
            rect3.y += EditorGUIUtility.singleLineHeight + 2;
            rect3.x += 25f;
            rect3.width = rect.width - rect2.width;
            EditorGUI.LabelField(rect2, style.assetReferenceAssetCommon);
            EditorGUI.BeginChangeCheck();
            info.assetCommonFolder = EditorGUI.TextField(rect3, info.assetCommonFolder);
            info.assetCommonFolder = OnDrawElementAcceptDrop(rect3, info.assetCommonFolder, true);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.EndVertical();
            valueChanged |= EditorGUI.EndChangeCheck();

            if (valueChanged)
            {
                EditorUtility.SetDirty(m_AssetDanshariSetting);
                AssetDatabase.SaveAssets();
                Repaint();
            }
        }

        private string OnDrawElementAcceptDrop(Rect rect, string label, bool onlyOne = false)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0 && !string.IsNullOrEmpty(DragAndDrop.paths[0]))
                {
                    if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        GUI.changed = true;

                        string result = label;
                        string path = AssetDanshariUtility.PathArrayToStr(DragAndDrop.paths);
                        if (string.IsNullOrEmpty(label))
                        {
                            label = "";
                        }
                        if (!label.Contains(path))
                        {
                            result = onlyOne ? path
                                    : label + AssetDanshariUtility.PathArrayToListUseStr(DragAndDrop.paths);
                        }
                        return result;
                    }
                }
            }

            return label;
        }

        private string OnDrawElementAcceptDropDel(Rect rect, string label)
        {
            if (rect.Contains(Event.current.mousePosition))
            {
                if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0 && !string.IsNullOrEmpty(DragAndDrop.paths[0]))
                {
                    if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    }

                    if (Event.current.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        GUI.changed = true;

                        string path = AssetDanshariUtility.PathArrayToListUseStr(DragAndDrop.paths);
                        if (label.Contains(path))
                        {
                            label = label.Length == 0 ? path : label.Remove(label.IndexOf(path), path.Length);
                        }
                        return label;
                    }
                }
            }

            return label;
        }

        private void OnContextAbout()
        {
            Application.OpenURL("https://github.com/akof1314/UnityAssetDanshari");
        }
    }
}