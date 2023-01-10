using UnityEngine;
using UnityEditor;
using GameFrameworkPackage;

namespace EditorTools
{
    [ExecuteInEditMode]
    public static class RecyclableScrollViewEditorTool
    {
        const string PrefabPath = "Assets/GameMain/_Logic/Scripts/Common/UI/RecycleScl/Prefab/Recyclable Scroll View.prefab";

        [MenuItem("GameObject/UI/Recyclable Scroll View")]
        private static void CreateRecyclableScrollView()
        {
            GameObject selected = Selection.activeGameObject;

            //If selected isn't a UI gameobject then find a Canvas
            if (!selected || !(selected.transform is RectTransform))
            {
                selected = GameObject.FindObjectOfType<Canvas>().gameObject;
            }

            if (!selected) return;

            GameObject asset = AssetDatabase.LoadAssetAtPath(PrefabPath, typeof(GameObject)) as GameObject;

            GameObject item = Object.Instantiate(asset);
            item.name = "Recyclable Scroll View";

            item.transform.SetParent(selected.transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            Selection.activeGameObject = item;
            Undo.RegisterCreatedObjectUndo(item, "Create Recycalable Scroll view");
        }
    }
}
