using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using System.Linq;
using UnityEngine.Profiling;

namespace UnityGameFramework.Editor
{
    public static class SpriteCreator
    {
        [MenuItem("GameObject/UI/统计选中GameObj数量", false, 100000)]
        public static void StatisticsSelectNum()
        {
            Debug.Log("选中GameObj数量" + Selection.objects.Length.ToString());
        }

        [MenuItem("Assets/GF/Sprite/AtlasMaker")]
        public static void MakeAtlas()
        {
            foreach (var obj in Selection.objects)
            {
                GameObject goParent = new GameObject(obj.name);
                DirectoryInfo rootDirInfo = new DirectoryInfo(AssetDatabase.GetAssetPath(obj));
                List<GameObject> listGo = new List<GameObject>();
                foreach (FileInfo pngFile in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    string allPath = pngFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (sprite != null)
                    {
                        GameObject go = new GameObject(sprite.name);
                        go.AddComponent<SpriteRenderer>().sprite = sprite;
                        listGo.Add(go);
                        go.transform.parent = goParent.transform;
                    }
                }
                string spriteParent = Application.dataPath + "/GameMain/UISprite/" + obj.name + ".prefab";
                string prefabPath = spriteParent.Substring(spriteParent.IndexOf("Assets"));
                PrefabUtility.SaveAsPrefabAsset(goParent, prefabPath);
                GameObject.DestroyImmediate(goParent);
                for (int i = listGo.Count - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(listGo[i]);
                }
            }
        }


        private static void _MakeAtlas(GameObject a_goParent, List<GameObject> a_listAllGo, string a_szPath, string a_szName)
        {
            GameObject go = new GameObject(a_szName);
            DirectoryInfo rootDirInfo = new DirectoryInfo(a_szPath);
            go.transform.parent = a_goParent.transform;
            DirectoryInfo[] arrDr = rootDirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);
            if (arrDr.Length == 0)
            {
                foreach (FileInfo pngFile in rootDirInfo.GetFiles("*", SearchOption.AllDirectories))
                {
                    string allPath = pngFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (sprite != null)
                    {
                        GameObject goSprite = new GameObject(sprite.name);
                        goSprite.AddComponent<SpriteRenderer>().sprite = sprite;
                        a_listAllGo.Add(goSprite);
                        goSprite.transform.parent = go.transform;
                    }
                }
            }
            else
            {
                foreach (DirectoryInfo dr in arrDr)
                {
                    _MakeAtlas(go, a_listAllGo, dr.FullName, dr.Name);
                }
            }

        }
        [MenuItem("Assets/GF/Sprite/AtlasMaker递归")]
        public static void MakeAtlasStudentDisplay()
        {
            foreach (var obj in Selection.objects)
            {
                GameObject goParent = new GameObject(obj.name);
                List<GameObject> listGo = new List<GameObject>();
                _MakeAtlas(goParent, listGo, AssetDatabase.GetAssetPath(obj), obj.name);
                string spriteParent = Application.dataPath + "/Resources/UISprite/" + obj.name + ".prefab";
                string prefabPath = spriteParent.Substring(spriteParent.IndexOf("Assets"));
                PrefabUtility.SaveAsPrefabAsset(goParent.transform.GetChild(0).gameObject, prefabPath);
                GameObject.DestroyImmediate(goParent);
                for (int i = listGo.Count - 1; i >= 0; i--)
                {
                    GameObject.DestroyImmediate(listGo[i]);
                }
            }
        }

        [MenuItem("GameObject/UI/CopySelectName")]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            StringBuilder code = new StringBuilder();
            GameObject[] arrObj = Selection.gameObjects;
            List<GameObject> listObj = new List<GameObject>();
            for (int i = 0; i < arrObj.Length; i++)
            {
                listObj.Add(arrObj[i]);
            }

            listObj.Sort((obj1, obj2) => { return obj1.transform.GetSiblingIndex().CompareTo(obj2.transform.GetSiblingIndex()); });

            for (int i = 0; i < listObj.Count; i++)
            {
                if (listObj[i].name.Contains("Btn"))
                {
                    code.AppendLine("[SerializeField]");
                    code.AppendLine(string.Format("private Button {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Lb"))
                {
                    code.AppendLine("[SerializeField]");
                    code.AppendLine(string.Format("private Text {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Img"))
                {
                    code.AppendLine("[SerializeField]");
                    code.AppendLine(string.Format("private Image {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Prefab"))
                {
                    code.AppendLine("[SerializeField]");
                    code.AppendLine(string.Format("private GameObject {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Go"))
                {
                    code.AppendLine("[SerializeField]");
                    code.AppendLine(string.Format("private GameObject {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Content"))
                {
                    code.AppendLine("[SerializeField]");
                    code.AppendLine(string.Format("private Transform {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Scl"))
                {
                    code.AppendLine("[SerializeField]");
                    code.AppendLine(string.Format("private ScrollRect {0};", listObj[i].name));
                }
            }
            GUIUtility.systemCopyBuffer = code.ToString();
        }

        [MenuItem("GameObject/UI/获得子UI组件到代码")]
        public static void AllCustomUIChildren(MenuCommand menuCommand)
        {
            if (Selection.gameObjects.Length != 1) return;
            StringBuilder code = new StringBuilder();
            GameObject Origin = Selection.gameObjects[0];
            List<GameObject> listObj = new List<GameObject>();
            _GetGameObjsInCurLevelLogic(Origin, listObj);
            for (int i = 0; i < listObj.Count; i++)
            {
                if (listObj[i].name.Contains("Btn"))
                {
                    code.AppendLine(string.Format("private Button {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Lb"))
                {
                    code.AppendLine(string.Format("private Text {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Img"))
                {
                    code.AppendLine(string.Format("private Image {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Prefab"))
                {
                    code.AppendLine(string.Format("private GameObject {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Go"))
                {
                    code.AppendLine(string.Format("private GameObject {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Content"))
                {
                    code.AppendLine(string.Format("private CUIFormItemContent {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Scl"))
                {
                    code.AppendLine(string.Format("private ScrollRect {0};", listObj[i].name));
                }
                else if (listObj[i].name.Contains("Input"))
                {
                    code.AppendLine(string.Format("private InputField {0};", listObj[i].name));
                }
            }
            code.AppendLine("protected override void _InitComponents()");
            code.AppendLine("{");
            string prePath = string.Empty;
            for (int i = 0; i < Origin.transform.childCount; i++)
            {
                _SelfThenNextChildLayer(Origin.transform.GetChild(i).gameObject, prePath, code);
            }
            code.AppendLine("}");
            GUIUtility.systemCopyBuffer = code.ToString();
        }

        [MenuItem("GameObject/UI/获得当前UI中物体路径")]
        public static void GetPathInUI(MenuCommand menuCommand)
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj == null)
            {
                Debug.LogError("Select OBJ is Null!");
                return;
            }
            string result = AssetDatabase.GetAssetPath(obj);
            Transform selectChild = Selection.activeTransform;
            if (selectChild != null)
            {
                result = selectChild.name;
                while (null == selectChild.parent.GetComponent<UIComponent>())
                {
                    selectChild = selectChild.parent;
                    result = string.Format("{0}/{1}", selectChild.name, result);
                }
            }
            GUIUtility.systemCopyBuffer = result;
        }

        [MenuItem("GameObject/UI/获得当前建筑中物体路径")]
        public static void GetPathInBuild(MenuCommand menuCommand)
        {
            UnityEngine.Object obj = Selection.activeObject;
            if (obj == null)
            {
                Debug.LogError("Select OBJ is Null!");
                return;
            }
            string result = AssetDatabase.GetAssetPath(obj);
            Transform selectChild = Selection.activeTransform;
            if (selectChild != null)
            {
                result = selectChild.name;
                while (null == selectChild.parent.GetComponent<CanvasScaler>())
                {
                    selectChild = selectChild.parent;
                    result = string.Format("{0}/{1}", selectChild.name, result);
                }
            }
            GUIUtility.systemCopyBuffer = result;
        }

        private static bool _CheckHaveSelfLogic(Transform a_tran)
        {
            return false;
        }

        private static void _GetGameObjsInCurLevelLogic(GameObject a_root, List<GameObject> a_listObj)
        {
            for (int i = 0; i < a_root.transform.childCount; i++)
            {
                Transform child = a_root.transform.GetChild(i);
                a_listObj.Add(child.gameObject);
                if (!_CheckHaveSelfLogic(child))
                {
                    _GetGameObjsInCurLevelLogic(child.gameObject, a_listObj);
                }
            }

        }

        [MenuItem("GameObject/UI/CopyFromUIMainPath")]
        static void CopyFromUIMainPath(MenuCommand menuCommand)
        {
            if (Selection.gameObjects.Length != 1) return;
            Transform transform = Selection.gameObjects[0].transform;
            StringBuilder path = new StringBuilder();
            path.Append(Selection.gameObjects[0].name + '"');
            while (transform.GetComponent<Canvas>() == null)
            {
                transform = transform.parent;
                if (transform == null) return;
                if (transform.GetComponent<Canvas>() == null) path.Insert(0, transform.gameObject.name + '/');
            }
            path.Insert(0, '"');
            GUIUtility.systemCopyBuffer = path.ToString();
        }
        private static void _SelfThenNextChildLayer(GameObject gameObject, string prePath, StringBuilder code)
        {
            _CheckLine(gameObject, prePath, code); //Debug.LogError(transform.Find("GoContent/Prefabs"));
            if (gameObject.transform.childCount > 0 && !_CheckHaveSelfLogic(gameObject.transform))
            {
                prePath += gameObject.name + '/';
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    _SelfThenNextChildLayer(gameObject.transform.GetChild(i).gameObject, prePath, code);
                }
                prePath.Replace(gameObject.name + '/', "");
            }
        }
        private static void _CheckLine(GameObject gameObject, string prePath, StringBuilder code)
        {
            string func = "transform.Find";
            if (gameObject.name.Contains("Btn"))
            {
                code.AppendLine(string.Format("{1} = {2}(\"{0}{1}\").GetComponent<Button>();", prePath, gameObject.name, func));
            }
            else if (gameObject.name.Contains("Lb"))
            {
                code.AppendLine(string.Format("{1} = {2}(\"{0}{1}\").GetComponent<Text>();", prePath, gameObject.name, func));
            }
            else if (gameObject.name.Contains("Img"))
            {
                code.AppendLine(string.Format("{1} = {2}(\"{0}{1}\").GetComponent<Image>();", prePath, gameObject.name, func));
            }
            else if (gameObject.name.Contains("Prefab"))
            {
                code.AppendLine(string.Format("{1} = {2}(\"{0}{1}\").gameObject;", prePath, gameObject.name, func));
            }
            else if (gameObject.name.Contains("Go"))
            {
                code.AppendLine(string.Format("{1} = {2}(\"{0}{1}\").gameObject;", prePath, gameObject.name, func));
            }
            else if (gameObject.name.Contains("Content"))
            {
                code.AppendLine(string.Format("{0} = CreateChildItemCtrl<CUIFormItemContent>(this,\"{0}\");", gameObject.name));
            }
            else if (gameObject.name.Contains("Scl"))
            {
                code.AppendLine(string.Format("{1} = {2}(\"{0}{1}\").GetComponent<ScrollRect>();", prePath, gameObject.name, func));
            }
            else if (gameObject.name.Contains("Input"))
            {
                code.AppendLine(string.Format("{1} = {2}(\"{0}{1}\").GetComponent<InputField>();", prePath, gameObject.name, func));
            }
        }
        [MenuItem("Assets/GF/Sprite/Slice to Sprites")]
        static void ProcessToSprite()
        {
            Texture2D image = Selection.activeObject as Texture2D;//获取旋转的对象
            string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(image));//获取路径名称
            string path = rootPath + "/" + image.name + ".PNG";//图片路径名称
            int width = 150;
            int high = 150;

            TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter;

            AssetDatabase.CreateFolder(rootPath, image.name);//创建文件夹

            foreach (SpriteMetaData metaData in texImp.spritesheet)//遍历小图集
            {
                Texture2D myimage = new Texture2D(width, high);
                Color colorBlank = new Color(1, 1, 1, 0);
                for (int y = 0; y < high; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        myimage.SetPixel(x, y, colorBlank);
                    }
                }
                for (int y = (int)metaData.rect.y; y < metaData.rect.y + metaData.rect.height; y++)//Y轴像素
                {
                    for (int x = (int)metaData.rect.x; x < metaData.rect.x + metaData.rect.width; x++)
                        myimage.SetPixel(x - (int)metaData.rect.x + (int)(width - metaData.rect.width) / 2
                                        , y - (int)metaData.rect.y + (int)(high - metaData.rect.height) / 2
                                        , image.GetPixel(x, y));
                }

                //转换纹理到EncodeToPNG兼容格式
                if (myimage.format != TextureFormat.ARGB32 && myimage.format != TextureFormat.RGB24)
                {
                    Texture2D newTexture = new Texture2D(myimage.width, myimage.height);
                    newTexture.SetPixels(myimage.GetPixels(0), 0);
                    myimage = newTexture;
                }
                var pngData = myimage.EncodeToPNG();

                File.WriteAllBytes(rootPath + "/" + image.name + "/" + metaData.name + ".PNG", pngData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [MenuItem("Assets/GF/Sprite/生成主场景用的prefab")]
        public static void MakeMainScenePrefab()
        {
            foreach (var obj in Selection.objects)
            {
                DirectoryInfo rootDirInfo = new DirectoryInfo(AssetDatabase.GetAssetPath(obj));
                List<GameObject> listGo = new List<GameObject>();
                foreach (FileInfo pngFile in rootDirInfo.GetFiles("*.png", SearchOption.AllDirectories))
                {
                    string allPath = pngFile.FullName;
                    string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    GameObject go = new GameObject(sprite.name);
                    SpriteRenderer render = go.AddComponent<SpriteRenderer>();
                    render.sprite = sprite;
                    render.spriteSortPoint = SpriteSortPoint.Pivot;
                    string spriteParent = Application.dataPath + "/GameMain/SpinePrefab/ScenePrefab/MainScene/static/" + sprite.name + ".prefab";
                    string prefabPath = spriteParent.Substring(spriteParent.IndexOf("Assets"));
                    PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
                    GameObject.DestroyImmediate(go);
                }
            }
        }

        [MenuItem("Assets/GF/Sprite/文件夹内图片大小检查,运行内存>512")]
        static void CheckGetRuntimeMemorySizeBy2048()
        {
            CheckGetRuntimeMemorySize(512, Selection.objects[0]);
        }

        static void CheckGetRuntimeMemorySize(float size, Object a_object)
        {
            Debug.Log(a_object.name);
            DirectoryInfo rootDirInfo = new DirectoryInfo(AssetDatabase.GetAssetPath(a_object));
            List<FileInfo> listFile = rootDirInfo.GetFiles("*", SearchOption.AllDirectories).ToList();
            List<Texture> listBigTexture = new List<Texture>();
            foreach (FileInfo file in listFile)
            {
                string allPath = file.FullName;
                string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                Texture target = AssetDatabase.LoadAssetAtPath<Texture>(assetPath);
                if (target == null)
                {
                    continue;
                }
                float runtimeMemorySize = Profiler.GetRuntimeMemorySizeLong(target) / 1024f;
                if (runtimeMemorySize > size)
                {
                    listBigTexture.Add(target);
                }
            }
            listBigTexture.Sort((a, b) =>
            {
                return Profiler.GetRuntimeMemorySizeLong(b).CompareTo(Profiler.GetRuntimeMemorySizeLong(a));
            });

            foreach (var target in listBigTexture)
            {
                Debug.LogWarning(string.Format("NAME:{0}，{1} X {2},  运行占用内存: {3} KB", AssetDatabase.GetAssetPath(target), target.width, target.height, Profiler.GetRuntimeMemorySizeLong(target) / 1024f));
            }
        }



    }
}
