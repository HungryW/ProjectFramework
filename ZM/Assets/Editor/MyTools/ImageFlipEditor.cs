using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

namespace GameFrameworkPackage
{
    [CustomEditor(typeof(ImageFlip))]
    public class ImageFlipEditor : ImageEditor
    {
        public new ImageFlip target;

        private SerializedProperty _spFlipHor;
        private SerializedProperty _spFlipVer;
        private GUIContent _gcFlipHor;
        private GUIContent _gcFlipVer;

        protected override void OnEnable()
        {
            base.OnEnable();

            target = base.target as ImageFlip;

            _spFlipHor = serializedObject.FindProperty("flipHor");
            _spFlipVer = serializedObject.FindProperty("flipVer");
            _gcFlipHor = EditorGUIUtility.TrTextContent("flipHorizontal", null);
            _gcFlipVer = EditorGUIUtility.TrTextContent("flipVertical", null);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(_spFlipHor, _gcFlipHor);
            EditorGUILayout.PropertyField(_spFlipVer, _gcFlipVer);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
