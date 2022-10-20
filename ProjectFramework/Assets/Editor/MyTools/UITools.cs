using GameFramework;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using GameFrameworkPackage;
using UI;

namespace UnityGameFramework.Editor
{
    public static class UITools
    {
        [MenuItem("Assets/GF/UITools/字体/ReplaceFonts", false, 1)]
        public static void ReplaceFonts()
        {
            Font fontoldHeiti = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/heiti.otf");
            Font fontoldchina = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/fz_black_simpleCN.TTF");
            Font fontoldhszz = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/hszz.OTF");
            Font fontoldhszz_light = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/hszz_light.otf");

            Font fontChinaLight = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/light.ttf");
            Font fontChinaMain = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/main.ttf");

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/GameMain/UIToolsPrefab" });
            if (guids != null && guids.Length > 0)
            {
                EditorUtility.DisplayProgressBar("Replacing...", "Start replace", 0);
                int progress = 0;
                foreach (string guid in guids)
                {
                    progress++;
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    EditorUtility.DisplayProgressBar("Replacing....", path, ((float)progress / guids.Length));
                    GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

                    bool isChanged = false;
                    Text[] lblist = obj.GetComponentsInChildren<Text>(true);
                    foreach (Text item in lblist)
                    {
                        if (item.font == fontoldHeiti
                            || item.font == fontoldchina
                            || item.font == fontoldhszz)
                        {
                            item.font = fontChinaMain;
                        }
                        else if (item.font == fontoldhszz_light)
                        {
                            item.font = fontChinaLight;
                        }
                        item.font = fontChinaMain;
                        isChanged = true;
                    }
                    if (isChanged)
                    {
                        EditorUtility.SetDirty(obj);
                    }
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.SaveAssets();

                }
            }
        }

        [MenuItem("Assets/GF/UITools/字体/文本框适应字体大小", false, 1)]
        public static void FitFontsTextSize()
        {
            Font fontChinaLight = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/light.ttf");
            Font fontChinaMain = AssetDatabase.LoadAssetAtPath<Font>("Assets/GameMain/Font/main.ttf");

            GameObject[] arrObj = Selection.gameObjects;
            string selectionPath = AssetDatabase.GetAssetPath(arrObj[0]);
            bool bIsDir = Directory.Exists(selectionPath);
            string[] guids = null;
            if (bIsDir)
            {
                guids = AssetDatabase.FindAssets("t:Prefab", new string[] { selectionPath });
            }
            else
            {
                List<string> listGuid = new List<string>();
                for (int i = 0; i < arrObj.Length; i++)
                {
                    selectionPath = AssetDatabase.GetAssetPath(arrObj[i]);
                    listGuid.Add(AssetDatabase.AssetPathToGUID(selectionPath));
                }
                guids = listGuid.ToArray();
            }
            if (guids != null && guids.Length > 0)
            {
                EditorUtility.DisplayProgressBar("Replacing...", "Start replace", 0);
                int progress = 0;
                foreach (string guid in guids)
                {
                    progress++;
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    EditorUtility.DisplayProgressBar("Replacing....", path, ((float)progress / guids.Length));
                    GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

                    Text[] lblist = obj.GetComponentsInChildren<Text>(true);
                    foreach (Text item in lblist)
                    {
                        TextGenerator textGenerator = item.cachedTextGeneratorForLayout;
                        TextGenerationSettings textGenerationSettings = item.GetGenerationSettings(Vector2.zero);
                        float fNeedH = textGenerator.GetPreferredHeight(9.ToString(), textGenerationSettings);
                        float fTranH = item.rectTransform.sizeDelta.y;
                        if (fTranH < fNeedH
                            && item.rectTransform.anchorMin == item.rectTransform.anchorMax
                            && (item.alignment == TextAnchor.MiddleCenter
                                    || item.alignment == TextAnchor.MiddleLeft
                                    || item.alignment == TextAnchor.MiddleRight))
                        {
                            item.rectTransform.SetSizeHeight(fNeedH);
                            Debug.Log(Utility.Text.Format("重新设置{0}中的{1}高度,由{2}变为{3}", path, item.name, fTranH, fNeedH));
                            EditorUtility.SetDirty(obj);
                        }
                    }
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.SaveAssets();
                }
            }
        }

        [MenuItem("Assets/GF/UITools/TextBestFitSize", false, 10)]
        public static void TextBestFitSize()
        {
            string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/GameMain/UI" });
            if (guids != null && guids.Length > 0)
            {
                EditorUtility.DisplayProgressBar("Set BestFitSize...", "Start Set", 0);
                int progress = 0;
                foreach (string guid in guids)
                {
                    progress++;
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    EditorUtility.DisplayProgressBar("Set....", path, ((float)progress / guids.Length));
                    GameObject obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;

                    bool isChanged = false;
                    Text[] lblist = obj.GetComponentsInChildren<Text>(true);
                    foreach (Text item in lblist)
                    {
                        item.resizeTextForBestFit = true;
                        item.resizeTextMinSize = 14;
                        item.resizeTextMaxSize = item.fontSize;
                        isChanged = true;
                    }
                    if (isChanged)
                    {
                        EditorUtility.SetDirty(obj);
                    }
                    EditorUtility.ClearProgressBar();
                    AssetDatabase.SaveAssets();

                }
            }
        }
        [MenuItem("Assets/GF/UITools/AddCloseBtnSound")]
        public static void AddCloseBtnSound(MenuCommand menuCommand)
        {
            GameObject[] arrObj = Selection.gameObjects;
            List<GameObject> listObj = new List<GameObject>();
            for (int i = 0; i < arrObj.Length; i++)
            {
                listObj.Add(arrObj[i]);
            }

            for (int i = 0; i < listObj.Count; i++)
            {
                CLogicUI logicUI = listObj[i].GetComponent<CLogicUI>();
                if (logicUI == null)
                {
                    continue;
                }
                Button[] listBtn = listObj[i].GetComponentsInChildren<Button>(true);
                for (int j = 0; j < listBtn.Length; j++)
                {
                    string szName = listBtn[j].name.ToLower();
                    if (szName.Contains("btnclose") || szName.Contains("btnexit"))
                    {
                        listBtn[j].gameObject.GetOrAddComponent<CUIToolButtonSound>().clickSound = EBtnSound.BtnClick;
                    }
                    else
                    {
                        listBtn[j].gameObject.GetOrAddComponent<CUIToolButtonSound>().clickSound = EBtnSound.BtnClick;
                    }
                }
                EditorUtility.SetDirty(listObj[i]);
                AssetDatabase.SaveAssets();
            }
        }

        [MenuItem("Assets/GF/UITools/重命名所有无逻辑功能的文本为txt_开头")]
        static void TxtToolReplaceLogicNessTxtName(MenuCommand menuCommand)
        {
            StringBuilder path = new StringBuilder();

            GameObject[] arrObj = Selection.gameObjects;
            for (int i = 0; i < arrObj.Length; i++)
            {
                Text[] arrTxt = arrObj[i].GetComponentsInChildren<Text>(true);
                for (int j = 0; j < arrTxt.Length; j++)
                {
                    Text txt = arrTxt[j];
                    string szName = txt.name.ToLower();
                    if (szName.Contains("txt_"))
                    {
                        string szKey = txt.text;
                        txt.name = Utility.Text.Format("txt_{0}", szKey);
                        txt.text = szKey;
                        string szPath = _GetUITranformPath(txt.transform);
                        path.AppendFormat("路径={0}  szName={1} to Name = {2}", szPath, szName, txt.name);
                        path.AppendLine();
                    }
                }
                EditorUtility.SetDirty(arrObj[i]);
            }
            GUIUtility.systemCopyBuffer = path.ToString();
            Debug.Log(path);
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/GF/UITools/查找所有无逻辑功能的文本")]
        static void CopyFromUIMainPath(MenuCommand menuCommand)
        {
            StringBuilder path = new StringBuilder();

            GameObject[] arrObj = Selection.gameObjects;
            for (int i = 0; i < arrObj.Length; i++)
            {
                Text[] arrTxt = arrObj[i].GetComponentsInChildren<Text>(true);
                for (int j = 0; j < arrTxt.Length; j++)
                {
                    Text txt = arrTxt[j];
                    string szName = txt.name.ToLower();
                    if (!szName.Contains("lb"))
                    {
                        string szKey = System.Text.RegularExpressions.Regex.Replace(txt.text, "[^a-zA-Z0-9]", "");
                        string szPath = _GetUITranformPath(txt.transform);
                        path.AppendFormat("路径={0}       key={1}      val={2}", szPath, szKey, txt.text);
                        path.AppendLine();
                    }
                }
            }
            GUIUtility.systemCopyBuffer = path.ToString();
            Debug.Log(path);
        }

        private static string _GetUITranformPath(Transform a_tran)
        {
            StringBuilder path = new StringBuilder();
            path.Append(a_tran.name);
            while (true)
            {
                a_tran = a_tran.parent;
                if (a_tran == null) break;
                path.Insert(0, a_tran.gameObject.name + '/');
                if (a_tran.GetComponent<CanvasScaler>() != null) break;
            }
            return path.ToString();
        }

        [MenuItem("GameObject/UI/设置粒子层级")]
        public static void SetParDeath(MenuCommand menuCommand)
        {
            const int nAddOrder = 40;
            GameObject[] arrObj = Selection.gameObjects;
            List<ParticleSystem> listPar = new List<ParticleSystem>();
            for (int i = 0; i < arrObj.Length; i++)
            {
                listPar.AddRange(arrObj[i].GetComponentsInChildren<ParticleSystem>());
            }

            for (int i = 0; i < arrObj.Length; i++)
            {
                arrObj[i].GetComponent<Transform>().localScale = Vector3.one * 100;
                arrObj[i].GetComponent<Transform>().localPosition = Vector3.zero;
            }

            for (int i = 0; i < listPar.Count; i++)
            {
                int nCurOrder = listPar[i].GetComponent<Renderer>().sortingOrder;
                if (nCurOrder < nAddOrder)
                {
                    listPar[i].GetComponent<Renderer>().sortingOrder = nCurOrder + nAddOrder;
                }
            }

            for (int i = 0; i < arrObj.Length; i++)
            {
                EditorUtility.SetDirty(arrObj[i]);
            }

        }
    }

    public static class GameLogicTool
    {

        [MenuItem("GameObject/2D Object/新建Actor挂点")]
        public static void SetParDeath(MenuCommand menuCommand)
        {
            GameObject[] arrObj = Selection.gameObjects;
            for (int i = 0; i < arrObj.Length; i++)
            {
                if (arrObj[i].transform.Find("GoContent/GoHitPos") != null)
                {
                    continue;
                }

                GameObject goHitPos = new GameObject("GoHitPos", new System.Type[] { typeof(RectTransform) });
                goHitPos.transform.parent = arrObj[i].transform.Find("GoContent");
                goHitPos.transform.localPosition = Vector3.zero;
                goHitPos.transform.localScale = Vector3.one;
                goHitPos.transform.SetAsLastSibling();

                GameObject go1 = new GameObject("center", new System.Type[] { typeof(RectTransform) });
                GameObject go2 = new GameObject("up", new System.Type[] { typeof(RectTransform) });
                GameObject go3 = new GameObject("bulletTarget", new System.Type[] { typeof(RectTransform) });

                go1.transform.parent = goHitPos.transform;
                go1.transform.localPosition = Vector3.zero;
                go1.transform.localScale = Vector3.one;

                go2.transform.parent = goHitPos.transform;
                go2.transform.localPosition = Vector3.zero;
                go2.transform.localScale = Vector3.one;

                go3.transform.parent = goHitPos.transform;
                go3.transform.localPosition = Vector3.zero;
                go3.transform.localScale = Vector3.one;
            }

            for (int i = 0; i < arrObj.Length; i++)
            {
                EditorUtility.SetDirty(arrObj[i]);
            }

        }


    }
}



