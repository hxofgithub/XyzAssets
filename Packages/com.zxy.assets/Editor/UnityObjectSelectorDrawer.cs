using UnityEngine;
using UnityEditor;
namespace XyzAssets.Editor
{

    [CustomPropertyDrawer(typeof(UnityObjectSelectorAttribute), true)]
    public class UnityObjectSelectorDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            Object obj = AssetDatabase.LoadMainAssetAtPath(property.stringValue);

            obj = EditorGUI.ObjectField(new Rect(position.x, position.y, position.width, position.height), obj, typeof(UnityEngine.Object), false);
            if (obj != null)
                property.stringValue = AssetDatabase.GetAssetPath(obj);
            else
                property.stringValue = "";

            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }
    }

}
