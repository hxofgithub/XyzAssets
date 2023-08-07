using System;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace XyzAssets.Editor
{

    [CustomPropertyDrawer(typeof(TypeSelectorAttribute), true)]
    public class TypeSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var selector = attribute as TypeSelectorAttribute;

            var collection = TypeCache.GetTypesDerivedFrom(selector.SelectedType).ToArray().
                Select<Type, string>((t, result) =>
            {
                return t.FullName;
            }).ToList();
            collection.Insert(0, "None");
            EditorGUI.BeginChangeCheck();

            var selectedIndex = collection.IndexOf(property.stringValue);
            selectedIndex = EditorGUI.Popup(position, property.displayName, selectedIndex, collection.ToArray());
            if (selectedIndex != -1)
                property.stringValue = collection[selectedIndex];
            else
                property.stringValue = null;


            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();
        }
    }
}